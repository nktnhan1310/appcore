using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service
{
    public class PermitObjectPermissionCoreService : DomainService<PermitObjectPermissionCores, BaseSearch>, IPermitObjectPermissionCoreService
    {
        public PermitObjectPermissionCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
