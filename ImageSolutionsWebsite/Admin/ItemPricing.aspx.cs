using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemPricing : BasePageAdminUserWebSiteAuth
    {
        protected string mItemPricingID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Item.ItemPricing mItemPricing = null;
        protected ImageSolutions.Item.ItemPricing _ItemPricing
        {
            get
            {
                if (mItemPricing == null)
                {
                    if (string.IsNullOrEmpty(mItemPricingID))
                        mItemPricing = new ImageSolutions.Item.ItemPricing();
                    else
                        mItemPricing = new ImageSolutions.Item.ItemPricing(mItemPricingID);
                }
                return mItemPricing;
            }
            set
            {
                mItemPricing = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + (_ItemPricing.IsNew ? mItemID : _ItemPricing.ItemID) + "&tab=4";
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
            mItemPricingID = Request.QueryString.Get("id");
            mItemID = Request.QueryString.Get("itemid");

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
                InitializeTabs(top_1_tab, top_1, null, null, null, null, null, null);
                BindWebsiteGroup();

                if (!_ItemPricing.IsNew)
                {
                    ddlGroup.SelectedValue = _ItemPricing.WebsiteGroupID;
                    txtPrice.Text = Convert.ToString(_ItemPricing.Price);
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
                _ItemPricing.WebsiteGroupID = ddlGroup.SelectedValue;
                _ItemPricing.Price = Convert.ToDouble(txtPrice.Text);

                if (_ItemPricing.IsNew)
                {
                    _ItemPricing.ItemID = mItemID;
                    _ItemPricing.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemPricing.Create();
                    ReturnURL = "/Admin/Item.aspx?id=" +mItemID + "&tab=4";
                }
                else
                {
                    blnReturn = _ItemPricing.Update();
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
                blnReturn = _ItemPricing.Delete();
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
            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }
    }
}