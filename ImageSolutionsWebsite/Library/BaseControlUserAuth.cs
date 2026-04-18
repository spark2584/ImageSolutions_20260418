using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BaseControlUserAuth : BaseControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.ThisPage.UserAuth();
            base.OnInit(e);
        }
    }
}