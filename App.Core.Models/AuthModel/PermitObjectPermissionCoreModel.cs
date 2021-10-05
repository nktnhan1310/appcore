using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Models
{
    /// <summary>
    /// Danh mục quyền ứng với chức năng người dùng
    /// </summary>
    public class PermitObjectPermissionCoreModel : AppCoreDomainModel
    {
        /// <summary>
        /// Chức năng
        /// </summary>
        public int PermitObjectId { get; set; }
        /// <summary>
        /// Quyền
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int? UserGroupId { get; set; }
        /// <summary>
        /// Người dùng
        /// </summary>
        public int? UserId { get; set; }
    }
}
