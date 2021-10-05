using App.Core.Entities.DomainEntity;
using App.Core.Extensions;
using App.Core.Interface.Services.Base;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.Core.Controllers
{
    [ApiController]
    public abstract class BaseCatalogueController<E, T, R, DomainSearch> : BaseController<E, T, R, DomainSearch> where E : AppCoreCatalogueDomain where T : AppCoreCatalogueDomainModel where R : RequestCoreCatalogueModel where DomainSearch : BaseSearch, new()
    {
        protected ICatalogueService<E, DomainSearch> catalogueService;

        public BaseCatalogueController(IServiceProvider serviceProvider, ILogger<BaseController<E, T, R, DomainSearch>> logger, IWebHostEnvironment env) : base(serviceProvider, logger, env)
        {
        }

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AppAuthorize(new string[] { CoreContants.View })]
        public override async Task<AppDomainResult> GetById(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            var item = await this.catalogueService.GetByIdAsync(id);
            if (item != null)
            {
                var itemModel = mapper.Map<T>(item);
                appDomainResult = new AppDomainResult()
                {
                    Success = true,
                    Data = itemModel,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new KeyNotFoundException("Item không tồn tại");

            return appDomainResult;
        }

        /// <summary>
        /// Thêm mới item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AppAuthorize(new string[] { CoreContants.AddNew })]
        public override async Task<AppDomainResult> AddItem([FromBody] R itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<E>(itemModel);
                item.Created = DateTime.UtcNow.AddHours(7);
                item.CreatedBy = LoginContext.Instance.CurrentUser.UserName;
                item.Active = true;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.CreateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");
                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");

            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Cập nhật thông tin item
        /// </summary>
        /// <param name="itemModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [AppAuthorize(new string[] { CoreContants.Update })]
        public override async Task<AppDomainResult> UpdateItem([FromBody] R itemModel)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = false;
            if (ModelState.IsValid)
            {
                var item = mapper.Map<E>(itemModel);
                item.Updated = DateTime.UtcNow.AddHours(7);
                item.UpdatedBy = LoginContext.Instance.CurrentUser.UserName;
                if (item != null)
                {
                    // Kiểm tra item có tồn tại chưa?
                    var messageUserCheck = await this.catalogueService.GetExistItemMessage(item);
                    if (!string.IsNullOrEmpty(messageUserCheck))
                        throw new AppException(messageUserCheck);
                    success = await this.catalogueService.UpdateAsync(item);
                    if (success)
                        appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                    else
                        throw new Exception("Lỗi trong quá trình xử lý");

                    appDomainResult.Success = success;
                }
                else
                    throw new KeyNotFoundException("Item không tồn tại");
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Xóa item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AppAuthorize(new string[] { CoreContants.Delete })]
        public override async Task<AppDomainResult> DeleteItem(int id)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            bool success = await this.catalogueService.DeleteAsync(id);
            if (success)
            {
                appDomainResult.ResultCode = (int)HttpStatusCode.OK;
                appDomainResult.Success = success;
            }
            else
                throw new Exception("Lỗi trong quá trình xử lý");
            return appDomainResult;
        }

        /// <summary>
        /// Lấy danh sách item phân trang
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        [HttpGet]
        [AppAuthorize(new string[] { CoreContants.ViewAll })]
        public override async Task<AppDomainResult> Get([FromQuery] DomainSearch baseSearch)
        {
            AppDomainResult appDomainResult = new AppDomainResult();

            if (ModelState.IsValid)
            {
                PagedList<E> pagedData = await this.catalogueService.GetPagedListData(baseSearch);
                PagedList<T> pagedDataModel = mapper.Map<PagedList<T>>(pagedData);
                appDomainResult = new AppDomainResult
                {
                    Data = pagedDataModel,
                    Success = true,
                    ResultCode = (int)HttpStatusCode.OK
                };
            }
            else
                throw new AppException(ModelState.GetErrorMessage());

            return appDomainResult;
        }

        /// <summary>
        /// Down load template file import
        /// </summary>
        /// <returns></returns>
        [HttpGet("download-template-import")]
        [AppAuthorize(new string[] { CoreContants.Download })]
        public virtual async Task<ActionResult> DownloadTemplateImport(string fileName)
        {
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(currentDirectory, TEMPLATE_FOLDER_NAME, CATALOGUE_TEMPLATE_NAME);
            if (!System.IO.File.Exists(path))
                throw new AppException("File template không tồn tại!");
            var file = await System.IO.File.ReadAllBytesAsync(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TemplateImport.xlsx");
        }

        /// <summary>
        /// Tải về file kết quả sau khi import
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("download-import-result-file/{fileName}")]
        [AppAuthorize(new string[] { CoreContants.Download })]
        public virtual async Task<ActionResult> DownloadImportFileResult(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name không tồn tại!");
            if (env == null)
                throw new Exception("IHostingEnvironment is null => inject to constructor");
            var webRoot = env.ContentRootPath;
            string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
            var file = await System.IO.File.ReadAllBytesAsync(path);
            // Xóa file thư mục temp
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("KetQuaCD-{0:dd-MM-yyyy_HH_mm_ss}{1}", DateTime.UtcNow.AddHours(7), Path.GetExtension(fileName)));
        }

        /// <summary>
        /// Import file danh mục
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("import-template-file")]
        [AppAuthorize(new string[] { CoreContants.Import })]
        public virtual async Task<AppDomainImportResult> ImportTemplateFile(IFormFile file)
        {
            AppDomainImportResult appDomainImportResult = new AppDomainImportResult();
            var fileStream = file.OpenReadStream();
            appDomainImportResult = await this.catalogueService.ImportTemplateFile(fileStream, LoginContext.Instance.CurrentUser.UserName);
            if (appDomainImportResult.ResultFile != null)
            {
                var webRoot = env.ContentRootPath;
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string path = Path.Combine(webRoot, UPLOAD_FOLDER_NAME, TEMP_FOLDER_NAME, fileName);
                FileUtils.CreateDirectory(Path.Combine(webRoot, TEMP_FOLDER_NAME));
                FileUtils.SaveToPath(path, appDomainImportResult.ResultFile);
                appDomainImportResult.ResultFile = null;
                appDomainImportResult.DownloadFileName = fileName;
            }
            return appDomainImportResult;
        }


        #region Contants

        public const string TEMPLATE_FOLDER_NAME = "Template";
        public const string CATALOGUE_TEMPLATE_NAME = "CatalogueTemplate.xlsx";


        #endregion

    }
}
