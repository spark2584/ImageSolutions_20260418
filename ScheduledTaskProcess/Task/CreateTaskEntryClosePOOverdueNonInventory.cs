using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTaskProcess.Task
{
    public class CreateTaskEntryClosePOOverdueNonInventory : NetSuiteBase
    {
        public void Execute()
        {
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter NetSuitePurchaseOrderFilter = null;
            List<NetSuiteLibrary.PurchaseOrder.PurchaseOrder> NetSuitePurchaseOrders = null;
            NetSuiteLibrary.Item.Item Item = null;
            TransactionSearchAdvanced objTransactionSearchAdvanced = null;
            SearchResult objSearchResult = null;
            List<string> InternalIDs = null;

            try
            {
                NetSuitePurchaseOrderFilter = new NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter();

                //staging
                //NetSuitePurchaseOrderFilter.InternalIDs = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "23251");

                //Production
                NetSuitePurchaseOrderFilter.InternalIDs = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "118379");

                NetSuitePurchaseOrders = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(Service, NetSuitePurchaseOrderFilter);

                if (NetSuitePurchaseOrders != null)
                {
                    if (NetSuitePurchaseOrders != null && NetSuitePurchaseOrders.Count > 0)
                    {
                        foreach (NetSuiteLibrary.PurchaseOrder.PurchaseOrder _purchaseorder in NetSuitePurchaseOrders)
                        {
                            if (_purchaseorder.NetSuitePurchaseOrder != null)
                            {
                                ImageSolutions.TaskEntry.TaskEntryFilter objFilter = new ImageSolutions.TaskEntry.TaskEntryFilter();
                                objFilter.TaskID = new Database.Filter.StringSearch.SearchFilter();
                                objFilter.TaskID.SearchString = "6";
                                objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                                objFilter.ExternalID.SearchString = _purchaseorder.NetSuitePurchaseOrder.internalId;

                                ImageSolutions.TaskEntry.TaskEntry TaskEntry = ImageSolutions.TaskEntry.TaskEntry.GetTaskEntry(objFilter);

                                if (TaskEntry == null)
                                {
                                    TaskEntry = new ImageSolutions.TaskEntry.TaskEntry();
                                    TaskEntry.TaskID = "6";
                                    TaskEntry.ExternalID = _purchaseorder.NetSuitePurchaseOrder.internalId;
                                    TaskEntry.Status = "Pending";
                                    TaskEntry.Create();

                                    Console.WriteLine(string.Format("Task {0} created with ExternalID: {1}", TaskEntry.TaskID, TaskEntry.ExternalID));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
    }
}
