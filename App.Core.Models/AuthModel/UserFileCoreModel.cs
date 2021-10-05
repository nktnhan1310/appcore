using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AuthModel
{
    public class UserFileCoreModel : AppCoreDomainFileModel
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }
    }
}
