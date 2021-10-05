using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AuthModel.RequestModel
{
    public class RequestPermitObjectPermissionCoreModel : RequestCoreModel
    {
        /// <summary>
        /// Mã chức năng
        /// </summary>
        public int? PermitObjectId { get; set; }
        /// <summary>
        /// Mã quyền
        /// </summary>
        public int? PermissionId { get; set; }
        /// <summary>
        /// Mã nhóm
        /// </summary>
        public int? UserGroupId { get; set; }
        /// <summary>
        /// Mã người dùng
        /// </summary>
        public int? UserId { get; set; }
    }
}
