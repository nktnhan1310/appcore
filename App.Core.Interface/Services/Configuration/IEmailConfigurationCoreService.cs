using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface.Services.Configuration
{
    public interface IEmailConfigurationCoreService : IDomainService<EmailConfigurationCores, BaseSearch>
    {
        Task<EmailSendConfigure> GetEmailConfig();
        EmailContent GetEmailContent();
        Task Send(string subject, string body, string[] Tos);
        Task Send(string subject, string body, string[] Tos, string[] CCs);
        Task Send(string subject, string body, string[] Tos, string[] CCs, string[] BCCs);
        Task Send(string subject, string[] Tos, string[] CCs, string[] BCCs, EmailContent emailContent);
        Task SendMail(string subject, string Tos, string[] CCs, string[] BCCs, EmailContent emailContent);
    }
}
