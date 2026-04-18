using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EmailMarketing
{
    class EmailCampaign
    {
        public void SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            //UserInfoFilter objFilter = null;
            //UserInfo objUserInfo = null;

            //try
            //{
            //    //if ( !(CurrentWebsite.WebsiteID == "42" && toemail.ToLower().Contains("@imageinc.com")) )//Test email from staging
            //    //{
            //    //If the environment is sandbox, default it to steve@imageinc.com or the web.config application variable unless the user email address is whitelisted
            //    if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
            //        || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production"
            //    )
            //    {
            //        toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);

            //        objFilter = new UserInfoFilter();
            //        objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
            //        objFilter.EmailAddress.SearchString = toemail;
            //        objUserInfo = UserInfo.GetUserInfo(objFilter);
            //        if (objUserInfo != null)
            //        {
            //            if (objUserInfo.EmailWhiteListed) toemail = objUserInfo.EmailAddress;
            //        }
            //    }
            //    //}


            //    string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
            //    SendGridClient Client = new SendGridClient(strAPIKey);
            //    EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
            //    EmailAddress ToEmailAddress = new EmailAddress(toemail);
            //    SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, subject, null, htmlcontent);

            //    if (ccs != null)
            //    {
            //        foreach (string _cc in ccs)
            //            SendGridMessage.AddCc(_cc);
            //    }

            //    Client.SendEmailAsync(SendGridMessage);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    objFilter = null;
            //    objUserInfo = null;
            //}
        }

        //protected bool SendOrderConfirmation(SalesOrder SalesOrder, string ToEmailAddress)
        //{
        //    string strHTMLContent = @"<!DOCTYPE html>
        //                                <html>
        //                                    <head></head>
        //                                    <body>
        //                                        <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
        //                                            <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

        //                                            <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

        //                                            <div style='clear:left;padding-top:40px;'>
        //                                                <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
        //                                                <p>Your order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been processed${Approval}</p>
        //                                                <p>Items stocked in our warehouse will ship same day if your order is placed by 2:30pm PST and next business day if after. Custom items will ship within 10-12 business days.</p>
        //                                            </div>
        //                                            <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
        //                                                <thead>
        //                                                    <tr>
        //                                                        <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
        //                                                        <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
        //                                                        <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
        //                                                        <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
        //                                                        <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
        //                                                    </tr>
        //                                                </thead>
        //                                                <tbody>
        //                                                    ${SalesOrderLine}
        //                                                </tbody>
        //                                            </table>
        //                                            <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
        //                                                <tbody>
        //                                                    <tr>
        //                                                        <td>
        //                                                            <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
        //                                                                <tbody>
        //                                                                    <tr>
        //                                                                        <td>
        //                                                                            <b>Ship To:</b>
        //                                                                            <br />
        //                                                                            ${ShipTo}
        //                                                                            <br />
        //                                                                        </td>
        //                                                                    </tr>
        //                                                                </tbody>
        //                                                            </table>
        //                                                        </td>
        //                                                        <td align='right'>
        //                                                            <table>
        //                                                                <tr>
        //                                                                    <td>
        //                                                                        <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
        //                                                                            <tbody>
        //                                                                                <tr>
        //                                                                                    <td align='right'><b>Subtotal</b></td>
        //                                                                                    <td align='right'>${Subtotal}</td>
        //                                                                                </tr>
        //                                                                                <tr>
        //                                                                                    <td align='right'><b>Shipping & Handling</b></td>
        //                                                                                    <td align='right'>${Shipping}</td>
        //                                                                                </tr>
        //                                                                                <tr>
        //                                                                                    <td align='right'><b>Tax</b></td>
        //                                                                                    <td align='right'>${Tax}</td>
        //                                                                                </tr>
        //                                                                                <tr>
        //                                                                                    <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
        //                                                                                    <td align='right' style='font-weight:bold;'>${Total}</td>
        //                                                                                </tr>
        //                                                                            </tbody>
        //                                                                        </table>
        //                                                                    </td>
        //                                                                </tr>
        //                                                            </table>
        //                                                        </td>
        //                                                    </tr>
        //                                                </tbody>
        //                                            </table>

        //                                            <div style='margin-top:30px;text-align:center;'>
        //                                                We appreciate your business! Providing you with a great experience is very important to us. <br />
        //                                                - Image Solutions Team
        //                                            </div>
        //                                        </div>
        //                                    </body>
        //                                </html>";

        //    try
        //    {
        //        string strSalesOrderLine = string.Empty;

        //        foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in SalesOrder.SalesOrderLines)
        //        {
        //            string strOptions = string.Empty;

        //            strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
        //            strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
        //            strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";

        //            if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
        //            {
        //                foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
        //                {
        //                    strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
        //                }
        //            }

        //            if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
        //            {
        //                foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
        //                {
        //                    if (_SalesOrderLineSelectableLogo.SelectableLogo != null)
        //                    {
        //                        strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
        //                    }
        //                }
        //            }

        //            if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
        //            {
        //                strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
        //            }

        //            strSalesOrderLine += string.Format(@"<tr>
        //                                                <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
        //                                                    <span style='font-weight:bold; color:#333;'>{0}</span><br />
        //                                                    <span style='font-style:italic;'>Item #{1}</span>
        //                                                </td>
        //                                                <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
        //                                                <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
        //                                                <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
        //                                                <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
        //                                            </tr>", objSalesOrderLine.Item.StoreDisplayName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.UnitPrice), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
        //        }

        //        strHTMLContent = strHTMLContent.Replace("${Logo}", string.IsNullOrEmpty(SalesOrder.Website.EmailLogoPath) ? SalesOrder.Website.LogoPath : SalesOrder.Website.EmailLogoPath);
        //        strHTMLContent = strHTMLContent.Replace("${FirstName}", SalesOrder.UserInfo.FirstName);
        //        strHTMLContent = strHTMLContent.Replace("${OrderNumber}", SalesOrder.SalesOrderID);

        //        if (CurrentUser.CurrentUserWebSite.WebSite.OrderApprovalRequired)
        //        {
        //            strHTMLContent = strHTMLContent.Replace("${Approval}", " and is currently under review, you will receive a confirmation email once your order is approved.");
        //        }
        //        else
        //        {
        //            strHTMLContent = strHTMLContent.Replace("${Approval}", ".");
        //        }

        //        strHTMLContent = strHTMLContent.Replace("${ShipTo}", SalesOrder.DisplayDeliveryAddress());
        //        strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", SalesOrder.LineTotal));
        //        strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", SalesOrder.ShippingAmount));
        //        strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", SalesOrder.TaxAmount));
        //        strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", SalesOrder.Total));
        //        strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));

        //        SendEmail(ToEmailAddress, CurrentWebsite.Name + " Order Confirmation #" + SalesOrder.SalesOrderID, strHTMLContent);
        //    }
        //    catch { }
        //    finally { }
        //    return true;
        //}
    }
}
