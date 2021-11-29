using App.Core.Interface.Services;
using App.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace App.Core.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AppAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] Permissions;
        private readonly string ControllerName;

        public AppAuthorize(string[] permission, string controllerName = "")
        {
            Permissions = permission;
            ControllerName = controllerName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (UserLoginModel)context.HttpContext.Items["User"];//.User;
            string controllerName = string.Empty;
            if (!string.IsNullOrEmpty(ControllerName))
                controllerName = ControllerName;
            else if (context.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                controllerName = descriptor.ControllerName;
            }

            if (user == null)
            {
                context.Result = new JsonResult(new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.Unauthorized,
                    ResultMessage = "Unauthorized"
                });
                return;
            }

            IUserCoreService userService = (IUserCoreService)context.HttpContext.RequestServices.GetService(typeof(IUserCoreService));
            IConfiguration configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            var hasPermit = false;
#if DEBUG
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            if (appSettings != null && appSettings.GrantPermissionDebug)
            {
                hasPermit = true;
            }
            else
            {
                var userCheckResult = userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, controllerName, Permissions);
                hasPermit = userCheckResult.Result;
            }
#else
                var userCheckResult = userService.HasPermission(LoginContext.Instance.CurrentUser.UserId, controllerName, Permissions);
                hasPermit = userCheckResult.Result;
#endif

            if (!hasPermit)
            {
                context.Result = new JsonResult(new AppDomainResult()
                {
                    ResultCode = (int)HttpStatusCode.Unauthorized,
                    ResultMessage = "Unauthorized"
                });
                //new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                throw new UnauthorizedAccessException();
            }

        }
    }
}
