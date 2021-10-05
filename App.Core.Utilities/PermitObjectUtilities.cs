using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Utilities
{
    public class PermitObjectUtilities
    {
        public enum Roles
        {
            /// <summary>
            /// Chức năng người dùng
            /// </summary>
            User = 0,
            /// <summary>
            /// Chức năng quản lý bệnh viện
            /// </summary>
            Hospital = 1,
            /// <summary>
            /// Chức năng quản lý bác sĩ
            /// </summary>
            Doctor = 2,

        }
    }
}
