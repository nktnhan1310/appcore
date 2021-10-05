using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interface.Repository
{
    public interface ICatalogueRepository<T>: IDomainRepository<T> where T : AppCoreCatalogueDomain
    {
        T GetByCode(string code);
    }
}
