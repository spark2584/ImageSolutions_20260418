using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageAdminUserWebSiteAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserAdminWebSiteAuth();
            base.OnInit(e);
        }
    }
}