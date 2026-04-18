using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScheduledTask.Task
{
    public class UpdateItemDetail
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;
            string strWebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);
            string strCreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
            int RowCount = 0;
            try
            {
                strSQL = string.Format(@"
SELECT i.ItemID, ParentCode, ProductAttributeName, ProductAttributeValues, t.Sort
FROM Temp_Product_Description (NOLOCK) t
inner join item (NOLOCK) i on i.ItemNumber = RTRIM(LTRIM(t.ParentCode))
ORDER BY i.ItemID, CAST(ISNULL(t.Sort,'0') as int)
");
                objRead = Database.GetDataReader(strSQL);
                string PreviousParentCode = string.Empty;
                ImageSolutions.Item.ItemDetail ItemDetail = null;
                int counter = 0;

                while (objRead.Read())
                {
                    try
                    {
                        string ItemID = Convert.ToString(objRead["ItemID"]);
                        string ParentCode = Convert.ToString(objRead["ParentCode"]);
                        string ProductAttributeName = Convert.ToString(objRead["ProductAttributeName"]);
                        string ProductAttributeValues = Convert.ToString(objRead["ProductAttributeValues"]);

                        if (PreviousParentCode != ParentCode)
                        {
                            counter = 0;
                            ItemDetail = new ImageSolutions.Item.ItemDetail();
                            ItemDetail.ItemID = ItemID;
                            ItemDetail.Attribute = ProductAttributeName;
                            ItemDetail.Sort = 1;
                            ItemDetail.CreatedBy = strCreatedBy;
                            ItemDetail.Create();

                            PreviousParentCode = ParentCode;
                        }

                        counter++;
                        ImageSolutions.Item.ItemDetailValue ItemDetailValue = new ImageSolutions.Item.ItemDetailValue();
                        ItemDetailValue.ItemDetailID = ItemDetail.ItemDetailID;
                        ItemDetailValue.Value = ProductAttributeValues;
                        ItemDetailValue.Sort = counter;
                        ItemDetailValue.CreatedBy = strCreatedBy;
                        ItemDetailValue.Create();

                    }
                    catch (Exception ex)
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
