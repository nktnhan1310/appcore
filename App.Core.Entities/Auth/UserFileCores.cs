using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities
{
    public class UserFileCores : AppCoreDomainFile
    {
        /// <summary>
        /// Loại file
        /// </summary>
        public int? TypeId { get; set; }

        /// <summary>
        /// Id của user
        /// </summary>
        public int? UserId { get; set; }
    }
}
