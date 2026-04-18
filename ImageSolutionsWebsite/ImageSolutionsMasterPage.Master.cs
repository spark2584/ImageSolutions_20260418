using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ImageSolutionsWebsite.Controls;
using ImageSolutionsWebsite.Library;

namespace ImageSolutionsWebsite
{
    public partial class ImageSolutionsMasterPage1 : BaseMaster
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                imgLogo.Src = "/assets/images/icon/logo.png";
            }
        }
    }
}