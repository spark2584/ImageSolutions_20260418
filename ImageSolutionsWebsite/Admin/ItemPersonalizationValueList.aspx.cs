using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemPersonalizationValueList : BasePageAdminUserWebSiteAuth
    {
        protected string mItemID = string.Empty;
        protected string mItemPersonalizationID = string.Empty;
        protected string mItemPersonalizationValueListID = string.Empty;
        private ImageSolutions.Item.ItemPersonalizationValueList mItemPersonalizationValueList = null;
        protected ImageSolutions.Item.ItemPersonalizationValueList _ItemPersonalizationValueList
        {
            get
            {
                if (mItemPersonalizationValueList == null)
                {
                    if(string.IsNullOrEmpty(mItemPersonalizationValueListID))
                        mItemPersonalizationValueList = new ImageSolutions.Item.ItemPersonalizationValueList();
                    else
                        mItemPersonalizationValueList = new ImageSolutions.Item.ItemPersonalizationValueList(mItemPersonalizationValueListID);
                }
                return mItemPersonalizationValueList;
            }
        }
        private ImageSolutions.Item.ItemPersonalization mItemPersonalization = null;
        protected ImageSolutions.Item.ItemPersonalization _ItemPersonalization
        {
            get
            {
                if (mItemPersonalization == null)
                {
                    mItemPersonalization = new ImageSolutions.Item.ItemPersonalization(mItemPersonalizationID);
                }
                return mItemPersonalization;
            }
        }
        
        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/ItemPersonalization.aspx?itemid=" + mItemID + "&id=" + mItemPersonalizationID;
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
            mItemPersonalizationID = Request.QueryString.Get("itempersonalizationid");
            mItemPersonalizationValueListID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                if (!_ItemPersonalizationValueList.IsNew)
                {
                    txtValue.Text = _ItemPersonalizationValueList.Value;
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
                _ItemPersonalizationValueList.Value = txtValue.Text;

                if (_ItemPersonalizationValueList.IsNew)
                {
                    _ItemPersonalizationValueList.ItemPersonalizationID = mItemPersonalizationID;
                    _ItemPersonalizationValueList.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemPersonalizationValueList.Create();
                }
                else
                {
                    blnReturn = _ItemPersonalizationValueList.Update();
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
                blnReturn = _ItemPersonalizationValueList.Delete();
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