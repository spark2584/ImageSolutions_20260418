using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ImageSolutions.ZipTax
{
    public class ZipTax
    {
        public ZipTax()
        {
        }
        public ZipTaxResponse GetZipTax(string city, string state, string postalcode)
        {
            ZipTaxResponse ZipTaxResponse = null;

            string strBaseURL = "https://api.zip-tax.com/request/v40";
            string strKey = "RawkrOnPX5ijmKyv";
            string strURL = string.Format("{0}?key={1}&city={2}&state={3}&postalcode={4}"
                , strBaseURL
                , strKey
                , city
                , state
                , postalcode);

            string strResponse = string.Empty;

            HttpWebRequest HttpWebRequest = (HttpWebRequest)WebRequest.Create(strURL);
            HttpWebRequest.Accept = "application/json";
            HttpWebRequest.ContentType = "application/json";
            HttpWebRequest.Method = "GET";
            HttpWebRequest.Host = "api.zip-tax.com";

            using (HttpWebResponse _HttpWebResponse = (HttpWebResponse)HttpWebRequest.GetResponse())
            {
                using (StreamReader _StreamReader = new StreamReader(_HttpWebResponse.GetResponseStream()))
                {
                    strResponse = _StreamReader.ReadToEnd();

                    JavaScriptSerializer JavaScriptSerializer = new JavaScriptSerializer();
                    ZipTaxResponse = JavaScriptSerializer.Deserialize<ZipTaxResponse>(strResponse); 
                }
            }

            return ZipTaxResponse;
        }
    }
}
