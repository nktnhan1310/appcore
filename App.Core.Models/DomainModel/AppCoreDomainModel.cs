using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace App.Core.Models.DomainModel
{
    public class AppCoreDomainModel
    {
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public long RowNumber { get; set; }
        /// <summary>
        /// Khóa chính
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// Tạo bởi
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime? Updated { get; set; }
        /// <summary>
        /// Người cập nhật
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Cờ xóa
        /// </summary>
        [DefaultValue(false)]
        public bool Deleted { get; set; }
        /// <summary>
        /// Cờ active
        /// </summary>
        [DefaultValue(true)]
        public bool Active { get; set; }
    }
}
