using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteConversion.Task.Ahold
{
    public class AssignItemToTab
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;
            string strWebsiteID = "47";
            string strCreatedBy = "3";
            int RowCount = 0;
            try
            {
                strSQL = string.Format(@"
SELECT InternalID, ItemName, Tab
FROM Ahold_Item_Tab_20240116
");
                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    try
                    {
                        RowCount++;
                        ImageSolutions.Item.ItemWebsite ItemWebsite = new ImageSolutions.Item.ItemWebsite();
                        ImageSolutions.Item.ItemWebsiteFilter ItemWebsiteFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                        ItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        ItemWebsiteFilter.WebsiteID.SearchString = strWebsiteID;
                        ItemWebsiteFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        ItemWebsiteFilter.InternalID.SearchString = Convert.ToString(objRead["InternalID"]);
                        ItemWebsite = ImageSolutions.Item.ItemWebsite.GetItemWebsite(ItemWebsiteFilter);

                        //Console.WriteLine(String.Format("Row {0}: {1} {2} {3}", RowCount, Convert.ToString(objRead["InternalID"]), Convert.ToString(objRead["ItemName"]), Convert.ToString(objRead["Tab"])));

                        if (ItemWebsite != null && !string.IsNullOrEmpty(ItemWebsite.ItemWebsiteID))
                        {
                            string strTab = Convert.ToString(objRead["Tab"]);
                            string[] Tabs = strTab.Split('>');

                            int counter = 0;
                            string strParentTab = string.Empty;
                            foreach (string _tab in Tabs)
                            {
                                string strCurrent = _tab.Trim();
                                counter++;

                                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab();
                                ImageSolutions.Website.WebsiteTabFilter WebsiteTabFilter = new ImageSolutions.Website.WebsiteTabFilter();
                                WebsiteTabFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                WebsiteTabFilter.WebsiteID.SearchString = strWebsiteID;
                                WebsiteTabFilter.TabName = new Database.Filter.StringSearch.SearchFilter();
                                WebsiteTabFilter.TabName.SearchString = strCurrent;
                                WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                if (counter == 1)
                                {
                                    WebsiteTabFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                                }
                                else
                                {
                                    WebsiteTabFilter.ParentID.SearchString = strParentTab;
                                }

                                WebsiteTab = ImageSolutions.Website.WebsiteTab.GetWebsiteTab(WebsiteTabFilter);

                                if (WebsiteTab == null)
                                {
                                    Console.WriteLine(String.Format("Invalid Tab: {0}", strCurrent));
                                    break;
                                }
                                else
                                {
                                    if (_tab == Tabs[Tabs.Length - 1])
                                    {
                                        ImageSolutions.Website.WebsiteTabItem WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                                        ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                                        WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteTabItemFilter.WebsiteTabID.SearchString = WebsiteTab.WebsiteTabID;
                                        WebsiteTabItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                                        WebsiteTabItemFilter.ItemID.SearchString = ItemWebsite.ItemID;
                                        WebsiteTabItem = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                                        if (WebsiteTabItem != null && !string.IsNullOrEmpty(WebsiteTabItem.WebsiteTabItemID))
                                        {
                                            Console.WriteLine(String.Format("Item Already Exists: {0} - {1}", Convert.ToString(objRead["Name"]), Convert.ToString(objRead["Tab"])));
                                        }
                                        else
                                        {
                                            WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                                            WebsiteTabItem.WebsiteTabID = WebsiteTab.WebsiteTabID;
                                            WebsiteTabItem.ItemID = ItemWebsite.ItemID;
                                            WebsiteTabItem.CreatedBy = strCreatedBy;
                                            WebsiteTabItem.Create();
                                        }

                                    }
                                    else
                                    {
                                        strParentTab = WebsiteTab.WebsiteTabID;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine(String.Format("Missing Item: {0} - {1}", Convert.ToString(objRead["InternalID"]), Convert.ToString(objRead["ItemName"])));
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
    }
}
