using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task
{
    public class CopyCategoryItemFromStaging
    {
        public bool Execute()
        {
            string strFromWebsiteID = "53";
            string strToWebsiteID = "20";
            string strCreatedBy = "1";

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                string strConnectionString = "Data Source=image-solutions.cwfbgfgwdwwa.us-west-1.rds.amazonaws.com,1433;Initial Catalog=ImageSolutionsStaging;Persist Security Info=True;User ID=admin;Password=Password$1";

                strSQL = string.Format(@"
SELECT wt.WebsiteTabID, wt.TabName, AllowAllGroups, wt.ImageURL, Inactive, Sort
FROM WebsiteTab (NOLOCK) wt
WHERE wt.ParentID is null
and wt.WebsiteID = {0}
ORDER BY wt.Sort
"
                    , Database.HandleQuote(strFromWebsiteID)
                );

                objRead = Database.GetDataReader(strSQL, strConnectionString);

                while (objRead.Read())
                {
                    string strWebsiteTabID = Convert.ToString(objRead["WebsiteTabID"]);
                    string strTabName = Convert.ToString(objRead["TabName"]);
                    string strImageURL = Convert.ToString(objRead["ImageURL"]);
                    bool blnAllowAllGroups = Convert.ToBoolean(objRead["AllowAllGroups"]);
                    int intSort = Convert.ToInt32(objRead["Sort"]);
                    bool blnInactive = Convert.ToBoolean(objRead["Inactive"]);

                    ProcessWebsiteTab(strFromWebsiteID, strToWebsiteID, string.Empty, strCreatedBy, strWebsiteTabID, strTabName, strImageURL
                        , blnAllowAllGroups, intSort, blnInactive);

                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            return true;
        }

        public bool ProcessWebsiteTab(string fromwebsiteid, string towebsiteid, string parentid, string createdby, string stagingwebsitetabid, string tabname
            , string imageurl, bool allowallgroups, int sort, bool inactive, SqlConnection conn = null, SqlTransaction trans = null)
        {

            bool blnInit = false;
            try
            {
                if (conn == null)
                {
                    blnInit = true;

                    conn = new SqlConnection(Database.DefaultConnectionString);
                    conn.Open();
                    trans = conn.BeginTransaction();
                }

                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab();
                ImageSolutions.Website.WebsiteTabFilter WebsiteTabFilter = new ImageSolutions.Website.WebsiteTabFilter();
                WebsiteTabFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabFilter.WebsiteID.SearchString = towebsiteid;
                WebsiteTabFilter.TabName = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabFilter.TabName.SearchString = tabname;
                if (string.IsNullOrEmpty(parentid))
                {
                    WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteTabFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                }
                else
                {
                    WebsiteTabFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteTabFilter.ParentID.SearchString = parentid;
                }
                WebsiteTab = ImageSolutions.Website.WebsiteTab.GetWebsiteTab(WebsiteTabFilter);

                if (WebsiteTab == null)
                {
                    WebsiteTab = new ImageSolutions.Website.WebsiteTab();
                    WebsiteTab.WebsiteID = towebsiteid;
                    WebsiteTab.TabName = tabname;
                    WebsiteTab.ImageURL = imageurl;
                    WebsiteTab.AllowAllGroups = allowallgroups;
                    WebsiteTab.Sort = sort;
                    WebsiteTab.Inactive = inactive;
                    WebsiteTab.CreatedBy = createdby;

                    if (!string.IsNullOrEmpty(parentid))
                    {
                        WebsiteTab.ParentID = parentid;
                    }
                    
                    WebsiteTab.Create(conn, trans);
                }

                ProcessWebsiteTabItems(conn, trans, stagingwebsitetabid, WebsiteTab.WebsiteTabID, createdby);

                SqlDataReader objRead = null;
                string strConnectionString = "Data Source=image-solutions.cwfbgfgwdwwa.us-west-1.rds.amazonaws.com,1433;Initial Catalog=ImageSolutionsStaging;Persist Security Info=True;User ID=admin;Password=Password$1";

                string strSQL = string.Format(@"
SELECT wt.WebsiteTabID, wt.TabName, AllowAllGroups, ImageURL, Inactive, Sort
FROM WebsiteTab (NOLOCK) wt
WHERE wt.WebsiteID = {0}
and wt.ParentID = {1}
ORDER BY wt.Sort
"
                    , Database.HandleQuote(fromwebsiteid)
                    , Database.HandleQuote(stagingwebsitetabid)
                );

                objRead = Database.GetDataReader(strSQL, strConnectionString);

                while (objRead.Read())
                {
                    string strWebsiteTabID = Convert.ToString(objRead["WebsiteTabID"]);
                    string strTabName = Convert.ToString(objRead["TabName"]);
                    string strImageURL = Convert.ToString(objRead["ImageURL"]);
                    bool blnAllowAllGroups = Convert.ToBoolean(objRead["AllowAllGroups"]);
                    int intSort = Convert.ToInt32(objRead["Sort"]);
                    bool blnInactive = Convert.ToBoolean(objRead["Inactive"]);

                    ProcessWebsiteTab(fromwebsiteid, towebsiteid, WebsiteTab.WebsiteTabID, createdby, strWebsiteTabID, strTabName, strImageURL
                     , blnAllowAllGroups, intSort, blnInactive, conn, trans);
                }

                if (blnInit)
                {
                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                if (trans != null && trans.Connection != null) trans.Rollback();

                Console.Write(string.Format("{0}", ex.Message));
            }

            return true;
        }

        public bool ProcessWebsiteTabItems(SqlConnection conn, SqlTransaction trans, string stagingwebsitetabid, string websitetabid, string createdby)
        {
            string strConnectionString = "Data Source=image-solutions.cwfbgfgwdwwa.us-west-1.rds.amazonaws.com,1433;Initial Catalog=ImageSolutionsStaging;Persist Security Info=True;User ID=admin;Password=Password$1";

            SqlDataReader objRead = null;
            string strSQL = string.Format(@"
SELECT i.ItemNumber, wti.Inactive
FROM WebsiteTabItem (NOLOCK) wti
Inner Join Item (NOLOCK) i on i.ItemID = wti.ItemID
WHERE wti.WebsiteTabID = {0}
ORDER BY wti.Sort
"
                , Database.HandleQuote(stagingwebsitetabid)
            );

            objRead = Database.GetDataReader(strSQL, strConnectionString);

            int counter = 0;
            while (objRead.Read())
            {
                counter++;

                string strItemNumber = Convert.ToString(objRead["ItemNumber"]);
                bool blnInactive = Convert.ToBoolean(objRead["Inactive"]);

                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ItemNumber.SearchString = strItemNumber;
                ItemFilter.InActive = false;
                Item = ImageSolutions.Item.Item.GetItem(ItemFilter);

                if (Item == null)
                {
                    throw new Exception("Invalid Item");
                }

                ImageSolutions.Website.WebsiteTabItem WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                WebsiteTabItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.ItemID.SearchString = Item.ItemID;
                WebsiteTabItemFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteTabItemFilter.WebsiteTabID.SearchString = websitetabid;
                WebsiteTabItem = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItem(WebsiteTabItemFilter);

                if (WebsiteTabItem == null)
                {
                    WebsiteTabItem = new ImageSolutions.Website.WebsiteTabItem();
                    WebsiteTabItem.WebsiteTabID = websitetabid;
                    WebsiteTabItem.ItemID = Item.ItemID;
                    WebsiteTabItem.Sort = counter;
                    WebsiteTabItem.Inactive = blnInactive;
                    WebsiteTabItem.CreatedBy = createdby;
                    WebsiteTabItem.Create(conn, trans);
                }
            }

            return true;
        }
    }
}
