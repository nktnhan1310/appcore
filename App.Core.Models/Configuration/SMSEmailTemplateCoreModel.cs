using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace App.Core.Models
{
    /// <summary>
    /// Bảng câu hình template gửi đi
    /// </summary>
    public class SMSEmailTemplateCoreModel : AppCoreCatalogueDomainModel
    {
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Mẫu là SMS
        /// </summary>
        [DefaultValue(false)]
        public bool IsSMS { get; set; }
    }
}
