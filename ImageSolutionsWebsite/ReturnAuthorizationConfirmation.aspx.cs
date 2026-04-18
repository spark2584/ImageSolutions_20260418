using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ReturnAuthorizationConfirmation : BasePageUserAccountAuth
    {
        protected string mRMAID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mRMAID = Request.QueryString.Get("RMAID");

            if (!Page.IsPostBack)
            {
                ImageSolutions.RMA.RMA RMA = new ImageSolutions.RMA.RMA(mRMAID);

                lblRMAID.Text = RMA.RMAID;

                if (string.IsNullOrEmpty(RMA.ShippingLabelPath))
                {
                    pnlUseMyOwn.Visible = true;
                    pnlLabelInstruction.Visible = false;
                }
                else
                {
                    pnlUseMyOwn.Visible = false;
                    pnlLabelInstruction.Visible = true;
                    hlnkShippingLabel.NavigateUrl = RMA.ShippingLabelPath;
                }
            }

            base.HideBreadcrumb = true;
        }
    }
}