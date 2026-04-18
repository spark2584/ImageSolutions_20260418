using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite.Library
{
    public class BaseMaster : System.Web.UI.MasterPage
    {
        public BasePage ThisPage
        {
            get
            {
                if (this.Page is BasePage)
                    return (BasePage)this.Page;
                else
                    throw new Exception("Page containing this user control must inherit from BasePage");
            }
        }
    }
}