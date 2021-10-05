using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.Services;
using App.Core.Models;
using App.Core.Models.AuthModel.RequestModel;
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
using System.Reflection;
using System.Threading.Tasks;

namespace App.Core.Controllers.Auth
{
    [ApiController]
    public abstract class PermitObjectCoreController : BaseCatalogueController<PermitObjectCores, PermitObjectCoreModel, RequestPermitObjectCoreModel, BaseSearch>
    {
        protected PermitObjectCoreController(IServiceProvider serviceProvider, ILogger<BaseController<PermitObjectCores, PermitObjectCoreModel, RequestPermitObjectCoreModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IPermitObjectCoreService>();
        }

        /// <summary>
        /// Lấy thông tin những chức năng cần được phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-catalogue-controller")]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public async Task<AppDomainResult> GetCatalogueController()
        {
            return await Task.Run(() =>
            {
                AppDomainResult appDomainResult = new AppDomainResult();
                AppDomain currentDomain = AppDomain.CurrentDomain;
                Assembly[] assems = currentDomain.GetAssemblies();
                var controllers = new List<ControllerModel>();
                foreach (Assembly assem in assems)
                {
                    var controller = assem.GetTypes().Where(type =>
                    typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
                  .Select(e => new ControllerModel()
                  {
                      Id = e.Name.Replace("Controller", string.Empty),
                      Name = string.Format("{0}", ReflectionUtils.GetClassDescription(e)).Replace("Controller", string.Empty)
                  }).OrderBy(e => e.Name)
                      .Distinct();

                    controllers.AddRange(controller);
                }
                appDomainResult = new AppDomainResult()
                {
                    Data = controllers,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
                return appDomainResult;
            });
        }

        /// <summary>
        /// Lấy thông tin chức năng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();
            try
            {
                var item = await this.catalogueService.GetByIdAsync(id);
                if (item != null)
                {
                    var itemModel = mapper.Map<PermitObjectCoreModel>(item);
                    itemModel.ToView();
                    appDomainResult = new AppDomainResult()
                    {
                        Success = true,
                        Data = itemModel,
                        ResultCode = (int)HttpStatusCode.OK
                    };
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            catch (Exception ex)
            {
                this.logger.LogError(string.Format("{0} {1}: {2}", this.ControllerContext.RouteData.Values["controller"].ToString(), "GetById", ex.Message));
                throw new Exception(ex.Message);
            }
            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới chức năng
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override Task<AppDomainResult> AddItem([FromBody] RequestPermitObjectCoreModel itemModel)
        {
            itemModel.ToModel();
            return base.AddItem(itemModel);
        }

        /// <summary>
        /// Cập nhật thông tin chức năng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override Task<AppDomainResult> UpdateItem([FromBody] RequestPermitObjectCoreModel itemModel)
        {
            itemModel.ToModel();
            return base.UpdateItem(itemModel);
        }
    }
}
