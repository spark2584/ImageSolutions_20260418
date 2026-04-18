using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.ssologin
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser.Logout();
            CurrentUser.CurrentUserWebSite.Logout();
            CurrentUser.CurrentUserWebSite.CurrentUserAccount.Logout();

            Context.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
            {
                RedirectUri = "/"
            }, "Saml2");
        }
    }
}