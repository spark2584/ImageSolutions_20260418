using ImageSolutions.SalesOrder;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemOverview : BasePageAdminUserWebSiteAuth
    {
        protected string mItemNumber = string.Empty;
        protected string mItemName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mItemNumber = Request.QueryString.Get("itemnumber");
            mItemName = Request.QueryString.Get("itemname");

            if (!Page.IsPostBack)
            {
                this.txtItemNumber.Text = mItemNumber;
                this.txtItemName.Text = mItemName;

                BindItemWebsite();
            }
        }

        protected void BindItemWebsite()
        {
            List<ImageSolutions.Item.ItemWebsite> ItemWebsites = null;
            ImageSolutions.Item.ItemWebsiteFilter ItemWebsitesFilter = null;
            int intTotalRecord = 0;

            try
            {
                ItemWebsitesFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                ItemWebsitesFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                ItemWebsitesFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

                if (!string.IsNullOrEmpty(this.txtItemNumber.Text.Trim()))
                {
                    ItemWebsitesFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                    ItemWebsitesFilter.ItemNumber.SearchString = this.txtItemNumber.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtItemName.Text.Trim()))
                {
                    ItemWebsitesFilter.ItemName = new Database.Filter.StringSearch.SearchFilter();
                    ItemWebsitesFilter.ItemName.SearchString = this.txtItemName.Text.Trim();
                }

                ItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(ItemWebsitesFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);
                

                gvItems.DataSource = ItemWebsites;
                gvItems.DataBind();
                ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally 
            {
                ItemWebsites = null;
                ItemWebsitesFilter = null;
            }            
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CreateItem.aspx"));
        }

        protected void txtFilter_TextChanged(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/ItemOverview.aspx?itemname=" + Server.UrlEncode(this.txtItemName.Text.Trim()) + "&itemnumber=" + Server.UrlEncode(this.txtItemNumber.Text.Trim())));
        }
    }
}