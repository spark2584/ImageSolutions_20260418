using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveWearIntegration.Task
{
    public class CreateOrder
    {
        public static bool Start()
        {
            ActiveWearAPI.Request request = new ActiveWearAPI.Request();
            request.POST_Orders();

            //request.CreateOrder();


            return true;
        }

        public static bool Execute()
        {
            List<ImageSolutions.PurchaseOrder.PurchaseOrder> objPurchaseOrder = null;
            ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;
            ActiveWearAPI.Request request = null;

            try
            {
                objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();

                //objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //objFilter.ExternalID.SearchString = "B54738507";

                objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ExternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                //objFilter.PurchaseOrderID = new Database.Filter.StringSearch.SearchFilter();
                //objFilter.PurchaseOrderID.SearchString = "2";


                objPurchaseOrder = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(objFilter);

                int count = 0;
                int totalCount = 0;
                foreach (ImageSolutions.PurchaseOrder.PurchaseOrder _purchaseorder in objPurchaseOrder)
                {
                    try
                    {
                        Console.WriteLine(string.Format("{0}/{1}", totalCount, objPurchaseOrder.Count));

                        if (count == 60)
                        {
                            Thread.Sleep(60000);
                            count = 0;
                        }

                        request = new ActiveWearAPI.Request();
                        List<ActiveWearAPI.OrderHistory.Header> Orders = null;// request.CreateOrder(_purchaseorder);

                        _purchaseorder.ExternalID = Orders[0].orderNumber;
                        _purchaseorder.ErrorMessage = string.Empty;
                        _purchaseorder.Update();

                        count++;
                        totalCount++;
                    }
                    catch(Exception ex)
                    {
                        _purchaseorder.ErrorMessage = ex.Message;
                        _purchaseorder.Update();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                request = null;
                objPurchaseOrder = null;
                objFilter = null;
            }
            return true;
        }
    }
}
