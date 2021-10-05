using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Models.DomainModel
{
    public class AppCoreDomainFileModel : AppCoreDomainModel
    {
        /// <summary>
        /// Tên file
        /// </summary>
        [StringLength(500, ErrorMessage = "Tên file không được dài quá 500 kí tự")]
        public string FileName { get; set; }
        /// <summary>
        /// Loại file
        /// </summary>
        [StringLength(100, ErrorMessage = "Tên loại file không được dài quá 100 kí tự")]
        public string ContentType { get; set; }
        /// <summary>
        /// Đuôi file
        /// </summary>
        [StringLength(50, ErrorMessage = "Tên đuôi file không được dài quá 50 kí tự")]
        public string FileExtension { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        [StringLength(1000, ErrorMessage = "Mô tả không được dài quá 1000 kí tự")]
        public string Description { get; set; }

        /// <summary>
        /// Tên lưu trong thư mục
        /// </summary>
        public string FileRandomName { get; set; }

        /// <summary>
        /// Link download File
        /// </summary>
        public string FileUrl { get; set; }

    }
}
