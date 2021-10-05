using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AuthModel.RequestModel
{
    public class RequestPermitObjectCoreModel : RequestCoreCatalogueModel
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
