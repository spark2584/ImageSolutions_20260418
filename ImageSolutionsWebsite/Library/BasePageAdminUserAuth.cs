using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BasePageAdminUserAuth : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.UserAdminAuth();
            base.OnInit(e);
        }
    }
}