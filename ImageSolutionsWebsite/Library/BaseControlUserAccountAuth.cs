using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class BaseControlUserAccountAuth : BaseControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.ThisPage.UserAccountAuth();
            base.OnInit(e);
        }
    }
}