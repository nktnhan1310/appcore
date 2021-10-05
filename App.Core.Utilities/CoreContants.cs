using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Utilities
{
    public class CoreContants
    {
        public const string ViewAll = "ViewAll";
        public const string Delete = "Delete";
        public const string AddNew = "AddNew";
        public const string View = "View";
        public const string FullControl = "FullControl";
        public const string Update = "Update";
        public const string Approve = "Approve";
        public const string Import = "Import";
        public const string Upload = "Upload";
        public const string Download = "Download";
        public const string DeleteFile = "DeleteFile";
        public const string Export = "Export";

        public const string USER_GROUP = "USER";
        public const string ADMIN_GROUP = "ADMIN";

        public const string UPLOAD_FOLDER_NAME = "upload";
        public const string TEMP_FOLDER_NAME = "Temp";
        public const string QR_CODE_FOLDER_NAME = "qrcode";
        public const string USER_FOLDER_NAME = "user";

        public const string GET_TOTAL_NOTIFICATION = "get-total-notification";

        /// <summary>
        /// Danh mục quyền
        /// </summary>
        public enum PermissionContants
        {
            ViewAll = 1,
            View = 2,
            AddNew = 3,
            Update = 4,
            Delete = 5,
            Import = 6,
            Upload = 7,
            Download = 8,
            Export = 9
        }

        #region Catalogue Name

        /// <summary>
        /// Phường
        /// </summary>
        public const string WARD_CATALOGUE_NAME = "Ward";

        /// <summary>
        /// QUÔC GIA
        /// </summary>
        public const string COUNTRY_CATALOGUE_NAME = "Country";

        /// <summary>
        /// QUẬN
        /// </summary>
        public const string DISTRICT_CATALOGUE_NAME = "District";

        /// <summary>
        /// THÀNH PHỐ
        /// </summary>
        public const string CITY_CATALOGUE_NAME = "City";

        /// <summary>
        /// DÂN TỘC
        /// </summary>
        public const string NATION_CATALOGUE_NAME = "Nation";

        /// <summary>
        /// CHUYÊN KHOA
        /// </summary>
        public const string SPECIALIST_TYPE_CATALOGUE_NAME = "SpecialistType";

        /// <summary>
        /// PHÒNG KHÁM CỦA BỆNH VIỆN
        /// </summary>
        public const string ROOM_EXAMINATION_CATALOGUE_NAME = "RoomExamination";

        /// <summary>
        /// Bác sĩ của bệnh viện
        /// </summary>
        public const string DOCTOR_CATALOGUE_NAME = "Doctor";

        /// <summary>
        /// CÔNG VIỆC
        /// </summary>
        public const string JOB_CATALOGUE_NAME = "Job";

        /// <summary>
        /// LOẠI DỊCH VỤ
        /// </summary>
        public const string SERVICE_TYPE_CATALOGUE_NAME = "ServiceType";

        /// <summary>
        /// Buổi khám bệnh
        /// </summary>
        public const string SESSION_TYPE_CATALOGUE_NAME = "SessionType";

        /// <summary>
        /// Bệnh viện
        /// </summary>
        public const string HOSPITAL_CATALOGUE_NAME = "Hospital";

        /// <summary>
        /// Kênh đăng ký
        /// </summary>
        public const string CHANNEL_CATALOGUE_NAME = "Channel";

        /// <summary>
        /// Chức vị của bác sĩ
        /// </summary>
        public const string DEGREE_TYPE_CATALOGUE_NAME = "DegreeType";

        /// <summary>
        /// Loại thông báo
        /// </summary>
        public const string NOTIFICATION_TYPE_CATALOGUE_NAME = "NotificationType";

        /// <summary>
        /// Dịch vụ phát sinh
        /// </summary>
        public const string ADDITION_SERVICE_TYPE_CATALOGUE_NAME = "AdditionService";

        #endregion

        #region SMS Template

        /// <summary>
        /// Xác nhận OTP SMS
        /// </summary>
        public const string SMS_XNOTP = "XNOTP";

        #endregion

        #region EmailTemplate


        #endregion

    }
}
