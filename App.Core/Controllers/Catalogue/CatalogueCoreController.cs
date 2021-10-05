using App.Core.Interface.Services.Catalogue;
using App.Core.Models.Catalogue;
using App.Core.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.Core.Controllers.Catalogue
{
    [ApiController]
    public abstract class CatalogueCoreController : ControllerBase
    {
        #region Catalogue

        protected IWardCoreService wardService;
        protected IDistrictCoreService districtService;
        protected ICountryCoreService countryService;
        protected INationCoreService nationService;
        protected ICityCoreService cityService;

        #endregion

        #region Configuration

        protected IMapper mapper;
        protected IConfiguration configuration;

        #endregion

        public CatalogueCoreController(IServiceProvider serviceProvider, IMapper mapper, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.configuration = configuration;

            wardService = serviceProvider.GetRequiredService<IWardCoreService>();
            districtService = serviceProvider.GetRequiredService<IDistrictCoreService>();
            cityService = serviceProvider.GetRequiredService<ICityCoreService>();
            countryService = serviceProvider.GetRequiredService<ICountryCoreService>();
            nationService = serviceProvider.GetRequiredService<INationCoreService>();
        }

        /// <summary>
        /// Lấy danh sách quốc gia
        /// </summary>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-country-catalogue")]
        public async Task<AppDomainResult> GetCountryCatalogue(string searchContent)
        {
            var countries = await this.countryService.GetAsync(e => !e.Deleted && e.Active
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<CountryCoreModel>>(countries),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách thành phố
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-city-catalogue/countryId")]
        public async Task<AppDomainResult> GetCityCatalogue(int? countryId, string searchContent)
        {
            var cities = await this.cityService.GetAsync(e => !e.Deleted && e.Active
            && (!countryId.HasValue || e.CountryId == countryId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<CityCoreModel>>(cities),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách quận
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-district-catalogue/cityId")]
        public async Task<AppDomainResult> GetDistrictCatalogue(int? cityId, string searchContent)
        {
            var districts = await this.districtService.GetAsync(e => !e.Deleted && e.Active
            && (!cityId.HasValue || e.CityId == cityId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<DistrictCoreModel>>(districts),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách phường
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="districtid"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-ward-catalogue/cityId/districtId")]
        public async Task<AppDomainResult> GetWardCatalogue(int? cityId, int? districtid, string searchContent)
        {
            var wards = await this.wardService.GetAsync(e => !e.Deleted && e.Active
            && (!cityId.HasValue || e.CityId == cityId.Value)
            && (!districtid.HasValue || e.DistrictId == districtid.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<WardCoreModel>>(wards),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Lấy danh sách dân tộc
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="searchContent"></param>
        /// <returns></returns>
        [HttpGet("get-nation-catalogue/countryId")]
        public async Task<AppDomainResult> GetNationCatalogue(int? countryId, string searchContent)
        {
            var nations = await this.nationService.GetAsync(e => !e.Deleted && e.Active
            && (!countryId.HasValue || e.CountryId == countryId.Value)
            && (string.IsNullOrEmpty(searchContent) ||
            (e.Code.Contains(searchContent)
            || e.Name.Contains(searchContent)
            ))
            );
            return new AppDomainResult()
            {
                Data = mapper.Map<IList<NationCoreModel>>(nations),
                Success = true,
                ResultCode = (int)HttpStatusCode.OK
            };
        }

    }
}
