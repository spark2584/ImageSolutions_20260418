using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class PromotionImport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fileName = "PromotionTemplate.csv";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuPromotion.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\Promotion\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    string strFileNamExtension = System.IO.Path.GetExtension(fuPromotion.PostedFile.FileName);
                    string strFileName = System.IO.Path.GetFileNameWithoutExtension(fuPromotion.PostedFile.FileName) + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + strFileNamExtension;
                    string strFilePath = string.Format("{0}{1}", strPath, strFileName);
                    fuPromotion.PostedFile.SaveAs(strFilePath);

                    UploadFile(strFilePath);
                }
                else
                {
                    gvPromotion.DataSource = null;
                    gvPromotion.DataBind();

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
                    gvPromotion.DataSource = null;
                    gvPromotion.DataBind();

                    btnSubmit.Visible = false;
                    break;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/PromotionOverview.aspx"));
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
                        strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;";
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
            List<ImageSolutions.Promotion.Promotion> Promotions = GetCSV(filepath);
            if (Promotions != null && Promotions.Count > 0)
            {
                gvPromotion.DataSource = Promotions;
                gvPromotion.DataBind();

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

                    List<ImageSolutions.Promotion.Promotion> Promotions = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (ImageSolutions.Promotion.Promotion _Promotion in Promotions)
                    {
                        if (_Promotion.IsNew)
                        {
                            _Promotion.Create(objConn, objTran);
                        }
                        else
                        {
                            _Promotion.Update(objConn, objTran);
                        }
                    }

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
        public List<ImageSolutions.Promotion.Promotion> GetCSV(string filepath)
        {
            List<ImageSolutions.Promotion.Promotion> Promotions = null;

            try
            {
                using (StreamReader _StreamReader = new StreamReader(filepath))
                {
                    int counter = 0;
                    String[] HeaderColumnValues = null;
                    List<string> HeaderFields = new List<string>();
                    string strCurrentLine = string.Empty;

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

                            if (!HeaderFields.Contains("id")) throw new Exception("[id] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("promotion_code")) throw new Exception("[promotion_code] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("name")) throw new Exception("[name] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("from_date")) throw new Exception("[start_date] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("to_date")) throw new Exception("[endd_ate] is missing from the excel, please contact administrator");
                            
                            Promotions = new List<ImageSolutions.Promotion.Promotion>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("id")]).Trim();
                            string strPromotionCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("promotion_code")]);
                            string strName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("name")]);
                            string strDiscountPercent = Convert.ToString(ColumnValues[HeaderFields.IndexOf("discount_percent")]);
                            string strDiscountAmount = Convert.ToString(ColumnValues[HeaderFields.IndexOf("discount_amount")]);
                            string strMinOrderAmount = Convert.ToString(ColumnValues[HeaderFields.IndexOf("min_order_amount")]);
                            string strMaxOrderAmount     = Convert.ToString(ColumnValues[HeaderFields.IndexOf("max_order_amount")]);
                            string strFromDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("from_date")]);
                            string strToDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("to_date")]);
                            string strMaxUsageCount = Convert.ToString(ColumnValues[HeaderFields.IndexOf("max_usage_count")]);
                            string strFreeShippingService = Convert.ToString(ColumnValues[HeaderFields.IndexOf("free_shipping_service")]);
                            string strIsActive = Convert.ToString(ColumnValues[HeaderFields.IndexOf("is_active")]);

                            if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"\d"))
                            {
                                throw new Exception(string.Format("Invalid ID: {0}", strID));
                            }

                            if (!string.IsNullOrEmpty(strDiscountPercent) && !ValidateDecimal(strDiscountPercent))
                            {
                                throw new Exception(string.Format("Invalid Discount Percent: {0}", strDiscountPercent));
                            }

                            if (!string.IsNullOrEmpty(strDiscountPercent) && !string.IsNullOrEmpty(strDiscountAmount))
                            {
                                throw new Exception(string.Format("Invalid Discount Percent: {0} - discount percent and amount cannot be combined", strDiscountPercent));
                            }

                            if (!string.IsNullOrEmpty(strDiscountAmount) && !ValidateDecimal(strDiscountAmount))
                            {
                                throw new Exception(string.Format("Invalid Discount Amount: {0}", strDiscountAmount));
                            }

                            if (!string.IsNullOrEmpty(strMinOrderAmount) && !ValidateDecimal(strMinOrderAmount))
                            {
                                throw new Exception(string.Format("Invalid Min Order Amount: {0}", strMinOrderAmount));
                            }

                            if (!string.IsNullOrEmpty(strMaxOrderAmount) && !ValidateDecimal(strMaxOrderAmount))
                            {
                                throw new Exception(string.Format("Invalid Max Order Amount: {0}", strMaxOrderAmount));
                            }

                            if (!string.IsNullOrEmpty(strFromDate) && !ValidateDate(strFromDate))
                            {
                                throw new Exception(string.Format("Invalid From Date: {0} - Date must be in MM/DD/YYYY format", strFromDate));
                            }

                            if (!string.IsNullOrEmpty(strToDate) && !ValidateDate(strToDate))
                            {
                                throw new Exception(string.Format("Invalid To Date: {0} - Date must be in MM/DD/YYYY format", strToDate));
                            }

                            if (!string.IsNullOrEmpty(strMaxUsageCount) && !Regex.IsMatch(strMaxUsageCount, @"\d"))
                            {
                                throw new Exception(string.Format("Invalid Usage Count: {0}", strMaxUsageCount));
                            }

                            ImageSolutions.Promotion.Promotion Promotion = null;

                            if (!string.IsNullOrEmpty(strID))
                            {
                                Promotion = new ImageSolutions.Promotion.Promotion(CurrentWebsite.WebsiteID, strID);
                                if (Promotion == null)
                                {
                                    throw new Exception(string.Format("Invalid ID: {0}", strID));
                                }
                            }
                            else
                            {
                                Promotion = new ImageSolutions.Promotion.Promotion();
                            }

                            if (!string.IsNullOrEmpty(strFreeShippingService))
                            {
                                ImageSolutions.Shipping.ShippingService ShippingService = new ImageSolutions.Shipping.ShippingService();
                                ImageSolutions.Shipping.ShippingServiceFilter ShippingServiceFilter = new ImageSolutions.Shipping.ShippingServiceFilter();
                                ShippingServiceFilter.Description = new Database.Filter.StringSearch.SearchFilter();
                                ShippingServiceFilter.Description.SearchString = strFreeShippingService;
                                ShippingService = ImageSolutions.Shipping.ShippingService.GetShippingService(ShippingServiceFilter);

                                if (ShippingService != null && !string.IsNullOrEmpty(ShippingService.ShippingServiceID))
                                {
                                    Promotion.FreeShippingServiceID = ShippingService.ShippingServiceID;
                                }
                                else
                                {
                                    throw new Exception(string.Format("Invalid Shipping Service: {0}", strFreeShippingService));
                                }
                            }
                            else
                            {
                                Promotion.FreeShippingServiceID = String.Empty;
                            }

                            if (!string.IsNullOrEmpty(strIsActive) && strIsActive != "yes" && strIsActive != "no")
                            {
                                throw new Exception("Column [is_active] must be 'yes' or 'no'");
                            }

                            Promotion.WebsiteID = CurrentWebsite.WebsiteID;
                            Promotion.PromotionCode = strPromotionCode;
                            Promotion.PromotionName = strName;
                            //if(!string.IsNullOrEmpty(strDiscountPercent))
                            //{
                            //    Promotion.DiscountPercent = Convert.ToDecimal(strDiscountPercent) / 100;
                            //}
                            Promotion.DiscountPercent = !string.IsNullOrEmpty(strDiscountPercent) ? Convert.ToDecimal(strDiscountPercent) / 100 : (decimal?)null;
                            //if (!string.IsNullOrEmpty(strDiscountAmount))
                            //{
                            //    Promotion.DiscountAmount = Convert.ToDecimal(strDiscountAmount);
                            //}
                            Promotion.DiscountAmount = !string.IsNullOrEmpty(strDiscountAmount) ? Convert.ToDecimal(strDiscountAmount) : (decimal?)null;
                            //if (!string.IsNullOrEmpty(strMinOrderAmount))
                            //{
                            //    Promotion.MinOrderAmount = Convert.ToDecimal(strMinOrderAmount);
                            //}
                            Promotion.MinOrderAmount = !string.IsNullOrEmpty(strMinOrderAmount) ? Convert.ToDecimal(strMinOrderAmount) : (decimal?)null;
                            //if (!string.IsNullOrEmpty(strMaxOrderAmount))
                            //{
                            //    Promotion.MaxOrderAmount = Convert.ToDecimal(strMaxOrderAmount);
                            //}
                            Promotion.MaxOrderAmount = !string.IsNullOrEmpty(strMaxOrderAmount) ? Convert.ToDecimal(strMaxOrderAmount) : (decimal?)null;
                            //if (!string.IsNullOrEmpty(strFromDate))
                            //{
                            //    Promotion.FromDate = Convert.ToDateTime(strFromDate);
                            //}
                            Promotion.FromDate = !string.IsNullOrEmpty(strFromDate) ? Convert.ToDateTime(strFromDate) : (DateTime?)null;
                            //if (!string.IsNullOrEmpty(strToDate))
                            //{
                            //    Promotion.ToDate = Convert.ToDateTime(strToDate);
                            //}
                            Promotion.ToDate = !string.IsNullOrEmpty(strToDate) ? Convert.ToDateTime(strToDate) : (DateTime?)null;
                            //if (!string.IsNullOrEmpty(strMaxUsageCount))
                            //{
                            //    Promotion.MaxUsageCount = Convert.ToInt32(strMaxUsageCount);
                            //}
                            Promotion.MaxUsageCount = !string.IsNullOrEmpty(strMaxUsageCount) ? Convert.ToInt32(strMaxUsageCount) : (int?)null;

                            Promotion.IsActive = string.IsNullOrEmpty(strIsActive) || Convert.ToString(strIsActive) == "yes";
                            Promotion.CreatedBy = CurrentUser.UserInfoID;

                            if (Promotion.DiscountPercent > 0 || Promotion.DiscountAmount > 0)
                            {
                                throw new Exception("Discount percent/amount must be less than 0");
                            }

                            Promotions.Add(Promotion);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Promotions;
        }
        public bool ValidateDate(string dateinput)
        {
            DateTime DateTime = new DateTime();
            return DateTime.TryParse(dateinput, out DateTime);
        }
        public bool ValidateDecimal(string decimalinput)
        {
            decimal value;
            return Decimal.TryParse(decimalinput, out value);
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

                        if (objData != null && objData.Tables[0].Rows.Count > 0)
                        {
                            gvPromotion.DataSource = objData;
                            gvPromotion.DataBind();

                            hfFilePath.Value = Convert.ToString(filepath);
                            btnSubmit.Visible = true;
                        }
                        else
                        {
                            btnSubmit.Visible = false;
                            throw new Exception("There is no data to import, please make sure to name the import spreadsheet 'toimport'");
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

                List<ImageSolutions.Promotion.Promotion> Promotions = new List<ImageSolutions.Promotion.Promotion>();

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
                                //Excel
                            }
                        }
                        else
                        {
                            throw new Exception("There is no data to import, please make sure to name the import spreadsheet 'toimport'");
                        }
                    }
                }


                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    foreach (ImageSolutions.Promotion.Promotion _Promotion in Promotions)
                    {
                        if (_Promotion != null && !string.IsNullOrEmpty(_Promotion.PromotionID))
                        {
                            //Update
                        }
                        else
                        {
                            //Create
                        }

                    }
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
            catch (Exception ex)
            {
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
    }
}