using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Logout : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser.Logout();
            CurrentUser.CurrentUserWebSite.Logout();
            CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();
            //CurrentWebsite.Logout();
            Response.Redirect("/Login.aspx");
        }
    }
}