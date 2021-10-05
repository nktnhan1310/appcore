using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Models
{
    public class SMSConfiguartionCoreModel : AppCoreDomainModel
    {
        [Required(ErrorMessage = "Vui lòng nhập API Key")]
        [DataType(DataType.Password)]
        public string APIKey { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Secret Key")]
        [DataType(DataType.Password)]
        public string SecretKey { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Brand name")]
        public string BrandName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Loại SMS")]
        public int SMSType { get; set; }
        /// <summary>
        /// Cú pháp tin nhắn mẫu
        /// </summary>
        public string TemplateText { get; set; }
        /// <summary>
        /// Url web service
        /// </summary>
        public string WebServiceUrl { get; set; }
    }
}
