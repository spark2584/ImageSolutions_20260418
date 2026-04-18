using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public delegate void SendAccountMessageToThePageHandler(string message);
    public partial class AccountSearchModal : BaseControl
    {
        public string WebsiteID
        {
            get { return hfWebsiteID.Value; }
            set { hfWebsiteID.Value = value; }
        }
        public event SendAccountMessageToThePageHandler SendMessageToThePage;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Visible = false;
            }
        }
        public bool Show()
        {
            try
            {
                WebUtility.ClearForm(this);

                this.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }

            return true;
        }
        protected void BindAccount()
        {
            List<ImageSolutions.Account.Account> Accounts = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                Accounts = new List<ImageSolutions.Account.Account>();
                strSQL = string.Format(@"
SELECT DISTINCT a.AccountID, a.AccountName
FROM Account (NOLOCK) a
Left Outer Join UserAccount (NOLOCK) ua on ua.AccountID = a.AccountID
Left Outer Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ua.UserWebsiteID
Left Outer Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
WHERE a.WebsiteID = {0}
{1}
"
                    , Convert.ToString(hfWebsiteID.Value)
                    , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) && string.IsNullOrEmpty(this.txtEmail.Text.Trim()) 
                        ? string.Empty
                        : string.Format(@"
and (
    1 = 1
    {0}
    {1}
)"
                            , string.IsNullOrEmpty(this.txtAccountName.Text.Trim()) ? string.Empty : string.Format("and (a.AccountName like '%{0}%') ", this.txtAccountName.Text.Trim())
                            , string.IsNullOrEmpty(this.txtEmail.Text.Trim()) ? string.Empty : string.Format("and (u.EmailAddress like '%{0}%') ", this.txtEmail.Text.Trim())
                    )
                );

                objRead = Database.GetDataReader(strSQL);

                while (objRead.Read())
                {
                    Accounts.Add(new ImageSolutions.Account.Account(Convert.ToString(objRead["AccountID"])));
                }

                gvAccounts.DataSource = Accounts.OrderBy(x => x.AccountName);
                gvAccounts.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }
            finally { }
        }
        protected void txtAccountName_TextChanged(object sender, EventArgs e)
        {
            if(txtAccountName.Text.Length > 3)
            {
                BindAccount();
            }
            else
            {
                txtAccountName.Text = string.Empty;
            }
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            BindAccount();
        }

        protected void gvAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectLine")
            {
                string strCustomizationID = Convert.ToString(e.CommandArgument);
                SendMessageToThePage(strCustomizationID);

                gvAccounts.DataSource = null;
                gvAccounts.DataBind();

                this.Visible = false;
            }
        }

        public void Hide()
        {
            this.Visible = false;
        }
    }
}