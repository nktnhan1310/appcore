using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace App.Core.Entities
{
    /// <summary>
    /// Thành phố
    /// </summary>
    public class CityCores : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        [StringLength(1000)]
        public string CountryName { get; set; }
    }
}
