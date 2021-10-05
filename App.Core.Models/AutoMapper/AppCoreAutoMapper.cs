using App.Core.Entities;
using App.Core.Entities.DomainEntity;
using App.Core.Models.AuthModel;
using App.Core.Models.AuthModel.RequestModel;
using App.Core.Models.Catalogue;
using App.Core.Models.DomainModel;
using App.Core.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.Models.AutoMapper
{

    public class AppCoreAutoMapper : Profile
    {
        public AppCoreAutoMapper()
        {
            CreateMap<UserCoreModel, UserCores>().ReverseMap();
            CreateMap<UserCores, RequestUserCoreModel>().ReverseMap();
            CreateMap<PagedList<UserCoreModel>, PagedList<UserCores>>().ReverseMap();

            CreateMap<UserGroupCoreModel, UserGroupCores>().ReverseMap();
            CreateMap<UserGroupCores, RequestUserGroupCoreModel>().ReverseMap();
            CreateMap<PagedList<UserGroupCoreModel>, PagedList<UserGroupCores>>().ReverseMap();

            CreateMap<UserInGroupCoreModel, UserInGroupCores>().ReverseMap();
            CreateMap<PagedList<UserInGroupCoreModel>, PagedList<UserInGroupCores>>().ReverseMap();

            CreateMap<PermissionCoreModel, PermissionCores>().ReverseMap();
            CreateMap<PermissionCores, RequestCoreCatalogueModel>().ReverseMap();
            CreateMap<PagedList<PermissionCoreModel>, PagedList<PermissionCores>>().ReverseMap();

            CreateMap<PermitObjectCoreModel, PermitObjectCores>().ReverseMap();
            CreateMap<PermitObjectCores, RequestPermitObjectCoreModel>().ReverseMap();
            CreateMap<PagedList<PermitObjectCoreModel>, PagedList<PermitObjectCores>>().ReverseMap();

            CreateMap<PermitObjectPermissionCoreModel, PermitObjectPermissionCores>().ReverseMap();
            CreateMap<PermitObjectPermissionCores, RequestPermitObjectPermissionCoreModel>().ReverseMap();
            CreateMap<PagedList<PermitObjectPermissionCoreModel>, PagedList<PermitObjectPermissionCores>>().ReverseMap();

            CreateMap<UserFileCoreModel, UserFileCores>().ReverseMap();
            CreateMap<PagedList<UserFileCoreModel>, PagedList<UserFileCores>>().ReverseMap();

            #region Catalogue

            CreateMap<CountryCoreModel, CountryCores>().ReverseMap();
            CreateMap<PagedList<CountryCoreModel>, PagedList<CountryCores>>().ReverseMap();

            CreateMap<NationCoreModel, NationCores>().ReverseMap();
            CreateMap<PagedList<NationCoreModel>, PagedList<NationCores>>().ReverseMap();

            CreateMap<CityCoreModel, CityCores>().ReverseMap();
            CreateMap<PagedList<CityCoreModel>, PagedList<CityCores>>().ReverseMap();

            CreateMap<DistrictCoreModel, DistrictCores>().ReverseMap();
            CreateMap<PagedList<DistrictCoreModel>, PagedList<DistrictCores>>().ReverseMap();

            CreateMap<WardCoreModel, WardCores>().ReverseMap();
            CreateMap<PagedList<WardCoreModel>, PagedList<WardCores>>().ReverseMap();

            #endregion

        }

    }
}
