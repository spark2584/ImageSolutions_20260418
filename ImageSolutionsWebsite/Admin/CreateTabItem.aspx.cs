using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateTabItem : BasePageAdminUserWebSiteAuth
    {
        private string mWebsiteTabID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("websitetabid")))
            {
                mWebsiteTabID = Request.QueryString.Get("websitetabid");
            }
            else
            {
                Response.Redirect("/Admin/TabOverview.aspx");
            }

            if (!Page.IsPostBack)
            {
                BindItem();
                GetWebsiteTabData(mWebsiteTabID);
            }
        }

        public void GetWebsiteTabData(string websitetabid)
        {
            try
            {
                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(Convert.ToString(websitetabid));
                lblHeader.Text = string.Format("Add Tab Item - {0}", WebsiteTab.TabName);
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/TabOverview.aspx"));
            }
            finally { }
        }

        protected void BindItem()
        {
            try
            {
                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebsiteTabID);

                List<ImageSolutions.Item.ItemWebsite> ItemWebsites = new List<ImageSolutions.Item.ItemWebsite>();
                ImageSolutions.Item.ItemWebsiteFilter ItemWebsiteFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                ItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                ItemWebsiteFilter.WebsiteID.SearchString = WebsiteTab.WebsiteID;
                ItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(ItemWebsiteFilter);

                //List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();
                //Items = ImageSolutions.Item.Item.GetItems();
                this.ddlItem.DataSource = ItemWebsites;
                this.ddlItem.DataTextField = "ItemNumber";
                this.ddlItem.DataValueField = "ItemID";
                this.ddlItem.DataBind();
                this.ddlItem.Items.Insert(0, new ListItem(String.Empty, string.Empty));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(string.Format("/Admin/TabOverview.aspx"));
            }
            finally { }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Website.WebsiteTabItem WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                WebsiteTabItem.WebsiteTabID = Convert.ToString(mWebsiteTabID);
                WebsiteTabItem.ItemID = Convert.ToString(ddlItem.SelectedValue);
                WebsiteTabItem.Inactive = cbItemInactive.Checked;
                WebsiteTabItem.Create();

                Response.Redirect(string.Format("/Admin/Tab.aspx?id={0}", mWebsiteTabID));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Admin/Tab.aspx?id={0}", mWebsiteTabID));
        }
    }
}