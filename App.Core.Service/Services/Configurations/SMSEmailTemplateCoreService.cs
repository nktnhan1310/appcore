using App.Core.Entities;
using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Configuration;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using App.Core.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public class SMSEmailTemplateCoreService : CatalogueService<SMSEmailTemplateCores, BaseSearch>, ISMSEmailTemplateCoreService
    {
        public SMSEmailTemplateCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

    }
}
