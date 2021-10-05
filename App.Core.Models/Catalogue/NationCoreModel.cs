using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.Catalogue
{
    public class NationCoreModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }
    }
}
