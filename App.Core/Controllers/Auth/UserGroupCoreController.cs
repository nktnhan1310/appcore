using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.Services;
using App.Core.Models;
using App.Core.Models.AuthModel;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.Core.Controllers.Auth
{
    [ApiController]
    public abstract class UserGroupCoreController : BaseCatalogueController<UserGroupCores, UserGroupCoreModel, RequestUserGroupCoreModel, BaseSearchUserInGroup>
    {
        private readonly IUserInGroupCoreService userInGroupService;
        private readonly IPermissionCoreService permissionService;
        private readonly IPermitObjectPermissionCoreService permitObjectPermissionService;

        protected UserGroupCoreController(IServiceProvider serviceProvider, ILogger<BaseController<UserGroupCores, UserGroupCoreModel, RequestUserGroupCoreModel, BaseSearchUserInGroup>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IUserGroupCoreService>();
            userInGroupService = serviceProvider.GetRequiredService<IUserInGroupCoreService>();
            permissionService = serviceProvider.GetRequiredService<IPermissionCoreService>();
            permitObjectPermissionService = serviceProvider.GetRequiredService<IPermitObjectPermissionCoreService>();
        }

        /// <summary>
        /// Lấy thông tin nhóm theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            var item = await this.catalogueService.GetByIdAsync(id);
            if (item != null)
            {
                if (LoginContext.Instance.CurrentUser != null)
                {
                    var itemModel = mapper.Map<UserGroupCoreModel>(item);
                    var userInGroups = await this.userInGroupService.GetAsync(e => !e.Deleted && e.UserGroupId == id);
                    if (userInGroups != null)
                        itemModel.UserIds = userInGroups.Select(e => e.UserId).ToList();

                    var permitObjectPermissions = await this.permitObjectPermissionService.GetAsync(e => !e.Deleted && e.UserGroupId == id);
                    if (permitObjectPermissions != null)
                        itemModel.PermitObjectPermissions = mapper.Map<IList<PermitObjectPermissionCoreModel>>(permitObjectPermissions);
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-permissions")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetPermissionCatalogue()
        {
            var listPermissions = await this.permissionService.GetAsync(e => !e.Deleted);
            var listPermissionModels = mapper.Map<List<PermissionCoreModel>>(listPermissions);
            return new AppDomainResult()
            {
                Data = listPermissionModels,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            }; ;
        }

        /// <summary>
        /// Lấy danh sách phân trang người dùng thuộc nhóm
        /// </summary>
        /// <param name="searchUserInGroup"></param>
        /// <returns></returns>
        [HttpGet("get-user-in-groups")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public async Task<AppDomainResult> GetUserInGroups([FromQuery] BaseSearchUserInGroup searchUserInGroup)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            var pagedList = await this.userInGroupService.GetPagedListData(searchUserInGroup);
            var pagedListModel = mapper.Map<PagedList<UserInGroupCoreModel>>(pagedList);
            appDomainResult = new AppDomainResult()
            {
                Data = pagedListModel,
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
            return appDomainResult;
        }

    }
}
