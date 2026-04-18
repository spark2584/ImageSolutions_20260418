using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask.Task
{
    public class ClosePurchaseOrder : NetSuiteBase
    {
        public bool Close()
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
                NetSuitePurchaseOrders = new List<NetSuiteLibrary.PurchaseOrder.PurchaseOrder>();

                //staging
                //objSearchResult = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "23251");

                //production
                //objSearchResult = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrderSavedSearch(Service, "118379");

                NetSuitePurchaseOrderFilter.InternalIDs = new List<string>();
                NetSuitePurchaseOrderFilter.InternalIDs.Add("34870966");

                //foreach (TransactionSearchRow objRow in objSearchResult.searchRowList)
                //{
                //    NetSuitePurchaseOrderFilter.InternalIDs.Add(objRow.basic.internalId[0].searchValue.internalId);
                //}


                NetSuitePurchaseOrders = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(Service, NetSuitePurchaseOrderFilter);

                if (NetSuitePurchaseOrders != null)
                {
                    if (NetSuitePurchaseOrders != null && NetSuitePurchaseOrders.Count > 0)
                    {
                        Parallel.ForEach(NetSuitePurchaseOrders, new ParallelOptions { MaxDegreeOfParallelism = 1 },
                        purchaseOrder =>
                        {
                            if (purchaseOrder.NetSuitePurchaseOrder != null)
                            {
                                Console.WriteLine("Closing PurchaseOrder: " + purchaseOrder.NetSuitePurchaseOrder.tranId);
                                purchaseOrder.Close();

                                if(purchaseOrder.NetSuitePurchaseOrder.createdFrom != null)
                                {
                                    NetSuiteLibrary.SalesOrder.SalesOrderFilter NSSalesOrderFilter = new NetSuiteLibrary.SalesOrder.SalesOrderFilter();
                                    NSSalesOrderFilter.InternalIDs = new List<string>();
                                    NSSalesOrderFilter.InternalIDs.Add(purchaseOrder.NetSuitePurchaseOrder.createdFrom.internalId);

                                    NetSuiteLibrary.SalesOrder.SalesOrder NSSalesOrder = NetSuiteLibrary.SalesOrder.SalesOrder.GetSalesOrder(Service, NSSalesOrderFilter);
                                    //NSSalesOrder.
                                }
                            }
                        });
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
            return true;

        }

      
    }
}
