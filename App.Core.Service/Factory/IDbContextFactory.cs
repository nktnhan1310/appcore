using App.Core.Interface.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Service.Factory
{
    public interface IDbContextFactory
    {
        string ConnectionString { get; set; }
        ICoreDbContext Create();
    }
}
