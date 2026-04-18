using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageUserNoAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserNoAuth();
            base.OnInit(e);
        }
    }
}