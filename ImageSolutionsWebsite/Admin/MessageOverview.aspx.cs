using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class MessageOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsiteMessage();
            }
        }

        protected void BindWebsiteMessage()
        {
            try
            {
                this.gvWebsiteMessages.DataSource = CurrentUser.CurrentUserWebSite.WebSite.WebsiteMessages;
                this.gvWebsiteMessages.DataBind();
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}