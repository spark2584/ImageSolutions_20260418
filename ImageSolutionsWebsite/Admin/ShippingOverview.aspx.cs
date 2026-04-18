using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ShippingOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsiteShippingService();
            }
        }

        protected void BindWebsiteShippingService()
        {
            try
            {
                gvWebsiteShippingService.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteShippingServices;
                gvWebsiteShippingService.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/Shipping.aspx"));
        }
    }
}