using ImageSolutions.User;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutionsWebsiteConsole.Task
{
    public class SendFulfillmentEmail
    {
        public void Execute()
        {
            List<ImageSolutions.Fulfillment.Fulfillment> Fulfillments = null;
            ImageSolutions.Fulfillment.FulfillmentFilter FulfillmentFilter = null;

            try
            {
                Fulfillments = new List<ImageSolutions.Fulfillment.Fulfillment>();
                FulfillmentFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
                FulfillmentFilter.ShipConfirmationSent = false;

                //FulfillmentFilter.FulfillmentID = new Database.Filter.StringSearch.SearchFilter();
                //FulfillmentFilter.FulfillmentID.SearchString = "790896";

                Fulfillments = ImageSolutions.Fulfillment.Fulfillment.GetFulfillments(FulfillmentFilter);

                foreach(ImageSolutions.Fulfillment.Fulfillment _Fulfillment in Fulfillments)
                {
                    try
                    {
                        if (_Fulfillment.SalesOrder.UserWebsite.OptInForNotification)
                        {
                            Console.WriteLine(string.Format("{0}: {1}", _Fulfillment.FulfillmentID, string.IsNullOrEmpty(_Fulfillment.SalesOrder.UserWebsite.NotificationEmail) ? _Fulfillment.SalesOrder.UserInfo.EmailAddress : _Fulfillment.SalesOrder.UserWebsite.NotificationEmail));
                            //Send Email
                            SendShipConfirationEmail(_Fulfillment, string.IsNullOrEmpty(_Fulfillment.SalesOrder.UserWebsite.NotificationEmail) ? _Fulfillment.SalesOrder.UserInfo.EmailAddress : _Fulfillment.SalesOrder.UserWebsite.NotificationEmail);
                        }

                        if (_Fulfillment.SalesOrder.Account != null && _Fulfillment.SalesOrder.Account.ParentAccount != null && _Fulfillment.SalesOrder.Account.ParentAccount.GetSubAccountNotification)
                        {
                            foreach (ImageSolutions.User.UserAccount _UserAccount in _Fulfillment.SalesOrder.Account.ParentAccount.UserAccounts)
                            {
                                if (_UserAccount.UserWebsite.OptInForNotification && _UserAccount.UserWebsite.OrderManagement)
                                {
                                    Console.WriteLine(string.Format("(Parent) {0}: {1}", _Fulfillment.FulfillmentID, string.IsNullOrEmpty(_UserAccount.UserWebsite.NotificationEmail) ? _UserAccount.UserWebsite.UserInfo.EmailAddress : _UserAccount.UserWebsite.NotificationEmail));
                                    SendShipConfirationEmail(_Fulfillment, string.IsNullOrEmpty(_UserAccount.UserWebsite.NotificationEmail) ? _UserAccount.UserWebsite.UserInfo.EmailAddress : _UserAccount.UserWebsite.NotificationEmail);
                                }
                            }
                        }


                        //if (fulfillment.SalesOrder.Account.ParentAccount.GetSubAccountNotification)
                        //{
                        //    foreach (ImageSolutions.User.UserAccount _UserAccount in fulfillment.SalesOrder.Account.ParentAccount.UserAccounts)
                        //    {
                        //        if (_UserAccount.UserWebsite.OptInForNotification && _UserAccount.UserWebsite.OrderManagement)
                        //        {
                        //            parentemails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);
                        //        }
                        //    }
                        //}


                        _Fulfillment.ShipConfirmationSent = true;
                        _Fulfillment.ErrorMessage = string.Empty;
                        _Fulfillment.Update();
                    }
                    catch (Exception ex)
                    {
                        _Fulfillment.ErrorMessage = string.Format("Send Email Failed: {0}", ex.Message);
                        _Fulfillment.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Fulfillments = null;
                FulfillmentFilter = null;
            }
        }

        public bool SendShipConfirationEmail(ImageSolutions.Fulfillment.Fulfillment fulfillment, string toemail)
        {
            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>Your order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been shipped</p>
                                                    </div>
                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            <tr>
                                                                <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                                <th colspan='3' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            ${FulfillmentLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Tracking Number:</b>
                                                                                    <br />
                                                                                    ${TrackingNumber}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>

                                                </div>
                                            </body>
                                        </html>";

            try
            {
                string strFulfillmentLine = string.Empty;

                foreach (ImageSolutions.Fulfillment.FulfillmentLine _FulfillmentLine in fulfillment.FulfillmentLines)
                {
                    string strOptions = string.Empty;

                    strOptions += _FulfillmentLine.SalesOrderLine.UserInfo == null ? string.Empty : "User: " + _FulfillmentLine.SalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += _FulfillmentLine.SalesOrderLine.CustomListValue_1 == null ? string.Empty : _FulfillmentLine.SalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + _FulfillmentLine.SalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += _FulfillmentLine.SalesOrderLine.CustomListValue_2 == null ? string.Empty : _FulfillmentLine.SalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + _FulfillmentLine.SalesOrderLine.CustomListValue_2.ListValue + "<br />";

                    strFulfillmentLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                    </tr>", _FulfillmentLine.SalesOrderLine.Item.StoreDisplayName, _FulfillmentLine.SalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", _FulfillmentLine.SalesOrderLine.UnitPrice), _FulfillmentLine.SalesOrderLine.Quantity, string.Format("{0:c}", _FulfillmentLine.SalesOrderLine.LineSubTotal));
                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(fulfillment.SalesOrder.Website.EmailLogoPath) ? fulfillment.SalesOrder.Website.LogoPath : fulfillment.SalesOrder.Website.EmailLogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", fulfillment.SalesOrder.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", fulfillment.SalesOrder.SalesOrderID);

                strHTMLContent = strHTMLContent.Replace("${ShipTo}", fulfillment.SalesOrder.DisplayDeliveryAddress());
                //strHTMLContent = strHTMLContent.Replace("${TrackingNumber}", fulfillment.TrackingNumber.Replace(" ","<BR>"));
                
                string strTrackingNumber = string.Empty;
                string[] TrackingNumber = fulfillment.TrackingNumber.Replace(" ", "|").Replace("<BR>", "|").Split('|');
                foreach(string _trackingnumber in TrackingNumber)
                {
                    if(fulfillment.ShippingCarrier == "FedEx")
                    {
                        strTrackingNumber = string.Format(@"{0} <a href=""https://www.fedex.com/fedextrack/?tracknumbers={1}"">{1}</a>", strTrackingNumber, _trackingnumber);
                    }
                    else if (fulfillment.ShippingCarrier == "USPS")
                    {
                        strTrackingNumber = string.Format(@"{0} <a href=""https://tools.usps.com/go/TrackConfirmAction?tLabels={1}"">{1}</a>", strTrackingNumber, _trackingnumber);
                    }
                    else if (fulfillment.ShippingCarrier == "UPS")
                    {
                        strTrackingNumber = string.Format(@"{0} <a href=""https://www.ups.com/track?tracknum={1}"">{1}</a>", strTrackingNumber, _trackingnumber);
                    }
                    else
                    {
                        strTrackingNumber = string.Format(@"{0} {1}", strTrackingNumber, _trackingnumber);
                    }
                }
                strHTMLContent = strHTMLContent.Replace("${TrackingNumber}", strTrackingNumber);
                

                strHTMLContent = strHTMLContent.Replace("${FulfillmentLine}", string.Format("{0:c}", strFulfillmentLine));


                ////CC Parent account if enabled
                //List<string> parentemails = new List<string>();
                //if (fulfillment.SalesOrder.Account.ParentAccount.GetSubAccountNotification)
                //{
                //    foreach (ImageSolutions.User.UserAccount _UserAccount in fulfillment.SalesOrder.Account.ParentAccount.UserAccounts)
                //    {
                //        if (_UserAccount.UserWebsite.OptInForNotification && _UserAccount.UserWebsite.OrderManagement)
                //        {
                //            parentemails.Add(_UserAccount.UserWebsite.UserInfo.EmailAddress);
                //        }
                //    }
                //}

                
                SendEmail(toemail, fulfillment.SalesOrder.Website.Name + " Order Shipment - Order #" + fulfillment.SalesOrder.SalesOrderID, strHTMLContent); //, parentemails);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return true;
        }

        public void SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            UserInfoFilter objFilter = null;
            UserInfo objUserInfo = null;

            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);

                    objFilter = new UserInfoFilter();
                    objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.EmailAddress.SearchString = toemail;
                    objUserInfo = UserInfo.GetUserInfo(objFilter);
                    if (objUserInfo != null)
                    {
                        if (objUserInfo.EmailWhiteListed) toemail = objUserInfo.EmailAddress;
                    }
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(toemail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, subject, null, htmlcontent);

                if (ccs != null)
                {
                    foreach (string _cc in ccs)
                        SendGridMessage.AddCc(_cc);
                }

                Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
                objUserInfo = null;
            }
        }
    }
}
