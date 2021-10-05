using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using App.Core.Interface;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service
{
    public class OTPHistoryCoreService : DomainService<OTPHistoryCores, BaseSearch>, IOTPHistoryCoreService
    {
        public OTPHistoryCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
