using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;


namespace ScheduledTask.Task
{
    public class Test : NetSuiteBase
    {
        public bool ExecuteItemReceipt()
        {
            try
            {

                NetSuiteLibrary.ItemReceipt.ItemReceipt NSItemReceipt = new NetSuiteLibrary.ItemReceipt.ItemReceipt(); ;
                NSItemReceipt.LoadNetSuiteInboundShipment();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            { }
            return true;
        }
        public bool ExecuteCreateCustomer()
        {
            try
            {

                NetSuiteLibrary.ItemReceipt.ItemReceipt NSItemReceipt = new NetSuiteLibrary.ItemReceipt.ItemReceipt(); ;
                NSItemReceipt.LoadNetSuiteInboundShipment();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            { }
            return true;
        }

        public bool PrintSalesOrder()
        {
            try
            {
                NetSuiteLibrary.SalesOrder.SalesOrder NSSalesOrder = null;

                NetSuiteLibrary.SalesOrder.SalesOrderFilter objFilter = new NetSuiteLibrary.SalesOrder.SalesOrderFilter();
                objFilter.InternalIDs = new List<string>();
                objFilter.InternalIDs.Add("37678826");

                NSSalesOrder = NetSuiteLibrary.SalesOrder.SalesOrder.GetSalesOrder(Service, objFilter);

                if(NSSalesOrder.NetSuiteSalesOrder != null)
                {
                    Console.WriteLine(string.Format("{0}", NSSalesOrder.NetSuiteSalesOrder.internalId));

                    string strInternalID = "4252261";


                    NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;

                
                    NSSalesOrder.GetFile(strInternalID);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            { }
            return true;
        }
    }
}
