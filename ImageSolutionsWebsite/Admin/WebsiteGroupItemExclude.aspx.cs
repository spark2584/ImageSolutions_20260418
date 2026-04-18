using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class WebsiteGroupItemExclude : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteGroupItemExcludeID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Website.WebsiteGroupItemExclude mWebsiteGroupItemExclude = null;
        protected ImageSolutions.Website.WebsiteGroupItemExclude _WebsiteGroupItemExclude
        {
            get
            {
                if (mWebsiteGroupItemExclude == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteGroupItemExcludeID))
                        mWebsiteGroupItemExclude = new ImageSolutions.Website.WebsiteGroupItemExclude();
                    else
                        mWebsiteGroupItemExclude = new ImageSolutions.Website.WebsiteGroupItemExclude(mWebsiteGroupItemExcludeID);
                }
                return mWebsiteGroupItemExclude;
            }
            set
            {
                mWebsiteGroupItemExclude = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebsiteGroupItemExcludeID = Request.QueryString.Get("id");
            mItemID = Request.QueryString.Get("itemid");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindWebsiteGroup();

                if (!_WebsiteGroupItemExclude.IsNew)
                {
                    ddlWebsiteGroup.SelectedValue = _WebsiteGroupItemExclude.WebsiteGroupID;
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }

                if (!string.IsNullOrEmpty(mItemID))
                {
                    aCancel.HRef = String.Format("/Admin/item.aspx?id={0}", mItemID);
                }
                else if (!string.IsNullOrEmpty(_WebsiteGroupItemExclude.ItemID))
                {
                    aCancel.HRef = String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItemExclude.ItemID);
                }

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindWebsiteGroup()
        {

            List<ImageSolutions.Website.WebsiteGroup> WebsiteGroups = null;
            ImageSolutions.Website.WebsiteGroupFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                WebsiteGroups = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroups(objFilter);

                this.ddlWebsiteGroup.DataSource = WebsiteGroups.OrderBy(x => x.GroupName).ToList();
                this.ddlWebsiteGroup.DataBind();
                this.ddlWebsiteGroup.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _WebsiteGroupItemExclude.WebsiteGroupID = Convert.ToString(ddlWebsiteGroup.SelectedValue);

                if (_WebsiteGroupItemExclude.IsNew)
                {
                    _WebsiteGroupItemExclude.ItemID = mItemID;
                    _WebsiteGroupItemExclude.WebsiteID = CurrentWebsite.WebsiteID;
                    _WebsiteGroupItemExclude.CreatedBy = CurrentUser.UserInfoID;
                    _WebsiteGroupItemExclude.Create();
                }
                else
                {
                    _WebsiteGroupItemExclude.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (!string.IsNullOrEmpty(mItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", mItemID));
            }
            else if (!string.IsNullOrEmpty(_WebsiteGroupItemExclude.ItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItemExclude.ItemID));
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                _WebsiteGroupItemExclude.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (!string.IsNullOrEmpty(mItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", mItemID));
            }
            else if (!string.IsNullOrEmpty(_WebsiteGroupItemExclude.ItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItemExclude.ItemID));
            }
        }
    }
}