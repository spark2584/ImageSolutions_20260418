using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateCustomization : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CustomizationOverview.aspx"));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.Customization Customization = new ImageSolutions.Customization.Customization();
                Customization.CustomizationName = Convert.ToString(txtName.Text);
                Customization.Create();

                Response.Redirect(String.Format("/Admin/CustomizationOverview.aspx"));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}