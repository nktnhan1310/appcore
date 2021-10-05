using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace App.Core.Entities
{
    [Table("Users")]
    public class UserCores : AppCoreDomain
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; }
        /// <summary>
        /// Họ
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// Địa chỉ
        /// </summary>
        [StringLength(1000)]
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
        [StringLength(4000)]
        public string Password { get; set; }

        /// <summary>
        /// Token đăng nhập
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Thời gian hết hạn token
        /// </summary>
        public DateTime? ExpiredDate { get; set; }

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
        /// Giới tính
        /// 0 => Nữ
        /// 1 => Nam
        /// </summary>
        public bool? Gender { get; set; }

        /// <summary>
        /// Cờ kiểm tra OTP của user
        /// </summary>
        [DefaultValue(false)]
        public bool IsCheckOTP { get; set; }

        /// <summary>
        /// Cờ check login = facebook
        /// </summary>
        [DefaultValue(false)]
        public bool IsLoginFaceBook { get; set; }
        /// <summary>
        /// Cờ check login = google
        /// </summary>
        [DefaultValue(false)]
        public bool IsLoginGoogle { get; set; }

        /// <summary>
        /// Mã quốc gia
        /// </summary>
        public int? CountryId { get; set; }
        /// <summary>
        /// Mã thành phố
        /// </summary>
        public int? CityId { get; set; }
        /// <summary>
        /// Mã quận
        /// </summary>
        public int? DistrictId { get; set; }
        /// <summary>
        /// Mã phường
        /// </summary>
        public int? WardId { get; set; }

        /// <summary>
        /// Mã dân tộc
        /// </summary>
        public int? NationId { get; set; }

        /// <summary>
        /// Mã công việc nếu có
        /// </summary>
        public string JobDescription { get; set; }

        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Hoa hồng
        /// </summary>
        public int? PercentCommission { get; set; }

        #region Extension Properties

        /// <summary>
        /// Cờ xét reset mật khẩu
        /// </summary>
        [NotMapped]
        public bool IsResetPassword { get; set; }

        /// <summary>
        /// List id nhóm người dùng được chọn
        /// </summary>
        [NotMapped]
        public List<int> UserGroupIds { get; set; }

        ///// <summary>
        ///// Những nhóm người dùng thuộc
        ///// </summary>
        //[NotMapped]
        //public IList<UserInGroups> UserInGroups { get; set; }

        /// <summary>
        /// Danh mục quyền ứng với chức năng người dùng
        /// </summary>
        [NotMapped]
        public IList<PermitObjectPermissionCores> PermitObjectPermissions { get; set; }

        /// <summary>
        /// List file của người dùng
        /// </summary>
        [NotMapped]
        public IList<UserFileCores> UserFiles { get; set; }


        #endregion

    }
}
