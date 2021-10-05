using App.Core.Entities.DomainEntity;
using App.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interface.Services.Base
{
    public interface ICatalogueService<T, E> : IDomainService<T, E> where T: AppCoreCatalogueDomain where E : BaseSearch
    {
        T GetByCode(string code);
        Task<AppDomainImportResult> ImportTemplateFile(Stream stream, string createdBy);
    }
}
