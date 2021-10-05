using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services;
using App.Core.Models;
using App.Core.Models.DomainModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Controllers.Auth
{
    [ApiController]
    public abstract class PermissionCoreController : BaseCatalogueController<PermissionCores, PermissionCoreModel, RequestCoreCatalogueModel, BaseSearch>
    {
        protected PermissionCoreController(IServiceProvider serviceProvider, ILogger<BaseController<PermissionCores, PermissionCoreModel, RequestCoreCatalogueModel, BaseSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
            this.catalogueService = serviceProvider.GetRequiredService<IPermissionCoreService>();
        }
    }
}
