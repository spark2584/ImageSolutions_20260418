using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CustomizationField : BasePageAdminUserWebSiteAuth
    {
        private string mCustomizationFieldID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("id")))
            {
                mCustomizationFieldID = Request.QueryString.Get("id");
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(mCustomizationFieldID))
                {
                    GetCustomizationFieldData();
                }
            }
        }
        public void GetCustomizationFieldData()
        {
            try
            {
                ImageSolutions.Customization.CustomizationField CustomizationField = new ImageSolutions.Customization.CustomizationField(Convert.ToString(mCustomizationFieldID));
                txtName.Text = CustomizationField.CustomizationFieldName;
                ddlType.SelectedValue = CustomizationField.Type;

                lblHeader.Text = CustomizationField.CustomizationFieldName;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ImageSolutions.Customization.CustomizationField CustomizationField = null;
            try
            {
                CustomizationField = new ImageSolutions.Customization.CustomizationField(mCustomizationFieldID);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            Response.Redirect(string.Format("/Admin/Customization.aspx?id={0}", CustomizationField.CustomizationID));
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.CustomizationField CustomizationField = new ImageSolutions.Customization.CustomizationField(mCustomizationFieldID);
                CustomizationField.Delete();

                Response.Redirect(string.Format("/Admin/Customization.aspx?id={0}", CustomizationField.CustomizationID));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.CustomizationField CustomizationField = new ImageSolutions.Customization.CustomizationField();
                CustomizationField.CustomizationFieldName = Convert.ToString(txtName.Text);
                CustomizationField.Type = Convert.ToString(ddlType.SelectedValue);
                CustomizationField.Create();

                WebUtility.DisplayJavascriptMessage(this, "Update completed");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnAddFieldValue_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Admin/CreateCustomizationFieldValue.aspx?id={0}", mCustomizationFieldID));
        }

        protected void gvCustomizationFieldValue_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }
    }
}