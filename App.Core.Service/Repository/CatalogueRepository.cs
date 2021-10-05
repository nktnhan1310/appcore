using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Core.Service
{
    public class CatalogueRepository<T> : DomainRepository<T>, ICatalogueRepository<T> where T: AppCoreCatalogueDomain
    {
        public CatalogueRepository(ICoreDbContext context) : base(context)
        {

        }
        //public CatalogueRepository(IDbContextFactory dbContextFactory) : base(dbContextFactory)
        //{
        //}

        public T GetByCode(string code)
        {
            return Context.Set<T>().FirstOrDefault(e => e.Code == code);
        }

        public void Delete(int id)
        {
            T entity = Context.Set<T>().FirstOrDefault(e => e.Id == id);
            entity.Deleted = true;
            Context.Set<T>().Update(entity);
        }
        public override void Delete(T entity)
        {
            entity.Deleted = true;
            Context.Set<T>().Update(entity);
        }
    }
}
