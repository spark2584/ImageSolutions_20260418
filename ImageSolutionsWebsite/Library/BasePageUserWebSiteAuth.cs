using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageUserWebSiteAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserWebSiteAuth();
            base.OnInit(e);
        }
    }
}