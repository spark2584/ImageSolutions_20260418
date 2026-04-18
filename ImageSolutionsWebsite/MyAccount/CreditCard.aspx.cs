using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class CreditCard : BasePageUserAccountAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindCreditCard();
            }
        }
        protected void BindCreditCard()
        {
            try
            {
                List<ImageSolutions.User.UserCreditCard> UserCreditCards = new List<ImageSolutions.User.UserCreditCard>();
                ImageSolutions.User.UserCreditCardFilter UserCreditCardFilter = new ImageSolutions.User.UserCreditCardFilter();
                UserCreditCardFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                UserCreditCardFilter.UserInfoID.SearchString = CurrentUser.UserInfoID;
                UserCreditCards = ImageSolutions.User.UserCreditCard.GetUserCreditCards(UserCreditCardFilter);

                rptCreditCard.DataSource = UserCreditCards;
                rptCreditCard.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void rptCreditCard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //Reference the Repeater Item.
                RepeaterItem item = e.Item;

                Image imgLogo = (Image)item.FindControl("imgLogo");
                HiddenField hfCreditCardType = (HiddenField)item.FindControl("hfCreditCardType");

                switch (hfCreditCardType.Value)
                {
                    case "AMEX":
                        imgLogo.ImageUrl = "../assets/images/amex.png";
                        break;
                    case "MasterCard":
                        imgLogo.ImageUrl = "../assets/images/mastercard.png";
                        break;
                    case "Visa":
                        imgLogo.ImageUrl = "../assets/images/visa.png";
                        break;
                    case "Discover":
                        imgLogo.ImageUrl = "../assets/images/discover.png";
                        break;
                    default:
                        imgLogo.ImageUrl = string.Empty; ;
                        break;
                }
            }
        }

        protected void rptCreditCard_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remove")
            {
                string strUserCreditCardID = Convert.ToString(e.CommandArgument);
                try
                {
                    //ImageSolutions.CreditCard.CreditCard CreditCard = new ImageSolutions.CreditCard.CreditCard(strCreditCardID);
                    //CreditCard.Delete();

                    ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard(strUserCreditCardID);
                    UserCreditCard.Delete();

                    BindCreditCard();
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
                finally { }
            }
        }
    }
}