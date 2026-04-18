using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BaseControlWebSiteAuth : BaseControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.ThisPage.UserWebSiteAuth();
            base.OnInit(e);
        }
    }
}