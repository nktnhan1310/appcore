using App.Core.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace App.Core.Models.AuthModel.RequestModel
{
    public class RequestUserCoreModel : RequestCoreModel
    {
        [Required(ErrorMessage = "Vui lòng nhập User Name!")]
        public string UserName { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(100, ErrorMessage = "Số kí tự của tên phải nhỏ hơn 100!")]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(100, ErrorMessage = "Số kí tự của họ phải nhỏ hơn 100!")]
        public string LastName { get; set; }
        [StringLength(12, ErrorMessage = "Số kí tự của số điện thoại phải lớn hơn 8 và nhỏ hơn 12!", MinimumLength = 9)]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9]+${9,11}", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [StringLength(50, ErrorMessage = "Số kí tự của email phải nhỏ hơn 50!")]
        [Required(ErrorMessage = "Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "Email có định dạng không hợp lệ!")]
        public string Email { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000, ErrorMessage = "Số kí tự của email phải nhỏ hơn 1000!")]
        public string Address { get; set; }
        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Tuổi
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// Phải là admin không
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        [StringLength(255, ErrorMessage = "Mật khẩu phải lớn hơn 8 kí tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Họ tên người dùng
        /// </summary>
        public string UserFullName { get; set; }

        /// <summary>
        /// Số lần vi phạm
        /// </summary>
        public int? TotalViolations { get; set; }

        /// <summary>
        /// Cờ khóa tài khoản
        /// </summary>
        [DefaultValue(false)]
        public bool IsLocked { get; set; }

        /// <summary>
        /// Khóa đến ngày
        /// </summary>
        public DateTime? LockedDate { get; set; }
        
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime? BirthDate { get; set; }

        #region Extension Properties

        /// <summary>
        /// Có reset mật khẩu không
        /// </summary>
        [DefaultValue(false)]
        public bool IsResetPassword { get; set; }

        /// <summary>
        /// Mật khẩu cũ
        /// </summary>
        [StringLength(255, ErrorMessage = "Mật khẩu phải lớn hơn 8 kí tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không giống mật khẩu cũ")]
        public string ConfirmPassWord { get; set; }

        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(255, ErrorMessage = "Mật khẩu phải lớn hơn 8 kí tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassWord { get; set; }


        /// <summary>
        /// Mật khẩu mới
        /// </summary>
        [StringLength(255, ErrorMessage = "Mật khẩu phải lớn hơn 8 kí tự", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("NewPassWord", ErrorMessage = "Mật khẩu xác nhận không giống mật khẩu mới")]
        public string ConfirmNewPassWord { get; set; }

        /// <summary>
        /// Giới tính
        /// 0 => Nữ
        /// 1 => Nam
        /// </summary>
        [DefaultValue(false)]
        public bool Gender { get; set; }

        /// <summary>
        /// List id nhóm người dùng được chọn
        /// </summary>
        public List<int> UserGroupIds { get; set; }
        /// <summary>
        /// Hoa hồng
        /// </summary>
        public int? PercentCommission { get; set; }

        /// <summary>
        /// Danh mục quyền ứng với chức năng người dùng
        /// </summary>
        public IList<RequestPermitObjectPermissionCoreModel> PermitObjectPermissions { get; set; }


        /// <summary>
        /// List file của người dùng
        /// </summary>
        public IList<RequestCoreFileModel> UserFiles { get; set; }


        #endregion
    }
}
