using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.HomePage
{
    public partial class Mavis : BasePageUserAccountAuth
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
            }
            else
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }

            litHeader.Text = String.Format("Welcome to the {0}", CurrentWebsite.Name.Replace("Mavis Gear: ", ""));

            if(CurrentWebsite.EnablePackagePayment)
            {
                litHeader2.Text = "This is the website to order your annual package.";
            }
            else
            {
                litHeader2.Text = "This is the website to order additional swag at your own expense.";
            }

            //switch (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID)
            //{
            //    //STS   
            //    case "240":
            //    case "246":
            //    case "247":
            //    case "248":
            //    case "262":
            //    case "263":
            //    case "264":
            //    case "265":
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-01-STS.png";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-07-STS.png";
            //        break;
            //    //Melvin
            //    case "249":
            //    case "250":
            //    case "251":
            //    case "252":
            //    case "266":
            //    case "267":
            //    case "268":
            //    case "269":
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-04-Melvin.png";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-10-Melvin.png";
            //        break;
            //    //Weldon's
            //    case "253":
            //    case "254":
            //    case "255":
            //    case "256":
            //    case "270":
            //    case "271":
            //    case "272":
            //    case "273":
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-05-Wledon.png";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-11-Weldon.png";
            //        break;
            //    //Mavis
            //    case "257":
            //    case "258":
            //    case "259":
            //    case "260":
            //    case "274":
            //    case "275":
            //    case "276":
            //    case "277":
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-02-Mavis.png";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-08-Mavis.png";
            //        break;

            //    //Adirondack
            //    case "299":
            //    case "300":
            //    case "301":
            //    case "302":
            //    case "303":
            //    case "304":
            //    case "305":
            //    case "306":
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20251008/Mavis_Adirondack.jpg";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20251008/Mavis_Adirondack_Mobile.jpg";
            //        break;

            //    default:
            //        imgHomePage.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-LG-02-Mavis.png";
            //        imgHomePageMobile.ImageUrl = "../assets/company/Mavis/Carousel/20250504/MavisBanner-SM-08-Mavis.png";
            //        break;
            //}

            if (CurrentUser.CurrentUserWebSite != null
               && CurrentUser.CurrentUserWebSite.UserAccounts != null
               && CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageImagePath))
            )
            {
                imgHomePage.ImageUrl = CurrentUser.CurrentUserWebSite.UserAccounts.Find(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageImagePath)).WebsiteGroup.HomePageImagePath;
                imgHomePageMobile.ImageUrl = imgHomePage.ImageUrl;
            }


            if (CurrentUser.CurrentUserWebSite != null
                && CurrentUser.CurrentUserWebSite.UserAccounts != null
                && CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageMobileImagePath))
            )
            {
                imgHomePageMobile.ImageUrl = CurrentUser.CurrentUserWebSite.UserAccounts.Find(x => !string.IsNullOrEmpty(x.WebsiteGroup.HomePageMobileImagePath)).WebsiteGroup.HomePageMobileImagePath;
            }
        }
    }
}