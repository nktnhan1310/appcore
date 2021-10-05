using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace App.Core.Models.DomainModel
{
    public class RequestCoreModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Cờ check xóa
        /// </summary>
        [DefaultValue(false)]
        public bool Deleted { get; set; }

        /// <summary>
        /// Cờ active
        /// </summary>
        [DefaultValue(true)]
        public bool Active { get; set; }
    }
}
