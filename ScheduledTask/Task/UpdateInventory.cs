using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using SanMarIntegration;

namespace ScheduledTask.Task
{
    public class UpdateInventory
    {
        public bool UpdateVendorInventory(ImageSolutions.Item.Item item)
        {
            try
            {
                //SanMarIntegration.Task.TestInvoicing.Start
                SanMarIntegration.Task.TestCreateSanMarPurchaseOrder.StartDev();

                //SanMarIntegration.Task.TestCreateSanMarPurchaseOrder.Start();
                //SanMarIntegration.Task.TestGetProductInfo.Start();

                //SanMarIntegration.Task.TestGetPricingAndConfiguration.Start();

                //SanMarIntegration.Task.TestGetPricing.Start();
                SanMarIntegration.Task.TestGetPricing.SyncDev(item);

                //SanMarIntegration.Task.TestGetPricing.Sync(item);

                //SanMarIntegration.Task.TestGetInventory.Start();
                SanMarIntegration.Task.TestGetInventory.Sync(item);
            }
            catch(Exception ex)
            {
                item.ErrorMessage = ex.Message;
                item.Update();
            }

            return true;
        }
    }
}
