using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities.Configuration
{
    public class GoogleSettings : AppCoreDomain
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
