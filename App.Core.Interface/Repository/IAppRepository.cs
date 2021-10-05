using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interface.Repository
{
    public interface IAppRepository<T> : IDomainRepository<T> where T : AppCoreDomain
    {
    }
}
