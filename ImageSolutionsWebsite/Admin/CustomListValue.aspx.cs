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
    public partial class CustomListValue : BasePageAdminUserWebSiteAuth
    {
        protected string mCustomListValueID = string.Empty;
        protected string mCustomListID = string.Empty;

        private ImageSolutions.Custom.CustomListValue mCustomListValue = null;
        protected ImageSolutions.Custom.CustomListValue _CustomListValue
        {
            get
            {
                if (mCustomListValue == null)
                {
                    if (string.IsNullOrEmpty(mCustomListValueID))
                        mCustomListValue = new ImageSolutions.Custom.CustomListValue();
                    else
                        mCustomListValue = new ImageSolutions.Custom.CustomListValue(mCustomListValueID);
                }
                return mCustomListValue;
            }
            set
            {
                mCustomListValue = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/CustomList.aspx?id=" + (_CustomListValue.IsNew ? mCustomListID : _CustomListValue.CustomListID) + "&tab=2";
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
            mCustomListValueID = Request.QueryString.Get("id");
            mCustomListID = Request.QueryString.Get("customlistid");

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
                if (!_CustomListValue.IsNew)
                {
                    txtListValue.Text = _CustomListValue.ListValue;
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
                _CustomListValue.ListValue = txtListValue.Text.Trim();

                if (_CustomListValue.IsNew)
                {
                    _CustomListValue.CustomListID = mCustomListID;
                    _CustomListValue.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _CustomListValue.Create();
                }
                else
                {
                    blnReturn = _CustomListValue.Update();
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
                blnReturn = _CustomListValue.Delete();
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