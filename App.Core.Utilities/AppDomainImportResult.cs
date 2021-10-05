using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Utilities
{
    public class AppDomainImportResult : AppDomainResult
    {
        /// <summary>
        /// File kết quả trả về
        /// </summary>
        public byte[] ResultFile { get; set; }
        /// <summary>
        /// Tên file kết quả
        /// </summary>
        public string DownloadFileName { get; set; }
    }
}
