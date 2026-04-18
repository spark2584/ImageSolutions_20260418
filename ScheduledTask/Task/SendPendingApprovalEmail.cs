using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task
{
    public class SendPendingApprovalEmail
    {
        public bool Execute()
        {
            try
            {
                List<ImageSolutions.Website.Website> Websites = new List<ImageSolutions.Website.Website>();
                Websites = ImageSolutions.Website.Website.GetWebsites();


                foreach (ImageSolutions.Website.Website _Website in Websites)
                {
                    if (!_Website.OrderApprovalRequired)
                    {
                        List<ApproverSalesOrder> AppoverSalesOrders = GetUserPendingApprovalOrders(_Website.WebsiteID);

                        List<ImageSolutions.SalesOrder.SalesOrder> UserSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                        string strCurrentUserInfoID = string.Empty;

                        List<ImageSolutions.SalesOrder.SalesOrder> SalesOrders = null;

                        foreach (ApproverSalesOrder _ApproverSalesOrder in AppoverSalesOrders.FindAll(x => !string.IsNullOrEmpty(x.UserInfoID)).OrderBy(x => x.UserInfoID))
                        {
                            if (string.IsNullOrEmpty(strCurrentUserInfoID))
                            {
                                strCurrentUserInfoID = _ApproverSalesOrder.UserInfoID;
                                UserSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                            }

                            //Send Email
                            if (strCurrentUserInfoID == _ApproverSalesOrder.UserInfoID)
                            {
                                ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(_ApproverSalesOrder.SalesOrderID);
                                UserSalesOrders.Add(SalesOrder);
                            }
                            else
                            {
                                if (UserSalesOrders != null && UserSalesOrders.Count > 0)
                                {
                                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.WebsiteID.SearchString = _Website.WebsiteID;
                                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.UserInfoID.SearchString = strCurrentUserInfoID;
                                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                    if (UserWebsite.OptInForNotification)
                                    {
                                        SendApprovalEmail(UserSalesOrders, UserWebsite).Wait();
                                    }
                                }

                                strCurrentUserInfoID = _ApproverSalesOrder.UserInfoID;
                                UserSalesOrders = null;
                                UserSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();
                            }
                        }

                        if (UserSalesOrders != null && UserSalesOrders.Count > 0)
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = _Website.WebsiteID;
                            UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.UserInfoID.SearchString = strCurrentUserInfoID;
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite.OptInForNotification)
                            {
                                SendApprovalEmail(UserSalesOrders, UserWebsite).Wait();
                            }
                        }
                    }
                    //else
                    //{
                    //    SalesOrders = GetPendingApprovalOrders();

                    //    MyApprovalSalesOrders = new List<ImageSolutions.SalesOrder.SalesOrder>();

                    //    foreach (ImageSolutions.SalesOrder.SalesOrder _SalesOrder in SalesOrders)
                    //    {
                    //        if (_SalesOrder.IsPendingApproval)
                    //        {
                    //            MyApprovalSalesOrders.Add(_SalesOrder);
                    //        }
                    //        else if (_SalesOrder.IsPendingItemPersonalizationApproval && CanApprovePersonalization(_SalesOrder))
                    //        {
                    //            MyApprovalSalesOrders.Add(_SalesOrder);
                    //        }
                    //    }
                    //}
                }

            }
            catch
            {

            }

            return true;
        }

        protected async Task<Response> SendApprovalEmail(List<ImageSolutions.SalesOrder.SalesOrder> salesorders, ImageSolutions.User.UserWebsite userwebsite)
        {
            try
            {
                string strToEmailAddress = string.IsNullOrEmpty(userwebsite.NotificationEmail) ? userwebsite.UserInfo.EmailAddress : userwebsite.NotificationEmail;

                string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'></div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>Orders have been submitted for approval, please <a href='https://portal.imageinc.com/admin/OrderPendingApproval.aspx'>login</a> to the portal to approve the pending orders.</p>
                                                    </div>


                                                    
                                                    <div style='margin-top:30px;text-align:center;'>
                                                        We appreciate your business! Providing you with a great experience is very important to us.<br />
                                                        - Image Solutions Team
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";


                                                    //                <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                    //    <thead>
                                                    //        <tr>
                                                    //            <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                    //            <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                    //            <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                    //            <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                    //            <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                    //        </tr>
                                                    //    </thead>
                                                    //    <tbody>
                                                    //        ${SalesOrderLine}
                                                    //    </tbody>
                                                    //</table>

                //string strSalesOrderLine = string.Empty;

                //foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in salesorder.SalesOrderLines)
                //{
                //    string strOptions = string.Empty;

                //    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                //    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                //    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";

                //    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                //    {
                //        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                //        {
                //            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                //        }
                //    }

                //    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                //    {
                //        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                //        {
                //            if (_SalesOrderLineSelectableLogo.SelectableLogo != null)
                //            {
                //                strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                //            }
                //            else if (_SalesOrderLineSelectableLogo.HasNoLogo)
                //            {
                //                strOptions += "No Logo" + "<br />";
                //            }
                //        }
                //    }

                //    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                //    {
                //        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                //    }

                //    strSalesOrderLine += string.Format(@"<tr>
                //                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                //                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                //                                            <span style='font-style:italic;'>Item #{1}</span>
                //                                        </td>
                //                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                //                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                //                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                //                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                //                                    </tr>", objSalesOrderLine.Item.StoreDisplayName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.UnitPrice), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
                //}

                strHTMLContent = strHTMLContent.Replace("${Logo}", Convert.ToString(userwebsite.WebSite.EmailLogoPath));
                strHTMLContent = strHTMLContent.Replace("${FirstName}", userwebsite.UserInfo.FirstName); //salesorder.UserInfo.FirstName);
                //strHTMLContent = strHTMLContent.Replace("${OrderNumber}", salesorder.SalesOrderID);
                //strHTMLContent = strHTMLContent.Replace("${ShipTo}", salesorder.DisplayDeliveryAddress());
                //strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", salesorder.LineTotal));
                //strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", salesorder.ShippingAmount));
                //strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", salesorder.TaxAmount));
                //strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", salesorder.Total));
                //strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));


                //return await SendEmail(strToEmailAddress, userwebsite.WebSite.Name + " - Pending Approvals", strHTMLContent);

                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    strToEmailAddress = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "it@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(strToEmailAddress);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, userwebsite.WebSite.Name + " - Pending Approvals", null, strHTMLContent);

                return await Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        public List<ApproverSalesOrder> GetUserPendingApprovalOrders(string websiteid)
        {
            List<ApproverSalesOrder> ApproverSalesOrders = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                ApproverSalesOrders = new List<ApproverSalesOrder>();

                strSQL = string.Format(@"
SELECT s.SalesOrderID, line.Amount, ordertotal.Amount, approve.UserInfoID, s.IsPendingApproval, s.IsPendingItemPersonalizationApproval
FROM SalesOrder (NOLOCK) s
Outer Apply
(
	SELECT SUM(sl.Quantity * sl.UnitPrice) Amount FROM SalesOrderLine (NOLOCK) sl WHERE sl.SalesOrderID = s.SalesOrderID
) line
Outer Apply
(
	SELECT line.Amount + s.ShippingAmount + s.TaxAmount as Amount
) ordertotal
Outer Apply
(
	SELECT MAX(a.Amount) Amount
	FROM AccountOrderApproval (NOLOCK) a
	Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = a.UserWebsiteID
	WHERE a.AccountID = s.AccountID
	and a.Amount <= ordertotal.Amount
) maxApprovalAmount
Outer Apply
(
	SELECT uw.UserInfoID
	FROM AccountOrderApproval (NOLOCK) a
	Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = a.UserWebsiteID
	WHERE a.AccountID = s.AccountID
	and a.Amount = maxApprovalAmount.Amount
) approve
WHERE s.WebsiteID = {0}
and s.IsClosed = 0 
and ( s.IsPendingApproval = 1 or s.IsPendingItemPersonalizationApproval = 1 )
"
                        , Database.HandleQuote(websiteid));
                objRead = Database.GetDataReader(strSQL);

                while (objRead.Read())
                {
                    bool IsUpdated = false;

                    string strUserInfoID = Convert.ToString(objRead["UserInfoID"]);
                    string strSalesOrderID = Convert.ToString(objRead["SalesOrderID"]);

                    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(strSalesOrderID);

                    if (Convert.ToBoolean(objRead["IsPendingApproval"]))
                    {
                        if (!string.IsNullOrEmpty(strUserInfoID) && !ApproverSalesOrders.Exists(x => x.UserInfoID == strUserInfoID && x.SalesOrderID == strSalesOrderID))
                        {
                            ApproverSalesOrder ApproverSalesOrder = new ApproverSalesOrder();
                            ApproverSalesOrder.UserInfoID = strUserInfoID;
                            ApproverSalesOrder.SalesOrderID = strSalesOrderID;
                            ApproverSalesOrders.Add(ApproverSalesOrder);

                            IsUpdated = true;
                        }
                    }
                    else if (Convert.ToBoolean(objRead["IsPendingItemPersonalizationApproval"]))
                    {
                        string strPersonalizationApproverUserInfoID = CanApprovePersonalization(SalesOrder);

                        if (!ApproverSalesOrders.Exists(x => x.UserInfoID == strPersonalizationApproverUserInfoID && x.SalesOrderID == strSalesOrderID))
                        {
                            ApproverSalesOrder ApproverSalesOrder = new ApproverSalesOrder();
                            ApproverSalesOrder.UserInfoID = strPersonalizationApproverUserInfoID;
                            ApproverSalesOrder.SalesOrderID = strSalesOrderID;
                            ApproverSalesOrders.Add(ApproverSalesOrder);

                            IsUpdated = true;
                        }
                    }

                    if (!IsUpdated)
                    {
                        foreach (ImageSolutions.Payment.Payment _Payment in SalesOrder.Payments)
                        {
                            if (!string.IsNullOrEmpty(_Payment.BudgetAssignmentID)
                                && !string.IsNullOrEmpty(_Payment.BudgetAssignment.Budget.ApproverUserWebsiteID)
                                && _Payment.AmountPaid != SalesOrder.PaymentTotal)
                            {
                                string strBudgetApproverUserInfoID = _Payment.BudgetAssignment.Budget.ApproverUserWebsite.UserInfoID;

                                if (!ApproverSalesOrders.Exists(x => x.UserInfoID == strBudgetApproverUserInfoID && x.SalesOrderID == strSalesOrderID))
                                {
                                    ApproverSalesOrder ApproverSalesOrder = new ApproverSalesOrder();
                                    ApproverSalesOrder.UserInfoID = strBudgetApproverUserInfoID;
                                    ApproverSalesOrder.SalesOrderID = strSalesOrderID;
                                    ApproverSalesOrders.Add(ApproverSalesOrder);

                                    IsUpdated = true;
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

            return ApproverSalesOrders;
        }

        protected string CanApprovePersonalization(ImageSolutions.SalesOrder.SalesOrder salesorder)
        {
            string strReturn = string.Empty;
            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            List<ImageSolutions.User.UserWebsite> UserWebsites = null;

            try
            {
                UserWebsites = new List<ImageSolutions.User.UserWebsite>();

                UserWebsite = GetPersonalizationAppover(salesorder.AccountID);

                if (UserWebsite != null)
                {
                    strReturn = UserWebsite.UserInfoID;
                }

                if (string.IsNullOrEmpty(strReturn))
                {
                    UserWebsite = GetPersonalizationAppover2(salesorder.AccountID);

                    if (UserWebsite != null)
                    {
                        strReturn = UserWebsite.UserInfoID;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UserWebsite = null;
            }
            return strReturn;
        }

        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApproverUserWebsiteID);
                }
                else if (!string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }
        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover2(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApprover2UserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApprover2UserWebsiteID);
                }
                else if (string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID) && !string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover2(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        public class ApproverSalesOrder
        {
            public string UserInfoID { get; set; }
            public string SalesOrderID { get; set; }            
        }


        static async Task<Response> SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "spark2584@gamil.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
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

                return await Client.SendEmailAsync(SendGridMessage);
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
