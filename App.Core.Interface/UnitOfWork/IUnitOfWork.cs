using App.Core.Entities.DomainEntity;
using App.Core.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Interface.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICatalogueRepository<T> CatalogueRepository<T>() where T : AppCoreCatalogueDomain;
        IDomainRepository<T> Repository<T>() where T : AppCoreDomain;
        void Save();
        Task SaveAsync();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
    }
}
