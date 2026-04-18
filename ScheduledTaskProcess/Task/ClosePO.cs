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
    public class ClosePO : NetSuiteBase
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

                string strVendorInternalID = NetSuitePurchaseOrder.NetSuitePurchaseOrder.entity.internalId;

                if (NetSuitePurchaseOrder.Close())
                {
                    Console.WriteLine(string.Format("PO closed InternalID: {0}", taskentry.ExternalID));

                    NetSuitePurchaseOrder = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(Service, NetSuitePurchaseOrderFilter);

                    NetSuiteLibrary.Vendor.VendorFilter objVendorFilter = new NetSuiteLibrary.Vendor.VendorFilter();
                    objVendorFilter.VendorInternalIDs = new List<string>();
                    objVendorFilter.VendorInternalIDs.Add(strVendorInternalID);

                    NetSuiteLibrary.Vendor.Vendor objVendor = NetSuiteLibrary.Vendor.Vendor.GetVendor(Service, objVendorFilter);

                    if (string.IsNullOrEmpty(objVendor.NetSuiteVendor.email))
                    {
                        Console.WriteLine(string.Format("Vendor {0} has no email", objVendor.NetSuiteVendor.entityId));
                    }
                    string strVendorEmail = objVendor.NetSuiteVendor.email;
                    string strVendorCompanyName = objVendor.NetSuiteVendor.companyName;

                    if (taskentry.Parameter != "Embroidery PO")
                    {
                        if (string.IsNullOrEmpty(strVendorEmail) || string.IsNullOrEmpty(strVendorCompanyName))
                        {
                            throw new Exception("Vendor email or company name is invalid");
                        }
                        else
                        {
                            SendEmail(objVendor, NetSuitePurchaseOrder);
                        }

                        ImageSolutions.TaskEntry.TaskEntry TaskEntry = new ImageSolutions.TaskEntry.TaskEntry();
                        TaskEntry.TaskID = "5";
                        TaskEntry.ExternalID = NetSuitePurchaseOrder.NetSuitePurchaseOrder.createdFrom.internalId;
                        TaskEntry.Status = "Pending";
                        TaskEntry.ScheduledRunDate = DateTime.UtcNow.AddDays(7);

                        TaskEntry.Create();

                        Console.WriteLine(string.Format("TaskEntryID {0}, TaskID {1} Created", TaskEntry.TaskEntryID, TaskEntry.TaskID));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                NetSuitePurchaseOrderFilter = null;
                NetSuitePurchaseOrder = null;
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
                    if (objPurchaseOrderItem.isClosed == true)
                    {
                        strItems += objPurchaseOrderItem.vendorName + " " + objPurchaseOrderItem.quantity + " unit(s)" + Environment.NewLine;
                    }
                }

                MailMessage.Subject = "Image Solution Item Cancellation PO " + nspurchaseorder.NetSuitePurchaseOrder.tranId + " for " + nsvendor.NetSuiteVendor.companyName;
                MailMessage.Body = "Hello " + nsvendor.NetSuiteVendor.companyName + " Team," + Environment.NewLine + Environment.NewLine +
                                   "Please see below for items canceled from PO " + nspurchaseorder.NetSuitePurchaseOrder.tranId + ". Please confirm you have canceled these items and they will not ship." + Environment.NewLine + Environment.NewLine +
                                   "**Shipment of these items will result in returned goods with a provided shipping label from you**" + Environment.NewLine + Environment.NewLine +
                                   "Canceled items: " + Environment.NewLine +
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
