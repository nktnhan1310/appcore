using App.Core.Entities;
using App.Core.Extensions;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Auth;
using App.Core.Models.AuthModel;
using App.Core.Models.AuthModel.RequestModel;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace App.Core.Controllers.Auth
{
    [ApiController]
    public abstract class UserCoreController : BaseController<UserCores, UserCoreModel, RequestUserCoreModel, BaseSearchUser>
    {
        protected IUserCoreService userService;
        protected IUserInGroupCoreService userInGroupService;
        protected IUserFileCoreService userFileService;
        protected IUserGroupCoreService userGroupCoreService;
        private IConfiguration configuration;
        public UserCoreController(IServiceProvider serviceProvider, ILogger<BaseController<UserCores, UserCoreModel, RequestUserCoreModel, BaseSearchUser>> logger
            , IConfiguration configuration
            , IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.domainService = serviceProvider.GetRequiredService<IUserCoreService>();
            this.userService = serviceProvider.GetRequiredService<IUserCoreService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupCoreService>();
            userFileService = serviceProvider.GetRequiredService<IUserFileCoreService>();
            userGroupCoreService = serviceProvider.GetRequiredService<IUserGroupCoreService>();
            this.configuration = configuration;
        }

        /// <summary>
        /// Lấy thông tin danh sách user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("get-list-user")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInfos([FromQuery] string userName)
        {
            var UserCores = await this.userService.GetAsync(e => !e.Deleted
            && (!LoginContext.Instance.CurrentUser.HospitalId.HasValue || e.Id == LoginContext.Instance.CurrentUser.HospitalId)
            && (string.IsNullOrEmpty(userName) || (e.UserName.Contains(userName) || e.LastName.Contains(userName) || e.FirstName.Contains(userName)))
            , e => new UserCores()
            {
                Id = e.Id,
                UserName = e.UserName,
                FirstName = e.FirstName,
                LastName = e.LastName,
            });
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<UserCoreModel>>(UserCores),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            if (id == 0)
            {
                throw new KeyNotFoundException("id không tồn tại");
            }
            var item = await this.domainService.GetByIdAsync(id);

            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null)
                {
                    var itemModel = mapper.Map<UserCoreModel>(item);
                    itemModel.ConfirmPassWord = item.Password;
                    var userInGroups = await this.userInGroupService.GetAsync(e => !e.Deleted && e.UserId == id);
                    if (userInGroups != null)
                        itemModel.UserGroupIds = userInGroups.Select(e => e.UserGroupId).ToList();
                    var userFiles = await this.userFileService.GetAsync(e => !e.Deleted && e.UserId == id);
                    if (userFiles != null)
                        itemModel.UserFiles = mapper.Map<IList<UserFileCoreModel>>(userFiles);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Không có quyền truy cập");
            }
            else
            {
                throw new KeyNotFoundException("Item không tồn tại");
            }
            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] RequestUserCoreModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<UserCores>(itemModel);
                item.Updated = DateTime.UtcNow.AddHours(7);
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                if (itemModel.IsResetPassword)
                    item.Password = SecurityUtils.HashSHA1(itemModel.NewPassWord);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);

                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            // ------- START GET URL FOR FILE
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
                                // ------- END GET URL FOR FILE
                                filePaths.Add(filePath);
                                file.Created = DateTime.UtcNow.AddHours(7);
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.UserId = item.Id;
                                file.ContentType = ContentFileTypeUtilities.GetMimeType(filePath);
                                file.FileUrl = fileUrl;
                            }
                            else
                            {
                                file.Updated = DateTime.UtcNow.AddHours(7);
                                file.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                            }
                        }
                    }
                    success = await this.domainService.UpdateAsync(item);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }
                    else
                    {
                        if (folderUploadPaths.Any())
                        {
                            foreach (var folderUploadPath in folderUploadPaths)
                            {
                                System.IO.File.Delete(folderUploadPath);
                            }
                        }
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới user
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] RequestUserCoreModel itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            bool success = false;
            if (ModelState.IsValid)
            {

                var item = mapper.Map<UserCores>(itemModel);
                item.Created = DateTime.UtcNow.AddHours(7);
                item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;
                item.Password = SecurityUtils.HashSHA1(itemModel.Password);
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.domainService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    List<string> filePaths = new List<string>();
                    List<string> folderUploadPaths = new List<string>();
                    if (item.UserFiles != null && item.UserFiles.Any())
                    {
                        foreach (var file in item.UserFiles)
                        {
                            string folderUploadPath = string.Empty;
                            var isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                            if (isProduct)
                                folderUploadPath = configuration.GetValue<string>("MySettings:FolderUpload");
                            else
                                folderUploadPath = Path.Combine(Directory.GetCurrentDirectory());
                            string filePath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, file.FileName);
                            string folderUploadUrl = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME);
                            string fileUploadPath = Path.Combine(folderUploadPath, UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));


                            // Kiểm tra có tồn tại file trong temp chưa?
                            if (System.IO.File.Exists(filePath) && !System.IO.File.Exists(fileUploadPath))
                            {
                                FileUtils.CreateDirectory(folderUploadUrl);
                                FileUtils.SaveToPath(fileUploadPath, System.IO.File.ReadAllBytes(filePath));
                                folderUploadPaths.Add(fileUploadPath);
                                string fileUrl = Path.Combine(UPLOAD_FOLDER_NAME, USER_FOLDER_NAME, Path.GetFileName(filePath));
                                filePaths.Add(filePath);
                                file.Created = DateTime.UtcNow.AddHours(7);
                                file.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                                file.Active = true;
                                file.Deleted = false;
                                file.FileName = Path.GetFileName(filePath);
                                file.FileExtension = Path.GetExtension(filePath);
                                file.UserId = item.Id;
                                file.FileUrl = fileUrl;
                            }
                        }
                    }
                    success = await this.domainService.CreateAsync(item);
                    if (success)
                    {
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }
                    else
                    {
                        if (folderUploadPaths.Any())
                        {
                            foreach (var folderUploadPath in folderUploadPaths)
                            {
                                System.IO.File.Delete(folderUploadPath);
                            }
                        }
                        // Remove file trong thư mục temp
                        if (filePaths.Any())
                        {
                            foreach (var filePath in filePaths)
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                        throw new Exception("Lỗi trong quá trình xử lý");
                    }
                    appDomainResult.Success = success;
                }
                else
                    throw new AppException("Item không tồn tại");
            }
            else
            {
                throw new AppException(ModelState.GetErrorMessage());
            }
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> Get([FromQuery] BaseSearchUser baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<UserCores> pagedData = await this.domainService.GetPagedListData(baseSearch);
                PagedList<UserCoreModel> pagedDataModel = mapper.Map<PagedList<UserCoreModel>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        #region Contants

        public const string USER_FOLDER_NAME = "user";

        #endregion

    }
}
