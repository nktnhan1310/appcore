using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc nhập")]
        public string UserName { set; get; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc nhập")]
        public string Password { set; get; }

        /// <summary>
        /// Mã OTP nêu đăng nhập bằng sdt
        /// </summary>
        //public string OTPValue { get; set; }
    }
}
