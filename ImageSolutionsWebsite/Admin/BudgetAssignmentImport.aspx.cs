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
    public partial class BudgetAssignmentImport : BasePage
    {
        protected string mBudgetID = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            mBudgetID = Request.QueryString.Get("budgetid");

            btnSubmit.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
            btnUpload.Enabled = !CurrentUser.CurrentUserWebSite.IsBudgetViewOnly;
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuUser.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\Budget\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

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
                    gvBudget.DataSource = null;
                    gvBudget.DataBind();

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
                    gvBudget.DataSource = null;
                    gvBudget.DataBind();

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
            List<ImageSolutions.Budget.BudgetAssignment> BudgetAssignments = GetCSV(filepath);
            if (BudgetAssignments != null && BudgetAssignments.Count > 0)
            {
                gvBudget.DataSource = BudgetAssignments;
                gvBudget.DataBind();

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

                    List<ImageSolutions.Budget.BudgetAssignment> BudgetAssignments = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in BudgetAssignments)
                    {
                        if (_BudgetAssignment.IsNew)
                        {
                            _BudgetAssignment.Create(objConn, objTran);
                        }
                        //else
                        //{
                        //    _BudgetAssignment.Update(objConn, objTran);
                        //}
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
        public List<ImageSolutions.Budget.BudgetAssignment> GetCSV(string filepath)
        {
            List<ImageSolutions.Budget.BudgetAssignment> BudgetAssignments = null;

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

                            //if (!HeaderFields.Contains("id")) throw new Exception("[id] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("email")) throw new Exception("[email] is missing from the report, excel contact administrator");
                            //if (!HeaderFields.Contains("inactive")) throw new Exception("[inactive] is missing from the report, excel contact administrator");

                            BudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            //string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("id")]).Trim();
                            string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("email")]);
                            //string strIsActive = Convert.ToString(ColumnValues[HeaderFields.IndexOf("is_active")]);

                            //if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"\d"))
                            //{
                            //    throw new Exception(string.Format("Invalid ID: {0}", strID));
                            //}

                            //if (!string.IsNullOrEmpty(strIsActive) && strIsActive != "yes" && strIsActive != "no")
                            //{
                            //    throw new Exception("Column [is_active] must be 'yes' or 'no'");
                            //}

                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmailAddress.SearchString = strEmail;
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite == null)
                            {
                                throw new Exception(string.Format("Invalid Email: {0}", strEmail));
                            }
                            //ImageSolutions.Budget.BudgetAssignment BudgetAssignment = null;

                            //if (!string.IsNullOrEmpty(strID))
                            //{
                            //    BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strID);
                            //    if (BudgetAssignment == null)
                            //    {
                            //        throw new Exception(string.Format("Invalid ID: {0}", strID));
                            //    }

                            //    BudgetAssignment.UserWebsite.UserInfo.EmailAddress;
                            //}
                            //else
                            //{
                            //    BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            //    BudgetAssignment.WebsiteID = CurrentWebsite.WebsiteID;
                            //    BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                            //    BudgetAssignment.CreatedBy = CurrentUser.UserInfoID;
                            //}

                            ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                            BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                            BudgetAssignmentFilter.BudgetID.SearchString = mBudgetID;
                            BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);

                            if(BudgetAssignment == null)
                            {
                                BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                BudgetAssignment.WebsiteID = CurrentWebsite.WebsiteID;
                                BudgetAssignment.BudgetID = mBudgetID;
                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                BudgetAssignment.CreatedBy = CurrentUser.UserInfoID;
                            }

                            BudgetAssignments.Add(BudgetAssignment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return BudgetAssignments;
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
                            gvBudget.DataSource = objData;
                            gvBudget.DataBind();

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

                List<ImageSolutions.Budget.Budget> Budgets = new List<ImageSolutions.Budget.Budget>();

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

                    foreach (ImageSolutions.Budget.Budget _Budget in Budgets)
                    {
                        if (_Budget != null && !string.IsNullOrEmpty(_Budget.BudgetID))
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
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/Budget.aspx?id=", mBudgetID));
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
            string fileName = "BudgetUserAssignmentTemplate.csv";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }
    }
}