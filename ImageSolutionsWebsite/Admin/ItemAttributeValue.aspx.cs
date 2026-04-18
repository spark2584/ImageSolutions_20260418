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
    public partial class ItemAttributeValue : BasePageAdminUserWebSiteAuth
    {
        protected string mAttributeValueID = string.Empty;
        protected string mAttributeID = string.Empty;

        private ImageSolutions.Attribute.AttributeValue mAttributeValue = null;
        protected ImageSolutions.Attribute.AttributeValue _AttributeValue
        {
            get
            {
                if (mAttributeValue == null)
                {
                    if (string.IsNullOrEmpty(mAttributeValueID))
                        mAttributeValue = new ImageSolutions.Attribute.AttributeValue();
                    else
                        mAttributeValue = new ImageSolutions.Attribute.AttributeValue(mAttributeValueID);
                }
                return mAttributeValue;
            }
            set
            {
                mAttributeValue = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/ItemAttribute.aspx?id=" + (_AttributeValue.IsNew ? mAttributeID : _AttributeValue.AttributeID) + "&tab=2";
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
            mAttributeValueID = Request.QueryString.Get("id");
            mAttributeID = Request.QueryString.Get("attributeid");

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
                if (!_AttributeValue.IsNew)
                {
                    txtAttributeValue.Text = _AttributeValue.Value;
                    txtAbbreviation.Text = _AttributeValue.Abbreviation;
                    txtBackgroundColor.Text = _AttributeValue.BackgroundColor;
                    cbIsDefault.Checked = _AttributeValue.IsDefault;
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
                _AttributeValue.Value = txtAttributeValue.Text.Trim();
                _AttributeValue.Abbreviation = txtAbbreviation.Text.Trim();
                _AttributeValue.BackgroundColor = txtBackgroundColor.Text.Trim();
                _AttributeValue.IsDefault = cbIsDefault.Checked;

                if (_AttributeValue.IsNew)
                {
                    _AttributeValue.AttributeID = mAttributeID;
                    _AttributeValue.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _AttributeValue.Create();
                }
                else
                {
                    blnReturn = _AttributeValue.Update();
                }

                if (_AttributeValue.IsDefault)
                {
                    foreach (ImageSolutions.Attribute.AttributeValue objAttributeValue in _AttributeValue.Attribute.AttributeValues.FindAll(m => m.IsDefault))
                    {
                        if (objAttributeValue.AttributeValueID != _AttributeValue.AttributeValueID)
                        {
                            objAttributeValue.IsDefault = false;
                            objAttributeValue.Update();
                        }
                    }
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
                blnReturn = _AttributeValue.Delete();
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