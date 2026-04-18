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

namespace ImageSolutions.Item
{
    public class MyGroupItem
    {
        public string WebsiteGroupID { get; set; }
        public string ItemID { get; set; }
        private Item mItem = null;
        public Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    try
                    {
                        mItem = new Item(ItemID);
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

        public MyGroupItem(string websitegroupid, string itemid)
        {
            WebsiteGroupID = websitegroupid;
            ItemID = itemid;
        }
    }
}
