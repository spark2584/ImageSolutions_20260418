using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CustomListOverview : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindCustomList();
            }
        }

        protected void BindCustomList()
        {
            try
            {
                this.gvCustomList.DataSource = CurrentUser.CurrentUserWebSite.WebSite.CustomLists;
                this.gvCustomList.DataBind();
            }
            catch(Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}