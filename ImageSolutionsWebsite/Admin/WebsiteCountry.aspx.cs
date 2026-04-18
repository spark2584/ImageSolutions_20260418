using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class WebsiteCountry : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteCountryID = string.Empty;

        private ImageSolutions.Website.WebsiteCountry mWebsiteCountry = null;
        protected ImageSolutions.Website.WebsiteCountry _WebsiteCountry
        {
            get
            {
                if (mWebsiteCountry == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteCountryID))
                        mWebsiteCountry = new ImageSolutions.Website.WebsiteCountry();
                    else
                        mWebsiteCountry = new ImageSolutions.Website.WebsiteCountry(mWebsiteCountryID);
                }
                return mWebsiteCountry;
            }
            set
            {
                mWebsiteCountry = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            mWebsiteCountryID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                BindCountry();

                if (!_WebsiteCountry.IsNew)
                {
                    ddlCountry.SelectedValue = _WebsiteCountry.CountryCode;
                    cbExclude.Checked = Convert.ToBoolean(_WebsiteCountry.Exclude);
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindCountry()
        {
            try
            {
                List<ImageSolutions.Address.AddressCountryCode> AddressCountryCodes = new List<ImageSolutions.Address.AddressCountryCode>();
                AddressCountryCodes = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCodes();

                this.ddlCountry.DataSource = AddressCountryCodes;
                this.ddlCountry.DataBind();
                this.ddlCountry.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _WebsiteCountry.CountryCode = Convert.ToString(ddlCountry.SelectedValue);
                _WebsiteCountry.Exclude = cbExclude.Checked;

                if (_WebsiteCountry.IsNew)
                {
                    _WebsiteCountry.WebsiteID = CurrentWebsite.WebsiteID;
                    _WebsiteCountry.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _WebsiteCountry.Create();
                }
                else
                {
                    blnReturn = _WebsiteCountry.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect("/Admin/WebsiteOverview.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/WebsiteOverview.aspx");
        }
    }
}