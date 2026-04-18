using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanMarIntegration.Task
{
    public class TestGetInventory
    {
        //webservice: https://ws.sanmar.com:8080/promostandards/InventoryServiceBindingV2final?wsdl

        public static bool Start()
        {
            com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsRequest request = new com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsRequest();
            request.wsVersion = com.sanmar.ws_inventoryservicebindingvfinal.wsVersion.Item200;
            request.id = "daniellesorge";
            request.password = "image11";
            request.productId = "RH240"; //stylenumber

            com.sanmar.ws_inventoryservicebindingvfinal.Filter Filter = new com.sanmar.ws_inventoryservicebindingvfinal.Filter();
            string[] Parts = new string[1];
            Parts[0] = "1257391";//partid
            Filter.partIdArray = Parts;
            request.Filter = Filter;

            com.sanmar.ws_inventoryservicebindingvfinal.InventoryServiceV2final service = new com.sanmar.ws_inventoryservicebindingvfinal.InventoryServiceV2final();
            com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsResponse response = service.getInventoryLevels(request);

           // response.Inventory.PartInventoryArray[0].quantityAvailable.Quantity

            return true;
        }

        public static bool Sync(ImageSolutions.Item.Item Item)
        {
            try
            {
                com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsRequest request = new com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsRequest();
                request.wsVersion = com.sanmar.ws_inventoryservicebindingvfinal.wsVersion.Item200;
                request.id = "DANIELLEMOORE1";
                request.password = "danie71046";
                request.productId = Item.StyleNumber;

                com.sanmar.ws_inventoryservicebindingvfinal.Filter Filter = new com.sanmar.ws_inventoryservicebindingvfinal.Filter();
                string[] Parts = new string[1];
                Parts[0] = Item.UniqueKey;
                Filter.partIdArray = Parts;
                request.Filter = Filter;

                com.sanmar.ws_inventoryservicebindingvfinal.InventoryServiceV2final service = new com.sanmar.ws_inventoryservicebindingvfinal.InventoryServiceV2final();
                com.sanmar.ws_inventoryservicebindingvfinal.GetInventoryLevelsResponse response = service.getInventoryLevels(request);

                if (response.ServiceMessageArray != null)
                {
                    throw new Exception(string.Format("Code: {0}, Message: {1}", response.ServiceMessageArray[0].code, response.ServiceMessageArray[0].description));
                }

                decimal quantityAvailable = response.Inventory.PartInventoryArray[0].quantityAvailable.Quantity.value;
                decimal quantityCount = 0;
                for( int i = 0; i < response.Inventory.PartInventoryArray[0].InventoryLocationArray.Length; i++)
                {
                    quantityCount += response.Inventory.PartInventoryArray[0].InventoryLocationArray[i].inventoryLocationQuantity.Quantity.value;
                }

                Item.VendorInventory = quantityAvailable;
                Item.VendorInventoryLastUpdatedOn = DateTime.Now; //need to find out if UTC or PST
            //    Item.ErrorMessage = string.Empty;
                Item.Update();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return true;
        }
    }
}
