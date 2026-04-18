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
    public class SendClosePOWarning : NetSuiteBase
    {
        public void Execute(ImageSolutions.TaskEntry.TaskEntry taskentry)
        {
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter NetSuitePurchaseOrderFilter = null;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrder NetSuitePurchaseOrder = null;

            try
            {
                NetSuitePurchaseOrderFilter = new NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter();

                NetSuitePurchaseOrderFilter.InternalIDs = new List<string>();
                //NetSuitePurchaseOrderFilter.InternalIDs.Add("34870966");

                if (!string.IsNullOrEmpty(taskentry.ExternalID))
                {
                    NetSuitePurchaseOrderFilter.InternalIDs.Add(taskentry.ExternalID);
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

                NetSuitePurchaseOrder = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(Service, NetSuitePurchaseOrderFilter);

                if (NetSuitePurchaseOrder.NetSuitePurchaseOrder == null)
                {
                    throw new Exception("Unable to locate NetSuite Purchase Order");
                }

                if (NetSuitePurchaseOrder.NetSuitePurchaseOrder.createdFrom == null)
                {
                    throw new Exception("Unable to locate NetSuite Created From Sales Order");
                }

                if (NetSuitePurchaseOrder.NetSuitePurchaseOrder.entity == null)
                {
                    throw new Exception("Unable to identify Vendor");
                }

                if (!(NetSuitePurchaseOrder.NetSuitePurchaseOrder.status == "Pending Receipt" || NetSuitePurchaseOrder.NetSuitePurchaseOrder.status == "Partially Received"))
                {
                    throw new Exception("PO no long pending receipt");
                }

                double numPendingClose = 0;

                foreach (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrderItem objPurchaseOrderItem in NetSuitePurchaseOrder.NetSuitePurchaseOrder.itemList.item)
                {
                    if(objPurchaseOrderItem.quantityReceived < objPurchaseOrderItem.quantity)
                    {
                        numPendingClose += objPurchaseOrderItem.quantity - objPurchaseOrderItem.quantityReceived;
                    }
                }

                if(numPendingClose <= 0)
                {
                    throw new Exception("PO has no items to close");
                }

                string strVendorInternalID = NetSuitePurchaseOrder.NetSuitePurchaseOrder.entity.internalId;

                NetSuiteLibrary.Vendor.VendorFilter objVendorFilter = new NetSuiteLibrary.Vendor.VendorFilter();
                objVendorFilter.VendorInternalIDs = new List<string>();
                objVendorFilter.VendorInternalIDs.Add(strVendorInternalID);

                NetSuiteLibrary.Vendor.Vendor objVendor = NetSuiteLibrary.Vendor.Vendor.GetVendor(Service, objVendorFilter);
                string strVendorEmail = objVendor.NetSuiteVendor.email;
                string strVendorCompanyName = objVendor.NetSuiteVendor.companyName;
                if (string.IsNullOrEmpty(strVendorEmail) || string.IsNullOrEmpty(strVendorCompanyName))
                {
                    throw new Exception("Vendor email or company name is invalid");
                }
                else
                {
                    SendEmail(objVendor, NetSuitePurchaseOrder);
                }

                ImageSolutions.TaskEntry.TaskEntry TaskEntry = new ImageSolutions.TaskEntry.TaskEntry();
                TaskEntry.TaskID = "4";
                TaskEntry.ExternalID = NetSuitePurchaseOrder.NetSuitePurchaseOrder.internalId;
                TaskEntry.Status = "Pending";
                TaskEntry.ScheduledRunDate = DateTime.UtcNow.AddDays(5);

                TaskEntry.Create();

                Console.WriteLine(string.Format("Task {0} is created for PO {1}", TaskEntry.TaskID, NetSuitePurchaseOrder.NetSuitePurchaseOrder.tranId));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public bool SendEmail(NetSuiteLibrary.Vendor.Vendor nsvendor, NetSuiteLibrary.PurchaseOrder.PurchaseOrder nspurchaseorder)
        {
            try
            {
                SmtpClient SmptClient = new SmtpClient("smtp.office365.com", 587);
                SmptClient.UseDefaultCredentials = false;
                SmptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmptClient.Credentials = new System.Net.NetworkCredential("CS@imageinc.com", "S0lutions2022");
                //SmptClient.Credentials = new System.Net.NetworkCredential("bliou97@outlook.com", "Starwarsbl22625!");

                SmptClient.EnableSsl = true;

                MailMessage MailMessage = new MailMessage();
                MailMessage.From = new MailAddress("CS@imageinc.com", "Image Solutions");
                MailMessage.CC.Add(new MailAddress("domesticSC@imageinc.com"));
                MailMessage.Bcc.Add(new MailAddress("brandon@imageinc.com"));
                MailMessage.To.Add(new MailAddress(nsvendor.NetSuiteVendor.email));
                //MailMessage.To.Add(new MailAddress("brandon@imageinc.com"));

                string strItems = string.Empty;

                foreach (NetSuiteLibrary.com.netsuite.webservices.PurchaseOrderItem objPurchaseOrderItem in nspurchaseorder.NetSuitePurchaseOrder.itemList.item)
                {
                    if (objPurchaseOrderItem.quantity - objPurchaseOrderItem.quantityReceived > 0)
                    {
                        strItems += objPurchaseOrderItem.vendorName + " " + objPurchaseOrderItem.quantity + " unit(s)" + Environment.NewLine;
                    }
                }

                MailMessage.Subject = "Image Solution 5 day notification Item Cancellation PO  " + nspurchaseorder.NetSuitePurchaseOrder.tranId + " for " + nsvendor.NetSuiteVendor.companyName;
                MailMessage.Body = "Hello " + nsvendor.NetSuiteVendor.companyName + " Team," + Environment.NewLine + Environment.NewLine +
                                   "We have not yet received the below backordered items from PO " + nspurchaseorder.NetSuitePurchaseOrder.tranId + ". We will be cancelling these backorders in our system in 5 days if not received. We will notify you again once these items are cancelled in our system." + Environment.NewLine + Environment.NewLine +
                                   "**Shipment of these items after closure will result in returned goods with a provided shipping label form you**" + Environment.NewLine + Environment.NewLine +
                                   "Backordered items: " + Environment.NewLine +
                                   strItems + Environment.NewLine +
                                   "Thanks," + Environment.NewLine +
                                   "Image Solutions" + Environment.NewLine + Environment.NewLine +
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
