using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Entities
{
    /// <summary>
    /// Phường
    /// </summary>
    public class WardCores : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Mã quận
        /// </summary>
        public int? DistrictId { get; set; }

        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CityName { get; set; }

        /// <summary>
        /// Tên quận trực thuộc
        /// </summary>
        [StringLength(1000)]
        public string DistrictName { get; set; }
    }
}
