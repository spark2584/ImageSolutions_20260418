using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTaskProcess.Task
{
    public class CloseSOByPO : NetSuiteBase
    {
        public void Execute(ImageSolutions.TaskEntry.TaskEntry taskentry)
        {
            NetSuiteLibrary.SalesOrder.SalesOrderFilter objFilter = null;
            NetSuiteLibrary.SalesOrder.SalesOrder SalesOrder = null;
            string strSONumber = string.Empty;

            try
            {
                objFilter = new NetSuiteLibrary.SalesOrder.SalesOrderFilter();

                objFilter.InternalIDs = new List<string>();


                if (!string.IsNullOrEmpty(taskentry.ExternalID))
                {
                    objFilter.InternalIDs.Add(taskentry.ExternalID);
                }
                //else if(!string.IsNullOrEmpty(taskentry.Parameter))
                //{
                //    //NetSuitePurchaseOrderFilter.ponu = new List<string>();
                //    //NetSuitePurchaseOrderFilter.InternalIDs.Add("21173511");
                //}
                else
                {
                    throw new Exception("Task Entry missing data");
                }

                SalesOrder = NetSuiteLibrary.SalesOrder.SalesOrder.GetSalesOrder(Service, objFilter);

                if (SalesOrder.NetSuiteSalesOrder == null)
                {
                    throw new Exception("Unable to locate NetSuite Sales Order");
                }

                if (SalesOrder.NetSuiteSalesOrder.entity == null)
                {
                    throw new Exception("Unable to identify Customer");
                }

                string strCustomerInternalID = SalesOrder.NetSuiteSalesOrder.entity.internalId;

                //if (!(SalesOrder.NetSuiteSalesOrder.status == "Pending Fulfillment" || SalesOrder.NetSuiteSalesOrder.status == "Partially Fulfilled"))
                //{
                //    throw new Exception("Status is no longer pending receipt");
                //}

                strSONumber = SalesOrder.NetSuiteSalesOrder.tranId;

                if (SalesOrder.CloseByPO())
                {
                    Console.WriteLine(string.Format("Sales Order {0} Closed", strSONumber));

                    SalesOrder = NetSuiteLibrary.SalesOrder.SalesOrder.GetSalesOrder(Service, objFilter);

                    NetSuiteLibrary.Customer.CustomerFilter objCustomerFilter = new NetSuiteLibrary.Customer.CustomerFilter();
                    objCustomerFilter.CustomerInternalIDs = new List<string>();
                    objCustomerFilter.CustomerInternalIDs.Add(strCustomerInternalID);

                    NetSuiteLibrary.Customer.Customer objCustomer = NetSuiteLibrary.Customer.Customer.GetCustomer(Service, objCustomerFilter);
                    string strCustomerEmail = objCustomer.NetSuiteCustomer.email;

                    if (string.IsNullOrEmpty(strCustomerEmail))
                    {
                        throw new Exception("Customer email is invalid");
                    }
                    else
                    {
                        SendEmail(strCustomerEmail, SalesOrder);
                    }

                    foreach (SalesOrderItem salesOrderItem in SalesOrder.NetSuiteSalesOrder.itemList.item)
                    {
                        NetSuiteLibrary.com.netsuite.webservices.SelectCustomFieldRef strEmbroideryPO = (SelectCustomFieldRef)NetSuiteHelper.GetCustomFieldValue(salesOrderItem, "custcol_emb_po");

                        if (strEmbroideryPO != null && salesOrderItem.isClosed)
                        {
                            ImageSolutions.TaskEntry.TaskEntry TaskEntry = new ImageSolutions.TaskEntry.TaskEntry();
                            TaskEntry.TaskID = "4";
                            TaskEntry.Parameter = "Embroidery PO";
                            TaskEntry.ExternalID = strEmbroideryPO.value.internalId;
                            TaskEntry.Status = "Pending";
                            TaskEntry.Create();
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

        public bool SendEmail(string email, NetSuiteLibrary.SalesOrder.SalesOrder nssalesorder)
        {
            try
            {
                SmtpClient SmptClient = new SmtpClient("smtp.office365.com", 587);
                SmptClient.UseDefaultCredentials = false;
                SmptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmptClient.Credentials = new System.Net.NetworkCredential("CS@imageinc.com", "S0lutions2022");
                //SmptClient.Credentials = new System.Net.NetworkCredential("bliou97@outlook.com", "Starwarsbl22625!");

                SmptClient.EnableSsl = true;

                string strItems = string.Empty;

                foreach (NetSuiteLibrary.com.netsuite.webservices.SalesOrderItem objSalesOrderItem in nssalesorder.NetSuiteSalesOrder.itemList.item)
                {
                    if (objSalesOrderItem.isClosed == true)
                    {
                        strItems += objSalesOrderItem.description + " " + objSalesOrderItem.quantity + " unit(s)" + Environment.NewLine;
                    }
                }

                MailMessage MailMessage = new MailMessage();
                MailMessage.From = new MailAddress("CS@imageinc.com", "Image Solutions");
                MailMessage.Bcc.Add(new MailAddress("brandon@imageinc.com"));
                MailMessage.To.Add(new MailAddress(email));
                //MailMessage.To.Add(new MailAddress("brandon@imageinc.com"));

                MailMessage.Subject = "Image Solution Item Cancellation SO " + nssalesorder.NetSuiteSalesOrder.tranId;
                MailMessage.Body = "Dear Valued Customer," + Environment.NewLine + Environment.NewLine +
                                    "Please be advised that one or more items on your order are currently not available due to a back order. " + Environment.NewLine +
                                    "These item(s) have been cancelled and a refund is being issued. " + Environment.NewLine +
                                    "Please allow 3-5 days for your refund to process to your original form of payment. " + Environment.NewLine + Environment.NewLine +
                                    "Canceled items: " + Environment.NewLine +
                                    strItems + Environment.NewLine +
                                    "For any additional questions regarding this notification, " +
                                    "please contact Customer Service M-F 7a-4p PST at 888-756-9898 or respond to this e-mail. " + Environment.NewLine + Environment.NewLine +
                                    "Thank you for your business, " + Environment.NewLine +
                                    "Image Solutions Customer Service" + Environment.NewLine + Environment.NewLine +
                                    "Please REPLY ALL to this e-mail with any responses";

                SmptClient.Send(MailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
