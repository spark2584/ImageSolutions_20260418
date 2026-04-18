using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CustomizationOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindCustomizationGroup();
            }
        }

        protected void BindCustomizationGroup()
        {
            try
            {
                List<ImageSolutions.Customization.Customization> Customizations = new List<ImageSolutions.Customization.Customization>();
                Customizations = ImageSolutions.Customization.Customization.GetCustomizations();

                gvCustomization.DataSource = Customizations;
                gvCustomization.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(String.Format("/Admin/CreateGroup.aspx?websiteid={0}", ddlWebsite.SelectedValue));
            }
            finally { }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CreateCustomization.aspx"));
        }

        protected void gvCustomization_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineUpdate")
            {
                string strCustomizationID = Convert.ToString(e.CommandArgument);
                Response.Redirect(String.Format("/Admin/Customization.aspx?id={0}", strCustomizationID));
            }
        }
    }
}