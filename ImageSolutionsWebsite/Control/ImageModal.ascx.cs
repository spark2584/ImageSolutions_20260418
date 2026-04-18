using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class ImageModal : BaseControl
    {
        public string DialogImageURL
        {
            get { return imgDialogImage.ImageUrl; }
            set { imgDialogImage.ImageUrl = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        public bool Show()
        {
            ImageSolutions.Item.Item objItem = null;

            try
            {
                WebUtility.ClearForm(this);


                this.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }

            return true;
        }
    }
}