using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class WebsiteGroupItem : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteGroupItemID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Website.WebsiteGroupItem mWebsiteGroupItem = null;
        protected ImageSolutions.Website.WebsiteGroupItem _WebsiteGroupItem
        {
            get
            {
                if (mWebsiteGroupItem == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteGroupItemID))
                        mWebsiteGroupItem = new ImageSolutions.Website.WebsiteGroupItem();
                    else
                        mWebsiteGroupItem = new ImageSolutions.Website.WebsiteGroupItem(mWebsiteGroupItemID);
                }
                return mWebsiteGroupItem;
            }
            set
            {
                mWebsiteGroupItem = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebsiteGroupItemID = Request.QueryString.Get("id");
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

                if (!_WebsiteGroupItem.IsNew)
                {
                    ddlWebsiteGroup.SelectedValue = _WebsiteGroupItem.WebsiteGroupID;
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
                else if (!string.IsNullOrEmpty(_WebsiteGroupItem.ItemID))
                {
                    aCancel.HRef = String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItem.ItemID);
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
                _WebsiteGroupItem.WebsiteGroupID = Convert.ToString(ddlWebsiteGroup.SelectedValue);

                if (_WebsiteGroupItem.IsNew)
                {
                    _WebsiteGroupItem.ItemID = mItemID;
                    _WebsiteGroupItem.WebsiteID = CurrentWebsite.WebsiteID;
                    _WebsiteGroupItem.CreatedBy = CurrentUser.UserInfoID;
                    _WebsiteGroupItem.Create();
                }
                else
                {
                    _WebsiteGroupItem.Update();
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
            else if (!string.IsNullOrEmpty(_WebsiteGroupItem.ItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItem.ItemID));
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                _WebsiteGroupItem.Delete();
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
            else if (!string.IsNullOrEmpty(_WebsiteGroupItem.ItemID))
            {
                Response.Redirect(String.Format("/Admin/item.aspx?id={0}", _WebsiteGroupItem.ItemID));
            }
        }
    }
}