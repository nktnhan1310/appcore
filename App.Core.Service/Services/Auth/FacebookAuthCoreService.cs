using App.Core.Entities.Auth;
using App.Core.Entities.Configuration;
using App.Core.Interface.Services.Auth;
using App.Core.Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Service.Services.Auth
{
    public class FacebookAuthCoreService : IFaceBookAuthCoreService
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=id,name,email,picture&access_token={0}";
        protected readonly IUnitOfWork unitOfWork;
        private readonly HttpClient httpClient;
        public FacebookAuthCoreService(IAppUnitOfWork unitOfWork, HttpClient httpClient)
        {
            this.unitOfWork = unitOfWork;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Lấy thông tin người dùng facebook
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<FaceBookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var faceBookAuthSettingInfo = await unitOfWork.Repository<FaceBookAuthSettings>().GetQueryable().Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();
            if (faceBookAuthSettingInfo != null)
            {
                var formattedUserInfoUrl = string.Format(UserInfoUrl, accessToken);

                var result = await httpClient.GetAsync(formattedUserInfoUrl);
                result.EnsureSuccessStatusCode();
                var responseAsString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FaceBookUserInfoResult>(responseAsString);

            }
            return null;
        }

        /// <summary>
        /// Kiểm tra dữ liệu facebook with appid - appsecret
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<FaceBookTokenValidateResult> ValidateTokenAsync(string accessToken)
        {
            var faceBookAuthSettingInfo = await unitOfWork.Repository<FaceBookAuthSettings>().GetQueryable().Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();
            if (faceBookAuthSettingInfo != null)
            {
                var formattedUserInfoUrl = string.Format(TokenValidationUrl, accessToken, faceBookAuthSettingInfo.AppId, faceBookAuthSettingInfo.AppSecret);
                var result = await httpClient.GetAsync(formattedUserInfoUrl);
                result.EnsureSuccessStatusCode();
                var responseAsString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FaceBookTokenValidateResult>(responseAsString);
            }
            return null;
        }
    }
}