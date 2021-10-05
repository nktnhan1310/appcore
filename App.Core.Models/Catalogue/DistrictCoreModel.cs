using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace App.Core.Models.Catalogue
{
    public class DistrictCoreModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CityName { get; set; }
    }
}
