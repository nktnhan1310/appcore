using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities
{
    public class BaseSearchUserInGroup : BaseSearch
    {
        /// <summary>
        /// Người
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int? UserGroupId { get; set; }
    }
}
