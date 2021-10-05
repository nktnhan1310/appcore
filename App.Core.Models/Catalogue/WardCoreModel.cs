using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace App.Core.Models.Catalogue
{
    public class WardCoreModel : AppCoreCatalogueDomainModel 
    {
        /// <summary>
        /// Quận
        /// </summary>
        public int? DistrictId { get; set; }

        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Tên quận trực thuộc
        /// </summary>
        public string DistrictName { get; set; }
    }
}
