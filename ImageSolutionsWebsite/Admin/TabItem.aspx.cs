using ImageSolutions.User;
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class TabItem : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteTabItemID = string.Empty;
        protected string mWebsiteTabID = string.Empty;

        private ImageSolutions.Website.WebsiteTabItem mWebsiteTabItem = null;
        protected ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem
        {
            get
            {
                if (mWebsiteTabItem == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteTabItemID))
                        mWebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                    else
                        mWebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem(mWebsiteTabItemID);
                }
                return mWebsiteTabItem;
            }
            set
            {
                mWebsiteTabItem = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Tab.aspx?id=" + (_WebsiteTabItem.IsNew ? mWebsiteTabID : _WebsiteTabItem.WebsiteTabID) + "&tab=3";
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
            mWebsiteTabItemID = Request.QueryString.Get("id");
            mWebsiteTabID = Request.QueryString.Get("websitetabid");

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
                BindItem();

                if (!_WebsiteTabItem.IsNew)
                {
                    ddlItem.SelectedValue = _WebsiteTabItem.ItemID;
                    cbInactive.Checked = _WebsiteTabItem.Inactive;
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

        protected void BindItem()
        {
            List<ImageSolutions.Item.ItemWebsite> objItemWebsites = null;
            ImageSolutions.Item.ItemWebsiteFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                objFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ItemNumber.SearchString = this.txtItemNumber.Text.Trim();
                objFilter.ItemName = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ItemName.SearchString = this.txtItemName.Text.Trim();
                objFilter.SalesDescription = new Database.Filter.StringSearch.SearchFilter();
                objFilter.SalesDescription.SearchString = this.txtSalesDescription.Text.Trim();
                objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                objItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(objFilter);

                this.ddlItem.DataSource = objItemWebsites.OrderBy(x => x.ItemNumber).ToList();
                this.ddlItem.DataBind();
                this.ddlItem.Items.Insert(0, new ListItem(String.Empty, string.Empty));
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
                _WebsiteTabItem.ItemID = Convert.ToString(ddlItem.SelectedValue);

                if (_WebsiteTabItem.IsNew)
                {
                    _WebsiteTabItem.WebsiteTabID = mWebsiteTabID;
                    _WebsiteTabItem.Sort = _WebsiteTabItem.WebsiteTab.WebsiteTabItems.Count+1;
                    _WebsiteTabItem.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _WebsiteTabItem.Create();
                }
                else
                {
                    blnReturn = _WebsiteTabItem.Update();
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
                blnReturn = _WebsiteTabItem.Delete();
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