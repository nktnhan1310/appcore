using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Entities.Configuration
{
    /// <summary>
    /// Bảng cấu hình Email
    /// </summary>
    public class EmailConfigurationCores : AppCoreDomain
    {
        [Required]
        [MaxLength(1000)]
        public string SmtpServer { set; get; }
        [Required]
        public int Port { set; get; }
        [Required]
        public bool EnableSsl { set; get; }
        [Required]
        public int ConnectType { set; get; }
        [Required]
        [MaxLength(1000)]
        public string DisplayName { set; get; }
        [Required]
        [MaxLength(1000)]
        public string UserName { set; get; }
        [MaxLength(1000)]
        public string Email { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Password { set; get; }
        public int ItemSendCount { get; set; }
        public int TimeSend { get; set; }
    }
}
