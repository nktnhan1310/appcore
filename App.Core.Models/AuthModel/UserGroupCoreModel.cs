using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Models
{
    /// <summary>
    /// Nhóm người dùng
    /// </summary>
    public class UserGroupCoreModel : AppCoreCatalogueDomainModel
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
        public IList<PermitObjectPermissionCoreModel> PermitObjectPermissions { get; set; }

        #endregion
    }
}
