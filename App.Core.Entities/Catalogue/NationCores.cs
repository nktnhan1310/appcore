using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;
namespace App.Core.Entities
{
    public class NationCores : AppCoreCatalogueDomain 
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }
    }
}
