using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemDetail : BasePageAdminUserWebSiteAuth
    {
        protected string mItemDetailID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Item.ItemDetail mItemDetail = null;
        protected ImageSolutions.Item.ItemDetail _ItemDetail
        {
            get
            {
                if (mItemDetail == null)
                {
                    if (string.IsNullOrEmpty(mItemDetailID))
                        mItemDetail = new ImageSolutions.Item.ItemDetail();
                    else
                        mItemDetail = new ImageSolutions.Item.ItemDetail(mItemDetailID);
                }
                return mItemDetail;
            }
            set
            {
                mItemDetail = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + (_ItemDetail.IsNew ? mItemID : _ItemDetail.ItemID) + "&tab=5";
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
            mItemDetailID = Request.QueryString.Get("id");
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
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, null, null, null, null);

                if (!_ItemDetail.IsNew)
                {
                    txtAttribute.Text = _ItemDetail.Attribute;
                    txtSort.Text = Convert.ToString(_ItemDetail.Sort);
                    top_2_tab.Visible = true;
                    btnSave.Text = "Save";

                    BindItemAttributeValue();
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemAttributeValue()
        {
            try
            {
                gvItemDetailValue.DataSource = _ItemDetail.ItemDetailValues;
                gvItemDetailValue.DataBind();
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
                _ItemDetail.Attribute = txtAttribute.Text;
                _ItemDetail.Sort = Convert.ToInt32(txtSort.Text);

                if (_ItemDetail.IsNew)
                {
                    _ItemDetail.ItemID = mItemID;
                    _ItemDetail.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemDetail.Create();
                    ReturnURL = "/admin/itemdetail.aspx?id=" + _ItemDetail.ItemDetailID;
                }
                else
                {
                    blnReturn = _ItemDetail.Update();
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
                blnReturn = _ItemDetail.Delete();
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