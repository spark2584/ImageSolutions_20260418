using ImageSolutions.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class SuperceedingItem : BaseControlUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = false;

            if (ThisPage.CurrentWebsite.IsLoggedIn && HttpContext.Current.Request.Cookies["WebsiteMessage"] == null && !Request.Url.ToString().ToLower().Contains("login.aspx"))
            {
                if (ThisPage.CurrentWebsite.WebsiteMessages != null && ThisPage.CurrentWebsite.WebsiteMessages.Count > 0)
                {
                    ImageSolutions.Website.WebsiteMessage objWebsiteMessage = ThisPage.CurrentWebsite.WebsiteMessages.Find(m => m.StartDate <= DateTime.Now.Date && m.EndDate > DateTime.Now.Date);
                    if (objWebsiteMessage != null)
                    {
                        //this.lblMessage.Text = objWebsiteMessage.Message;
                        this.Visible = true;
                        HttpContext.Current.Response.Cookies["WebsiteMessage"].Value = objWebsiteMessage.WebsiteMessageID;
                    }
                }
            }
        }

        public string ItemID
        {
            get { return Convert.ToString(ViewState["ItemID"]); }
            set { ViewState["ItemID"] = value; }
        }

        public bool Show()
        {
            ImageSolutions.Item.Item objItem = null;

            try
            {
                WebUtility.ClearForm(this);

                objItem = new ImageSolutions.Item.Item(ItemID);

                this.gvSuperceedingItems.DataSource = objItem.SuperceedingItems;
                this.gvSuperceedingItems.DataBind();

                this.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }

            return true;
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            ImageSolutions.Item.Item objReplacementItem = null;

            foreach (GridViewRow objRow in this.gvSuperceedingItems.Rows)
            {
                string strReplacementItemID = this.gvSuperceedingItems.DataKeys[objRow.RowIndex].Values["ReplacementItemID"].ToString();
                string strQuantity = ((TextBox)objRow.FindControl("txtQuantity")).Text.Trim();
                objReplacementItem = new Item(strReplacementItemID);

                if (Utility.IsInteger(strQuantity))
                {
                    ImageSolutions.ShoppingCart.ShoppingCartLine objShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    objShoppingCartLine.ShoppingCartID = ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    objShoppingCartLine.ItemID = objReplacementItem.ItemID;
                    objShoppingCartLine.Quantity = Convert.ToInt32(strQuantity);
                    objShoppingCartLine.UnitPrice = objReplacementItem.BasePrice.Value;
                    objShoppingCartLine.Create();
                }
                ThisPage.CurrentUser.CurrentUserWebSite.ShoppingCart.Update();
            }
            Response.Redirect("/ShoppingCart.aspx");
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {

        }
    }
}