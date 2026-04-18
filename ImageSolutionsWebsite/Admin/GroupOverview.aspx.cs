using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class GroupOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsiteGroup();
            }
        }

        protected void BindWebsiteGroup()
        {
            try
            {
                this.gvWebsiteGroup.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteGroups;
                this.gvWebsiteGroup.DataBind();
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}