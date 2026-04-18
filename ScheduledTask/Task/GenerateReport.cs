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
    public class GenerateReport : NetSuiteBase
    {
        public bool ClosedPO()
        {
            List<NetSuiteLibrary.PurchaseOrder.PurchaseOrder> NSPurchaseOrders = null;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter objNSPOFilter = null;

            List<ImageSolutions.TaskEntry.TaskEntry> objTaskEntries = null;
            ImageSolutions.TaskEntry.TaskEntryFilter objTaskEntryFilter = new ImageSolutions.TaskEntry.TaskEntryFilter();

            try
            {
                objTaskEntryFilter = new ImageSolutions.TaskEntry.TaskEntryFilter();
                objTaskEntryFilter.TaskID = new Database.Filter.StringSearch.SearchFilter();
                objTaskEntryFilter.TaskID.SearchString = "4";
                objTaskEntryFilter.Status = new Database.Filter.StringSearch.SearchFilter();
                objTaskEntryFilter.Status.SearchString = "Completed";

                
                objTaskEntries = ImageSolutions.TaskEntry.TaskEntry.GetTaskEntries(objTaskEntryFilter);

                if(objTaskEntries != null || objTaskEntries.Count > 0)
                {
                    objNSPOFilter = new NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter();
                    objNSPOFilter.InternalIDs = new List<string>();

                    foreach(ImageSolutions.TaskEntry.TaskEntry _task in objTaskEntries)
                    {
                        if(!string.IsNullOrEmpty(_task.ExternalID) && _task.TaskID == "4")
                        objNSPOFilter.InternalIDs.Add(_task.ExternalID);
                    }

                    NSPurchaseOrders = NetSuiteLibrary.PurchaseOrder.PurchaseOrder.GetPurchaseOrders(Service, objNSPOFilter);

                    SendEmail(NSPurchaseOrders, objTaskEntries);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }

            return true;
        }

        public bool SendEmail(List<NetSuiteLibrary.PurchaseOrder.PurchaseOrder> NSPurchaseorders, List<ImageSolutions.TaskEntry.TaskEntry> TaskEntries)
        {
            try
            {
                SmtpClient SmptClient = new SmtpClient("smtp.office365.com", 587);
                SmptClient.UseDefaultCredentials = false;
                SmptClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //SmptClient.Credentials = new System.Net.NetworkCredential("CS@imageinc.com", "S0lutions22");
                SmptClient.Credentials = new System.Net.NetworkCredential("brandon@imageinc.com", "Starwarsbl22625!");

                SmptClient.EnableSsl = true;

                MailMessage MailMessage = new MailMessage();
                MailMessage.From = new MailAddress("brandon@imageinc.com", "Image Solutions");
                //MailMessage.CC.Add(new MailAddress("domesticSC@imageinc.com"));
                //MailMessage.Bcc.Add(new MailAddress("brandon@imageinc.com"));
                MailMessage.To.Add(new MailAddress("brandon@imageinc.com"));
                //MailMessage.To.Add(new MailAddress("brandon@imageinc.com"));

                string strItems = string.Empty;

                string strClosedPO = string.Empty;

                double totalAmountClosed = 0;

                foreach(NetSuiteLibrary.PurchaseOrder.PurchaseOrder _po in NSPurchaseorders)
                {
                    foreach(PurchaseOrderItem _item in _po.NetSuitePurchaseOrder.itemList.item)
                    {
                        if(_item.isClosed)
                        {
                            totalAmountClosed += _item.amount;
                        }
                    }

                    strClosedPO += "PO InternalID: " + _po.NetSuitePurchaseOrder.internalId
                        + " - PO: " + _po.NetSuitePurchaseOrder.tranId
                        + " - Status: " + _po.NetSuitePurchaseOrder.status
                        + " - Amount Closed: $" + Convert.ToString(totalAmountClosed)
                        + " - DateClosed: " + Convert.ToString(TaskEntries.Find(m=>m.ExternalID == _po.NetSuitePurchaseOrder.internalId).ProcessedOn)
                        + Environment.NewLine ;

                    totalAmountClosed = 0;
                }


                MailMessage.Subject = "Image Solution Closed Non Inventory PO";
                MailMessage.Body = "Hello Team," + Environment.NewLine + Environment.NewLine +
                                   "Please see below for PO's closed " + Environment.NewLine + Environment.NewLine +
                                   "Canceled PO's: " + Environment.NewLine +
                                   strClosedPO + Environment.NewLine +
                                   "Thanks," + Environment.NewLine +
                                   "Image Solutions";

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
