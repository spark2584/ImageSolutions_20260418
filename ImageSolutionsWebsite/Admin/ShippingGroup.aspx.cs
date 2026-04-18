using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ShippingGroup : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteShippingServiceGroupID = string.Empty;
        protected string mWebsiteShippingServiceID = string.Empty;
        private ImageSolutions.Website.WebsiteShippingServiceGroup mWebsiteShippingServiceGroup = null;
        protected ImageSolutions.Website.WebsiteShippingServiceGroup WebsiteShippingServiceGroup
        {
            get
            {
                if (mWebsiteShippingServiceGroup == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteShippingServiceGroupID))
                        mWebsiteShippingServiceGroup = new ImageSolutions.Website.WebsiteShippingServiceGroup();
                    else
                        mWebsiteShippingServiceGroup = new ImageSolutions.Website.WebsiteShippingServiceGroup(mWebsiteShippingServiceGroupID);
                }
                return mWebsiteShippingServiceGroup;
            }
            set
            {
                mWebsiteShippingServiceGroup = value;
            }
        }
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                {
                    if (!string.IsNullOrEmpty(mWebsiteShippingServiceID))
                        return "/Admin/Shipping.aspx?id=" + mWebsiteShippingServiceID + "&tab=2";
                    else
                        return "/Admin/ShippingOverview.aspx";
                }
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
            mWebsiteShippingServiceGroupID = Request.QueryString.Get("id");
            mWebsiteShippingServiceID = Request.QueryString.Get("websiteshippingserviceid");

            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                BindWebsiteGroup();

                if (!WebsiteShippingServiceGroup.IsNew)
                {
                    this.ddlGroup.SelectedIndex = this.ddlGroup.Items.IndexOf(this.ddlGroup.Items.FindByValue(WebsiteShippingServiceGroup.WebsiteGroupID));
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
            }
            finally { }
        }
        protected void BindWebsiteGroup()
        {
            try
            {
                ddlGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
                ddlGroup.DataBind();
                ddlGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
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
                WebsiteShippingServiceGroup.WebsiteShippingServiceID = mWebsiteShippingServiceID;
                WebsiteShippingServiceGroup.WebsiteGroupID = ddlGroup.SelectedValue;

                if (WebsiteShippingServiceGroup.IsNew)
                {
                    WebsiteShippingServiceGroup.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = WebsiteShippingServiceGroup.Create();
                }
                else
                {
                    blnReturn = WebsiteShippingServiceGroup.Update();
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
                blnReturn = WebsiteShippingServiceGroup.Delete();
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
    }
}