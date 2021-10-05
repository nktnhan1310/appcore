using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Entities
{
    /// <summary>
    /// Quận
    /// </summary>
    public class DistrictCores : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CityName { get; set; }
    }
}
