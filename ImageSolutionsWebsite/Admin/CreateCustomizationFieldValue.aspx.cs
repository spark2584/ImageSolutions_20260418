using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateCustomizationFieldValue : BasePageAdminUserWebSiteAuth
    {
        private string mCustomizationFieldID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("customizationfieldid ")))
            {
                mCustomizationFieldID = Request.QueryString.Get("customizationfieldid");
            }
            else
            {
                Response.Redirect(String.Format("/Admin/CustomizationOverview.aspx"));
            }

            if (!Page.IsPostBack)
            {
                Init();
            }
        }
        protected void Init()
        {

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CustomizationField.aspx?id={0}", mCustomizationFieldID));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.CustomizationFieldValue CustomizationFieldValue = new ImageSolutions.Customization.CustomizationFieldValue();
                CustomizationFieldValue.CustomizationFieldID = Convert.ToString(mCustomizationFieldID);
                CustomizationFieldValue.Value = txtFieldValue.Text.Trim();
                CustomizationFieldValue.Query = txtQuery.Text.Trim();
                CustomizationFieldValue.Create();

                Response.Redirect(String.Format("/Admin/CustomizationField.aspx?id={0}", mCustomizationFieldID));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}