using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class UserAccount : BasePageUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindUserAccounts();
            }
        }

        protected void BindUserAccounts()
        {
            this.gvUserAccounts.DataSource = CurrentUser.CurrentUserWebSite.UserAccounts.FindAll(m => m.WebsiteGroup.WebsiteID == CurrentUser.CurrentUserWebSite.WebsiteID).OrderBy(x => x.WebsiteGroup.GroupName);
            this.gvUserAccounts.DataBind();

            if (CurrentUser.CurrentUserWebSite.IsPendingApproval)
            {
                lblMessage.Text = "Your account is pending approval by your organization, you will receive an email notification once the account is reviewed";
                this.gvUserAccounts.Enabled = false;
            }
            else if (this.gvUserAccounts.Rows.Count == 0)
            {
                lblMessage.Text = "You have not yet been assigned to a user group";
                this.gvUserAccounts.Enabled = false;
            }
            else if (this.gvUserAccounts.Rows.Count == 1)
            {
                string strUserAccountID = this.gvUserAccounts.DataKeys[0].Value.ToString();

                ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount(strUserAccountID);

                if (objUserAccount.Account.IsPendingApproval)
                {
                    lblMessage.Text = "Your account is pending approval by your organization, you will receive an email notification once the account is reviewed";
                    this.gvUserAccounts.Enabled = false;
                }
                else
                {
                    SetUserAccount(objUserAccount.GUID);
                }
            }
            else
            {
                if(CurrentWebsite.CombineWebsiteGroup)
                {
                    bool hasMultipleUserAccount = false;
                    string strUserAccountID = this.gvUserAccounts.DataKeys[0].Value.ToString();
                    ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount(strUserAccountID);

                    foreach (ImageSolutions.User.UserAccount _UserAccount in CurrentUser.CurrentUserWebSite.UserAccounts)
                    {
                        if(_UserAccount.AccountID != objUserAccount.AccountID)
                        {
                            hasMultipleUserAccount = true;
                        }
                    }                    

                    if(!hasMultipleUserAccount)
                    {
                        if (objUserAccount.Account.IsPendingApproval)
                        {
                            lblMessage.Text = "Your account is pending approval by your organization, you will receive an email notification once the account is reviewed";
                            this.gvUserAccounts.Enabled = false;
                        }
                        else
                        {
                            SetUserAccount(objUserAccount.GUID);
                        }
                    }
                }
            }
            if (this.gvUserAccounts.HeaderRow != null) this.gvUserAccounts.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

        protected void gvUserAccounts_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string strUserAccountID = this.gvUserAccounts.DataKeys[e.RowIndex].Value.ToString();

            ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount(strUserAccountID);
            SetUserAccount(objUserAccount.GUID);
        }

        protected void SetUserAccount(string GUID)
        {
            CurrentUser.CurrentUserWebSite.CurrentUserAccount.Login(GUID);
            CurrentUser.LastVisitedUserAccountID = CurrentUser.CurrentUserWebSite.CurrentUserAccount.UserAccountID;
            CurrentUser.Update();

            if (string.IsNullOrEmpty(Request.QueryString.Get("target")))
            {
                if (!string.IsNullOrEmpty(CurrentWebsite.StartingPath))
                {
                    Response.Redirect(CurrentWebsite.StartingPath);
                }
                else
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
            }
            else
            {
                Response.Redirect(Request.QueryString.Get("target"));
            }
        }

        protected void gvUserAccounts_Sorting(object sender, GridViewSortEventArgs e)
        {

        }
    }
}