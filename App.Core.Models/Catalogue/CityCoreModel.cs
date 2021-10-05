using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace App.Core.Models.Catalogue
{
    public class CityCoreModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Tên thành phố
        /// </summary>
        public string CountryName { get; set; }
    }
}
