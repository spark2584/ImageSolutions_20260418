using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestCreateSanMarPurchaseOrder
    {
        public static bool StartDev()
        {
            test_sm_poservicebinding.SendPORequest request = new test_sm_poservicebinding.SendPORequest();
            request.id = "71046ws";
            request.password = "12341234";

            test_sm_poservicebinding.PO PO = new test_sm_poservicebinding.PO();
            PO.orderNumber = "TestPO123";
            PO.orderDate = DateTime.Now;
            PO.totalAmount = 0;
            PO.termsAndConditions = "TestPO123";

            test_sm_poservicebinding.ContactDetails contactDetails = new test_sm_poservicebinding.ContactDetails();
            contactDetails.attentionTo = "Test attention";
            contactDetails.address1 = "Addr123";
            contactDetails.address2 = "Addr123";
            contactDetails.city = "City123";
            contactDetails.region = "Region123";
            contactDetails.postalCode = "123456";
            contactDetails.countrySpecified = true;

            contactDetails.country = test_sm_poservicebinding.ContactDetailsCountry.US;

            PO.OrderContactArray[0] = new test_sm_poservicebinding.Contact();
            PO.OrderContactArray[0].ContactDetails = contactDetails;

            PO.LineItemArray = new test_sm_poservicebinding.LineItem[4];
            PO.LineItemArray[0].lineNumber++;
            PO.LineItemArray[0].description = "Item 123";

            test_sm_poservicebinding.Quantity quantity = new test_sm_poservicebinding.Quantity();
            quantity.value = 1;

            PO.LineItemArray[0].Quantity = quantity;
            PO.LineItemArray[0].productId = "123";

            test_sm_poservicebinding.Part[] parts = new test_sm_poservicebinding.Part[1];
            parts[0].partId = "123";

            PO.LineItemArray[0].PartArray = parts;

            test_sm_poservicebinding.POService service = new test_sm_poservicebinding.POService();
            test_sm_poservicebinding.SendPOResponse response = service.sendPO(request);

            //response.ServiceMessageArray[0].
            // response.Inventory.PartInventoryArray[0].quantityAvailable.Quantity

            return true;
        }
        public static bool Start()
        {
            com.sanmar.ws_poservicebinding.SendPORequest request = new com.sanmar.ws_poservicebinding.SendPORequest();
            request.id = "daniellesorge";
            request.password = "image11";

            com.sanmar.ws_poservicebinding.PO PO = new com.sanmar.ws_poservicebinding.PO();
            PO.orderNumber = "TestPO123";
            PO.orderDate = DateTime.Now;
            PO.totalAmount = 0;
            PO.termsAndConditions = "TestPO123";

            com.sanmar.ws_poservicebinding.ContactDetails contactDetails = new com.sanmar.ws_poservicebinding.ContactDetails();
            contactDetails.attentionTo = "Test attention";
            contactDetails.address1 = "Addr123";
            contactDetails.address2 = "Addr123";
            contactDetails.city = "City123";
            contactDetails.region = "Region123";
            contactDetails.postalCode = "123456";
            contactDetails.countrySpecified = true;

            contactDetails.country = com.sanmar.ws_poservicebinding.ContactDetailsCountry.US;

            PO.OrderContactArray[0] = new com.sanmar.ws_poservicebinding.Contact();
            PO.OrderContactArray[0].ContactDetails = contactDetails;

            PO.LineItemArray = new com.sanmar.ws_poservicebinding.LineItem[4];
            PO.LineItemArray[0].lineNumber++;
            PO.LineItemArray[0].description = "Item 123";

            com.sanmar.ws_poservicebinding.Quantity quantity = new com.sanmar.ws_poservicebinding.Quantity();
            quantity.value = 1;

            PO.LineItemArray[0].Quantity = quantity;
            PO.LineItemArray[0].productId = "123";

            com.sanmar.ws_poservicebinding.Part[] parts = new com.sanmar.ws_poservicebinding.Part[1];
            parts[0].partId = "123";

            PO.LineItemArray[0].PartArray = parts;

            com.sanmar.ws_poservicebinding.POService service = new com.sanmar.ws_poservicebinding.POService();
            com.sanmar.ws_poservicebinding.SendPOResponse response = service.sendPO(request);

            //response.ServiceMessageArray[0].
            // response.Inventory.PartInventoryArray[0].quantityAvailable.Quantity

            return true;
        }
    }
}
