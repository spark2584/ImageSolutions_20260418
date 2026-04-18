using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class GroupTab : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteGroupTabID = string.Empty;
        protected string mWebsiteTabID = string.Empty;

        private ImageSolutions.Website.WebsiteGroupTab mWebsiteGroupTab = null;
        protected ImageSolutions.Website.WebsiteGroupTab _WebsiteGroupTab
        {
            get
            {
                if (mWebsiteGroupTab == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteGroupTabID))
                        mWebsiteGroupTab = new ImageSolutions.Website.WebsiteGroupTab();
                    else
                        mWebsiteGroupTab = new ImageSolutions.Website.WebsiteGroupTab(mWebsiteGroupTabID);
                }
                return mWebsiteGroupTab;
            }
            set
            {
                mWebsiteGroupTab = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Tab.aspx?id=" + (_WebsiteGroupTab.IsNew ? mWebsiteTabID : _WebsiteGroupTab.WebsiteTabID) + "&tab=4";
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
            mWebsiteGroupTabID = Request.QueryString.Get("id");
            mWebsiteTabID = Request.QueryString.Get("websitetabid");

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        protected void InitializePage()
        {
            try
            {
                BindGroups();

                if (!_WebsiteGroupTab.IsNew)
                {
                    this.ddlGroup.SelectedIndex = this.ddlGroup.Items.IndexOf(this.ddlGroup.Items.FindByValue(_WebsiteGroupTab.WebsiteGroupID));
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

        protected void BindGroups()
        {
            this.ddlGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups.OrderBy(x => x.GroupName);
            this.ddlGroup.DataBind();
            this.ddlGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _WebsiteGroupTab.WebsiteGroupID = Convert.ToString(ddlGroup.SelectedValue);

                if (_WebsiteGroupTab.IsNew)
                {
                    _WebsiteGroupTab.WebsiteTabID = mWebsiteTabID;
                    _WebsiteGroupTab.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _WebsiteGroupTab.Create();
                }
                else
                {
                    blnReturn = _WebsiteGroupTab.Update();
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
                blnReturn = _WebsiteGroupTab.Delete();
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