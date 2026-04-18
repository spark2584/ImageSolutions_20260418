using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskProcess.Task
{
    public class CreateNetSuiteFulfillment
    {
        public void Execute()
        {
            List<ImageSolutions.Fulfillment.Fulfillment> ISFulfillments = null;
            ImageSolutions.Fulfillment.FulfillmentFilter ISFulfillmentFilter = null;
            NetSuiteLibrary.Fulfillment.Fulfillment NetSuiteFulfillment = null;
            NetSuiteLibrary.Fulfillment.FulfillmentFilter objFilter = null;

            try
            {
                ISFulfillmentFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
                ISFulfillmentFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                ISFulfillmentFilter.InternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                ISFulfillmentFilter.PurchaseOrderID = new Database.Filter.StringSearch.SearchFilter();
                ISFulfillmentFilter.PurchaseOrderID.SearchString = "3";

                //ISFulfillmentFilter.PurchaseOrderID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;

                ISFulfillments = ImageSolutions.Fulfillment.Fulfillment.GetFulfillments(ISFulfillmentFilter);

                foreach (ImageSolutions.Fulfillment.Fulfillment _Fulfillment in ISFulfillments)
                {
                    try
                    {
                        NetSuiteFulfillment = new NetSuiteLibrary.Fulfillment.Fulfillment(_Fulfillment);
                        //NetSuiteFulfillment.CreateFulfillment();
                    }
                    catch (Exception ex)
                    {
                    }
                }

            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));


            }
            finally
            {

            }
        }
    }
}
