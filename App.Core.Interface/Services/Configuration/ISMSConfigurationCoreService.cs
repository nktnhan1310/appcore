using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface.Services.Configuration
{
    public interface ISMSConfigurationCoreService : IDomainService<SMSConfigurationCores, BaseSearch>
    {
        /// <summary>
        /// Gửi SMS qua SDT
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        Task<bool> SendSMS(string Phone, string Content);
    }
}
