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
    public partial class SuperceedingItem : BasePageAdminUserWebSiteAuth
    {
        protected string mItemID = string.Empty;
        protected string mSuperceedingItemID = string.Empty;

        private ImageSolutions.Item.SuperceedingItem mSuperceedingItem = null;
        protected ImageSolutions.Item.SuperceedingItem _SuperceedingItem
        {
            get
            {
                if (mSuperceedingItem == null)
                {
                    if (string.IsNullOrEmpty(mSuperceedingItemID))
                        mSuperceedingItem = new ImageSolutions.Item.SuperceedingItem();
                    else
                        mSuperceedingItem = new ImageSolutions.Item.SuperceedingItem(mSuperceedingItemID);
                }
                return mSuperceedingItem;
            }
        }


        private ImageSolutions.Item.Item mItem = null;
        protected ImageSolutions.Item.Item _Item
        {
            get
            {
                if (mItem == null)
                {
                    mItem = new ImageSolutions.Item.Item(mItemID);
                }
                return mItem;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + mItemID + "&tab=6";
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
            mItemID = Request.QueryString.Get("itemid");
            mSuperceedingItemID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindItem();

                if (!_SuperceedingItem.IsNew)
                {
                    ddlItem.SelectedValue = _SuperceedingItem.ReplacementItemID;
                    cbInactive.Checked = _SuperceedingItem.Inactive;
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

                if (!string.IsNullOrEmpty(txtItemNumber.Text))
                {
                    objFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.ItemNumber.SearchString = this.txtItemNumber.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtItemName.Text))
                {
                    objFilter.ItemName = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.ItemName.SearchString = this.txtItemName.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtSalesDescription.Text))
                {
                    objFilter.SalesDescription = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.SalesDescription.SearchString = this.txtSalesDescription.Text.Trim();
                }

                //objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                //objFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;

                objItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(objFilter);

                //this.ddlItem.DataSource = CurrentUser.CurrentUserWebSite.WebSite.ItemWebsites.FindAll(m => (m.Item.ParentItem != null || !m.Item.IsVariation) && m.ItemID != mItemID);
                this.ddlItem.DataSource = objItemWebsites.FindAll(m => (m.Item.ParentItem != null || !m.Item.IsVariation) && m.ItemID != mItemID);
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
                _SuperceedingItem.ItemID = Convert.ToString(ddlItem.SelectedValue);

                if (_SuperceedingItem.IsNew)
                {
                    _SuperceedingItem.ItemID = mItemID;
                    _SuperceedingItem.ReplacementItemID = this.ddlItem.SelectedValue;
                    _SuperceedingItem.Sort = _Item.SuperceedingItems.Count+1;
                    _SuperceedingItem.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _SuperceedingItem.Create();
                }
                else
                {
                    blnReturn = _SuperceedingItem.Update();
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
                blnReturn = _SuperceedingItem.Delete();
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

        protected void Filter_TextChanged(object sender, EventArgs e)
        {
            InitializePage();
        }
    }
}