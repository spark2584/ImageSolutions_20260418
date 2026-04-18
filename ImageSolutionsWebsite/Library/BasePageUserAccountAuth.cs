using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageUserAccountAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserAccountAuth();
            base.OnInit(e);
        }
    }
}