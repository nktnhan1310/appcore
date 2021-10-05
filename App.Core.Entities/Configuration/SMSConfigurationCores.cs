using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;
namespace App.Core.Entities.Configuration
{
    /// <summary>
    /// Cấu hình SMS
    /// </summary>
    public class SMSConfigurationCores : AppCoreDomain 
    {
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
        public string BrandName { get; set; }
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
