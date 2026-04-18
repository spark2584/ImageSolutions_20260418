using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestGetPricingAndConfiguration
    {
        //webservice: https:'//ws.sanmar.com:8080/promostandards/PricingAndConfigurationServiceBinding?wsdl

        public static bool Start()
        {
            com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingRequest request = new com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingRequest();
            request.wsVersion = "1.0.0";
            request.id = "daniellesorge";
            request.password = "image11";
            request.productId = "286772";
            request.partId = "431771";
            request.currency = com.sanmar.ws_pricingandconfigurationservicebinding.CurrencyCodeType.USD;
            request.fobId = "1";
            request.priceType = com.sanmar.ws_pricingandconfigurationservicebinding.priceType.Customer;
            request.localizationCountry = "US";
            request.localizationLanguage = "EN";
            request.configurationType = com.sanmar.ws_pricingandconfigurationservicebinding.configurationType.Blank;

           
            com.sanmar.ws_pricingandconfigurationservicebinding.PricingAndConfigurationService service = new com.sanmar.ws_pricingandconfigurationservicebinding.PricingAndConfigurationService();
            com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingResponse response = service.getConfigurationAndPricing(request);
            return true;
        }

        //public static bool Start()
        //{
        //    com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingRequest request = new com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingRequest();
        //    request.wsVersion = "1.0.0";
        //    request.id = "daniellesorge";
        //    request.password = "image11";
        //    request.productId = "F226";
        //    request.partId = "695553";
        //    request.currency = com.sanmar.ws_pricingandconfigurationservicebinding.CurrencyCodeType.USD;
        //    request.fobId = "1";
        //    request.priceType = com.sanmar.ws_pricingandconfigurationservicebinding.priceType.Customer;
        //    request.localizationCountry = "US";
        //    request.localizationLanguage = "EN";
        //    request.configurationType = com.sanmar.ws_pricingandconfigurationservicebinding.configurationType.Blank;


        //    com.sanmar.ws_pricingandconfigurationservicebinding.PricingAndConfigurationService service = new com.sanmar.ws_pricingandconfigurationservicebinding.PricingAndConfigurationService();
        //    com.sanmar.ws_pricingandconfigurationservicebinding.GetConfigurationAndPricingResponse response = service.getConfigurationAndPricing(request);
        //    return true;
        //}
    }
}
