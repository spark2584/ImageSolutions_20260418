using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemDetailImport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuUser.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\ItemDetail\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    string strFileNamExtension = System.IO.Path.GetExtension(fuUser.PostedFile.FileName);
                    string strFileName = System.IO.Path.GetFileNameWithoutExtension(fuUser.PostedFile.FileName) + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + strFileNamExtension;
                    string strFilePath = string.Format("{0}{1}", strPath, strFileName);
                    fuUser.PostedFile.SaveAs(strFilePath);

                    UploadFile(strFilePath);
                }
                else
                {
                    gvItemDetailPreview.DataSource = null;
                    gvItemDetailPreview.DataBind();

                    btnSubmit.Visible = false;
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        protected void UploadFile(string filepath)
        {
            string strFullPath = filepath;
            string strExtension = Path.GetExtension(strFullPath);
            string strConnectionString = String.Empty;

            switch (strExtension)
            {
                case ".xls": //Excel 97-03
                    strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;""";
                    UploadExcel(filepath, strConnectionString);
                    break;
                case ".xlsx": //Excel 07
                    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                    UploadExcel(filepath, strConnectionString);
                    break;
                case ".csv": //CSV
                    UploadCSV(filepath);
                    break;
                default:
                    gvItemDetailPreview.DataSource = null;
                    gvItemDetailPreview.DataBind();

                    btnSubmit.Visible = false;
                    break;
            }
        }
        protected void SubmitFile(string filepath)
        {
            string strFullPath = filepath;
            string strExtension = Path.GetExtension(strFullPath);
            string strConnectionString = String.Empty;

            try
            {
                switch (strExtension)
                {
                    case ".xls": //Excel 97-03
                        strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;""";
                        ProcessExcel(strConnectionString);
                        break;
                    case ".xlsx": //Excel 07
                        strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                        ProcessExcel(strConnectionString);
                        break;
                    case ".csv":
                        ProcessCSV(Convert.ToString(hfFilePath.Value));
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public void UploadCSV(string filepath)
        {
            List<ItemDetailPreview> ItemDetailPreviews = GetCSV(filepath);
            if (ItemDetailPreviews != null && ItemDetailPreviews.Count > 0)
            {
                gvItemDetailPreview.DataSource = ItemDetailPreviews;
                gvItemDetailPreview.DataBind();

                hfFilePath.Value = Convert.ToString(filepath);
                btnSubmit.Visible = true;
            }
            else
            {
                btnSubmit.Visible = false;
                throw new Exception("There is no data to import");
            }
        }
        public void ProcessCSV(string filepath)
        {

            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    using (StreamReader _StreamReader = new StreamReader(filepath))
                    {
                        int counter = 0;
                        String[] HeaderColumnValues = null;
                        List<string> HeaderFields = new List<string>();
                        string strCurrentLine = string.Empty;


                        string strCurrentItemID = string.Empty;
                        string strCurrentAttributeHeader = string.Empty;

                        while (!string.IsNullOrEmpty(strCurrentLine = _StreamReader.ReadLine()))
                        {
                            counter++;
                            if (counter == 1)
                            {
                                HeaderColumnValues = strCurrentLine.Split(',');
                                foreach (string _value in HeaderColumnValues)
                                {
                                    HeaderFields.Add(_value);
                                }

                                if (!HeaderFields.Contains("ItemNumber")) throw new Exception("[ItemNumber] is missing from the report, excel contact administrator");
                                if (!HeaderFields.Contains("AttributeHeader")) throw new Exception("[AttrubuteHeader] is missing from the report, excel contact administrator");
                                if (!HeaderFields.Contains("HeaderSort")) throw new Exception("[HeaderSort] is missing from the report, excel contact administrator");
                                if (!HeaderFields.Contains("AttributeLine")) throw new Exception("[AttributeLine] is missing from the report, excel contact administrator");
                                if (!HeaderFields.Contains("LineSort")) throw new Exception("[LineSort] is missing from the report, excel contact administrator");
                            }
                            else
                            {
                                String[] ColumnValues = null;
                                ColumnValues = strCurrentLine.Split(',');

                                string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ID")]);
                                string strItemNumber = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ItemNumber")]);
                                string strAttributeHeader = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeHeader")]);
                                string strHeaderSort = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HeaderSort")]);
                                string strAttributeLine = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeLine")]);
                                string strLineSort = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LineSort")]);

                                if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid ID: {0}", strID));
                                }

                                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                                ItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                                ItemFilter.ItemNumber.SearchString = strItemNumber;
                                Item = ImageSolutions.Item.Item.GetItem(ItemFilter);

                                if (Item == null || !Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                                {
                                    throw new Exception(string.Format("Invalid Item Number: {0}", strItemNumber));
                                }

                                if (string.IsNullOrEmpty(strHeaderSort))
                                {
                                    throw new Exception(string.Format("Header Sort is required"));
                                }
                                if (!string.IsNullOrEmpty(strHeaderSort) && !Regex.IsMatch(strHeaderSort, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid Header Sort: {0}", strHeaderSort));
                                }

                                ImageSolutions.Item.ItemDetail ItemDetail = new ImageSolutions.Item.ItemDetail();
                                ImageSolutions.Item.ItemDetailFilter ItemDetailFilter = new ImageSolutions.Item.ItemDetailFilter();
                                ItemDetailFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                                ItemDetailFilter.ItemID.SearchString = Item.ItemID;
                                ItemDetailFilter.Attribute = new Database.Filter.StringSearch.SearchFilter();
                                ItemDetailFilter.Attribute.SearchString = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeHeader")]);
                                ItemDetail = ImageSolutions.Item.ItemDetail.GetItemDetail(ItemDetailFilter);

                                if (ItemDetail == null)
                                {
                                    ItemDetail = new ImageSolutions.Item.ItemDetail();
                                    ItemDetail.ItemID = Item.ItemID;
                                    ItemDetail.Attribute = strAttributeHeader;
                                    ItemDetail.Sort = Convert.ToInt32(strHeaderSort);
                                    ItemDetail.CreatedBy = CurrentUser.UserInfoID;
                                    ItemDetail.Create(objConn, objTran);
                                }
                                else
                                {
                                    ItemDetail.Sort = Convert.ToInt32(strHeaderSort);
                                    ItemDetail.Update(objConn, objTran);
                                }

                                if (string.IsNullOrEmpty(strLineSort))
                                {
                                    throw new Exception(string.Format("Line Sort is required"));
                                }
                                if (!string.IsNullOrEmpty(strLineSort) && !Regex.IsMatch(strLineSort, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid Line Sort: {0}", strLineSort));
                                }

                                ImageSolutions.Item.ItemDetailValue ItemDetailValue = null;
                                if (!string.IsNullOrEmpty(strID))
                                {
                                    ItemDetailValue = new ImageSolutions.Item.ItemDetailValue(strID);

                                    if (ItemDetailValue == null)
                                    {
                                        throw new Exception(string.Format("Invalid ID: {0}", strID));
                                    }

                                    ItemDetailValue.Value = strAttributeLine;
                                    ItemDetailValue.Sort = Convert.ToInt32(strLineSort);
                                    ItemDetailValue.Update(objConn, objTran);
                                }
                                else
                                {
                                    ItemDetailValue = new ImageSolutions.Item.ItemDetailValue();
                                    ItemDetailValue.ItemDetailID = ItemDetail.ItemDetailID;
                                    ItemDetailValue.Value = strAttributeLine;
                                    ItemDetailValue.Sort = Convert.ToInt32(strLineSort);
                                    ItemDetailValue.CreatedBy = CurrentUser.UserInfoID;
                                    ItemDetailValue.Create(objConn, objTran);
                                }
                            }
                        }
                    }


                    //List<ImageSolutions.Item.ItemDetail> ItemDetails = GetCSV(Convert.ToString(hfFilePath.Value));
                    //foreach (ImageSolutions.Item.ItemDetail _ItemDetail in ItemDetails)
                    //{
                    //    if (_ItemDetail.IsNew)
                    //    {
                    //        _ItemDetail.Create(objConn, objTran);
                    //    }
                    //    else
                    //    {
                    //        _ItemDetail.Update(objConn, objTran);
                    //    }
                    //}

                    objTran.Commit();
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
                    throw ex;
                }
                finally
                {
                    if (objTran != null) objTran.Dispose();
                    objTran = null;
                    if (objConn != null) objConn.Dispose();
                    objConn = null;
                }
            }
            else
            {
                throw new Exception("File Missing");
            }
        }
        public List<ItemDetailPreview> GetCSV(string filepath)
        {
            List<ItemDetailPreview> ItemDetailPreviews = null;

            try
            {
                using (StreamReader _StreamReader = new StreamReader(filepath))
                {
                    int counter = 0;
                    String[] HeaderColumnValues = null;
                    List<string> HeaderFields = new List<string>();
                    string strCurrentLine = string.Empty;


                    string strCurrentItemID = string.Empty;
                    string strCurrentAttributeHeader = string.Empty;

                    while (!string.IsNullOrEmpty(strCurrentLine = _StreamReader.ReadLine()))
                    {
                        counter++;
                        if (counter == 1)
                        {
                            HeaderColumnValues = strCurrentLine.Split(',');
                            foreach (string _value in HeaderColumnValues)
                            {
                                HeaderFields.Add(_value);
                            }

                            if (!HeaderFields.Contains("ItemNumber")) throw new Exception("[ItemNumber] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("AttributeHeader")) throw new Exception("[AttrubuteHeader] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("HeaderSort")) throw new Exception("[HeaderSort] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("AttributeLine")) throw new Exception("[AttributeLine] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("LineSort")) throw new Exception("[LineSort] is missing from the report, excel contact administrator");

                            ItemDetailPreviews = new List<ItemDetailPreview>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ID")]);
                            string strItemNumber = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ItemNumber")]);
                            string strAttributeHeader = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeHeader")]);
                            string strHeaderSort = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HeaderSort")]);
                            string strAttributeLine = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeLine")]);
                            string strLineSort = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LineSort")]);



                            if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"^\d+$"))
                            {
                                throw new Exception(string.Format("Invalid ID: {0}", strID));
                            }

                            ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                            ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                            ItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                            ItemFilter.ItemNumber.SearchString = strItemNumber;
                            Item = ImageSolutions.Item.Item.GetItem(ItemFilter);

                            if (Item == null || !Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                            {
                                throw new Exception(string.Format("Invalid Item Number: {0}", strItemNumber));
                            }

                            if(string.IsNullOrEmpty(strHeaderSort))
                            {
                                throw new Exception(string.Format("Header Sort is required"));
                            }
                            if (!string.IsNullOrEmpty(strHeaderSort) && !Regex.IsMatch(strHeaderSort, @"^\d+$"))
                            {
                                throw new Exception(string.Format("Invalid Header Sort: {0}", strHeaderSort));
                            }

                            ImageSolutions.Item.ItemDetail ItemDetail = new ImageSolutions.Item.ItemDetail();
                            ImageSolutions.Item.ItemDetailFilter ItemDetailFilter = new ImageSolutions.Item.ItemDetailFilter();
                            ItemDetailFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                            ItemDetailFilter.ItemID.SearchString = Item.ItemID;
                            ItemDetailFilter.Attribute = new Database.Filter.StringSearch.SearchFilter();
                            ItemDetailFilter.Attribute.SearchString = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AttributeHeader")]);
                            ItemDetail = ImageSolutions.Item.ItemDetail.GetItemDetail(ItemDetailFilter);

                            //if (ItemDetail == null)
                            //{
                            //    ItemDetail = new ImageSolutions.Item.ItemDetail();
                            //}
                            //ItemDetail.ItemID = Item.ItemID;
                            //ItemDetail.Attribute = strAttributeHeader;
                            //ItemDetail.Sort = Convert.ToInt32(strHeaderSort);
                            //ItemDetail.CreatedBy = CurrentUser.UserInfoID;
                            //ItemDetail.Create();

                            if (string.IsNullOrEmpty(strLineSort))
                            {
                                throw new Exception(string.Format("Line Sort is required"));
                            }
                            if (!string.IsNullOrEmpty(strLineSort) && !Regex.IsMatch(strLineSort, @"^\d+$"))
                            {
                                throw new Exception(string.Format("Invalid Line Sort: {0}", strLineSort));
                            }

                            //ImageSolutions.Item.ItemDetailValue ItemDetailValue = null;
                            if (!string.IsNullOrEmpty(strID))
                            {
                                ImageSolutions.Item.ItemDetailValue ItemDetailValue = new ImageSolutions.Item.ItemDetailValue(strID);

                                if (ItemDetailValue == null)
                                {
                                    throw new Exception(string.Format("Invalid ID: {0}", strID));
                                }

                                if(ItemDetailValue.ItemDetailID != ItemDetail.ItemDetailID)
                                {
                                    throw new Exception(string.Format("Invalid ID: {0} - Header does not match", strID));
                                }
                            }
                            //else
                            //{
                            //    ItemDetailValue = new ImageSolutions.Item.ItemDetailValue();
                            //}

                            //ItemDetailValue.Value = strAttrubuteLine;
                            //ItemDetailValue.Sort = Convert.ToInt32(strLineSort);

                            //ItemDetail.ItemDetailValues = new List<ImageSolutions.Item.ItemDetailValue>();
                            //ItemDetail.ItemDetailValues.Add(ItemDetailValue);

                            ItemDetailPreview ItemDetailPreview = new ItemDetailPreview();
                            ItemDetailPreview.ID = strID;
                            ItemDetailPreview.ItemNumber = strItemNumber;
                            ItemDetailPreview.AttributeHeader = strAttributeHeader;
                            ItemDetailPreview.HeaderSort = strHeaderSort;
                            ItemDetailPreview.AttributeLine = strAttributeLine;
                            ItemDetailPreview.LineSort = strLineSort;
                            ItemDetailPreviews.Add(ItemDetailPreview);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ItemDetailPreviews;
        }
        public void UploadExcel(string filepath, string connectionstring)
        {
            //OleDbConnection objOleDbConn = null;
            DataTable objSchema = null;
            OleDbDataAdapter objAdapter = null;
            DataSet objData = null;
            DataColumnCollection objColumns = null;
            Hashtable dicParam = new Hashtable();
            string strSheetName = string.Empty;
            string strFullPath = string.Empty;
            string strExtension = string.Empty;

            try
            {
                using (OleDbConnection objOleDbConn = new OleDbConnection(connectionstring))
                {
                    //objOleDbConn = new OleDbConnection(connectionstring);
                    objOleDbConn.ConnectionString = connectionstring;
                    objOleDbConn.Open();
                    objSchema = objOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (objSchema == null || objSchema.Rows.Count < 1)
                    {
                        throw new Exception("Error: Could not determine the name of the first worksheet.");
                    }
                    else
                    {
                        for (int i = 0; i < objSchema.Rows.Count; i++)
                        {
                            strSheetName = objSchema.Rows[i]["TABLE_NAME"].ToString() + "A1:P65535";
                        }
                    }
                    if (string.IsNullOrEmpty(strSheetName))
                        throw new Exception("Missing Sheet Name");
                }

                using (OleDbConnection objConn = new OleDbConnection(connectionstring))
                {
                    objConn.ConnectionString = connectionstring;
                    objConn.Open();

                    using (OleDbCommand objComm = objConn.CreateCommand())
                    {
                        objComm.CommandText = @"SELECT * FROM [" + strSheetName + "]";

                        objAdapter = new OleDbDataAdapter(objComm);
                        objAdapter.TableMappings.Add("Table", "Query");
                        objData = new DataSet();
                        objAdapter.Fill(objData);

                        List<ItemDetailPreview> ItemDetailPreviews = new List<ItemDetailPreview>();

                        if (objData != null && objData.Tables[0].Rows.Count > 0)
                        {
                            if (objData != null && objData.Tables[0].Rows.Count > 0)
                            {
                                objColumns = objData.Tables[0].Rows[0].Table.Columns;

                                for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                                {
                                    Console.WriteLine(i);

                                    string strID = Convert.ToString(objData.Tables[0].Rows[i]["ID"]).Trim();
                                    string strItemNumber = Convert.ToString(objData.Tables[0].Rows[i]["Item_Number"]).Trim();
                                    string strAttributeHeader = Convert.ToString(objData.Tables[0].Rows[i]["Attribute_Header"]).Trim();
                                    string strHeaderSort = Convert.ToString(objData.Tables[0].Rows[i]["Header_Sort"]).Trim();
                                    string strAttributeLine = Convert.ToString(objData.Tables[0].Rows[i]["Attribute_Line"]).Trim();
                                    string strLineSort = Convert.ToString(objData.Tables[0].Rows[i]["Line_Sort"]).Trim();

                                    

                                    if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"^\d+$"))
                                    {
                                        throw new Exception(string.Format("Invalid ID: {0}", strID));
                                    }

                                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                                    ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                                    ItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                                    ItemFilter.ItemNumber.SearchString = strItemNumber;
                                    Item = ImageSolutions.Item.Item.GetItem(ItemFilter);

                                    if (Item == null || !Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                                    {
                                        throw new Exception(string.Format("Invalid Item Number: {0}", strItemNumber));
                                    }

                                    if (string.IsNullOrEmpty(strHeaderSort))
                                    {
                                        throw new Exception(string.Format("Header Sort is required"));
                                    }
                                    if (!string.IsNullOrEmpty(strHeaderSort) && !Regex.IsMatch(strHeaderSort, @"^\d+$"))
                                    {
                                        throw new Exception(string.Format("Invalid Header Sort: {0}", strHeaderSort));
                                    }

                                    if (string.IsNullOrEmpty(strLineSort))
                                    {
                                        throw new Exception(string.Format("Line Sort is required"));
                                    }
                                    if (!string.IsNullOrEmpty(strLineSort) && !Regex.IsMatch(strLineSort, @"^\d+$"))
                                    {
                                        throw new Exception(string.Format("Invalid Line Sort: {0}", strLineSort));
                                    }

                                    ImageSolutions.Item.ItemDetailValue ItemDetailValue = null;
                                    if (!string.IsNullOrEmpty(strID))
                                    {
                                        ItemDetailValue = new ImageSolutions.Item.ItemDetailValue(strID);

                                        if (ItemDetailValue == null)
                                        {
                                            throw new Exception(string.Format("Invalid ID: {0}", strID));
                                        }

                                        ItemDetailPreview ItemDetailPreview = new ItemDetailPreview();

                                        ItemDetailPreview.ID = strID;
                                        ItemDetailPreview.ItemNumber = strItemNumber;
                                        ItemDetailPreview.AttributeHeader = strAttributeHeader;
                                        ItemDetailPreview.HeaderSort = strHeaderSort;
                                        ItemDetailPreview.AttributeLine = strAttributeLine;
                                        ItemDetailPreview.LineSort = Convert.ToString(strLineSort);

                                        ItemDetailPreviews.Add(ItemDetailPreview);
                                    }
                                    else
                                    {
                                        string[] AttributeLines = strAttributeLine.Split(
                                            new string[] { "\r\n", "\r", "\n" },
                                            StringSplitOptions.None
                                        );

                                        int counter = 0;
                                        foreach (string _AttributeLine in AttributeLines)
                                        {
                                            ItemDetailPreview ItemDetailPreview = new ItemDetailPreview();

                                            ItemDetailPreview.ID = strID;
                                            ItemDetailPreview.ItemNumber = strItemNumber;
                                            ItemDetailPreview.AttributeHeader = strAttributeHeader;
                                            ItemDetailPreview.HeaderSort = strHeaderSort;
                                            ItemDetailPreview.AttributeLine = _AttributeLine;
                                            ItemDetailPreview.LineSort = Convert.ToString(Convert.ToInt32(strLineSort) + counter);

                                            ItemDetailPreviews.Add(ItemDetailPreview);
                                            counter++;
                                        }
                                    }                                  
                                }
                            }
                            else
                            {
                                throw new Exception("There is no data to upload, please make sure to name the import spreadsheet 'product_detail'");
                            }



                            gvItemDetailPreview.DataSource = ItemDetailPreviews; //objData;
                            gvItemDetailPreview.DataBind();

                            hfFilePath.Value = Convert.ToString(filepath);
                            btnSubmit.Visible = true;
                        }
                        else
                        {
                            btnSubmit.Visible = false;
                            throw new Exception("There is no data to import");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
        public void ProcessExcel(string connectionstring)
        {
            //OleDbConnection objOleDbConn = null;
            DataTable objSchema = null;
            OleDbDataAdapter objAdapter = null;
            DataSet objData = null;
            DataColumnCollection objColumns = null;
            Hashtable dicParam = new Hashtable();
            string strSheetName = string.Empty;
            string strFullPath = string.Empty;
            string strExtension = string.Empty;

            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                using (OleDbConnection objOleDbConn = new OleDbConnection(connectionstring))
                {
                    //objOleDbConn = new OleDbConnection(connectionstring);
                    objOleDbConn.ConnectionString = connectionstring;
                    objOleDbConn.Open();
                    objSchema = objOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (objSchema == null || objSchema.Rows.Count < 1)
                    {
                        throw new Exception("Error: Could not determine the name of the first worksheet.");
                    }
                    else
                    {
                        for (int i = 0; i < objSchema.Rows.Count; i++)
                        {
                            strSheetName = objSchema.Rows[i]["TABLE_NAME"].ToString() + "A1:P65535";
                        }
                    }
                    if (string.IsNullOrEmpty(strSheetName))
                        throw new Exception("Missing Sheet Name");
                }

                //List<ImageSolutions.Budget.Budget> Budgets = new List<ImageSolutions.Budget.Budget>();

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                using (OleDbConnection objOleDbConn = new OleDbConnection(connectionstring))
                {
                    objOleDbConn.ConnectionString = connectionstring;
                    objOleDbConn.Open();

                    using (OleDbCommand objComm = objOleDbConn.CreateCommand())
                    {
                        objComm.CommandText = @"SELECT * FROM [" + strSheetName + "]";

                        objAdapter = new OleDbDataAdapter(objComm);
                        objAdapter.TableMappings.Add("Table", "Query");
                        objData = new DataSet();
                        objAdapter.Fill(objData);

                        if (objData != null && objData.Tables[0].Rows.Count > 0)
                        {
                            objColumns = objData.Tables[0].Rows[0].Table.Columns;

                            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                            {
                                Console.WriteLine(i);

                                string strID = Convert.ToString(objData.Tables[0].Rows[i]["ID"]).Trim();
                                string strItemNumber = Convert.ToString(objData.Tables[0].Rows[i]["Item_Number"]).Trim();
                                string strAttributeHeader = Convert.ToString(objData.Tables[0].Rows[i]["Attribute_Header"]).Trim();
                                string strHeaderSort = Convert.ToString(objData.Tables[0].Rows[i]["Header_Sort"]).Trim();
                                string strAttributeLine = Convert.ToString(objData.Tables[0].Rows[i]["Attribute_Line"]).Trim();
                                string strLineSort = Convert.ToString(objData.Tables[0].Rows[i]["Line_Sort"]).Trim();

                                if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid ID: {0}", strID));
                                }

                                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                                ItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                                ItemFilter.ItemNumber.SearchString = strItemNumber;
                                Item = ImageSolutions.Item.Item.GetItem(ItemFilter);

                                if (Item == null || !Item.ItemWebsites.Exists(x => x.WebsiteID == CurrentWebsite.WebsiteID))
                                {
                                    throw new Exception(string.Format("Invalid Item Number: {0}", strItemNumber));
                                }

                                if (string.IsNullOrEmpty(strHeaderSort))
                                {
                                    throw new Exception(string.Format("Header Sort is required"));
                                }
                                if (!string.IsNullOrEmpty(strHeaderSort) && !Regex.IsMatch(strHeaderSort, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid Header Sort: {0}", strHeaderSort));
                                }



                                ImageSolutions.Item.ItemDetail ItemDetail = new ImageSolutions.Item.ItemDetail();

                                if (!string.IsNullOrEmpty(strID))
                                {
                                    ImageSolutions.Item.ItemDetailValue ExistItemDetailValue = new ImageSolutions.Item.ItemDetailValue(strID);

                                    ItemDetail = ExistItemDetailValue.ItemDetail;
                                }
                                else
                                {
                                    ImageSolutions.Item.ItemDetailFilter ItemDetailFilter = new ImageSolutions.Item.ItemDetailFilter();
                                    ItemDetailFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                                    ItemDetailFilter.ItemID.SearchString = Item.ItemID;
                                    ItemDetailFilter.Attribute = new Database.Filter.StringSearch.SearchFilter();
                                    ItemDetailFilter.Attribute.SearchString = strAttributeHeader;
                                    ItemDetail = ImageSolutions.Item.ItemDetail.GetItemDetail(ItemDetailFilter);
                                }


                                if (ItemDetail == null)
                                {
                                    ItemDetail = new ImageSolutions.Item.ItemDetail();
                                    ItemDetail.ItemID = Item.ItemID;
                                    ItemDetail.Attribute = strAttributeHeader;
                                    ItemDetail.Sort = Convert.ToInt32(strHeaderSort);
                                    ItemDetail.CreatedBy = CurrentUser.UserInfoID;
                                    ItemDetail.Create(objConn, objTran);
                                }
                                else
                                {
                                    ItemDetail.Attribute = strAttributeHeader;
                                    ItemDetail.Sort = Convert.ToInt32(strHeaderSort);
                                    ItemDetail.Update(objConn, objTran);
                                }

                                if (string.IsNullOrEmpty(strLineSort))
                                {
                                    throw new Exception(string.Format("Line Sort is required"));
                                }
                                if (!string.IsNullOrEmpty(strLineSort) && !Regex.IsMatch(strLineSort, @"^\d+$"))
                                {
                                    throw new Exception(string.Format("Invalid Line Sort: {0}", strLineSort));
                                }

                                
                                ImageSolutions.Item.ItemDetailValue ItemDetailValue = null;
                                if (!string.IsNullOrEmpty(strID))
                                {
                                    ItemDetailValue = new ImageSolutions.Item.ItemDetailValue(strID);

                                    if (ItemDetailValue == null)
                                    {
                                        throw new Exception(string.Format("Invalid ID: {0}", strID));
                                    }

                                    ItemDetailValue.Value = strAttributeLine;
                                    ItemDetailValue.Sort = Convert.ToInt32(strLineSort);
                                    ItemDetailValue.Update(objConn, objTran);
                                }
                                else
                                {
                                    string[] AttributeLines = strAttributeLine.Split(
                                           new string[] { "\r\n", "\r", "\n" },
                                           StringSplitOptions.None
                                       );

                                    int counter = 0;
                                    foreach (string _AttributeLine in AttributeLines)
                                    {
                                        ItemDetailValue = new ImageSolutions.Item.ItemDetailValue();
                                        ItemDetailValue.ItemDetailID = ItemDetail.ItemDetailID;
                                        ItemDetailValue.Value = _AttributeLine;
                                        ItemDetailValue.Sort = Convert.ToInt32(strLineSort) + counter;
                                        ItemDetailValue.CreatedBy = CurrentUser.UserInfoID;
                                        ItemDetailValue.Create(objConn, objTran);

                                        counter++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("There is no data to import, please make sure to name the import spreadsheet 'toimport'");
                        }
                    }
                }

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/ItemOverview.aspx"));
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            }
            else
            {
                WebUtility.DisplayJavascriptMessage(this, "File missing");
            }
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fileName = "ProductDetailTemplate.xlsx";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }
    }

    public class ItemDetailPreview
    {
        public string ID { get; set; }
        public string ItemNumber { get; set; }
        public string AttributeHeader { get; set; }
        public string HeaderSort { get; set; }
        public string AttributeLine { get; set; }
        public string LineSort { get; set; }
    }
}