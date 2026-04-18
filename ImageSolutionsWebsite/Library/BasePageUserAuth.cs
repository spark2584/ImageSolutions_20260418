using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageUserAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserAuth();
            base.OnInit(e);
        }
    }
}