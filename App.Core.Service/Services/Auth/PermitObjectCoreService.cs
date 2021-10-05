using AutoMapper;
using App.Core.Service.Services.DomainService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;

namespace App.Core.Service
{
    public class PermitObjectCoreService : CatalogueService<PermitObjectCores, BaseSearch>, IPermitObjectCoreService
    {
        public PermitObjectCoreService(IAppUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper, configuration)
        {
        }
    }
}
