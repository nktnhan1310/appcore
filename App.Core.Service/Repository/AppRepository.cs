using App.Core.Entities.DomainEntity;
using App.Core.Interface.DbContext;
using App.Core.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service
{
    public class AppRepository<T> : DomainRepository<T>, IAppRepository<T> where T : AppCoreDomain
    {
        public AppRepository(IAppDbContext context) : base(context)
        {

        }

    }
}
