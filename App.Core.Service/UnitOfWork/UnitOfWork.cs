using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Core.Interface.UnitOfWork;
using App.Core.Interface.DbContext;
using App.Core.Interface.Repository;
using App.Core.Entities.DomainEntity;
using App.Core.Service.Factory;

namespace App.Core.Service
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected ICoreDbContext context;
        public UnitOfWork(ICoreDbContext context)
        {
            this.context = context;
            if (this.context != null)
            {
                this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }

        public UnitOfWork(IDbContextFactory dbContextFactory)
        {
            context = dbContextFactory.Create();
            if (context != null)
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }
        public abstract ICatalogueRepository<T> CatalogueRepository<T>() where T : AppCoreCatalogueDomain;

        public abstract IDomainRepository<T> Repository<T>() where T : AppCoreDomain;

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return context.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
