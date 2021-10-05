using App.Core.Entities;
using App.Core.Entities.Configuration;
using App.Core.Extensions;
using App.Core.Interface;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Auth;
using App.Core.Interface.Services.Configuration;
using App.Core.Models;
using App.Core.Models.AuthModel;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Controllers
{
    [Route("api/v1/authenticate")]
    [ApiController]
    public abstract class AuthCoreController : ControllerBase
    {
        protected readonly ILogger<AuthCoreController> logger;
        protected IUserCoreService userService;
        protected IConfiguration configuration;
        protected IMapper mapper;
        private IEmailConfigurationCoreService emailConfigurationService;
        private readonly ISMSConfigurationCoreService sMSConfigurationService;
        private readonly IOTPHistoryCoreService oTPHistoryService;
        private readonly ISMSEmailTemplateCoreService sMSEmailTemplateService;
        private readonly ITokenManagerService tokenManagerService;
        protected IUserFileCoreService userFileCoreService;
        public AuthCoreController(IServiceProvider serviceProvider
            , IConfiguration configuration
            , IMapper mapper, ILogger<AuthCoreController> logger
            )
        {
            this.logger = logger;
            this.configuration = configuration;
            this.mapper = mapper;

            userService = serviceProvider.GetRequiredService<IUserCoreService>();
            tokenManagerService = serviceProvider.GetRequiredService<ITokenManagerService>();
            emailConfigurationService = serviceProvider.GetRequiredService<IEmailConfigurationCoreService>();
            sMSConfigurationService = serviceProvider.GetRequiredService<ISMSConfigurationCoreService>();
            oTPHistoryService = serviceProvider.GetRequiredService<IOTPHistoryCoreService>();
            sMSEmailTemplateService = serviceProvider.GetRequiredService<ISMSEmailTemplateCoreService>();
            userFileCoreService = serviceProvider.GetRequiredService<IUserFileCoreService>();
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public virtual async Task<AppDomainResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                success = await this.userService.Verify(loginModel.UserName, loginModel.Password);
                if (success)
                {
                    var userInfos = await this.userService.GetAsync(e => !e.Deleted
                    && (e.UserName == loginModel.UserName
                    || e.Phone == loginModel.UserName
                    || e.Email == loginModel.UserName
                    ));
                    if (userInfos != null && userInfos.Any())
                    {
                        var userModel = mapper.Map<UserCoreModel>(userInfos.FirstOrDefault());
                        var token = await GenerateJwtToken(userModel);
                        // Lưu giá trị token
                        await this.userService.UpdateUserToken(userModel.Id, token, true);
                        appDomainResult = new AppDomainResult()
                        {
                            Success = true,
                            Data = new
                            {
                                token = token,
                            },
                            ResultCode = (int)HttpStatusCode.OK
                        };

                    }
                }
                else
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không chính xác");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }

        /// <summary>
        /// Kiểm tra mã OTP đăng nhập hệ thống
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="otpValue"></param>
        /// <returns></returns>
        [HttpPost("send-otp/{phoneNumber}/{otpValue}")]
        [AllowAnonymous]
        public virtual async Task<AppDomainResult> SendOTP(string phoneNumber, string otpValue)
        {
            bool isValidPhoneNumber = ValidateUserName.IsPhoneNumber(phoneNumber);
            if (!isValidPhoneNumber) throw new AppException("Số điện thoại không hợp lệ!");
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Phone == phoneNumber);
            if (userInfos != null && userInfos.Any() && userInfos.Count == 1)
            {
                var userInfo = userInfos.FirstOrDefault();
                var otpHistoriesChecks = await this.oTPHistoryService.GetAsync(e => !e.Deleted && e.UserId == userInfo.Id && e.Phone == userInfo.Phone && e.OTPValue == otpValue);
                if (otpHistoriesChecks != null && otpHistoriesChecks.Any())
                {
                    var latestOTPCheck = otpHistoriesChecks.OrderByDescending(e => e.Created).FirstOrDefault();
                    if (DateTime.UtcNow.AddHours(7) > latestOTPCheck.ExpiredDate)
                        throw new AppException("OTP đã hết hạn, vui lòng lấy lại mã OTP khác!");
                    userInfo.IsCheckOTP = true;
                    userInfo.Updated = DateTime.UtcNow.AddHours(7);
                    userInfo.UpdatedBy = userInfo.UserName;
                    Expression<Func<UserCores, object>>[] inCludeProperties = new Expression<Func<UserCores, object>>[]
                    {
                        e => e.IsCheckOTP,
                        e => e.Updated,
                        e => e.UpdatedBy
                    };
                    await this.userService.UpdateFieldAsync(userInfo, inCludeProperties);
                }
                else throw new AppException("Mã OTP không chính xác!");
            }
            else throw new AppException("Số điện thoại chưa được đăng ký!");

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Kiểm tra OTP để đổi mật khẩu cho điện thoại
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="otpValue"></param>
        /// <returns></returns>
        [HttpPost("send-otp-forget-password/{phoneNumber}/{otpValue}")]
        [AllowAnonymous]
        public virtual async Task<AppDomainResult> SendOTPGetPassword(string phoneNumber, string otpValue)
        {
            bool isValidPhoneNumber = ValidateUserName.IsPhoneNumber(phoneNumber);
            if (!isValidPhoneNumber) throw new AppException("Số điện thoại không hợp lệ!");
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Phone == phoneNumber);
            if (userInfos != null && userInfos.Any() && userInfos.Count == 1)
            {
                var userInfo = userInfos.FirstOrDefault();
                var otpHistoriesChecks = await this.oTPHistoryService.GetAsync(e => !e.Deleted && !e.IsEmail && e.UserId == userInfo.Id && e.Phone == userInfo.Phone && e.OTPValue == otpValue);
                if (otpHistoriesChecks != null && otpHistoriesChecks.Any())
                {
                    var latestOTPCheck = otpHistoriesChecks.OrderByDescending(e => e.Created).FirstOrDefault();
                    if (DateTime.UtcNow.AddHours(7) > latestOTPCheck.ExpiredDate)
                        throw new AppException("OTP đã hết hạn, vui lòng lấy lại mã OTP khác!");
                    var userModel = mapper.Map<UserCoreModel>(userInfo);
                    var token = await GenerateJwtToken(userModel, true);
                    // Lưu giá trị token
                    await this.userService.UpdateUserToken(userModel.Id, token, true);
                    return new AppDomainResult()
                    {
                        Success = true,
                        Data = new
                        {
                            token = token,
                        },
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new AppException("Mã OTP không chính xác!");
            }
            else throw new AppException("Số điện thoại chưa được đăng ký!");
        }

        /// <summary>
        /// Kiểm tra OTP để đổi mật khẩu cho Email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otpValue"></param>
        /// <returns></returns>
        [HttpPost("send-otp-email-forget-password/{email}/{otpValue}")]
        [AllowAnonymous]
        public virtual async Task<AppDomainResult> SendOTPGetPasswordEmail(string email, string otpValue)
        {
            bool isValidEmail = ValidateUserName.IsEmail(email);
            if (!isValidEmail) throw new AppException("Email không hợp lệ!");
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Email == email);
            if (userInfos != null && userInfos.Any() && userInfos.Count == 1)
            {
                var userInfo = userInfos.FirstOrDefault();
                var otpHistoriesChecks = await this.oTPHistoryService.GetAsync(e => !e.Deleted && e.IsEmail && e.UserId == userInfo.Id && e.Email == userInfo.Email && e.OTPValue == otpValue);
                if (otpHistoriesChecks != null && otpHistoriesChecks.Any())
                {
                    var latestOTPCheck = otpHistoriesChecks.OrderByDescending(e => e.Created).FirstOrDefault();
                    if (DateTime.UtcNow.AddHours(7) > latestOTPCheck.ExpiredDate)
                        throw new AppException("OTP đã hết hạn, vui lòng lấy lại mã OTP khác!");
                    var userModel = mapper.Map<UserCoreModel>(userInfo);
                    var token = await GenerateJwtToken(userModel, true);
                    // Lưu giá trị token
                    await this.userService.UpdateUserToken(userModel.Id, token, true);
                    return new AppDomainResult()
                    {
                        Success = true,
                        Data = new
                        {
                            token = token,
                        },
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new AppException("Mã OTP không chính xác!");
            }
            else throw new AppException("Email chưa được đăng ký!");
        }

        /// <summary>
        /// Gửi mã OTP theo sdt có trong hệ thống
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-otp-code/{phoneNumber}")]
        public virtual async Task<AppDomainResult> GenerateOTPCode(string phoneNumber)
        {
            bool isValidPhoneNumber = ValidateUserName.IsPhoneNumber(phoneNumber);
            if (!isValidPhoneNumber) throw new AppException("Số điện thoại không hợp lệ!");
            var smsTemplateInfos = await this.sMSEmailTemplateService.GetAsync(e => !e.Deleted && e.Active && e.Code == CoreContants.SMS_XNOTP);
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Phone == phoneNumber);
            if (userInfos != null && userInfos.Any() && userInfos.Count == 1)
            {
                var userInfo = userInfos.FirstOrDefault();
                if (smsTemplateInfos != null && smsTemplateInfos.Any())
                {
                    var smsTemplateInfo = smsTemplateInfos.FirstOrDefault();
                    var otpValue = RandomUtilities.RandomOTPString(6);
                    bool isSendSMS = await sMSConfigurationService.SendSMS(userInfo.Phone, string.Format(smsTemplateInfo.Body, otpValue));
                    if (isSendSMS)
                    {
                        // Lưu lịch sử OTP ứng với thông tin user
                        OTPHistoryCores oTPHistories = new OTPHistoryCores()
                        {
                            Created = DateTime.Now,
                            CreatedBy = "system",
                            UserId = userInfo.Id,
                            Active = true,
                            Deleted = false,
                            OTPValue = otpValue,
                            Phone = userInfo.Phone,
                            ExpiredDate = DateTime.UtcNow.AddHours(7).AddMinutes(1),
                            Status = 0
                        };
                        await this.oTPHistoryService.CreateAsync(oTPHistories);
                    }
                    else throw new AppException("Gửi tin nhắn thất bại!");
                }
                else throw new AppException("Không tìm thấy nội dung tin nhắn!");
            }
            else throw new AppException("Số điện thoại chưa được đăng ký!");

            return new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Tạo OTP gửi qua email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("get-otp-code-email/{email}")]
        public virtual async Task<AppDomainResult> GenerateOTPCodeEmail(string email)
        {
            bool success = false;

            bool isValieEmail = ValidateUserName.IsEmail(email);
            if (!isValieEmail) throw new AppException("Email không hợp lệ!");
            var userInfos = await this.userService.GetAsync(e => !e.Deleted && e.Email == email);
            if (userInfos != null && userInfos.Any() && userInfos.Count == 1)
            {
                var userInfo = userInfos.FirstOrDefault();
                var otpValue = RandomUtilities.RandomOTPString(6);
                string emailBody = string.Format("<p>Mã OTP của bạn là: {0}. Thời hạn OTP hiệu lực trong 1 phút.</p>", otpValue);
                await emailConfigurationService.Send("OTP Xác thực", new string[] { userInfo.Email }, null, null, new EmailContent()
                {
                    Content = emailBody,
                    IsHtml = true,
                });
                // Lưu lịch sử OTP ứng với thông tin user
                OTPHistoryCores oTPHistories = new OTPHistoryCores()
                {
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = "system",
                    UserId = userInfo.Id,
                    Active = true,
                    Deleted = false,
                    OTPValue = otpValue,
                    Phone = userInfo.Phone,
                    Email = userInfo.Email,
                    IsEmail = true,
                    ExpiredDate = DateTime.UtcNow.AddHours(7).AddMinutes(1),
                    Status = 0
                };
                await this.oTPHistoryService.CreateAsync(oTPHistories);
            }
            else throw new AppException("Email chưa được đăng ký!");

            return new AppDomainResult
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK,
            };
        }

        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public virtual async Task<AppDomainResult> Register([FromBody] RegisterModel register)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                // Kiểm tra định dạng user name
                bool isValidUser = ValidateUserName.IsValidUserName(register.UserName);
                if (!isValidUser)
                    throw new AppException("Vui lòng nhập số điện thoại hoặc email!");

                var user = new UserCores()
                {
                    UserName = register.UserName,
                    Password = register.Password,
                    Created = DateTime.UtcNow.AddHours(7),
                    CreatedBy = register.UserName,
                    Active = true,
                    Phone = register.Phone,
                    Email = register.Email,
                    IsCheckOTP = false,
                };
                // Kiểm tra item có tồn tại chưa?
                var messageUserCheck = await this.userService.GetExistItemMessage(user);
                if (!string.IsNullOrEmpty(messageUserCheck))
                    throw new AppException(messageUserCheck);
                user.Password = SecurityUtils.HashSHA1(register.Password);
                appDomainResult.Success = await userService.CreateAsync(user);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
            {
                var resultMessage = ModelState.GetErrorMessage();
                throw new AppException(resultMessage);
            }
            return appDomainResult;
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="changePasswordModel"></param>
        /// <returns></returns>
        [HttpPut("changePassword/{userId}")]
        [Authorize]
        public virtual async Task<AppDomainResult> ChangePassword(int userId, [FromBody] ChangePasswordModel changePasswordModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (ModelState.IsValid)
            {
                // Check current user
                if (LoginContext.Instance.CurrentUser != null && LoginContext.Instance.CurrentUser.UserId != userId)
                    throw new AppException("Không phải người dùng hiện tại");
                // Check old Password + new Password
                string messageCheckPassword = await this.userService.CheckCurrentUserPassword(userId, changePasswordModel.OldPassword, changePasswordModel.NewPassword);
                if (!string.IsNullOrEmpty(messageCheckPassword))
                    throw new AppException(messageCheckPassword);

                var userInfo = await this.userService.GetByIdAsync(userId);
                string newPassword = SecurityUtils.HashSHA1(changePasswordModel.NewPassword);
                userInfo.IsResetPassword = true;
                appDomainResult.Success = await userService.UpdateUserPassword(userId, newPassword);
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
            }
            else
                throw new AppException(ModelState.GetErrorMessage());
            return appDomainResult;
        }

        /// <summary>
        /// Quên mật khẩu
        /// <para>Gửi mật khẩu mới qua Email nếu username là email</para>
        /// <para>Gửi mật khẩu mới qua SMS nếu username là phone</para>
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("forgot-password/{userName}")]
        public virtual async Task<AppDomainResult> ForgotPassword(string userName)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool isValidEmail = ValidateUserName.IsEmail(userName);
            bool isValidPhone = ValidateUserName.IsPhoneNumber(userName);
            // Kiểm tra đúng định dạng email và số điện thoại chưa
            //if (!isValidEmail && !isValidPhone)
            //    throw new AppException("Vui lòng nhập email hoặc số điện thoại!");
            // Tạo mật khẩu mới
            // Kiểm tra email/phone đã tồn tại chưa?
            var userInfos = await this.userService.GetAsync(e => !e.Deleted
            && (
            (isValidEmail == true && e.Email == userName)
            || (isValidPhone && e.Phone == userName)
            || e.UserName == userName
            )
            );
            UserCores userInfo = null;
            if (userInfos != null && userInfos.Any())
                userInfo = userInfos.FirstOrDefault();
            if (userInfo == null)
                throw new AppException("Số điện thoại hoặc email không tồn tại");
            // Cấp mật khẩu mới
            bool success = false;
            var newPasswordRandom = RandomUtilities.RandomString(8);
            if (isValidEmail)
            {
                userInfo.Password = SecurityUtils.HashSHA1(newPasswordRandom);
                userInfo.Updated = DateTime.UtcNow.AddHours(7);
                Expression<Func<UserCores, object>>[] includeProperties = new Expression<Func<UserCores, object>>[]
                {
                e => e.Password,
                e => e.Updated
                };
                success = await this.userService.UpdateFieldAsync(userInfo, includeProperties);
            }
            else success = true;
            return new AppDomainResult()
            {
                Success = success,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public virtual async Task<AppDomainResult> Logout()
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (LoginContext.Instance.CurrentUser != null)
                await this.userService.UpdateUserToken(LoginContext.Instance.CurrentUser.UserId, string.Empty, false);
            await this.tokenManagerService.DeactivateCurrentAsync();
            appDomainResult = new AppDomainResult()
            {
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

        #region Private methods

        /// <summary>
        /// Tạo token từ thông tin user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isConfirmOTP"></param>
        /// <returns></returns>
        protected async Task<string> GenerateJwtToken(UserCoreModel user, bool isConfirmOTP = false)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var appSettingsSection = configuration.GetSection("AppSettings");
            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            var fileAvatar = await this.userFileCoreService.GetSingleAsync(e => !e.Deleted && e.Active 
            && e.UserId == user.Id 
            && e.TypeId.HasValue && e.TypeId == 0
            );

            var userLoginModel = new UserLoginModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                IsCheckOTP = user.IsCheckOTP,
                IsConfirmOTP = isConfirmOTP,
                Avatar = fileAvatar != null ? fileAvatar.FileUrl : string.Empty
            };
           

            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] assems = currentDomain.GetAssemblies();
            var controllers = new List<ControllerModel>();
            var roles = new List<RoleModel>();
            foreach (Assembly assem in assems)
            {
                var controller = assem.GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
              .Select(e => new ControllerModel()
              {
                  Id = e.Name.Replace("Controller", string.Empty),
                  Name = string.Format("{0}", ReflectionUtils.GetClassDescription(e)).Replace("Controller", string.Empty)
              }).OrderBy(e => e.Name)
                  .Distinct();
                controllers.AddRange(controller);
            }
            if (controllers.Any())
            {
                foreach (var controller in controllers)
                {
                    roles.Add(new RoleModel()
                    {
                        RoleName = controller.Id,
                        IsView = await this.userService.HasPermission(userLoginModel.UserId, controller.Id, new string[] { CoreContants.ViewAll })
                    });
                }
            }
            userLoginModel.Roles = roles;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userLoginModel))
                            }),
                Expires = DateTime.UtcNow.AddHours(7).AddDays(1),
                //Expires = DateTime.Now.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }
}
