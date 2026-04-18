using NetSuiteLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScheduledTask.Task.DiscountTire
{
    public class CreateWebsiteTabs : NetSuiteBase
    {
        public bool Execute()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                CreateWebsiteGroupTab(string.Empty, string.Empty, objConn, objTran);

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }

            return true;
        }

        public void CreateWebsiteGroupTab(string parentid, string parentname, SqlConnection objconn, SqlTransaction objtran)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;
            string strParentID = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT a.Name as FullName
	, Name.Name
    , a.Parent
	, a.AudienceGroups
	, a.SortOrder
FROM Temp_Discount_Tire_Site_Tab (NOLOCK) a
Outer Apply
(
	SELECT CASE WHEN CHARINDEX(':', a.Name) > 0 THEN SUBSTRING(a.Name, LEN(a.Name) - CHARINDEX(':',REVERSE(a.Name)) + 3, LEN(a.Name)) ELSE a.Name END as Name
) Name
WHERE ISNULL(a.Parent,'') = ISNULL({0},'')
ORDER BY a.SortOrder
"
                    , Database.HandleQuote(parentname)
                );

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                while (objRead.Read())
                {
                    string strFullName = Convert.ToString(objRead["FullName"]);
                    string strName = Convert.ToString(objRead["Name"]);
                    string strParent = Convert.ToString(objRead["Parent"]);
                    string strSortOrder = Convert.ToString(objRead["SortOrder"]);
                    string strAudienceGroups = Convert.ToString(objRead["AudienceGroups"]);

                    Console.WriteLine(string.Format(@"{0}", strFullName));

                    ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab();
                    if (string.IsNullOrEmpty(parentid))
                    {
                        strParentID = null;
                    }
                    else
                    {
                        WebsiteTab.ParentID = parentid;
                    }

                    WebsiteTab.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    WebsiteTab.TabName = strName;
                    WebsiteTab.AllowAllGroups = string.IsNullOrEmpty(strAudienceGroups.Trim());
                    WebsiteTab.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                    WebsiteTab.Create(objconn, objtran);

                    if (!string.IsNullOrEmpty(strAudienceGroups))
                    {
                        string[] objGroups = strAudienceGroups.Split('|');

                        foreach (string _group in objGroups)
                        {
                            ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                            ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                            WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.GroupName.SearchString = _group;
                            WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                            if (WebsiteGroup == null)
                            {
                                WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                                WebsiteGroup.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                WebsiteGroup.GroupName = _group;
                                WebsiteGroup.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                WebsiteGroup.Create(objconn, objtran);
                            }

                            ImageSolutions.Website.WebsiteGroupTab WebsiteGroupTab = new ImageSolutions.Website.WebsiteGroupTab();
                            WebsiteGroupTab.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            WebsiteGroupTab.WebsiteTabID = WebsiteTab.WebsiteTabID;
                            WebsiteGroupTab.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            WebsiteGroupTab.Create(objconn, objtran);
                        }
                    }

                    CreateWebsiteGroupTab(WebsiteTab.WebsiteTabID, strFullName, objconn, objtran);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
        }
    }
}
