using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Utilities
{
    public class CatalogueUtilities
    {
        /// <summary>
        /// Loại thông báo
        /// </summary>
        public enum NotificationType
        {
            /// <summary>
            /// Hệ thống
            /// </summary>
            SYS = 0,
            /// <summary>
            /// Bệnh viện
            /// </summary>
            HOS = 1,
            /// <summary>
            /// Người dùng
            /// </summary>
            USER = 2,
            /// <summary>
            /// Nhóm người dùng
            /// </summary>
            USERGROUP = 3,
        }

    }
}
