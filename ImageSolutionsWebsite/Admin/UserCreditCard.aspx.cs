using ImageSolutions.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserCreditCard : BasePageAdminUserWebSiteAuth
    {
        protected string mUserCreditCardID = string.Empty;
        protected string mCreditCardID = string.Empty;

        private ImageSolutions.User.UserCreditCard mUserCreditCard = null;
        protected ImageSolutions.User.UserCreditCard _UserCreditCard
        {
            get
            {
                if (mUserCreditCard == null)
                {
                    if (string.IsNullOrEmpty(mUserCreditCardID))
                        mUserCreditCard = new ImageSolutions.User.UserCreditCard();
                    else
                        mUserCreditCard = new ImageSolutions.User.UserCreditCard(mUserCreditCardID);
                }
                return mUserCreditCard;
            }
            set
            {
                mUserCreditCard = value;
            }
        }
        private ImageSolutions.CreditCard.CreditCard mCreditCard = null;
        protected ImageSolutions.CreditCard.CreditCard _CreditCard
        {
            get
            {
                if (mCreditCard == null)
                {
                    if (string.IsNullOrEmpty(mCreditCardID))
                        mCreditCard = new ImageSolutions.CreditCard.CreditCard();
                    else
                        mCreditCard = new ImageSolutions.CreditCard.CreditCard(mCreditCardID);
                }
                return mCreditCard;
            }
            set
            {
                mCreditCard = value;
            }
        }
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/CreditCard.aspx?id=" + (_UserCreditCard.IsNew ? mCreditCardID : _UserCreditCard.CreditCardID) + "&tab=2";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mUserCreditCardID = Request.QueryString.Get("id");
            mCreditCardID = Request.QueryString.Get("creditcardid");

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                BindUser();
                if (!_UserCreditCard.IsNew)
                {
                    ddlUser.SelectedValue = _UserCreditCard.UserInfoID;
                    ddlDay.SelectedValue = Convert.ToString(_UserCreditCard.ResetDayOfTheMonth);
                    txtLimit.Text = Convert.ToString(_UserCreditCard.Limit);
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;                
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/GroupOverview.aspx"));
            }
            finally { }
        }
        protected void BindUser()
        {
            List<ImageSolutions.User.UserWebsite> objUserWebsite = null;
            ImageSolutions.User.UserWebsiteFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.User.UserWebsiteFilter();

                if (CurrentUser.IsSuperAdmin)
                {
                    objFilter = new ImageSolutions.User.UserWebsiteFilter();
                    objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                }
                else
                {
                    objFilter.AccountIDs = new List<string>();
                    foreach (ImageSolutions.Account.Account objAccount in CurrentUser.CurrentUserWebSite.Accounts)
                    {
                        objFilter.AccountIDs.Add(objAccount.AccountID);
                    }
                }
                objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                objFilter.EmailAddress.SearchString = this.txtEmailAddress.Text.Trim();
                objFilter.FirstName = new Database.Filter.StringSearch.SearchFilter();
                objFilter.FirstName.SearchString = this.txtFirstName.Text.Trim();
                objFilter.LastName = new Database.Filter.StringSearch.SearchFilter();
                objFilter.LastName.SearchString = this.txtLastName.Text.Trim();

                objUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsites(objFilter);

                ddlUser.DataSource = objUserWebsite; //CurrentUser.CurrentUserWebSite.WebSite.UserWebsites;
                ddlUser.DataBind();
                ddlUser.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/GroupOverview.aspx"));
            }
            finally { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _UserCreditCard.UserInfoID = ddlUser.SelectedValue;
                _UserCreditCard.Limit = string.IsNullOrEmpty(txtLimit.Text.Trim()) ? (double?) null :  Convert.ToDouble(txtLimit.Text.Trim());
                _UserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(ddlDay.SelectedValue);

                if (_UserCreditCard.IsNew)
                {
                    _UserCreditCard.CreditCardID = _CreditCard.CreditCardID;
                    _UserCreditCard.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _UserCreditCard.Create();
                }
                else
                {
                    blnReturn = _UserCreditCard.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _UserCreditCard.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void txtUserFilter_TextChanged(object sender, EventArgs e)
        {
            InitializePage();
        }
    }
}