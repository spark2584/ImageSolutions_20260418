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
    public class UpdateSalesOrderStatus : NetSuiteBase
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"


SELECT s.NetSuiteInternalID, s.Status, s.SalesOrderID
FROM SalesOrder s

--Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = s.UserWebsiteID
--Inner Join EnterpriseCustomer (NOLOCK) ec on ec.EmployeeID = uw.EmployeeID and ec.IsIndividual = 1

WHERE 
s.Status not in ('Closed','Rejected','Cancelled','Completed','Billed')
and isnull(s.NetSuiteInternalID,'') != ''

--s.Status = ('Billed')
--and s.websiteid = 20
--and ec.InActive = 1

--s.SalesOrderID in (
--808399
--)


");
                objRead = Database.GetDataReader(strSQL);

                int counter = 0;
                while (objRead.Read())
                {
                    try
                    {
                        counter++;
                        Console.WriteLine(String.Format("{0}. {1} - {2} ", counter, Convert.ToString(objRead["NetSuiteInternalID"]), Convert.ToString(objRead["SalesOrderID"])));
                        NetSuiteLibrary.SalesOrder.SalesOrder NSSalesOrder = new NetSuiteLibrary.SalesOrder.SalesOrder(Convert.ToString(objRead["NetSuiteInternalID"]));

                        Console.WriteLine(String.Format("NS - {0}", NSSalesOrder.NetSuiteSalesOrder.status));

                        if (NSSalesOrder != null)
                        {
                            ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));

                            if (SalesOrder != null)
                            {
                                Console.WriteLine(String.Format("DB - {0}", SalesOrder.Status));

                                SalesOrder.Status = NSSalesOrder.NetSuiteSalesOrder.status;
                                SalesOrder.Update();

                                Console.WriteLine(String.Format("{0} - {1}", SalesOrder.NetSuiteInternalID, SalesOrder.Status));
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Can not find Sales Order with Internal ID"))
                        {
                            ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(Convert.ToString(objRead["SalesOrderID"]));

                            if (SalesOrder != null)
                            {
                                Console.WriteLine(String.Format("DB - {0}", SalesOrder.Status));

                                SalesOrder.Status = "Closed";
                                SalesOrder.Update();

                                Console.WriteLine(String.Format("{0} - {1}", SalesOrder.NetSuiteInternalID, SalesOrder.Status));
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
            return true;
        }
    }
}
