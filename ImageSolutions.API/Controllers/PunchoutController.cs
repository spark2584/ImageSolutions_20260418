using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using RestSharp;

namespace ImageSolutions.API.Controllers
{
    [RoutePrefix("punchout")]
    public class PunchoutController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public HttpResponseMessage CreatePunchoutLogin([FromBody] PunchoutLoginBody punchoutloginbody)
        {
            string returnURL = String.Empty;

            string strPos = string.Empty;
            string strReturnUrl = string.Empty;
            string strParams = string.Empty;

            ImageSolutions.WebAPI.WebAPILog WebAPILog = null;

            try
            {
                string websiteid = String.Empty;
                ImageSolutions.Website.Website Website = null;
                var allUrlKeyValues = ControllerContext.Request.GetQueryNameValuePairs();
                if (allUrlKeyValues != null)
                {
                    websiteid = allUrlKeyValues.LastOrDefault(x => x.Key == "website").Value;

                    //temporaty to production
                    if (websiteid == "47")
                        websiteid = "10";
                    if (websiteid == "50")
                        websiteid = "13";

                    Website = new Website.Website(websiteid);

                    if (Website == null)
                    {
                        throw new Exception("Invalid Website");
                    }
                }
                else
                {
                    throw new Exception("Missing Website");
                }

                //Log
                WebAPILog = new ImageSolutions.WebAPI.WebAPILog();
                WebAPILog.Method = "Post";
                WebAPILog.Route = "login";
                WebAPILog.Body = JsonConvert.SerializeObject(punchoutloginbody);
                WebAPILog.RequestUri = Convert.ToString(Request.RequestUri);
                WebAPILog.Create();

                //if (punchoutlogin != null)
                //{
                //    NameValueCollection body = HttpUtility.ParseQueryString(punchoutsend.body);
                //    strPos = Convert.ToString(body["pos"]);
                //    strReturnUrl = Convert.ToString(body["return_url"]);
                //    strParams = Convert.ToString(body["params"]);
                //}
                //else
                //{
                //    throw new Exception("Missing API Body");
                //}


                if (punchoutloginbody == null)
                {
                    throw new Exception("Invalid FromBody");
                }

                string strEmail = string.Empty;
                string strPassword = string.Empty;

                if(websiteid == "13") //Enterprise
                {
                    Models.Enterprise.PunchoutLogin punchoutlogin = JsonConvert.DeserializeObject<Models.Enterprise.PunchoutLogin>(punchoutloginbody.@params);

                    //Validate Username and Password
                    //PucnhoutLogin PucnhoutLogin = JsonConvert.DeserializeObject<PucnhoutLogin>(strParams);
                    //string strEmail = punchoutlogin.body.data.UserEmail;
                    //string strPassword = "Im@geSolutions$1";

                    //if (punchoutlogin.body.contact != null && punchoutlogin.body.contact.data != null && !string.IsNullOrEmpty(punchoutlogin.body.contact.data.punchoutemail))
                    //{
                    //    strEmail = punchoutlogin.body.contact.data.punchoutemail;
                    //    strPassword = punchoutlogin.body.contact.data.punchoutpassword;
                    //}
                    if (punchoutlogin.body != null && punchoutlogin.body.data != null && !string.IsNullOrEmpty(punchoutlogin.body.data.employee_id))
                    {
                        strEmail = string.Format(@"{0}@ehi.com", punchoutlogin.body.data.employee_id);
                    }
                    else
                    {
                        throw new Exception("Missing employee_id");
                    }

                    strPassword = "Im@geSolutions$1";
                }
                else
                {
                    Models.Ahold.PunchoutLogin punchoutlogin = JsonConvert.DeserializeObject<Models.Ahold.PunchoutLogin>(punchoutloginbody.@params);

                    //Validate Username and Password
                    //PucnhoutLogin PucnhoutLogin = JsonConvert.DeserializeObject<PucnhoutLogin>(strParams);
                    //string strEmail = punchoutlogin.body.data.UserEmail;
                    //string strPassword = "Im@geSolutions$1";

                    strEmail = punchoutlogin.body.data.UserEmail;
                    
                    if (punchoutlogin.body.contact != null && punchoutlogin.body.contact.data != null && !string.IsNullOrEmpty(punchoutlogin.body.contact.data.punchoutemail))
                    {
                        strEmail = punchoutlogin.body.contact.data.punchoutemail;
                        strPassword = punchoutlogin.body.contact.data.punchoutpassword;
                    }
                    //if (punchoutlogin.body.data != null && !string.IsNullOrEmpty(punchoutlogin.body.data.employee_id))
                    //{
                    //    strEmail = string.Format(@"{0}@ehi.com", punchoutlogin.body.data.employee_id);
                    //}
                }

                ImageSolutions.User.UserInfo UserInfo = new User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new User.UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = strEmail;
                //UserInfoFilter.Password = new Database.Filter.StringSearch.SearchFilter();
                //UserInfoFilter.Password.SearchString = strPassword;
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                if (UserInfo == null)
                {
                    throw new Exception("Invalid Login");
                }

                //Store Session ID                
                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new User.UserWebsiteFilter();
                UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteFilter.WebsiteID.SearchString = websiteid;
                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                if(UserWebsite == null || UserWebsite.InActive)
                {
                    throw new Exception("Invalid Login");
                }

                UserWebsite.PunchoutReturnURL = punchoutloginbody.return_url;
                UserWebsite.PunchoutSessionID = punchoutloginbody.pos;

                Guid Guid = new Guid();
                UserWebsite.PunchoutGUID = Convert.ToString(Guid.NewGuid());
                UserWebsite.Update();

                returnURL = string.Format(@"{0}?website={1}&session={2}&guid={3}", ConfigurationManager.AppSettings["WebsiteURL"], Website.GUID, UserWebsite.PunchoutSessionID, UserWebsite.PunchoutGUID);
                WebAPILog.Response = returnURL;
                WebAPILog.Update();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.Moved);
                Response.Headers.Location = new Uri(returnURL);

