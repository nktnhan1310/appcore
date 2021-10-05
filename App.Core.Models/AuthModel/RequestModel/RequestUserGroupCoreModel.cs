using App.Core.Models.AuthModel.RequestModel;
using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AuthModel
{
    public class RequestUserGroupCoreModel : RequestCoreCatalogueModel
    {
        #region Extension Properties

        /// <summary>
        /// List id user của nhóm
        /// </summary>
        public List<int> UserIds { get; set; }

        ///// <summary>
        ///// Người dùng thuộc nhóm
        ///// </summary>
        //public IList<UserInGroupModel> UserInGroups { get; set; }

        /// <summary>
        /// Chức năng + quyền của nhóm
        /// </summary>
        public IList<RequestPermitObjectPermissionCoreModel> PermitObjectPermissions { get; set; }

        #endregion
    }
}
