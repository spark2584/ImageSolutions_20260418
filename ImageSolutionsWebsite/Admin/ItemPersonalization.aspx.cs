using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemPersonalization : BasePageAdminUserWebSiteAuth
    {
        protected string mItemID = string.Empty;
        protected string mItemPersonalizationID = string.Empty;
        private ImageSolutions.Item.ItemPersonalization mItemPersonalization = null;
        protected ImageSolutions.Item.ItemPersonalization _ItemPersonalization
        {
            get
            {
                if (mItemPersonalization == null)
                {
                    if (string.IsNullOrEmpty(mItemPersonalizationID))
                        mItemPersonalization = new ImageSolutions.Item.ItemPersonalization();
                    else
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
                    return "/Admin/Item.aspx?id=" + mItemID + "&tab=7";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            mItemID = Request.QueryString.Get("itemid");
            mItemPersonalizationID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                if (!_ItemPersonalization.IsNew)
                {
                    txtName.Text = _ItemPersonalization.Name;
                    ddlType.SelectedValue = _ItemPersonalization.Type;
                    ddlDefaultValue.SelectedValue = Convert.ToString(_ItemPersonalization.DefaultValue);
                    chkRequireVerification.Checked = _ItemPersonalization.RequireVerification;
                    chkRequireApproval.Checked = _ItemPersonalization.RequireApproval;
                    chkAllowBlank.Checked = _ItemPersonalization.AllowBlank;
                    chkInActive.Checked = _ItemPersonalization.InActive;                    
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }
                aCancel.HRef = ReturnURL;

                phDropdownValue.Visible = ddlType.SelectedValue == "dropdown";
                if (phDropdownValue.Visible) BindItemPersonalizationValueList();
                phRequireVerification.Visible = !phDropdownValue.Visible;
                pnlDefaultValue.Visible = ddlType.SelectedValue == "text";
                pnlAllowBlank.Visible = ddlType.SelectedValue == "text";

                //phDropdownValue.Visible = _ItemPersonalization != null && !string.IsNullOrEmpty(_ItemPersonalization.ItemPersonalizationID) && _ItemPersonalization.Type == "dropdown";
                //if (phDropdownValue.Visible) BindItemPersonalizationValueList();
                //phRequireVerification.Visible = !phDropdownValue.Visible;
                //pnlDefaultValue.Visible = _ItemPersonalization != null && string.IsNullOrEmpty(_ItemPersonalization.ItemPersonalizationID) && _ItemPersonalization.Type == "text";
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemPersonalizationValueList()
        {
            try
            {
                if(_ItemPersonalization != null && !string.IsNullOrEmpty(_ItemPersonalization.ItemPersonalizationID))
                {
                    gvItemPersonalizationValueList.DataSource = _ItemPersonalization.ItemPersonalizationValueLists.OrderBy(x => x.Value);
                    gvItemPersonalizationValueList.DataBind();
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
                _ItemPersonalization.Name = txtName.Text;
                _ItemPersonalization.RequireVerification = chkRequireVerification.Checked;
                _ItemPersonalization.RequireApproval = chkRequireApproval.Checked;
                _ItemPersonalization.AllowBlank = chkAllowBlank.Checked;
                _ItemPersonalization.InActive = chkInActive.Checked;
                _ItemPersonalization.Type = Convert.ToString(ddlType.SelectedValue);
                _ItemPersonalization.DefaultValue = Convert.ToString(ddlDefaultValue.SelectedValue);

                if (_ItemPersonalization.IsNew)
                {
                    _ItemPersonalization.ItemID = mItemID;
                    _ItemPersonalization.Sort = _Item.ItemPersonalizations.Count + 1;
                    _ItemPersonalization.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _ItemPersonalization.Create();
                }
                else
                {
                    blnReturn = _ItemPersonalization.Update();
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
                blnReturn = _ItemPersonalization.Delete();
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

        protected void gvItemPersonalizationValueList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void gvItemPersonalizationValueList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            phDropdownValue.Visible = ddlType.SelectedValue == "dropdown";
            if (phDropdownValue.Visible) BindItemPersonalizationValueList();
            phRequireVerification.Visible = !phDropdownValue.Visible;
            pnlDefaultValue.Visible = ddlType.SelectedValue == "text";
            pnlAllowBlank.Visible = ddlType.SelectedValue == "text";

            //phDropdownValue.Visible = _ItemPersonalization != null && string.IsNullOrEmpty(_ItemPersonalization.ItemPersonalizationID) && _ItemPersonalization.Type == "dropdown";
            //phRequireVerification.Visible = !phDropdownValue.Visible;

            //pnlDefaultValue.Visible = _ItemPersonalization != null && string.IsNullOrEmpty(_ItemPersonalization.ItemPersonalizationID) && _ItemPersonalization.Type == "text";
        }
    }
}