                return Response;
            }
            catch (Exception ex)
            {
                if (WebAPILog != null)
                {
                    WebAPILog.ErrorMessage = string.Format("{0}", ex.Message);
                    WebAPILog.Update();
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("punchoutpo")]
        public HttpResponseMessage PunchoutPO(string @params)
        {
            RestResponse RestResponse = null;
            RestRequest RestRequest = null;

            ImageSolutions.WebAPI.WebAPILog WebAPILog = null;

            try
            {
                //@params = "";
                //@params = Uri.UnescapeDataString(@params);

                WebAPILog = new ImageSolutions.WebAPI.WebAPILog();
                WebAPILog.Method = "Post";
                WebAPILog.Route = "punchoutpo";
                WebAPILog.Body = @params;

                int index = Convert.ToString(Request.RequestUri).IndexOf('?');

                WebAPILog.RequestUri = index >= 0 ? Convert.ToString(Request.RequestUri).Substring(0, index) : Convert.ToString(Request.RequestUri);
                WebAPILog.Create();

                RestClient RestClient = new RestClient("https://acct88641.extforms.netsuite.com");

                RestRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=178&deploy=1&compid=ACCT88641&ns-at=AAEJ7tMQkd8-zK-WaxwyBaT_hDK30zKyD4gxGQPJDfsHwzhmeGw", Method.Post);
                RestRequest.AddHeader("User-Agent", "Mozilla/5.0");

                //string strParams = Uri.UnescapeDataString(@params);

                RestRequest.AddParameter("params", @params);
                RestRequest.Timeout = TimeSpan.FromSeconds(300);
                RestResponse = RestClient.Execute(RestRequest);
                
                if (RestResponse != null && !string.IsNullOrEmpty(RestResponse.Content))
                {
                    WebAPILog.Response = Convert.ToString(RestResponse.Content);
                    WebAPILog.Update();
                }

                if (RestResponse.IsSuccessful)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    throw new Exception(String.Format("Failed: {0}", Convert.ToString(RestResponse.Content)));
                    //throw new Exception("Unable to create order in NetSuite");
                }
            }
            catch (Exception ex)
            {
                if (WebAPILog != null)
                {
                    WebAPILog.ErrorMessage = string.Format("{0}", ex.Message);
                    WebAPILog.Update();
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Route("order")]
        public HttpResponseMessage PunchoutOrder([FromBody] PunchoutOrderBody punchoutorderbody)
        {
            RestResponse RestResponse = null;
            RestRequest RestRequest = null;

            ImageSolutions.WebAPI.WebAPILog WebAPILog = null;

            string strParams = string.Empty;

            try
            {
                strParams = punchoutorderbody.@params; //JsonConvert.SerializeObject(punchoutorderbody.@params);
                strParams = Uri.UnescapeDataString(strParams);

                WebAPILog = new ImageSolutions.WebAPI.WebAPILog();
                WebAPILog.Method = "Post";
                WebAPILog.Route = "punchoutpo";
                WebAPILog.Body = strParams; //JsonConvert.SerializeObject(punchoutorderbody);

                WebAPILog.OrderID = punchoutorderbody.orderid;
                WebAPILog.OrderNumber = punchoutorderbody.ordernumber;

                int index = Convert.ToString(Request.RequestUri).IndexOf('?');

                WebAPILog.RequestUri = index >= 0 ? Convert.ToString(Request.RequestUri).Substring(0, index) : Convert.ToString(Request.RequestUri);
                WebAPILog.Create();

                RestClient RestClient = new RestClient("https://acct88641.extforms.netsuite.com");

                RestRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=178&deploy=1&compid=ACCT88641&ns-at=AAEJ7tMQkd8-zK-WaxwyBaT_hDK30zKyD4gxGQPJDfsHwzhmeGw", Method.Post);
                RestRequest.AddHeader("User-Agent", "Mozilla/5.0");

                RestRequest.AddParameter("params", strParams);
                RestRequest.Timeout = TimeSpan.FromSeconds(300);

                RestResponse = RestClient.Execute(RestRequest);

                if (RestResponse != null && !string.IsNullOrEmpty(RestResponse.Content))
                {
                    WebAPILog.Response = Convert.ToString(RestResponse.Content);
                    WebAPILog.Update();
                }

                if (RestResponse.IsSuccessful)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    throw new Exception(String.Format("Failed: {0}", Convert.ToString(RestResponse.Content)));
                    //throw new Exception("Unable to create order in NetSuite");
                }
            }
            catch (Exception ex)
            {
                if (WebAPILog != null)
                {
                    WebAPILog.ErrorMessage = string.Format("{0}", ex.Message);
                    WebAPILog.Update();
                }

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public class PunchoutOrderBody
        {
            public string orderid { get; set; }
            public string ordernumber { get; set; }
            public string @params { get; set; }
        }

        public class PunchoutLoginBody
        {
            public string pos { get; set; }
            public string return_url { get; set; }
            public string @params { get; set; }
        }

        //public class PunchoutLogin
        //{
        //    public Header header { get; set; }
        //    public string type { get; set; }
        //    public string operation { get; set; }
        //    public string mode { get; set; }
        //    public Body body { get; set; }
        //    public object[] custom { get; set; }
        //}

        //public class Header
        //{
        //    public To to { get; set; }
        //    public From from { get; set; }
        //    public Sender sender { get; set; }
        //}

        //public class To
        //{
        //    public object[] data { get; set; }
        //    public _0 _0 { get; set; }
        //}

        //public class _0
        //{
        //    public object[] data { get; set; }
        //    public string value { get; set; }
        //    public string domain { get; set; }
        //}

        //public class From
        //{
        //    public object[] data { get; set; }
        //    public _01 _0 { get; set; }
        //}

        //public class _01
        //{
        //    public object[] data { get; set; }
        //    public string value { get; set; }
        //    public string domain { get; set; }
        //}

        //public class Sender
        //{
        //    public Data data { get; set; }
        //    public _02 _0 { get; set; }
        //}

        //public class Data
        //{
        //    public string UserAgent { get; set; }
        //}

        //public class _02
        //{
        //    public Data1 data { get; set; }
        //    public string value { get; set; }
        //    public string domain { get; set; }
        //}

        //public class Data1
        //{
        //    public string SharedSecret { get; set; }
        //}

        //public class Body
        //{
        //    public Data2 data { get; set; }
        //    public Contact contact { get; set; }
        //    public string buyercookie { get; set; }
        //    public string postform { get; set; }
        //    public Shipping shipping { get; set; }
        //    public Item[] items { get; set; }
        //}

        //public class Data2
        //{
        //    public string User { get; set; }
        //    public string UserEmail { get; set; }
        //    public string UserPrintableName { get; set; }
        //    public string UserFirstName { get; set; }
        //    public string UserLastName { get; set; }
        //    public string employee_id { get; set; }
        //}

        //public class Contact
        //{
        //    public Data3 data { get; set; }
        //    public string email { get; set; }
        //    public string name { get; set; }
        //    public string unique { get; set; }
        //}

        //public class Data3
        //{
        //    public string website { get; set; }
        //    public string punchoutemail { get; set; }
        //    public string punchoutpassword { get; set; }
        //}

        //public class Shipping
        //{
        //    public Data4 data { get; set; }
        //}

        //public class Data4
        //{
        //    public string address_name { get; set; }
        //    public string shipping_id { get; set; }
        //    public string shipping_business { get; set; }
        //    public string shipping_to { get; set; }
        //    public string shipping_street { get; set; }
        //    public string shipping_city { get; set; }
        //    public string shipping_state { get; set; }
        //    public string shipping_zip { get; set; }
        //    public string shipping_country { get; set; }
        //    public string country_id { get; set; }
        //}

        //public class Item
        //{
        //    public string primaryId { get; set; }
        //    public string secondaryId { get; set; }
        //    public string type { get; set; }
        //}

        //public class PunchoutLoginEnterprise
        //{
        //    public Header header { get; set; }
        //    public string type { get; set; }
        //    public string operation { get; set; }
        //    public string mode { get; set; }
        //    public BodyEnterprise body { get; set; }
        //    public object[] custom { get; set; }
        //}
        //public class BodyEnterprise
        //{
        //    public Data2 data { get; set; }
        //    public ContactEnterprise contact { get; set; }
        //    public string buyercookie { get; set; }
        //    public string postform { get; set; }
        //    public Shipping shipping { get; set; }
        //    public Item[] items { get; set; }
        //}
        //public class ContactEnterprise
        //{
        //    public Data3[] data { get; set; }
        //    public string email { get; set; }
        //    public string name { get; set; }
        //    public string unique { get; set; }
        //}
    }
}