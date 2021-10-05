using App.Core.Interface.DbContext;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace App.Core.Service.Factory
{
    public class CurrentLoginUser
    {
        public Guid UserId { set; get; }
        public Guid? DepartmentId { set; get; }
        public string UserName { set; get; }
        public string DepartmentCode { get; set; }
    }

    public abstract class DbContextFactory : IDbContextFactory
    {
        public string ConnectionString { get; set; }
        protected readonly HttpContext httpContext;
        public DbContextFactory(string connectionString, IHttpContextAccessor httpContextAccessor)
        {
            ConnectionString = connectionString;
            httpContext = httpContextAccessor.HttpContext;
        }

        public abstract ICoreDbContext Create();

        public CurrentLoginUser GetCurrentUser()
        {
            if (httpContext != null && httpContext.User.Identity.IsAuthenticated)
            {
                var claim = httpContext.User.Claims.FirstOrDefault(e => e.Type == ClaimTypes.UserData);
                if (claim != null)
                {
                    return JsonConvert.DeserializeObject<CurrentLoginUser>(claim.Value);
                }
            }
            return null;
        }

    }

}
