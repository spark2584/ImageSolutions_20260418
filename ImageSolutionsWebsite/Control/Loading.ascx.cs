using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class Loading : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool Show()
        {
            exampleModal.Visible = true;
            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "Function", "Show()", true);

            return true;
        }

        public bool Hide()
        {
            exampleModal.Visible = false;

            return true;
        }
    }
}