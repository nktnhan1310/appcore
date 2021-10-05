using App.Core.Entities;
using App.Core.Interface.Services;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public class UserInGroupCoreService : DomainService<UserInGroupCores, BaseSearchUserInGroup>, IUserInGroupCoreService
    {
        public UserInGroupCoreService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override string GetStoreProcName()
        {
            return "UserInGroup_GetPagingData";
        }

        protected override SqlParameter[] GetSqlParameters(BaseSearchUserInGroup baseSearch)
        {
            SqlParameter[] parameters =
            {
                new SqlParameter("@PageIndex", baseSearch.PageIndex),
                new SqlParameter("@PageSize", baseSearch.PageSize),
                new SqlParameter("@UserId", baseSearch.UserId),
                new SqlParameter("@UserGroupId", baseSearch.UserGroupId),
                new SqlParameter("@OrderBy", baseSearch.OrderBy),
                new SqlParameter("@TotalPage", SqlDbType.Int, 0),
            };
            return parameters;
        }

    }
}
