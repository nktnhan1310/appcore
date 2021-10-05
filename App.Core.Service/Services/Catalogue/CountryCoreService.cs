using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Catalogue;
using App.Core.Interface.UnitOfWork;
using App.Core.Service.Services.DomainService;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service.Services.Catalogue
{
    public class CountryCoreService : CatalogueService<CountryCores, BaseSearch>, ICountryCoreService
    {
        public CountryCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
