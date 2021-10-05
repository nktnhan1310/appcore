using App.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface.Services.Auth
{
    public interface IFaceBookAuthCoreService
    {
        /// <summary>
        /// Validate login bằng facebook
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<FaceBookTokenValidateResult> ValidateTokenAsync(string accessToken);

        /// <summary>
        /// Lấy thông tin user
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<FaceBookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
