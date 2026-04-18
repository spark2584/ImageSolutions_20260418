using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemPersonalizationApproved : BasePageAdminUserWebSiteAuth
    {
        protected string mItemPersonalizationApprovedID = string.Empty;
        private ImageSolutions.Item.ItemPersonalizationValueApproved mItemPersonalizationValueApproved = null;
        protected ImageSolutions.Item.ItemPersonalizationValueApproved _ItemPersonalizationValueApproved
        {
            get
            {
                if (mItemPersonalizationValueApproved == null)
                {
                    if (string.IsNullOrEmpty(mItemPersonalizationApprovedID))
                        mItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved();
                    else
                        mItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved(mItemPersonalizationApprovedID);
                }
                return mItemPersonalizationValueApproved;
            }
            set
            {
                mItemPersonalizationValueApproved = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            mItemPersonalizationApprovedID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                if (!_ItemPersonalizationValueApproved.IsNew)
                {
                    _ItemPersonalizationValueApproved.ItemPersonalizationName = Convert.ToString(_ItemPersonalizationValueApproved.ItemPersonalizationName);
                    _ItemPersonalizationValueApproved.ItemPersonalizationApprovedValue = Convert.ToString(_ItemPersonalizationValueApproved.ItemPersonalizationApprovedValue);
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                }
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
                _ItemPersonalizationValueApproved.ItemPersonalizationName = Convert.ToString(txtName.Text);
                _ItemPersonalizationValueApproved.ItemPersonalizationApprovedValue = Convert.ToString(txtValue.Text);

                if (_ItemPersonalizationValueApproved.IsNew)
                {
                    _ItemPersonalizationValueApproved.WebsiteID = CurrentWebsite.WebsiteID;
                    _ItemPersonalizationValueApproved.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemPersonalizationValueApproved.Create();
                }
                else
                {
                    blnReturn = _ItemPersonalizationValueApproved.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect("/Admin/Website.aspx");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/Website.aspx");
        }
    }
}