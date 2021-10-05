using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Entities
{
    public class BaseSearchUser : BaseSearch
    {
        /// <summary>
        /// Tìm kiếm theo Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Tìm kiếm theo số điện thoại
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Theo nhom người dùng
        /// </summary>
        public int? UserGroupId { get; set; }
    }
}
