using ImageSolutions.Attribute;
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemDetailValue : BasePageAdminUserWebSiteAuth
    {
        protected string mItemDetailValueID = string.Empty;
        protected string mItemDetailID = string.Empty;

        private ImageSolutions.Item.ItemDetailValue mItemDetailValue = null;
        protected ImageSolutions.Item.ItemDetailValue _ItemDetailValue
        {
            get
            {
                if (mItemDetailValue == null)
                {
                    if (string.IsNullOrEmpty(mItemDetailValueID))
                        mItemDetailValue = new ImageSolutions.Item.ItemDetailValue();
                    else
                        mItemDetailValue = new ImageSolutions.Item.ItemDetailValue(mItemDetailValueID);
                }
                return mItemDetailValue;
            }
            set
            {
                mItemDetailValue = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/ItemDetail.aspx?id=" + mItemDetailID + "&tab=2";
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
            mItemDetailValueID = Request.QueryString.Get("id");
            mItemDetailID = Request.QueryString.Get("itemdetailid");

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
                if (!_ItemDetailValue.IsNew)
                {
                    txtAttributeValue.Text = _ItemDetailValue.Value;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _ItemDetailValue.Value = txtAttributeValue.Text.Trim();

                if (_ItemDetailValue.IsNew)
                {
                    _ItemDetailValue.ItemDetailID = mItemDetailID;
                    _ItemDetailValue.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemDetailValue.Create();
                }
                else
                {
                    blnReturn = _ItemDetailValue.Update();
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
                blnReturn = _ItemDetailValue.Delete();
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