using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AuthModel
{
    public class RoleModel
    {
        /// <summary>
        /// Tên chức năng (menu)
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Quyền của chức năng
        /// </summary>
        public bool IsView { get; set; }
    }
}
