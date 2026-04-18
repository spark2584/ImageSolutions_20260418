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
    public partial class BudgetImport : BasePage
    {
        //protected string mCreditCardID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            //mCreditCardID = Request.QueryString.Get("creditcardid");

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
            List<ImageSolutions.Budget.Budget> Budgets = GetCSV(filepath);
            if (Budgets != null && Budgets.Count > 0)
            {
                gvBudget.DataSource = Budgets;
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

                    List<ImageSolutions.Budget.Budget> Budgets = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (ImageSolutions.Budget.Budget _Budget in Budgets)
                    {
                        if(_Budget.IsNew)
                        {
                            _Budget.IncludeShippingAndTaxes = true;
                            _Budget.Create(objConn, objTran);
                        }
                        else
                        {
                            _Budget.Update(objConn, objTran);
                        }
                        //if (_Budget != null && !string.IsNullOrEmpty(_Budget.BudgetID))
                        //{
                        //    UpdateBudget(_Budget, _Budget.BudgetName, _Budget.StartDate, _Budget.EndDate, _Budget.BudgetAmount, _Budget.AllowOverBudget, _Budget.PaymentTermID, _Budget.ApproverUserWebsiteID, objConn, objTran);
                        //}
                        //else
                        //{
                        //    CreateBudget(_Budget, _Budget.BudgetName, _Budget.StartDate, _Budget.EndDate, Convert.ToDecimal(_Budget.BudgetAmount), _Budget.AllowOverBudget, _Budget.PaymentTermID, _Budget.ApproverUserWebsiteID, objConn, objTran);
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
        public List<ImageSolutions.Budget.Budget> GetCSV(string filepath)
        {
            List<ImageSolutions.Budget.Budget> Budgets = null;

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
                            if (!HeaderFields.Contains("email")) throw new Exception("[email] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("name")) throw new Exception("[name] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("start_date")) throw new Exception("[start_date] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("end_date")) throw new Exception("[endd_ate] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("amount")) throw new Exception("[amount] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("allow_overbudget")) throw new Exception("[allow_overbudget] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("payment_term")) throw new Exception("[payment_term] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("approver")) throw new Exception("[approver] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("division")) throw new Exception("[division] is missing from the excel, please contact administrator");

                            Budgets = new List<ImageSolutions.Budget.Budget>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("id")]).Trim();
                            string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("email")]);
                            string strName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("name")]);
                            string strStartDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("start_date")]);
                            string strEndDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("end_date")]);
                            string strAmount = Convert.ToString(ColumnValues[HeaderFields.IndexOf("amount")]);
                            string strAllowOverbudget = Convert.ToString(ColumnValues[HeaderFields.IndexOf("allow_overbudget")]).Trim();
                            string strPaymentTerm = Convert.ToString(ColumnValues[HeaderFields.IndexOf("payment_term")]);
                            string strApprover = Convert.ToString(ColumnValues[HeaderFields.IndexOf("approver")]);
                            string strDivision = Convert.ToString(ColumnValues[HeaderFields.IndexOf("division")]);

                            if (!string.IsNullOrEmpty(strID) && !Regex.IsMatch(strID, @"\d"))
                            {
                                throw new Exception(string.Format("Line {1} - Invalid ID: {0}", strID, counter));
                            }

                            if (string.IsNullOrEmpty(strID))
                            {
                                ImageSolutions.Budget.Budget ExistBudget = new ImageSolutions.Budget.Budget();
                                ImageSolutions.Budget.BudgetFilter ExistBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                                ExistBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                                ExistBudgetFilter.BudgetName.SearchString = strName;
                                ExistBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ExistBudgetFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                ExistBudget = ImageSolutions.Budget.Budget.GetBudget(ExistBudgetFilter);

                                if(ExistBudget != null && !string.IsNullOrEmpty(ExistBudget.BudgetID))
                                {
                                    throw new Exception(string.Format("Line {1} - Budget already exists: {0}", strName, counter));
                                }
                            }

                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmailAddress.SearchString = strEmail;
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite == null)
                            {
                                try
                                {
                                    UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                    UserWebsiteFilter.CustomerInternalID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.CustomerInternalID.SearchString = strEmail;
                                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                    if (UserWebsite == null)
                                    {
                                        throw new Exception(string.Format("Line {1} - Invalid Email: {0}", strEmail, counter));
                                    }
                                    else
                                    {
                                        strEmail = UserWebsite.UserInfo.EmailAddress;
                                    }
                                }
                                catch
                                {
                                    throw new Exception(string.Format("Line {1} - Invalid Email: {0}", strEmail, counter));
                                }
                            }

                            if (!ValidateDate(strStartDate))
                            {
                                throw new Exception(string.Format("Line {1} - Invalid Start Date: {0} - Date must be in MM/DD/YYYY format", strStartDate, counter));
                            }

                            if (!ValidateDate(strEndDate))
                            {
                                throw new Exception(string.Format("Line {1} - Invalid End Date: {0} - Date must be in MM/DD/YYYY format", strEndDate, counter));
                            }

                            if (!string.IsNullOrEmpty(strAmount) && !ValidateDecimal(strAmount))
                            {
                                throw new Exception(string.Format("Line {1} - Invalid Amount: {0}", strAmount, counter));
                            }

                            ImageSolutions.Budget.Budget Budget = null;

                            if (!string.IsNullOrEmpty(strID))
                            {
                                Budget = new ImageSolutions.Budget.Budget(strID);
                                if (Budget == null)
                                {
                                    throw new Exception(string.Format("Line {1} - Invalid ID: {0}", strID, counter));
                                }

                                if (Budget.BudgetAssignments.Exists(x => x.UserWebsiteID != UserWebsite.UserWebsiteID))
                                {
                                    throw new Exception(string.Format("Line {1} - Invalid User Budget Import: ID# {0} - Cannot change assigned user", strID, counter));
                                }
                            }
                            else
                            {
                                Budget = new ImageSolutions.Budget.Budget();
                                ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                BudgetAssignment.WebsiteID = CurrentWebsite.WebsiteID;
                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                BudgetAssignment.CreatedBy = CurrentUser.UserInfoID;
                                Budget.BudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                                Budget.BudgetAssignments.Add(BudgetAssignment);
                            }

                            if (!string.IsNullOrEmpty(strAllowOverbudget) && strAllowOverbudget != "yes" && strAllowOverbudget != "no")
                            {
                                throw new Exception(string.Format("Line {0} - Column [allow_overbudget] must be 'yes' or 'no'", counter));
                            }

                            Budget.WebsiteID = CurrentWebsite.WebsiteID;
                            Budget.BudgetName = strName;
                            Budget.StartDate = Convert.ToDateTime(strStartDate);
                            Budget.EndDate = Convert.ToDateTime(strEndDate);
                            Budget.BudgetAmount = Convert.ToDouble(strAmount);
                            Budget.AllowOverBudget = Convert.ToString(strAllowOverbudget) == "yes";
                            Budget.Division = Convert.ToString(strDivision);
                            Budget.CreatedBy = CurrentUser.UserInfoID;

                            if (!string.IsNullOrEmpty(strApprover))
                            {

                                ImageSolutions.User.UserWebsite ApproverUserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter ApproverUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                ApproverUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ApproverUserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                ApproverUserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                ApproverUserWebsiteFilter.EmailAddress.SearchString = strApprover;
                                ApproverUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(ApproverUserWebsiteFilter);

                                if (ApproverUserWebsite == null)
                                {
                                    throw new Exception(string.Format("Line {1} - Invalid Approver: {0}", strApprover, counter));
                                }

                                Budget.ApproverUserWebsiteID = ApproverUserWebsite.UserWebsiteID;
                            }


                            if (!string.IsNullOrEmpty(strPaymentTerm))
                            {
                                ImageSolutions.Payment.PaymentTerm PaymentTerm = new ImageSolutions.Payment.PaymentTerm();
                                ImageSolutions.Payment.PaymentTermFilter PaymentTermFilter = new ImageSolutions.Payment.PaymentTermFilter();
                                PaymentTermFilter.Description = new Database.Filter.StringSearch.SearchFilter();
                                PaymentTermFilter.Description.SearchString = strPaymentTerm;
                                PaymentTerm = ImageSolutions.Payment.PaymentTerm.GetPaymentTerm(PaymentTermFilter);
                                if (PaymentTerm != null && !string.IsNullOrEmpty(PaymentTerm.PaymentTermID))
                                {
                                    Budget.PaymentTermID = PaymentTerm.PaymentTermID;
                                }
                                else
                                {
                                    throw new Exception(string.Format("Line {1} - Invalid Payment Term: {0}", strPaymentTerm, counter));
                                }
                            }

                            Budgets.Add(Budget);                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Budgets;
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

        //protected void CreateBudget(ImageSolutions.Budget.Budget budget, SqlConnection conn, SqlTransaction tran)
        //{
        //    try
        //    {
        //        budget.CreatedBy = CurrentUser.UserInfoID;


        //        if (active)
        //        {
        //            ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard();
        //            UserCreditCard.UserInfoID = userinfoid;
        //            UserCreditCard.CreditCardID = mCreditCardID;
        //            if (resetdayofthemonth != null && resetdayofthemonth > 0)
        //            {
        //                UserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(resetdayofthemonth);
        //            }
        //            else
        //            {
        //                UserCreditCard.ResetDayOfTheMonth = null;
        //            }
        //            if (setlimit != null && setlimit > 0)
        //            {
        //                UserCreditCard.Limit = Convert.ToDouble(setlimit);
        //            }
        //            else
        //            {
        //                UserCreditCard.Limit = null;
        //            }
        //            UserCreditCard.CreatedBy = CurrentUser.UserInfoID;
        //            UserCreditCard.Create(conn, tran);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //protected void UpdateBudget(ImageSolutions.User.UserCreditCard usercreditcard, int? resetdayofthemonth, double? setlimit, bool active, SqlConnection conn, SqlTransaction tran)
        //{
        //    try
        //    {
        //        if (active)
        //        {
        //            usercreditcard.UserInfoID = usercreditcard.UserInfoID;
        //            usercreditcard.CreditCardID = mCreditCardID;
        //            if (resetdayofthemonth != null && resetdayofthemonth > 0)
        //            {
        //                usercreditcard.ResetDayOfTheMonth = Convert.ToInt32(resetdayofthemonth);
        //            }
        //            else
        //            {
        //                usercreditcard.ResetDayOfTheMonth = null;
        //            }
        //            if (setlimit != null && setlimit > 0)
        //            {
        //                usercreditcard.Limit = Convert.ToDouble(setlimit);
        //            }
        //            else
        //            {
        //                usercreditcard.Limit = null;
        //            }
        //            usercreditcard.Update(conn, tran);
        //        }
        //        else
        //        {
        //            usercreditcard.Delete(conn, tran);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/BudgetOverview.aspx"));
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
            string fileName = "BudgetTemplate.csv";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }       
    }
}