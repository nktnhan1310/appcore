using App.Core.Entities.Auth.Google;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface.Services.Auth.Google
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleAuths googleAuths);
    }
}
