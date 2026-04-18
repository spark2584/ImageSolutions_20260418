using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueGenerationsIntegration.Task
{
    public class TestGetInventory
    {
        //webservice: https://promostandards.bluegeneration.com/Services/PromoInventoryService.svc?wsdl

        public static bool Start()
        {
            com.bluegeneration.promostandards_promoinventoryservice.GetInventoryLevelsRequest request = new com.bluegeneration.promostandards_promoinventoryservice.GetInventoryLevelsRequest();
            request.wsVersion = com.bluegeneration.promostandards_promoinventoryservice.wsVersion.Item200;
            request.id = "C011197";
            request.password = "BlueGen01!";
            request.productId = "BG7219";

            com.bluegeneration.promostandards_promoinventoryservice.Filter Filter = new com.bluegeneration.promostandards_promoinventoryservice.Filter();
            string[] Parts = new string[1];
            Parts[0] = "7219-NAV-2XT-SOLID";
            Filter.partIdArray = Parts;
            request.Filter = Filter;

            com.bluegeneration.promostandards_promoinventoryservice.BasicHttpBinding_InventoryService service = new com.bluegeneration.promostandards_promoinventoryservice.BasicHttpBinding_InventoryService();
            com.bluegeneration.promostandards_promoinventoryservice.GetInventoryLevelsResponse response = service.getInventoryLevels(request);

            return true;
        }
    }
}