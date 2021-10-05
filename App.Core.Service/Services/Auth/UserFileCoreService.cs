using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Auth;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service
{
    public class UserFileCoreService : DomainService<UserFileCores, BaseSearch>, IUserFileCoreService
    {
        public UserFileCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
    }
}
