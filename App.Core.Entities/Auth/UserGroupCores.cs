using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace App.Core.Entities
{
    /// <summary>
    /// Nhóm người dùng
    /// </summary>
    public class UserGroupCores : AppCoreCatalogueDomain
    {

        #region Extension Properties

        [NotMapped]
        public List<int> UserIds { get; set; }

        ///// <summary>
        ///// Người dùng thuộc nhóm
        ///// </summary>
        //[NotMapped]
        //public IList<UserInGroups> UserInGroups { get; set; }

        /// <summary>
        /// Chức năng + quyền của nhóm
        /// </summary>
        [NotMapped]
        public IList<PermitObjectPermissionCores> PermitObjectPermissions { get; set; }

        #endregion
    }
}
