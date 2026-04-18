using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestGetPricing
    {
        //webservice: https://ws.sanmar.com:8080/SanMarWebService/SanMarPricingServicePort?wsdl

        public static bool Start()
        {
            com.sanmar.ws_sanmarpricingserviceport.item[] Items = new com.sanmar.ws_sanmarpricingserviceport.item[1];

            com.sanmar.ws_sanmarpricingserviceport.item Item = new com.sanmar.ws_sanmarpricingserviceport.item();
            Item.color = "Vivid Green";
            Item.size = "2XL";
            Item.style = "286772";
            Items[0] = Item;

            com.sanmar.ws_sanmarpricingserviceport.webServiceUser WebServiceUser = new com.sanmar.ws_sanmarpricingserviceport.webServiceUser();
            WebServiceUser.sanMarUserName = "DANIELLEMOORE1";
            WebServiceUser.sanMarUserPassword = "danie71046";
            WebServiceUser.sanMarCustomerNumber = "71046";

            com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService service = new com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService();

            com.sanmar.ws_sanmarpricingserviceport.responseBean response = service.getPricing(Items, WebServiceUser);
            return true;
        }
        //public static bool Start()
        //{
        //    com.sanmar.ws_sanmarpricingserviceport.item[] Items = new com.sanmar.ws_sanmarpricingserviceport.item[1];

        //    com.sanmar.ws_sanmarpricingserviceport.item Item = new com.sanmar.ws_sanmarpricingserviceport.item();
        //    Item.color = "Jet Black";
        //    Item.size = "S";
        //    Item.style = "PC55LS";
        //    Items[0] = Item;

        //    com.sanmar.ws_sanmarpricingserviceport.webServiceUser WebServiceUser = new com.sanmar.ws_sanmarpricingserviceport.webServiceUser();
        //    WebServiceUser.sanMarUserName = "DANIELLEMOORE1";
        //    WebServiceUser.sanMarUserPassword = "danie71046";
        //    WebServiceUser.sanMarCustomerNumber = "71046";

        //    com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService service = new com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService();

        //    com.sanmar.ws_sanmarpricingserviceport.responseBean response = service.getPricing(Items, WebServiceUser);
        //    return true;
        //}

        public static bool SyncDev(ImageSolutions.Item.Item Item)
        {
            test_sm_pricingserviceport.item[] Items = new test_sm_pricingserviceport.item[1];

            test_sm_pricingserviceport.item SanMarItem = new test_sm_pricingserviceport.item();
            SanMarItem.color = Item.Color;
            SanMarItem.size = Item.SizeCode;
            SanMarItem.style = Item.StyleNumber;
            Items[0] = SanMarItem;

            test_sm_pricingserviceport.webServiceUser WebServiceUser = new test_sm_pricingserviceport.webServiceUser();
            //WebServiceUser.sanMarUserName = "DANIELLEMOORE1";
            //WebServiceUser.sanMarUserPassword = "danie71046";
            //WebServiceUser.sanMarCustomerNumber = "71046";

            WebServiceUser.sanMarUserName = "71046ws";
            WebServiceUser.sanMarUserPassword = "12341234";
            WebServiceUser.sanMarCustomerNumber = "71046";

            test_sm_pricingserviceport.SanMarPricingServiceService service = new test_sm_pricingserviceport.SanMarPricingServiceService();

            test_sm_pricingserviceport.responseBean response = service.getPricing(Items, WebServiceUser);

            if (response.errorOccurred)
            {
                throw new Exception(string.Format("{0}", response.message));
            }

            test_sm_pricingserviceport.item updatedItem = (test_sm_pricingserviceport.item)response.listResponse[0];
            var pricing = updatedItem.myPrice;

            Item.PurchasePrice = (double?)pricing;
            Item.Update();

            return true;
        }

        public static bool Sync(ImageSolutions.Item.Item Item)
        {
            com.sanmar.ws_sanmarpricingserviceport.item[] Items = new com.sanmar.ws_sanmarpricingserviceport.item[1];

            com.sanmar.ws_sanmarpricingserviceport.item SanMarItem = new com.sanmar.ws_sanmarpricingserviceport.item();
            SanMarItem.color = Item.Color;
            SanMarItem.size = Item.SizeCode;
            SanMarItem.style = Item.StyleNumber;
            Items[0] = SanMarItem;

            com.sanmar.ws_sanmarpricingserviceport.webServiceUser WebServiceUser = new com.sanmar.ws_sanmarpricingserviceport.webServiceUser();
            //WebServiceUser.sanMarUserName = "DANIELLEMOORE1";
            //WebServiceUser.sanMarUserPassword = "danie71046";
            //WebServiceUser.sanMarCustomerNumber = "71046";

            WebServiceUser.sanMarUserName = "71046ws";
            WebServiceUser.sanMarUserPassword = "12341234";
            WebServiceUser.sanMarCustomerNumber = "71046";

            com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService service = new com.sanmar.ws_sanmarpricingserviceport.SanMarPricingServiceService();

            com.sanmar.ws_sanmarpricingserviceport.responseBean response = service.getPricing(Items, WebServiceUser);

            if(response.errorOccurred)
            {
                throw new Exception(string.Format("{0}", response.message));
            }

            com.sanmar.ws_sanmarpricingserviceport.item updatedItem = (com.sanmar.ws_sanmarpricingserviceport.item)response.listResponse[0];
            var pricing = updatedItem.myPrice;

            Item.PurchasePrice = (double?)pricing;
            Item.Update();

            return true;
        }
    }
}
