using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Shipping
{
    public class Restlet
    {
        //public RestClient objClient = new RestClient("https://acct88641.app.netsuite.com");
        //public RestClient objClient = new RestClient("https://acct88641.extforms.netsuite.com");

        public Restlet()
        {
            RestSharp.Authenticators.OAuth1Authenticator objAuthenticator = new RestSharp.Authenticators.OAuth1Authenticator();

            //production main
            //objRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=956&deploy=1", Method.Post);
            //objRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=2024&deploy=1&compid=ACCT88641&h=c97df74ab94d7a7b2a7e", Method.Get);

            //production test
            //objRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=2024&deploy=1", Method.Post);
           
            //objAuthenticator.ConsumerKey = "01099c3ecb51fa1578e5cf1e71d98ff117421f35ba5fd6727910227367962158";
            //objAuthenticator.ConsumerSecret = "57a73a0586c4e8af83d4d6cfe289907ab85c83169760a87197270aab1fbe1833";
            //objAuthenticator.Token = "6dc6482868406e7e933bc79d999300403349d0c84b269e349dff3ec9fe6ecef1";
            //objAuthenticator.TokenSecret = "b5867514ed7219a377873cb608a30ddd06d487e68bf1e5ee26c105dc619c2d89";
            //objAuthenticator.Realm = "ACCT88641";
            //objAuthenticator.SignatureMethod = RestSharp.Authenticators.OAuth.OAuthSignatureMethod.HmacSha256;
            //objAuthenticator.Type = RestSharp.Authenticators.OAuth.OAuthType.AccessToken;

            //objClient.Authenticator = objAuthenticator;


        }
        //public string getWebShipRate(Shipping.WebShipRate.WebShipRate shipSummaryPayload)
        //{
        //    RestResponse objResponse = null;
        //    string objReturn = string.Empty;

        //    try
        //    {
        //        if(shipSummaryPayload == null)
        //        {
        //            throw new Exception("Invalid rate request");
        //        }

        //        //Shipping.WebShipRate.Body Body = new WebShipRate.Body();
        //        //Body.shipSummaryPayload = rateRequest;

        //        //Shipping.WebShipRate.WebShipRate Body = new WebShipRate.Body();
        //        //Body.
        //        objRequest.AddBody(shipSummaryPayload);

        //        objResponse = objClient.Execute(objRequest);
        //        objReturn = objResponse.Content;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return objReturn;
        //}
        public static ImageSolutions.Shipping.WebShipRate.WebShipRateResponse getWebShipRate(ImageSolutions.Website.Website currentuserwbesite
            , ImageSolutions.ShoppingCart.ShoppingCart shoppingcart
            //, ImageSolutions.Address.AddressBook fromaddressbook
            , ImageSolutions.Address.AddressTrans toaddresstrans
            , string websitegroupid
            , bool isnoninventory = false)
        {
            RestResponse objResponse = null;
            ImageSolutions.Shipping.WebShipRate.WebShipRateResponse objReturn = null;
            RestRequest objRequest = null;

            try
            {
                RestClient objClient = new RestClient("https://acct88641.extforms.netsuite.com");

                objRequest = new RestRequest("/app/site/hosting/scriptlet.nl?script=2024&deploy=1&compid=ACCT88641&ns-at=AAEJ7tMQ8SPyYLnnBf0_ZKO6KXRPZXv7Hwh7R8mWp1U7x4tsO2g&h=c97df74ab94d7a7b2a7e", Method.Post);
                objRequest.AddHeader("User-Agent", "Mozilla/5.0");

                ImageSolutions.Shipping.WebShipRate.WebShipRate rateRequest = new ImageSolutions.Shipping.WebShipRate.WebShipRate();
                rateRequest.lineItems = new List<ImageSolutions.Shipping.WebShipRate.Item>();

                if (currentuserwbesite.DeliveryAddress == null) throw new Exception("Shipping calculator requires ship from address");
                rateRequest.SiteNumber = currentuserwbesite.Name; //"36";//albertons website for example
                rateRequest.FromLocation = currentuserwbesite.DeliveryAddress.InternalID;//Torrance warehouse

                if (isnoninventory)
                {
                    rateRequest.FromLocation = "3";
                }

                rateRequest.ShipAddress = new ImageSolutions.Shipping.WebShipRate.Address();
                rateRequest.ShipAddress.address1 = toaddresstrans.AddressLine1;
                rateRequest.ShipAddress.city = toaddresstrans.City;
                rateRequest.ShipAddress.state = toaddresstrans.State;
                rateRequest.ShipAddress.zip = toaddresstrans.PostalCode;
                rateRequest.ShipAddress.country = toaddresstrans.CountryCode;
                rateRequest.ShipAddress.is_residential = false;

                foreach(ShoppingCart.ShoppingCartLine _shoppingcartline in shoppingcart.ShoppingCartLines)
                {
                    if(string.IsNullOrEmpty(_shoppingcartline.Item.InternalID))
                    {
                        throw new Exception(string.Format("Item {0} - {1} is not set up correctly", _shoppingcartline.ItemID, _shoppingcartline.Item.ItemName));
                    }

                    ImageSolutions.Shipping.WebShipRate.Item objItem = new ImageSolutions.Shipping.WebShipRate.Item();
                    objItem.internalid = _shoppingcartline.Item.InternalID;
                    objItem.itemid = _shoppingcartline.Item.ItemNumber;
                    objItem.quantity = _shoppingcartline.Quantity;
                    objItem.weight = Convert.ToDouble(_shoppingcartline.Item.UnitWeight);
                    //objItem.itemtype = "NonInvtPart";
                    //objItem.flatOk = true;
                    //objItem.amount = 38.32;

                    rateRequest.lineItems.Add(objItem);
                }

                rateRequest.SiteMethods = new List<ImageSolutions.Shipping.WebShipRate.SiteMethods>();

                List<ImageSolutions.Website.WebsiteShippingService> WebsiteShippingServices = new List<Website.WebsiteShippingService>();
                ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new Website.WebsiteShippingServiceFilter();
                WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteShippingServiceFilter.WebsiteID.SearchString = currentuserwbesite.WebsiteID;
                WebsiteShippingServices = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingServices(WebsiteShippingServiceFilter);

                List<ShippingService> ShippingServices = new List<ShippingService>();
                foreach (Website.WebsiteShippingService _WebsiteShippingService in WebsiteShippingServices)
                {
                    if (string.IsNullOrEmpty(websitegroupid) 
                        || _WebsiteShippingService.WebsiteShippingServiceGroups == null 
                        || _WebsiteShippingService.WebsiteShippingServiceGroups.Count == 0
                        || _WebsiteShippingService.WebsiteShippingServiceGroups.Exists(x => x.WebsiteGroupID == websitegroupid)
                    )
                    {
                        ImageSolutions.Shipping.WebShipRate.SiteMethods objSiteMethod = new ImageSolutions.Shipping.WebShipRate.SiteMethods();
                        //objSiteMethod.Rate = "<!---->";
                        objSiteMethod.Name = _WebsiteShippingService.ShippingService.ServiceName;
                        objSiteMethod.shipmethod = _WebsiteShippingService.ShippingService.ServiceCode; //shipping item internalID from netsuite
                        objSiteMethod.shipcarrier = _WebsiteShippingService.ShippingService.Carrier;

                        rateRequest.SiteMethods.Add(objSiteMethod);
                    }
                }

                objRequest.AddBody(rateRequest);

                objResponse = objClient.Execute(objRequest);

                objReturn = JsonConvert.DeserializeObject<ImageSolutions.Shipping.WebShipRate.WebShipRateResponse>(objResponse.Content);

                if (currentuserwbesite.EnableIPD && toaddresstrans.CountryCode != "US")
                {
                    foreach (WebShipRate.ShippingMethod _ShippingMethod in objReturn.shippingMethods)
                    {
                        _ShippingMethod.rate = _ShippingMethod.rate * ((100.00 + Convert.ToDouble(currentuserwbesite.IPDShippingAdjustPercent))/100.00);

                        if(_ShippingMethod.rate != null) _ShippingMethod.rate = Math.Round(Convert.ToDouble(_ShippingMethod.rate), 2);

                        _ShippingMethod.rate_formatted = String.Format("${0}", _ShippingMethod.rate == null || string.IsNullOrEmpty(Convert.ToString(_ShippingMethod.rate)) ? "0" : Convert.ToString(_ShippingMethod.rate) );
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReturn;
        }
    }
}
