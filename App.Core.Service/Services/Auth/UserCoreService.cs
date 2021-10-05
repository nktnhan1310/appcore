using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Interface.DbContext;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public class UserCoreService : DomainService<UserCores, BaseSearchUser>, IUserCoreService
    {
        protected IAppDbContext coreDbContext;
        public UserCoreService(IAppUnitOfWork unitOfWork, IMapper mapper, IAppDbContext coreDbContext) : base(unitOfWork, mapper)
        {
            this.coreDbContext = coreDbContext;
        }
        protected override string GetStoreProcName()
        {
            return "User_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(BaseSearchUser baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@Email", string.IsNullOrEmpty(baseSearch.Email) ? DBNull.Value : (object)baseSearch.Email),
                new SqlParameter("@Phone", baseSearch.Phone),
                new SqlParameter("@UserGroupId", baseSearch.UserGroupId),
                new SqlParameter("@SearchContent", baseSearch.SearchContent),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

        /// <summary>
        /// Kiểm tra user đã tồn tại chưa?
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<string> GetExistItemMessage(UserCores item)
        {
            List<string> messages = new List<string>();
            string result = string.Empty;
            bool isExistEmail = !string.IsNullOrEmpty(item.Email) && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Email == item.Email);
            bool isExistPhone = !string.IsNullOrEmpty(item.Phone) && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id && x.Phone == item.Phone);
            bool isExistUserName = !string.IsNullOrEmpty(item.UserName)
                && await Queryable.AnyAsync(x => !x.Deleted && x.Id != item.Id
                && (x.UserName == item.UserName
                || x.Email == item.UserName
                || x.Phone == item.UserName
                ));
            bool isPhone = ValidateUserName.IsPhoneNumber(item.UserName);
            bool isEmail = ValidateUserName.IsEmail(item.UserName);


            if (isExistEmail)
                messages.Add("Email đã tồn tại!");
            if (isExistPhone)
                messages.Add("Số điện thoại đã tồn tại!");
            if (isExistUserName)
            {
                if (isPhone)
                    messages.Add("Số điện thoại đã tồn tại!");
                else if (isEmail)
                    messages.Add("Email đã tồn tại!");
                else
                    messages.Add("User name đã tồn tại!");
            }
            if (messages.Any())
                result = string.Join(" ", messages);
            return result;
        }

        /// <summary>
        /// Lưu thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> CreateAsync(UserCores item)
        {
            bool result = false;
            if (item != null)
            {
                if (item != null)
                {
                    // Tạo mới nhóm người dùng
                    item.Id = 0;
                    await this.unitOfWork.Repository<UserCores>().CreateAsync(item);
                    await this.unitOfWork.SaveAsync();

                    // Lưu thông tin user thuộc nhóm người dùng
                    if (item.UserGroupIds != null && item.UserGroupIds.Any())
                    {
                        foreach (var userGroupId in item.UserGroupIds)
                        {
                            UserInGroupCores userInGroup = new UserInGroupCores()
                            {
                                Created = DateTime.Now,
                                CreatedBy = item.CreatedBy,
                                UserId = item.Id,
                                UserGroupId = userGroupId,
                                Active = true,
                                Deleted = false,
                                Id = 0
                            };
                            await this.unitOfWork.Repository<UserInGroupCores>().CreateAsync(userInGroup);
                        }
                    }
                    // Lưu thông tin chức năng + quyền tương ứng
                    if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
                    {
                        foreach (var permitObjectPermission in item.PermitObjectPermissions)
                        {
                            permitObjectPermission.Created = DateTime.Now;
                            permitObjectPermission.UserId = item.Id;
                            permitObjectPermission.Active = true;
                            permitObjectPermission.Id = 0;
                            await this.unitOfWork.Repository<PermitObjectPermissionCores>().CreateAsync(permitObjectPermission);
                        }
                    }

                    // Lưu thông file của user
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var userFile in item.UserFiles)
                        {
                            userFile.Created = DateTime.Now;
                            userFile.UserId = item.Id;
                            userFile.Active = true;
                            userFile.Id = 0;
                            await this.unitOfWork.Repository<UserFileCores>().CreateAsync(userFile);
                        }
                    }
                    await this.unitOfWork.SaveAsync();
                    await this.coreDbContext.SaveChangesAsync();

                    this.coreDbContext.Entry<UserCores>(item).State = EntityState.Detached;

                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateAsync(UserCores item)
        {
            bool result = false;
            var existItem = await this.Queryable.Where(e => e.Id == item.Id).FirstOrDefaultAsync();
            if (existItem != null)
            {
                if (!item.IsResetPassword)
                    item.Password = existItem.Password;
                var currentCreated = existItem.Created;
                var currentCreatedByInfo = existItem.CreatedBy;
                existItem = mapper.Map<UserCores>(item);
                existItem.Created = currentCreated;
                existItem.CreatedBy = currentCreatedByInfo;

                
                this.unitOfWork.Repository<UserCores>().Update(existItem);
                await this.unitOfWork.SaveAsync();

                // Cập nhật thông tin user ở nhóm
                if (item.UserGroupIds != null && item.UserGroupIds.Any())
                {
                    foreach (var userGroupId in item.UserGroupIds)
                    {
                        var existUserInGroup = await this.unitOfWork.Repository<UserInGroupCores>().GetQueryable()
                            .Where(e => e.UserGroupId == userGroupId && e.UserId == existItem.Id).FirstOrDefaultAsync();
                        if (existUserInGroup != null)
                        {
                            existUserInGroup.UserGroupId = userGroupId;
                            existUserInGroup.UserId = item.Id;
                            existUserInGroup.Updated = DateTime.Now;
                            this.unitOfWork.Repository<UserInGroupCores>().Update(existUserInGroup);
                        }
                        else
                        {
                            UserInGroupCores userInGroup = new UserInGroupCores()
                            {
                                Created = DateTime.Now,
                                CreatedBy = item.CreatedBy,
                                UserId = item.Id,
                                UserGroupId = userGroupId,
                                Active = true,
                                Deleted = false,
                            };

                            userInGroup.Created = DateTime.Now;
                            userInGroup.UserId = item.Id;
                            userInGroup.Id = 0;
                            await this.unitOfWork.Repository<UserInGroupCores>().CreateAsync(userInGroup);
                        }
                    }

                    // Kiểm tra những item không có trong role chọn => Xóa đi
                    var existGroupOlds = await this.unitOfWork.Repository<UserInGroupCores>().GetQueryable().Where(e => !item.UserGroupIds.Contains(e.UserGroupId) && e.UserId == existItem.Id).ToListAsync();
                    if(existGroupOlds != null)
                    {
                        foreach (var existGroupOld in existGroupOlds)
                        {
                            this.unitOfWork.Repository<UserInGroupCores>().Delete(existGroupOld);
                        }
                    }
                }
                else
                {
                    var userInGroups = await this.unitOfWork.Repository<UserInGroupCores>().GetQueryable().Where(e => e.UserId == existItem.Id).ToListAsync();
                    if (userInGroups != null && userInGroups.Any())
                    {
                        foreach (var userInGroup in userInGroups)
                        {
                            //userInGroup.Updated = DateTime.Now;
                            //userInGroup.UpdatedBy = item.UpdatedBy;
                            //userInGroup.Deleted = true;
                            this.unitOfWork.Repository<UserInGroupCores>().Delete(userInGroup);
                        }
                    }
                }

                // Cập nhật thông tin quyền với chứng năng tương ứng của nhóm
                if (item.PermitObjectPermissions != null && item.PermitObjectPermissions.Any())
                {
                    foreach (var permitObjectPermission in item.PermitObjectPermissions)
                    {
                        var existPermitObjectPermission = await this.unitOfWork.Repository<PermitObjectPermissionCores>().GetQueryable()
                            .Where(e => e.Id == permitObjectPermission.Id).FirstOrDefaultAsync();
                        if (existPermitObjectPermission != null)
                        {
                            var currentCreatedPermitObject = existItem.Created;
                            var currentCreatedByPermitObject = existItem.CreatedBy;
                            existPermitObjectPermission = mapper.Map<PermitObjectPermissionCores>(permitObjectPermission);
                            existPermitObjectPermission.Created = currentCreatedPermitObject;
                            existPermitObjectPermission.CreatedBy = currentCreatedByPermitObject;
                            
                            existPermitObjectPermission.UserId = item.Id;
                            existPermitObjectPermission.Updated = DateTime.Now;
                            this.unitOfWork.Repository<PermitObjectPermissionCores>().Update(existPermitObjectPermission);
                        }
                        else
                        {
                            permitObjectPermission.Created = DateTime.Now;
                            permitObjectPermission.UserId = item.Id;
                            permitObjectPermission.Id = 0;
                            await this.unitOfWork.Repository<PermitObjectPermissionCores>().CreateAsync(permitObjectPermission);

                        }
                    }
                }
                // Cập nhật thông tin file người dùng
                if (item.UserFiles != null && item.UserFiles.Any())
                {
                    foreach (var userFile in item.UserFiles)
                    {
                        var existUserFile = await this.unitOfWork.Repository<UserFileCores>().GetQueryable().Where(e => e.Id == userFile.Id).FirstOrDefaultAsync();
                        if (existUserFile != null)
                        {
                            var currentCreatedFile = existItem.Created;
                            var currentCreatedByFile = existItem.CreatedBy;
                            existUserFile = mapper.Map<UserFileCores>(userFile);
                            existUserFile.Created = currentCreatedFile;
                            existUserFile.CreatedBy = currentCreatedByFile;

                            existUserFile.UserId = item.Id;
                            existUserFile.Updated = DateTime.Now;
                            this.unitOfWork.Repository<UserFileCores>().Update(existUserFile);
                        }
                        else
                        {
                            userFile.Created = DateTime.Now;
                            userFile.UserId = item.Id;
                            userFile.Id = 0;
                            await this.unitOfWork.Repository<UserFileCores>().CreateAsync(userFile);

                        }
                    }
                }
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Cập nhật password mới cho user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserPassword(int userId, string newPassword)
        {
            bool result = false;

            var existUserInfo = await this.unitOfWork.Repository<UserCores>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if(existUserInfo != null)
            {
                existUserInfo.Password = newPassword;
                existUserInfo.Updated = DateTime.Now;
                Expression<Func<UserCores, object>>[] includeProperties = new Expression<Func<UserCores, object>>[]
                {
                    e => e.Password,
                    e => e.Updated
                };
                await this.unitOfWork.Repository<UserCores>().UpdateFieldsSaveAsync(existUserInfo ,includeProperties);
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Kiểm tra user đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Verify(string userName, string password)
        {
            var user = await Queryable
                .Where(e => !e.Deleted
                && (e.UserName == userName
                || e.Phone == userName
                || e.Email == userName
                )
                )
                .FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.IsLocked)
                {
                    if (user.LockedDate.HasValue)
                    {
                        // Nếu qua thời hạn => unlock user
                        if (user.LockedDate.Value < DateTime.Now && user.Password == SecurityUtils.HashSHA1(password))
                        {
                            user.IsLocked = false;
                            user.LockedDate = null;
                            user.TotalViolations = 0;
                            Expression<Func<UserCores, object>>[] includeProperties = new Expression<Func<UserCores, object>>[]
                            {
                                e => e.IsLocked,
                                e => e.LockedDate,
                                e => e.TotalViolations
                            };
                            await this.unitOfWork.Repository<UserCores>().UpdateFieldsSaveAsync(user, includeProperties);
                            return true;
                        }
                        else throw new Exception(string.Format("Account is Locked! Unlock date: {0}", user.LockedDate.Value.ToString("dd/MM/yyyy")));
                    }
                    else throw new Exception("Account is Locked");
                }
                if (!user.Active)
                {
                    throw new Exception("Account is UnActive");
                }
                if (!user.IsAdmin && !user.IsCheckOTP)
                {
                    throw new Exception("Người dùng chưa xác thực otp");
                }
                if (user.Password == SecurityUtils.HashSHA1(password))
                {
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }

        /// <summary>
        /// Kiểm tra pass word cũ đã giống chưa
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> CheckCurrentUserPassword(int userId, string password, string newPasssword)
        {
            string message = string.Empty;
            List<string> messages = new List<string>();
            bool isCurrentPassword = await this.Queryable.AnyAsync(x => x.Id == userId && x.Password == SecurityUtils.HashSHA1(password));
            bool isDuplicateNewPassword = await this.Queryable.AnyAsync(x => x.Id == userId && x.Password == SecurityUtils.HashSHA1(newPasssword));
            if (!isCurrentPassword)
                messages.Add("Mật khẩu cũ không chính xác");
            else if (isDuplicateNewPassword)
                messages.Add("Mật khẩu mới không được trùng mật khẩu cũ");
            if (messages.Any())
                message = string.Join("; ", messages);
            return message;
        }

        /// <summary>
        /// Kiểm tra quyền của user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="controller"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> HasPermission(int userId, string controller, IList<string> permissions)
        {
            bool hasPermit = false;

            var userInfo = await unitOfWork.Repository<UserCores>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            if (userInfo != null && userInfo.IsLocked)
                throw new AppException("User account is locked!");

            // Lấy ra những nhóm user thuộc
            var userGroupIds = await unitOfWork.Repository<UserInGroupCores>().GetQueryable()
                .Where(e => e.UserId == userId)
                .Select(e => e.UserGroupId).ToListAsync();

            var permissionIds = new List<int>();
            var permitObjectIds = new List<int>();

            if (userGroupIds != null && userGroupIds.Any())
            {
                var permitObjectChecks = await unitOfWork.Repository<PermitObjectCores>().GetQueryable().Where(e => !e.Deleted
                && !string.IsNullOrEmpty(e.ControllerNames)
                && e.ControllerNames.Contains(controller)
                ).ToListAsync();
                permitObjectChecks = permitObjectChecks.Where(e => e.ControllerNames.Split(";", StringSplitOptions.None).Contains(controller)).ToList();
                if (permitObjectChecks != null && permitObjectChecks.Any())
                {
                    var permitObjectCheckIds = permitObjectChecks.Select(e => e.Id).ToList();
                    // Lấy ra những quyền user có trong chức năng cần kiểm tra
                    var permitObjectPermissions = await unitOfWork.Repository<PermitObjectPermissionCores>().GetQueryable()
                    .Where(e => e.UserGroupId.HasValue
                    && userGroupIds.Contains(e.UserGroupId.Value)
                    && permitObjectCheckIds.Contains(e.PermitObjectId)
                    )
                    .ToListAsync();
                    if (permitObjectPermissions != null && permitObjectPermissions.Any())
                    {
                        permitObjectIds = permitObjectPermissions.Select(e => e.PermitObjectId).Distinct().ToList();

                        foreach (var permitObjectId in permitObjectIds)
                        {
                            // Lấy danh mục mã quyền user cần kiểm tra
                            permissionIds = permitObjectPermissions.Where(e => e.PermitObjectId == permitObjectId).Select(e => e.PermissionId).ToList();
                            var permissionCodes = await unitOfWork.Repository<PermissionCores>().GetQueryable().Where(e => permissionIds.Contains(e.Id))
                                .Select(e => e.Code)
                                .ToListAsync();

                            // Lấy danh chức năng cần kiểm tra
                            var permitObjectControllers = await unitOfWork.Repository<PermitObjectCores>().GetQueryable().Where(e => permitObjectIds.Contains(e.Id))
                                .Select(e => e.ControllerNames.Split(";", StringSplitOptions.None))
                                .ToListAsync();

                            // Kiểm tra user có quyền trong chức năng không
                            if (permissionCodes != null && permissionCodes.Any() && permitObjectControllers != null && permitObjectControllers.Any())
                            {
                                hasPermit = permitObjectControllers.Any(x => x.Contains(controller)) && permissions.Any(x => permissionCodes.Contains(x));
                            }
                        }
                    }
                }

            }
            return hasPermit;
        }

        /// <summary>
        /// Kiểm tra user có trong nhóm chỉ định không
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userGroupCode"></param>
        /// <returns></returns>
        public async Task<bool> IsInUserGroup(int userId, string userGroupCode)
        {
            bool result = false;
            var userGroupInfo = await this.unitOfWork.Repository<UserGroupCores>().GetQueryable()
                .Where(e => !e.Deleted && e.Active && e.Code == userGroupCode).FirstOrDefaultAsync();
            if (userGroupInfo != null)
            {
                result = await this.unitOfWork.Repository<UserInGroupCores>().GetQueryable()
                    .AnyAsync(e => !e.Deleted && e.Active && e.UserGroupId == userGroupInfo.Id && e.UserId == userId);
            }
            else throw new AppException("Không tìm thấy thông tin nhóm người dùng");


            return result;
        }

        /// <summary>
        /// Cập nhật thông tin user token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="isLogin"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserToken(int userId, string token, bool isLogin = false)
        {
            bool result = false;

            var userInfo = await this.unitOfWork.Repository<UserCores>().GetQueryable().Where(e => e.Id == userId).FirstOrDefaultAsync();
            //this.medicalDbContext.Entry<Users>(userInfo).State = EntityState.Detached;
            if (userInfo != null)
            {
                if (isLogin)
                {
                    userInfo.Token = token;
                    userInfo.ExpiredDate = DateTime.Now.AddDays(1);
                    //userInfo.ExpiredDate = DateTime.Now.AddMinutes(1);
                }
                else
                {
                    userInfo.Token = string.Empty;
                    userInfo.ExpiredDate = null;
                }
                Expression<Func<UserCores, object>>[] includeProperties = new Expression<Func<UserCores, object>>[]
                {
                    e => e.Token,
                    e => e.ExpiredDate
                };
                this.unitOfWork.Repository<UserCores>().UpdateFieldsSave(userInfo, includeProperties);
                await this.unitOfWork.SaveAsync();
                result = true;
            }

            return result;
        }
    }
}
