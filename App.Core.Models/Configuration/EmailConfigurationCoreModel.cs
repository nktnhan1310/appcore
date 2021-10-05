using App.Core.Models.DomainModel;
using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Models
{
    /// <summary>
    /// Cấu hình email
    /// </summary>
    public class EmailConfigurationCoreModel : AppCoreDomainModel
    {
        /// <summary>
        /// SMTP Server
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập SMTP Server!")]
        [MaxLength(1000, ErrorMessage = "Giá trị smtp server phải nhỏ hơn 1000 kí tự!")]
        public string SmtpServer { set; get; }
        /// <summary>
        /// Port 
        /// </summary>
        [Required]
        public int Port { set; get; }
        /// <summary>
        /// Cờ mở SSL
        /// </summary>
        [Required]
        public bool EnableSsl { set; get; }
        /// <summary>
        /// Loại connect
        /// </summary>
        [Required]
        public int ConnectType { set; get; }
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị!")]
        [MaxLength(1000, ErrorMessage = "Tên hiển thị phải nhỏ hơn 1000 kí tự")]
        public string DisplayName { set; get; }
        /// <summary>
        /// Tên đăng nhập cấu hình
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập UserName!")]
        [MaxLength(1000, ErrorMessage = "Tên hiển thị phải nhỏ hơn 1000 kí tự")]
        public string UserName { set; get; }
        [MaxLength(1000)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [StringLength(128, ErrorMessage = "Mật khẩu phải ít nhất 8 kí tự", MinimumLength = 8)]
        public string Password { set; get; }
        public int ItemSendCount { get; set; }
        public int TimeSend { get; set; }

        public void EncryptPassword()
        {
            Password = StringCipher.Encrypt(Password, StringCipher.PassPhrase);
        }
    }
}
