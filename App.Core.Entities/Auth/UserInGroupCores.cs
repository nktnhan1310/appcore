using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace App.Core.Entities
{
    /// <summary>
    /// Người dùng thuộc nhóm
    /// </summary>
    public class UserInGroupCores : AppCoreDomain
    {
        /// <summary>
        /// Người dùng
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int UserGroupId { get; set; }


        #region Extension Properties

        /// <summary>
        /// Lấy thông tin Người dùng
        /// </summary>
        [NotMapped]
        public UserCores Users { get; set; }

        /// <summary>
        /// Lấy thông tin Nhóm người dùng
        /// </summary>
        [NotMapped]
        public UserGroupCores UserGroups { get; set; }

        #endregion


    }
}
