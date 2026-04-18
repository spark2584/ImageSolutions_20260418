using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.HomePage
{
    public partial class BurgerKing : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HideBreadcrumb = true;
            if (CurrentWebsite != null && !string.IsNullOrEmpty(CurrentWebsite.StartingPath))
            {
                if (CurrentWebsite.StartingPath.ToLower() != WebUtility.URL.RawUrlNoQuery.ToLower())
                {
                    Response.Redirect(CurrentWebsite.StartingPath);
                }
                litBannerHTML.Text = CurrentWebsite.BannerHTML;
                litFeaturedProduct.Text = CurrentWebsite.FeaturedProductHTML;
            }
            else
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }
        }
    }
}