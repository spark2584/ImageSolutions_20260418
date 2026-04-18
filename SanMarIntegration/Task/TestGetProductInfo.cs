using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestGetProductInfo
    {
        //webservice: https://ws.sanmar.com:8080/SanMarWebService/SanMarProductInfoServicePort?wsdl
        public static bool Start()
        {
            com.sanmar.ws_sanmarproductinfoserviceport.product[] Products = new com.sanmar.ws_sanmarproductinfoserviceport.product[1];

            com.sanmar.ws_sanmarproductinfoserviceport.product Product = new com.sanmar.ws_sanmarproductinfoserviceport.product();
            Product.color = "Vivid Green";
            Product.size = "2XL";
            Product.style = "286772";
            Products[0] = Product;

            com.sanmar.ws_sanmarproductinfoserviceport.webServiceUser WebServiceUser = new com.sanmar.ws_sanmarproductinfoserviceport.webServiceUser();
            WebServiceUser.sanMarUserName = "DANIELLEMOORE1";
            WebServiceUser.sanMarUserPassword = "danie71046";
            WebServiceUser.sanMarCustomerNumber = "71046";

            com.sanmar.ws_sanmarproductinfoserviceport.SanMarProductInfoServiceService service = new com.sanmar.ws_sanmarproductinfoserviceport.SanMarProductInfoServiceService();

            com.sanmar.ws_sanmarproductinfoserviceport.productInfoResponseBean response = service.getProductInfoByStyleColorSize(Products, WebServiceUser);

            return true;
        }
    }
}
