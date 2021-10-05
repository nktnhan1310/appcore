using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace App.Core.Entities
{
    /// <summary>
    /// Chức năng người dùng
    /// </summary>
    public class PermitObjectCores : AppCoreCatalogueDomain
    {
        /// <summary>
        /// Tên controller
        /// </summary>
        public string ControllerNames { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách tên controller
        /// </summary>
        [NotMapped]
        public IList<string> Controllers
        {
            get
            {
                return (!string.IsNullOrEmpty(ControllerNames)) ? ControllerNames.Split(';').ToList() : new List<string>();
            }
        }

        #endregion
    }
}
