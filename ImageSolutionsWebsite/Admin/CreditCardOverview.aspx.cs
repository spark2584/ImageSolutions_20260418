using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreditCardOverview : BasePageAdminUserWebSiteAuth
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

                gvCreditCard.DataSource = UserCreditCards;
                gvCreditCard.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}