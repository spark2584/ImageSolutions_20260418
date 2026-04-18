using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class WebsiteGroupItemSelectableLogo : BasePageAdminUserWebSiteAuth
    {
        protected string mWebsiteGroupItemSelectableLogoID = string.Empty;
        protected string mItemSelectableLogoID = string.Empty;

        private ImageSolutions.Website.WebsiteGroupItemSelectableLogo mWebsiteGroupItemSelectableLogo = null;
        protected ImageSolutions.Website.WebsiteGroupItemSelectableLogo _WebsiteGroupItemSelectableLogo
        {
            get
            {
                if (mWebsiteGroupItemSelectableLogo == null)
                {
                    if (string.IsNullOrEmpty(mWebsiteGroupItemSelectableLogoID))
                        mWebsiteGroupItemSelectableLogo = new ImageSolutions.Website.WebsiteGroupItemSelectableLogo();
                    else
                        mWebsiteGroupItemSelectableLogo = new ImageSolutions.Website.WebsiteGroupItemSelectableLogo(mWebsiteGroupItemSelectableLogoID);
                }
                return mWebsiteGroupItemSelectableLogo;
            }
            set
            {
                mWebsiteGroupItemSelectableLogo = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebsiteGroupItemSelectableLogoID = Request.QueryString.Get("id");

            if (!string.IsNullOrEmpty(mWebsiteGroupItemSelectableLogoID))
            {
                ImageSolutions.Website.WebsiteGroupItemSelectableLogo WebsiteGroupItemSelectableLogo = new ImageSolutions.Website.WebsiteGroupItemSelectableLogo(mWebsiteGroupItemSelectableLogoID);
                mItemSelectableLogoID = WebsiteGroupItemSelectableLogo.ItemSelectableLogoID;
            }
            else
            {
                mItemSelectableLogoID = Request.QueryString.Get("itemselectablelogoid");
            }

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

                if (!_WebsiteGroupItemSelectableLogo.IsNew)
                {
                    ddlWebsiteGroup.SelectedValue = _WebsiteGroupItemSelectableLogo.WebsiteGroupID;
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }


                if (!string.IsNullOrEmpty(mItemSelectableLogoID))
                {
                    ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                    aCancel.HRef = String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, mItemSelectableLogoID);
                }
                else if (!string.IsNullOrEmpty(_WebsiteGroupItemSelectableLogo.ItemSelectableLogoID))
                {
                    ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                    aCancel.HRef = String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, _WebsiteGroupItemSelectableLogo.ItemSelectableLogoID);
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
                _WebsiteGroupItemSelectableLogo.WebsiteGroupID = Convert.ToString(ddlWebsiteGroup.SelectedValue);

                if (_WebsiteGroupItemSelectableLogo.IsNew)
                {
                    _WebsiteGroupItemSelectableLogo.ItemSelectableLogoID = mItemSelectableLogoID;
                    _WebsiteGroupItemSelectableLogo.WebsiteID = CurrentWebsite.WebsiteID;
                    _WebsiteGroupItemSelectableLogo.CreatedBy = CurrentUser.UserInfoID;
                    _WebsiteGroupItemSelectableLogo.Create();
                }
                else
                {
                    _WebsiteGroupItemSelectableLogo.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (!string.IsNullOrEmpty(mItemSelectableLogoID))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                Response.Redirect(String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, mItemSelectableLogoID));
            }
            else if (!string.IsNullOrEmpty(_WebsiteGroupItemSelectableLogo.ItemSelectableLogoID))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                Response.Redirect(String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, _WebsiteGroupItemSelectableLogo.ItemSelectableLogoID));
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                _WebsiteGroupItemSelectableLogo.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (!string.IsNullOrEmpty(mItemSelectableLogoID))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                Response.Redirect(String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, mItemSelectableLogoID));
            }
            else if (!string.IsNullOrEmpty(_WebsiteGroupItemSelectableLogo.ItemSelectableLogoID))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                Response.Redirect(String.Format("/Admin/itemselectablelogo.aspx?itemid={0}&id={1}", ItemSelectableLogo.ItemID, _WebsiteGroupItemSelectableLogo.ItemSelectableLogoID));
            }
        }
    }
}