using App.Core.Entities.Configuration;
using App.Core.Entities.DomainEntity;
using App.Core.Interface.Services.Configuration;
using App.Core.Interface.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace App.Core.Service
{
    public class EsmsResult
    {
        public int CodeResult { get; set; }
        public int CountRegenerate { get; set; }
        public int SMSID { get; set; }
    }

    public class SMSConfigurationCoreService : DomainService<SMSConfigurationCores, BaseSearch>, ISMSConfigurationCoreService
    {
        private IConfiguration configuration;
        public SMSConfigurationCoreService(IAppUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : base(unitOfWork, mapper)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// gửi tin nhắn
        /// </summary>
        /// <param name="Phone"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public async Task<bool> SendSMS(string Phone, string Content)
        {
            try
            {
                string url = "http://api.esms.vn/MainService.svc/xml/SendMultipleMessage_V4/";
                var smsConfiguartionInfo = await this.unitOfWork.Repository<SMSConfigurationCores>().GetQueryable().Where(e => !e.Deleted && e.Active).FirstOrDefaultAsync();
                if (smsConfiguartionInfo != null)
                {
                    // declare ascii encoding
                    UTF8Encoding encoding = new UTF8Encoding();
                    string strResult = string.Empty;
                    string SampleXml = @"<RQST>"
                                       + "<APIKEY>" + smsConfiguartionInfo.APIKey + "</APIKEY>"
                                       + "<SECRETKEY>" + smsConfiguartionInfo.SecretKey + "</SECRETKEY>"
                                       + "<ISFLASH>0</ISFLASH>"
                                       + "<SMSTYPE>" + smsConfiguartionInfo.SMSType + "</SMSTYPE>"
                                       + "<BRANDNAME>" + smsConfiguartionInfo.BrandName + "</BRANDNAME>"  //De dang ky brandname rieng vui long lien he hotline 0902435340 hoac nhan vien kinh Doanh cua ban                              
                                       + "<CONTENT>" + Content + "</CONTENT>"
                                       + "<CONTACTS><CUSTOMER><PHONE>" + Phone + "</PHONE></CUSTOMER></CONTACTS>"

                   + "</RQST>";
                    string postData = SampleXml.Trim().ToString();
                    // convert xmlstring to byte using ascii encoding
                    byte[] data = encoding.GetBytes(postData);
                    // declare httpwebrequet wrt url defined above
                    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
                    // set method as post
                    webrequest.Method = "POST";
                    webrequest.Timeout = 500000;
                    // set content type
                    webrequest.ContentType = "application/x-www-form-urlencoded";
                    // set content length
                    webrequest.ContentLength = data.Length;
                    // get stream data out of webrequest object
                    Stream newStream = webrequest.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                    // declare & read response from service
                    HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();

                    // set utf8 encoding
                    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                    // read response stream from response object
                    StreamReader loResponseStream =
                        new StreamReader(webresponse.GetResponseStream(), enc);
                    // read string from stream data
                    strResult = loResponseStream.ReadToEnd();

                    // close the stream object
                    loResponseStream.Close();
                    // close the response object
                    webresponse.Close();
                    // below steps remove unwanted data from response string
                    strResult = strResult.Replace("</string>", "");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(strResult);
                    string json = JsonConvert.SerializeXmlNode(doc);
                    EsmsResult rs = JsonConvert.DeserializeObject<EsmsResult>(json);
                    bool isProduct = configuration.GetValue<bool>("MySettings:IsProduct");
                    if (!isProduct) return true;
                    if (rs.CodeResult == 100)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
