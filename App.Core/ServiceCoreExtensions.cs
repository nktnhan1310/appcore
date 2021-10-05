using App.Core.Interface;
using App.Core.Interface.DbContext;
using App.Core.Interface.Repository;
using App.Core.Interface.Services;
using App.Core.Interface.Services.Auth;
using App.Core.Interface.Services.Catalogue;
using App.Core.Interface.Services.Configuration;
using App.Core.Interface.UnitOfWork;
using App.Core.Service;
using App.Core.Service.Services.Catalogue;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core
{
    public static class ServiceCoreExtensions
    {
        public static void ConfigureCoreRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            services.AddScoped(typeof(ICatalogueRepository<>), typeof(CatalogueRepository<>));
            services.AddScoped(typeof(IAppRepository<>), typeof(AppRepository<>));
            services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        }

        public static void ConfigureCoreService(this IServiceCollection services)
        {
            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("he")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddTransient<ITokenManagerService, TokenManagerService>();

            #region Authenticate

            services.AddScoped<IUserCoreService, UserCoreService>();
            services.AddScoped<IUserFileCoreService, UserFileCoreService>();
            services.AddScoped<IUserInGroupCoreService, UserInGroupCoreService>();
            services.AddScoped<IUserGroupCoreService, UserGroupCoreService>();
            services.AddScoped<IPermissionCoreService, PermissionCoreService>();
            services.AddScoped<IPermitObjectCoreService, PermitObjectCoreService>();
            services.AddScoped<IPermitObjectPermissionCoreService, PermitObjectPermissionCoreService>();

            #endregion

            #region Catalogue

            services.AddScoped<IWardCoreService, WardCoreService>();
            services.AddScoped<IDistrictCoreService, DistrictCoreService>();
            services.AddScoped<ICityCoreService, CityCoreService>();
            services.AddScoped<ICountryCoreService, CountryCoreService>();
            services.AddScoped<INationCoreService, NationCoreService>();

            #endregion

            #region Configuration

            services.AddScoped<IEmailConfigurationCoreService, EmailConfigurationCoreService>();
            services.AddScoped<ISMSConfigurationCoreService, SMSConfigurationCoreService>();
            services.AddScoped<ISMSEmailTemplateCoreService, SMSEmailTemplateCoreService>();
            services.AddScoped<IOTPHistoryCoreService, OTPHistoryCoreService>();


            #endregion
        }

        public static void ConfigureCoreSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "QUANLY API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });

                var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                foreach (var fi in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(fi.FullName);
                }

                c.EnableAnnotations();
            });
        }

    }
}
