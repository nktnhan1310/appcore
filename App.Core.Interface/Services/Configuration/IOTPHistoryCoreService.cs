using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interface
{
    public interface IOTPHistoryCoreService : IDomainService<OTPHistoryCores, BaseSearch>
    {
    }
}
