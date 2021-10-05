using App.Core.Entities;
using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Interface.Services.Configuration
{
    public interface ISMSEmailTemplateCoreService : ICatalogueService<SMSEmailTemplateCores, BaseSearch>
    {
    }
}
