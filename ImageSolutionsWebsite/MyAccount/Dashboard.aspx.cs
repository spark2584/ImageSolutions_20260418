using ImageSolutions.Address;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class Dashboard : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Request.QueryString.Get("login") == "t")
            {
                if (!string.IsNullOrEmpty(CurrentWebsite.StartingPath))
                {
                    Response.Redirect(CurrentWebsite.StartingPath);
                }
            }

            this.lblCustomerName.Text = CurrentUser.FirstName + " (Account: " + CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountName + ")";

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        protected void InitializePage()
        {
            lblName.Text = String.Format(@"{0} {1}", CurrentUser.FirstName, CurrentUser.LastName);
            lblEmail.Text = CurrentUser.EmailAddress;
            imgBanner.ImageUrl = CurrentUser.CurrentUserWebSite.WebSite.BannerPath;

            if (CurrentUser.CurrentUserWebSite.SalesOrders != null)
            {
                litApprovedOrderCount.Text = CurrentUser.CurrentUserWebSite.SalesOrders.FindAll(m => !m.IsPendingApproval).Count.ToString();
                litPendingOrderCount.Text = CurrentUser.CurrentUserWebSite.SalesOrders.FindAll(m => m.IsPendingApproval).Count.ToString();
            }

            if (string.IsNullOrEmpty(CurrentUser.DefaultBillingAddressBookID))
            {
                litBillingAddress.Text = "You have not set a default billing address.";
            }
            else
            {
                litBillingAddress.Text = CurrentUser.DefaultBillingAddressBook.GetDisplayFormat(true);
            }

            if(string.IsNullOrEmpty(CurrentUser.DefaultShippingAddressBookID))
            {
                litShippingAddress.Text = "You have not set a default shipping address.";
            }
            else
            {
                litShippingAddress.Text = CurrentUser.DefaultShippingAddressBook.GetDisplayFormat(true);
            }

            if (CurrentWebsite.WebsiteMessages != null)
            {
                rptAnnoucnement.DataSource = CurrentWebsite.WebsiteMessages.FindAll(m => m.IsAnnouncement == true && m.StartDate <= DateTime.Now.Date && m.EndDate > DateTime.Now.Date).OrderBy(x => x.StartDate).ToList();
                rptAnnoucnement.DataBind();
            }

            //string strMessage = string.Empty;
            //string strWebsiteMessageIDs = string.Empty;
            //List<ImageSolutions.Website.WebsiteMessage> WebsiteMessages = CurrentWebsite.WebsiteMessages.FindAll(m => m.IsAnnouncement == true && m.StartDate <= DateTime.Now.Date && m.EndDate > DateTime.Now.Date).OrderBy(x => x.StartDate).ToList();
            //foreach (ImageSolutions.Website.WebsiteMessage _WebsiteMessage in WebsiteMessages)
            //{
            //    strMessage = string.Format(@"{0}<br>{1}<br>{2}<br>", strMessage, _WebsiteMessage.StartDate.ToString("MM/dd/yyyy"), _WebsiteMessage.Message);

            //    strWebsiteMessageIDs = String.Format(@"{0} {1}", strWebsiteMessageIDs, _WebsiteMessage.WebsiteMessageID);
            //}

            //if (!string.IsNullOrEmpty(strMessage))
            //{
            //    this.Visible = true;
            //    litMessage.Text = strMessage;
            //}

            string strAdminGuid = HttpContext.Current.Request.Cookies["ISAdminUserGUID"] != null ? Convert.ToString(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value) : string.Empty;
            
            aManageAddress.Visible = string.IsNullOrEmpty(CurrentUser.CurrentUserWebSite.AddressPermission) || CurrentUser.CurrentUserWebSite.UserManagement || CurrentUser.IsSuperAdmin || !string.IsNullOrEmpty(strAdminGuid);
            pnlManageAddress.Visible = Convert.ToString(CurrentUser.CurrentUserWebSite.AddressPermission) != "Account";

            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
            {
                switch (CurrentWebsite.WebsiteID)
                {
                    case "7": //ABS Companies
                        pnlWebsiteMessage.Visible = true;
                        litWebsiteMessage.Text = "Welcome to the Albertsons Companies internal uniform ordering website!";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (CurrentWebsite.WebsiteID)
                {
                    case "43": //ABS Companies
                        pnlWebsiteMessage.Visible = true;
                        litWebsiteMessage.Text = "Welcome to the Albertsons Companies internal uniform ordering website!";
                        break;
                    default:
                        break;
                }
            }

            lblEmail.Text = String.IsNullOrEmpty(CurrentUser.UserName) ? CurrentUser.EmailAddress : CurrentUser.UserName;
            pnlChangePassword.Visible = CurrentWebsite.EnablePasswordReset;

            if (CurrentWebsite.EnablePackagePayment && CurrentUser.CurrentUserWebSite.PackageAvailableDate != null && CurrentUser.CurrentUserWebSite.PackageAvailableDate > DateTime.UtcNow)
            {
                litPackageMessage.Text = String.Format(@"<p style=""color: red; font-size: 24px; font-family: 'Lato'; text-decoration: underline;"">The next available date to place your annual order is  <b>{0}</b></p>", Convert.ToDateTime(CurrentUser.CurrentUserWebSite.PackageAvailableDate).ToString("MM/dd/yyyy"));
            }

        }

        protected void rptAnnoucnement_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    Label lblDate = (Label)e.Item.FindControl("lblDate");
                    Literal litMessage = (Literal)e.Item.FindControl("litMessage");

                    string strWebsiteMessageID = ((HiddenField)e.Item.FindControl("hfWebsiteMessageID")).Value;
                    ImageSolutions.Website.WebsiteMessage WebsiteMessage = new ImageSolutions.Website.WebsiteMessage(strWebsiteMessageID);
                    lblDate.Text = WebsiteMessage.StartDate.ToString("MM/dd/yyyy");
                    litMessage.Text = WebsiteMessage.Message;

                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}