using ImageSolutions.Website;
using ImageSolutions.Item;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ImageSolutions.Website
{
    public class MyGroupWebsiteTabItem
    {
        public string WebsiteGroupID { get; set; }
        public string WebsiteTabItemID { get; set; }
        private WebsiteTabItem mWebsiteTabItem = null;
        public WebsiteTabItem WebsiteTabItem
        {
            get
            {
                if (mWebsiteTabItem == null && !string.IsNullOrEmpty(WebsiteTabItemID))
                {
                    try
                    {
                        mWebsiteTabItem = new WebsiteTabItem(WebsiteTabItemID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mWebsiteTabItem;
            }
        }
        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && WebsiteTabItem != null && !string.IsNullOrEmpty(WebsiteTabItem.ItemID))
                {
                    try
                    {
                        mItem = new Item.Item(WebsiteTabItem.ItemID);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                    }
                }
                return mItem;
            }
        }
        public double Price
        {
            get
            {
                double dbReturn = 0;

                if (Item != null)
                {
                    ItemPricing ItemPricing = Item.ItemPricings.Find(x => x.WebsiteGroupID == WebsiteGroupID);
                    if (ItemPricing != null && !string.IsNullOrEmpty(ItemPricing.ItemPricingID))
                    {
                        return ItemPricing.Price;
                    }
                    else
                    {
                        return Item.BasePrice == null ? 0 : Convert.ToDouble(Item.BasePrice);
                    }
                }

                return dbReturn < 0 ? 0 : dbReturn;
            }
        }
        public MyGroupWebsiteTabItem(string websitegroupid, string websitetabitemid)
        {
            WebsiteGroupID = websitegroupid;
            WebsiteTabItemID = websitetabitemid;
        }
    }
}
