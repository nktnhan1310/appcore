using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Models
{
    /// <summary>
    /// Chức năng người dùng
    /// </summary>
    public class PermitObjectCoreModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Tên controller
        /// </summary>
        public string ControllerNames { get; set; }

        #region Extension Properties

        /// <summary>
        /// Danh sách tên controller
        /// </summary>
        public IList<string> Controllers { get; set; }


        public void ToModel()
        {
            ControllerNames = string.Join(";", Controllers);
        }

        public void ToView()
        {
            if (!string.IsNullOrEmpty(ControllerNames))
            {
                Controllers = ControllerNames.Split(";");
            }
        }


        #endregion

    }
}
