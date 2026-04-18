using System;
using System.Collections.Generic;
using Shippo;
using System.Collections;
using System.IO;
using System.Net;
using System.Drawing;
using System.Linq;
using System.Configuration;

namespace ImageSolutions.Shipping
{
    public class ShippoShipping
    {
        public ShippoShipping()
        {
        }
        public static Shippo.Address GetAddress(Address.AddressTrans AddressTrans)
        {
            Shippo.Address objReturn = null;

            try
            {
                APIResource resource = new APIResource(Convert.ToString(ConfigurationManager.AppSettings["ShippoAPI"]));

                //APIResource resource = new APIResource("shippo_test_4e794d053e573bd96e115927389595a1cc03f131");
                //APIResource resource = new APIResource("shippo_live_5184a50da86d7c8b08111d3d9c6bae29368e4c76");

                ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                AddressCountryCodeFilter.Alpha2Code = new Database.Filter.StringSearch.SearchFilter();
                AddressCountryCodeFilter.Alpha2Code.SearchString = AddressTrans.CountryCode;
                AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                string strCountryCode = "USA";
                if(AddressCountryCode != null && !string.IsNullOrEmpty(AddressCountryCode.AddressCountryCodeID))
                {
                    strCountryCode = AddressCountryCode.Alpha3Code;
                }

                objReturn = resource.CreateAddress(new Hashtable(){
                    {"name", string.Format("{0}", AddressTrans.FullName)},
                    {"company", string.Format("{0}", AddressTrans.CompanyName)},
                    {"street1", string.Format("{0}", AddressTrans.AddressLine1)},
                    {"street2", string.Format("{0}", AddressTrans.AddressLine2)},
                    {"city", string.Format("{0}", AddressTrans.City)},
                    {"state", string.Format("{0}", AddressTrans.State)},
                    {"zip", string.Format("{0}", AddressTrans.PostalCode)},
                    {"country", string.Format("{0}", strCountryCode)},
                    {"phone", string.Format("{0}", AddressTrans.PhoneNumber)},
                    {"email", string.Format("{0}", AddressTrans.Email)},
                    {"validate", "true"}
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReturn;
        }
        public static List<ShippoRate> GetRate(string websiteid
            , ImageSolutions.ShoppingCart.ShoppingCart shoppingcart
            //, ImageSolutions.Address.AddressBook fromaddressbook
            , ImageSolutions.Address.AddressTrans toaddresstrans
            , string websitegroupid)
        {
            List<ShippoRate> result = null;
            try
            {
                //APIResource resource = new APIResource("shippo_test_4e794d053e573bd96e115927389595a1cc03f131");
                APIResource resource = new APIResource(Convert.ToString(ConfigurationManager.AppSettings["ShippoAPI"]));

                ImageSolutions.Address.AddressBook fromaddressbook = new ImageSolutions.Address.AddressBook();
                fromaddressbook.CompanyName = "Image Solutions";
                fromaddressbook.FirstName = "Steve";
                fromaddressbook.LastName = "Park";
                fromaddressbook.AddressLine1 = "19571 Magellan Dr";
                fromaddressbook.AddressLine2 = "";
                fromaddressbook.City = "Torrance";
                fromaddressbook.State = "CA";
                fromaddressbook.PostalCode = "90502";
                fromaddressbook.CountryCode = "USA";
                fromaddressbook.PhoneNumber = "310-464-8991";
                fromaddressbook.Email = "info@imageinc.com";

                List<ShippoRate> rates = new List<ShippoRate>();

                int weight = shoppingcart.ShoppingCartLines.Sum(x => x.Quantity);

                Shippo.Address validatedToAddress = resource.CreateAddress(new Hashtable(){
                    {"name", string.Format("{0}", toaddresstrans.FullName)},
                    //{"company", string.Format("{0}", packingSlip.ShipToAddressCompanyName)},
                    {"street1", string.Format("{0}", toaddresstrans.AddressLine1)},
                    {"street2", string.Format("{0}", toaddresstrans.AddressLine2)},
                    {"city", string.Format("{0}", toaddresstrans.City)},
                    {"state", string.Format("{0}", toaddresstrans.State)},
                    {"zip", string.Format("{0}", toaddresstrans.PostalCode)},
                    {"country", string.Format("{0}", string.IsNullOrEmpty(toaddresstrans.CountryCode) ? "USA" : toaddresstrans.CountryCode)},
                    {"phone", string.Format("{0}", toaddresstrans.PhoneNumber)},
                    {"email", string.Format("{0}", toaddresstrans.Email)},
                    {"validate", "false"}
                });

                Hashtable fromAddressTable = new Hashtable();

                fromAddressTable.Add("name", string.Format("{0}", fromaddressbook.FullName));
                fromAddressTable.Add("company", string.Format("{0}", fromaddressbook.CompanyName));
                fromAddressTable.Add("street1", string.Format("{0}", fromaddressbook.AddressLine1));
                fromAddressTable.Add("street2", string.Format("{0}", fromaddressbook.AddressLine2));
                fromAddressTable.Add("city", string.Format("{0}", fromaddressbook.City));
                fromAddressTable.Add("state", string.Format("{0}", fromaddressbook.State));
                fromAddressTable.Add("zip", string.Format("{0}", fromaddressbook.PostalCode));
                fromAddressTable.Add("country", string.Format("{0}", fromaddressbook.CountryCode));
                fromAddressTable.Add("phone", string.Format("{0}", fromaddressbook.PhoneNumber));
                fromAddressTable.Add("email", string.Format("{0}", fromaddressbook.Email));

                List<string> ShippingMethods = new List<string>();
                ShippingMethods.Add("");

                List<Parcel> parcels = new List<Parcel>();

                Hashtable parcelTable = null;
                Parcel parcel = null;

                foreach (string _shippingmethod in ShippingMethods)
                {
                    parcelTable = new Hashtable();

                    parcelTable.Add("length", 5);
                    parcelTable.Add("width", 6);
                    parcelTable.Add("height", 10);
                    parcelTable.Add("distance_unit", "in");
                    parcelTable.Add("weight", weight);
                    parcelTable.Add("mass_unit", "lb");
                    parcelTable.Add("servicelevel_token", _shippingmethod);

                    parcel = resource.CreateParcel(parcelTable);
                    parcels.Add(parcel);
                }

                foreach (Parcel _parcel in parcels)
                {
                    Hashtable extras = new Hashtable() { { "signature_confirmation", "STANDARD" } };

                    Shipment shipment = resource.CreateShipment(new Hashtable(){
                    { "address_to", validatedToAddress},
                    { "address_from", fromAddressTable},
                    //{ "carrier_accounts", carrierAccounts},
                    { "parcels", _parcel.ObjectId},
                    { "extra", new Hashtable(){} },
                    { "async", false}});


                    List<ImageSolutions.Website.WebsiteShippingService> WebsiteShippingServices = new List<Website.WebsiteShippingService>();
                    ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new Website.WebsiteShippingServiceFilter();
                    WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteShippingServiceFilter.WebsiteID.SearchString = websiteid;
                    WebsiteShippingServices = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingServices(WebsiteShippingServiceFilter);

                    List<ShippingService> ShippingServices = new List<ShippingService>();
                    foreach (Website.WebsiteShippingService _WebsiteShippingService in WebsiteShippingServices)
                    {
                        if (_WebsiteShippingService.WebsiteShippingServiceGroups == null || _WebsiteShippingService.WebsiteShippingServiceGroups.Count == 0
                            || _WebsiteShippingService.WebsiteShippingServiceGroups.Exists(x => x.WebsiteGroupID == websitegroupid))
                        ShippingServices.Add(_WebsiteShippingService.ShippingService);
                    }


                    foreach (Shippo.Rate _rate in shipment.Rates)
                    {
                        bool isExist = false;

                        foreach (ShippoRate _retrate in rates)
                        {
                            if (Convert.ToString(_rate.Servicelevel.Token) == _retrate.Token)
                            {
                                isExist = true;
                            }
                        }

                        if (!isExist)
                        {
                            ShippoRate rate = new ShippoRate();

                            rate.RateResponseID = Convert.ToString(_rate.ObjectId);
                            rate.ServiceTemplate = string.IsNullOrEmpty(_parcel.Template) ? "" : Convert.ToString(_parcel.Template);
                            rate.ServiceName = Convert.ToString(_rate.Servicelevel.Name);
                            rate.Token = Convert.ToString(_rate.Servicelevel.Token);
                            rate.Carrier = Convert.ToString(_rate.Provider);
                            rate.Amount = Convert.ToDecimal(_rate.Amount);
                            rate.label = string.Format("{0} - {1} -{2} ${3}", rate.Carrier, rate.ServiceName
                                , !string.IsNullOrEmpty(rate.ServiceTemplate) ? string.Format(" {0} -", rate.ServiceTemplate) : string.Empty, rate.Amount);

                            //rates.Add(rate);

                            ShippingService ShippingService = new ShippingService();
                            ShippingServiceFilter ShippingServiceFilter = new ShippingServiceFilter();
                            ShippingServiceFilter.ServiceCode = new Database.Filter.StringSearch.SearchFilter();
                            ShippingServiceFilter.ServiceCode.SearchString = rate.Token;
                            ShippingService = ShippingService.GetShippingService(ShippingServiceFilter);

                            if (ShippingServices.Find(x => x.ServiceCode == ShippingService.ServiceCode) != null
                                && !string.IsNullOrEmpty(ShippingServices.Find(x => x.ServiceCode == ShippingService.ServiceCode).ShippingServiceID))
                            {
                                rates.Add(rate);
                            }
                        }
                    }
                }

                result = rates.OrderBy(x => x.Amount).ToList();
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return result;
        }
    }
}

