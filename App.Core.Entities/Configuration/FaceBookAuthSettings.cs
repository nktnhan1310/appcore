using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities.Configuration
{
    public class FaceBookAuthSettings : AppCoreDomain
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
