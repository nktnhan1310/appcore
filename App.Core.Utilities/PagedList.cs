using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Utilities
{
    public class PagedList<T> where T : class
    {
        public PagedList()
        {
        }
        public int PageIndex { set; get; }
        public int PageSize { set; get; }
        public int TotalPage
        {
            get
            {
                decimal count = this.TotalItem;
                if (count > 0)
                    return (int)Math.Ceiling(count / PageSize);
                else return 0;
            }
        }
        public int TotalItem { set; get; }
        public IList<T> Items { set; get; }
    }

    public class PagedListReport<T> where T: class
    {
        public PagedListReport()
        {
        }
        public int PageIndex { set; get; }
        public int PageSize { set; get; }
        public int TotalPage
        {
            get
            {
                decimal count = this.TotalItem;
                if (count > 0)
                    return (int)Math.Ceiling(count / PageSize);
                else return 0;
            }
        }
        public int TotalItem { set; get; }
        



        public IList<T> Items { set; get; }

        #region Extension Properties

        /// <summary>
        /// Giá trị cho báo cáo doanh thu (REPORT REVENUE)
        /// </summary>
        public double? TotalRevenueValue { get; set; }

        //----------------------------------------------- REPORT EXAMINATION FORM
        /// <summary>
        /// Tổng lịch hẹn mới
        /// </summary>
        public int TotalNewForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn chờ xác nhận
        /// </summary>
        public int TotalWaitConfirmForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn đã xác nhận
        /// </summary>
        public int TotalConfirmedForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn hủy
        /// </summary>
        public int TotalCanceledForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn chờ xác nhận tái khám
        /// </summary>
        public int TotalWaitReExaminationForm { get; set; }
        /// <summary>
        /// Tổng lịch hẹn đã xác nhận tái khám
        /// </summary>
        public int TotalConfirmedReExaminationForm { get; set; }

        //------------------------------------------- REPORT USER EXAMINATION FORM
        public int TotalUserExamination { get; set; }

        #endregion
    }

}
