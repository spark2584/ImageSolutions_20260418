using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateCustomizationField : BasePageAdminUserWebSiteAuth
    {
        private string mCustomizationID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("customizationid")))
            {
                mCustomizationID = Request.QueryString.Get("customizationid");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CustomizationOverview.aspx"));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.CustomizationField CustomizationField = new ImageSolutions.Customization.CustomizationField();
                CustomizationField.CustomizationID = mCustomizationID;
                CustomizationField.CustomizationFieldName = Convert.ToString(txtName.Text);
                CustomizationField.Type = Convert.ToString(ddlType.SelectedValue);
                CustomizationField.Create();

                Response.Redirect(String.Format("/Admin/Customization.aspx?id={0}", mCustomizationID));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}