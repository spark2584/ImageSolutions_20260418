using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class WebsiteTab : BasePageUserAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindWebsiteTab(CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs);                
            }
        }
        protected void BindWebsiteTab(List<ImageSolutions.Website.WebsiteTab> websiteTabs)
        {
            this.gvWebsiteTab.DataSource = websiteTabs;
            this.gvWebsiteTab.DataBind();
        }

        protected void gvWebsiteTab_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //string strParentWebsiteTabID = Convert.ToString(e.CommandArgument);//this.gvShoppingCartLine.DataKeys[0].Value.ToString();

            GridViewRow row = (GridViewRow)((System.Web.UI.Control)e.CommandSource).Parent.Parent;

            if (e.CommandName == "OpenWebsiteTab")
            {
                string WebSiteTabID = Convert.ToString(e.CommandArgument);

                Response.Redirect(string.Format("/items.aspx?WebsiteTabID={0}", WebSiteTabID));
            }
            else if (e.CommandName == "expandWebsiteTab")
            {
                int strRowIndex = row.RowIndex;

                if (CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs[strRowIndex].ChildWebsiteTabs != null 
                    && CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs[strRowIndex].ChildWebsiteTabs.Count > 0)
                {
                    this.gvWebsiteTab.DataSource = null;
                    BindWebsiteTab(CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs[strRowIndex].ChildWebsiteTabs);
                }
            }
            else if(e.CommandName == "retractWebsiteTab")
            {
                this.gvWebsiteTab.DataSource = null;
                BindWebsiteTab(CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs);
            }
        }
    }
}