using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class InventoryReport : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindInventoryReport();
            }
        }
        protected void BindInventoryReport()
        {
            //List<ImageSolutions.Item.ItemWebsite> ItemWebsites = null;
            //ImageSolutions.Item.ItemWebsiteFilter ItemWebsitesFilter = null;
            //int intTotalRecord = 0;

            //try
            //{
            //    ItemWebsitesFilter = new ImageSolutions.Item.ItemWebsiteFilter();
            //    ItemWebsitesFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
            //    ItemWebsitesFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;

            //    ItemWebsitesFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
            //    ItemWebsitesFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
            //    ItemWebsitesFilter.InActive = false;
            //    ItemWebsitesFilter.IsOnline = true;

            //    ItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(ItemWebsitesFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

            //    gvInventoryReport.DataSource = ItemWebsites;
            //    gvInventoryReport.DataBind();
            //    ucPager.TotalRecord = intTotalRecord;
            //}
            //catch (Exception ex)
            //{
            //    WebUtility.DisplayJavascriptMessage(this, ex.Message);
            //}
            //finally
            //{
            //    ItemWebsites = null;
            //    ItemWebsitesFilter = null;
            //}


            int intTotalRecord = 0;
            DataSet DataSet = new DataSet();
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"

SELECT * FROM
(
    SELECT *, COUNT(*) OVER () AS TotalRecord FROM
    (
		SELECT *, ROW_NUMBER() OVER (ORDER BY ItemNumber, ISNULL(ColorSort,0), ISNULL(Color,''), ISNULL(SizeSort,0)) AS RowNumber FROM
		(

SELECT i.ItemName as VendorCode
	, p.ItemNumber
	, ISNULL(i.StoreDisplayName, i.ItemName) as Name
	, ISNULL(Color.Value,'') as Color
	, ISNULL(Color.Sort,0) as ColorSort
	, ISNULL(Size.Value,'') as Size
	, ISNULL(Size.Sort,0) as SizeSort
	, CASE WHEN i.ItemType = '_nonInventoryItem' THEN CAST(ISNULL(i.VendorInventory,0) as INT) ELSE CAST(iSNULL(i.QuantityAvailable,0) as INT) END as AvailableInventory
FROM Item (NOLOCK) i
Inner Join Item (NOLOCK) p on p.ItemID = i.ParentID
Inner Join ItemWebsite (NOLOCK) iw on iw.ItemID = i.ItemID
Outer Apply
(
	SELECT av2.Value, av2.Sort
	FROM ItemAttributeValue (NOLOCK) iav2
	Inner Join AttributeValue (NOLOCK) av2 on av2.AttributeValueID = iav2.AttributeValueID
	Inner JOIn Attribute (NOLOCK) a2 on a2.AttributeID = av2.AttributeID
	WHERE a2.AttributeName = 'Color'
	and iav2.ItemID = i.ItemID
) Color
Outer Apply
(
	SELECT av2.Value, av2.Sort
	FROM ItemAttributeValue (NOLOCK) iav2
	Inner Join AttributeValue (NOLOCK) av2 on av2.AttributeValueID = iav2.AttributeValueID
	Inner JOIn Attribute (NOLOCK) a2 on a2.AttributeID = av2.AttributeID
	WHERE a2.AttributeName = 'Size'
	and iav2.ItemID = i.ItemID
) Size
WHERE i.InActive = 0
and i.IsOnline = 1
and iw.WebsiteID = {0} 


        ) t1
    ) t2
) t3
WHERE RowNumber >= {1} AND RowNumber <= {2}
ORDER BY RowNumber ASC

"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , (ucPager.CurrentPageNumber - 1) * ucPager.PageSize + 1
                        , ucPager.CurrentPageNumber * ucPager.PageSize);

                DataSet = Database.GetDataSet(strSQL);

                if (DataSet != null && DataSet.Tables[0].Rows.Count > 0)
                {
                    intTotalRecord = Convert.ToInt32(DataSet.Tables[0].Rows[0]["TotalRecord"]);
                }

                gvInventoryReport.DataSource = DataSet;
                gvInventoryReport.DataBind();

                ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (DataSet != null) DataSet.Dispose();
                DataSet = null;
            }
        }
        
        protected void btnExport_Click(object sender, EventArgs e)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            StringBuilder objCSV = null;

            try
            {
                string strPath = Server.MapPath("\\Export\\InventoryReport\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                string strFileExportPath = Server.MapPath(string.Format("\\Export\\InventoryReport\\{0}\\inventory_export{1}.csv", DateTime.UtcNow.ToString("yyyyMM"), DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")));

                objCSV = new StringBuilder();
                objCSV.Append("VendorCode,Name,Color,Size,Available");
                objCSV.AppendLine();

                strSQL = string.Format(@"

SELECT i.ItemName as VendorCode
	, p.ItemNumber
	, ISNULL(i.StoreDisplayName, i.ItemName) as Name
	, ISNULL(Color.Value,'') as Color
	, ISNULL(Color.Sort,0) as ColorSort
	, ISNULL(Size.Value,'') as Size
	, ISNULL(Size.Sort,0) as SizeSort
	, CASE WHEN i.ItemType = '_nonInventoryItem' THEN CAST(ISNULL(i.VendorInventory,0) as INT) ELSE CAST(iSNULL(i.QuantityAvailable,0) as INT) END as AvailableInventory
FROM Item (NOLOCK) i
Inner Join Item (NOLOCK) p on p.ItemID = i.ParentID
Inner Join ItemWebsite (NOLOCK) iw on iw.ItemID = i.ItemID
Outer Apply
(
	SELECT av2.Value, av2.Sort
	FROM ItemAttributeValue (NOLOCK) iav2
	Inner Join AttributeValue (NOLOCK) av2 on av2.AttributeValueID = iav2.AttributeValueID
	Inner JOIn Attribute (NOLOCK) a2 on a2.AttributeID = av2.AttributeID
	WHERE a2.AttributeName = 'Color'
	and iav2.ItemID = i.ItemID
) Color
Outer Apply
(
	SELECT av2.Value, av2.Sort
	FROM ItemAttributeValue (NOLOCK) iav2
	Inner Join AttributeValue (NOLOCK) av2 on av2.AttributeValueID = iav2.AttributeValueID
	Inner JOIn Attribute (NOLOCK) a2 on a2.AttributeID = av2.AttributeID
	WHERE a2.AttributeName = 'Size'
	and iav2.ItemID = i.ItemID
) Size
WHERE i.InActive = 0
and i.IsOnline = 1
and iw.WebsiteID = {0} 
ORDER BY p.ItemNumber, ISNULL(Color.Sort,0), ISNULL(Color.Value,''), ISNULL(Size.Sort,0)
"
                        , Database.HandleQuote(CurrentWebsite.WebsiteID) );

                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    objCSV.Append(Convert.ToString(objRead["VendorCode"]).Replace(",", "")).Append(",");
                    //objCSV.Append(Convert.ToString(objRead["ItemNumber"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["Name"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["Color"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["Size"]).Replace(",", "")).Append(",");
                    objCSV.Append(Convert.ToString(objRead["AvailableInventory"])).Append(",");
                    objCSV.AppendLine();
                }

                if (objCSV != null)
                {
                    using (StreamWriter objWriter = new StreamWriter(strFileExportPath))
                    {
                        objWriter.Write(objCSV.ToString());
                    }
                }

                Response.ContentType = "text/csv";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + System.IO.Path.GetFileName(strFileExportPath));
                Response.WriteFile(strFileExportPath);

                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
                objCSV = null;
            }
        }
    }
}