using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserCreditCardImport : BasePage
    {
        protected string mCreditCardID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            mCreditCardID = Request.QueryString.Get("creditcardid");
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuUser.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\UserCreditCard\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

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
                    gvUserUpload.DataSource = null;
                    gvUserUpload.DataBind();

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
                    gvUserUpload.DataSource = null;
                    gvUserUpload.DataBind();

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
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public void UploadCSV(string filepath)
        {
            List<ImageSolutions.User.UserCreditCard> UserCreditCards = GetCSV(filepath);
            if (UserCreditCards != null && UserCreditCards.Count > 0)
            {
                gvUserUpload.DataSource = UserCreditCards;
                gvUserUpload.DataBind();

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

                    List<ImageSolutions.User.UserCreditCard> UserCreditCards = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (ImageSolutions.User.UserCreditCard _UserCreditCard in UserCreditCards)
                    {
                        if (_UserCreditCard != null && !string.IsNullOrEmpty(_UserCreditCard.UserCreditCardID))
                        {
                            UpdateUserCreditCard(_UserCreditCard, _UserCreditCard.ResetDayOfTheMonth, _UserCreditCard.Limit, _UserCreditCard.IsActive, objConn, objTran);
                        }
                        else
                        {
                            CreateUserCreditCard(_UserCreditCard.UserInfoID, _UserCreditCard.ResetDayOfTheMonth, _UserCreditCard.Limit, _UserCreditCard.IsActive, objConn, objTran);
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
                //WebUtility.DisplayJavascriptMessage(this, "File missing");
            }
        }
        public List<ImageSolutions.User.UserCreditCard> GetCSV(string filepath)
        {
            List<ImageSolutions.User.UserCreditCard> UserCreditCards = null;

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
                            if (!HeaderFields.Contains("reset_day_of_the_month")) throw new Exception("[reset_day_of_the_month] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("set_limit")) throw new Exception("[set_limit] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("active")) throw new Exception("[active] is missing from the excel, please contact administrator");
                            
                            UserCreditCards = new List<ImageSolutions.User.UserCreditCard>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("id")]).Trim();
                            string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("email")]);
                            string strResetDayOfTheMonth = Convert.ToString(ColumnValues[HeaderFields.IndexOf("reset_day_of_the_month")]);
                            string strSetLimit = Convert.ToString(ColumnValues[HeaderFields.IndexOf("set_limit")]);
                            string strActive = Convert.ToString(ColumnValues[HeaderFields.IndexOf("active")]);

                            if(strActive != "yes" && strActive != "no")
                            {
                                throw new Exception("column [active] must be 'yes' or 'non");
                            }

                            ImageSolutions.User.UserCreditCard UserCreditCard = null;
                            if (!string.IsNullOrEmpty(strID))
                            {
                                UserCreditCard = new ImageSolutions.User.UserCreditCard(strID);
                                if (UserCreditCard == null)
                                {
                                    throw new Exception(string.Format("Invalid ID: {0}", strID));
                                }
                                if (UserCreditCard.UserInfo.EmailAddress != strEmail)
                                {
                                    throw new Exception(string.Format("Email cannot be updated: {0} - {1}", strID, strEmail));
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
                                throw new Exception(string.Format("Invalid Email: {0}", strEmail));
                            }

                            if (UserCreditCard == null)
                            {
                                ImageSolutions.User.UserCreditCard NewUserCreditCard = new ImageSolutions.User.UserCreditCard();
                                NewUserCreditCard.UserInfoID = UserWebsite.UserInfoID;
                                if (!string.IsNullOrEmpty(strResetDayOfTheMonth))
                                {
                                    NewUserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(strResetDayOfTheMonth);
                                }
                                else
                                {
                                    NewUserCreditCard.ResetDayOfTheMonth = null;
                                }
                                if (!string.IsNullOrEmpty(strSetLimit))
                                {
                                    NewUserCreditCard.Limit = Convert.ToDouble(strSetLimit);
                                }
                                else
                                {
                                    NewUserCreditCard.Limit = null;
                                }
                                NewUserCreditCard.IsActive = string.IsNullOrEmpty(strActive) || strActive == "yes";
                                UserCreditCards.Add(NewUserCreditCard);
                            }
                            else
                            {
                                ImageSolutions.User.UserCreditCard UpdateUserCreditCard = new ImageSolutions.User.UserCreditCard(UserCreditCard.UserCreditCardID);
                                UpdateUserCreditCard.UserInfoID = UserWebsite.UserInfoID;
                                if (!string.IsNullOrEmpty(strResetDayOfTheMonth))
                                {
                                    UpdateUserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(strResetDayOfTheMonth);
                                }
                                else
                                {
                                    UpdateUserCreditCard.ResetDayOfTheMonth = null;
                                }
                                if (!string.IsNullOrEmpty(strSetLimit))
                                {
                                    UpdateUserCreditCard.Limit = Convert.ToDouble(strSetLimit);
                                }
                                else
                                {
                                    UpdateUserCreditCard.Limit = null;
                                }
                                UpdateUserCreditCard.IsActive = string.IsNullOrEmpty(strActive) || strActive == "yes";
                                UserCreditCards.Add(UpdateUserCreditCard);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return UserCreditCards;
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
                            gvUserUpload.DataSource = objData;
                            gvUserUpload.DataBind();

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

                List<ImageSolutions.User.UserCreditCard> UserCreditCards = new List<ImageSolutions.User.UserCreditCard>();

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
                                string strID = Convert.ToString(objData.Tables[0].Rows[i]["id"]).Trim();
                                string strEmail = Convert.ToString(objData.Tables[0].Rows[i]["email"]).Trim();
                                string strResetDayOfTheMonth = Convert.ToString(objData.Tables[0].Rows[i]["reset_day_of_the_month"]).Trim();
                                string strSetLimit = Convert.ToString(objData.Tables[0].Rows[i]["set_limit"]).Trim();
                                string strActive = Convert.ToString(objData.Tables[0].Rows[i]["active"]).Trim();

                                ImageSolutions.User.UserCreditCard UserCreditCard = null;
                                if (!string.IsNullOrEmpty(strID))
                                {
                                    UserCreditCard = new ImageSolutions.User.UserCreditCard(strID);
                                    if (UserCreditCard == null)
                                    { 
                                        throw new Exception(string.Format("Invalid ID: {0}", strID));
                                    }
                                    if(UserCreditCard.UserInfo.EmailAddress != strEmail)
                                    {
                                        throw new Exception(string.Format("Email cannot be updated: {0} - {1}", strID, strEmail));
                                    }
                                }

                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.EmailAddress.SearchString = strEmail;
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                                if(UserWebsite == null)
                                {
                                    throw new Exception(string.Format("Invalid Email: {0}", strEmail));
                                }

                                if (UserCreditCard == null)
                                {
                                    ImageSolutions.User.UserCreditCard NewUserCreditCard = new ImageSolutions.User.UserCreditCard();
                                    NewUserCreditCard.UserInfoID = UserWebsite.UserInfoID;
                                    if (!string.IsNullOrEmpty(strResetDayOfTheMonth))
                                    {
                                        NewUserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(strResetDayOfTheMonth);
                                    }
                                    else
                                    {
                                        NewUserCreditCard.ResetDayOfTheMonth = null;
                                    }
                                    if (!string.IsNullOrEmpty(strSetLimit))
                                    {
                                        NewUserCreditCard.Limit = Convert.ToDouble(strSetLimit);
                                    }
                                    else
                                    {
                                        NewUserCreditCard.Limit = null;
                                    }
                                    NewUserCreditCard.IsActive = string.IsNullOrEmpty(strActive) || strActive == "yes";
                                    UserCreditCards.Add(NewUserCreditCard);
                                    //CreateUserCreditCard(UserWebsite.UserInfoID, strResetDayOfTheMonth, strSetLimit, strActive, objConn, objTran);
                                }
                                else
                                {
                                    ImageSolutions.User.UserCreditCard UpdateUserCreditCard = new ImageSolutions.User.UserCreditCard(UserCreditCard.UserCreditCardID);
                                    UpdateUserCreditCard.UserInfoID = UserWebsite.UserInfoID;
                                    if (!string.IsNullOrEmpty(strResetDayOfTheMonth))
                                    {
                                        UpdateUserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(strResetDayOfTheMonth);
                                    }
                                    else
                                    {
                                        UpdateUserCreditCard.ResetDayOfTheMonth = null;
                                    }
                                    if (!string.IsNullOrEmpty(strSetLimit))
                                    {
                                        UpdateUserCreditCard.Limit = Convert.ToDouble(strSetLimit);
                                    }
                                    else
                                    {
                                        UpdateUserCreditCard.Limit = null;
                                    }
                                    UpdateUserCreditCard.IsActive = string.IsNullOrEmpty(strActive) || strActive == "yes";
                                    UserCreditCards.Add(UpdateUserCreditCard);
                                    //UpdateUserCreditCard(UserCreditCard, UserWebsite.UserInfoID, strResetDayOfTheMonth, strSetLimit, strActive, objConn, objTran);
                                }
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

                    foreach (ImageSolutions.User.UserCreditCard _UserCreditCard in UserCreditCards)
                    {
                        if (_UserCreditCard != null && !string.IsNullOrEmpty(_UserCreditCard.UserCreditCardID))
                        {
                            UpdateUserCreditCard(_UserCreditCard, _UserCreditCard.ResetDayOfTheMonth, _UserCreditCard.Limit, _UserCreditCard.IsActive, objConn, objTran);
                        }
                        else
                        {
                            CreateUserCreditCard(_UserCreditCard.UserInfoID, _UserCreditCard.ResetDayOfTheMonth, _UserCreditCard.Limit, _UserCreditCard.IsActive, objConn, objTran);
                        }

                    }
                    objTran.Commit();
                }
                catch(Exception ex)
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
        protected void CreateUserCreditCard(string userinfoid, int? resetdayofthemonth, double? setlimit, bool active, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                if(active)
                {
                    ImageSolutions.User.UserCreditCard UserCreditCard = new ImageSolutions.User.UserCreditCard();
                    UserCreditCard.UserInfoID = userinfoid;
                    UserCreditCard.CreditCardID = mCreditCardID;
                    if (resetdayofthemonth != null && resetdayofthemonth > 0)
                    {
                        UserCreditCard.ResetDayOfTheMonth = Convert.ToInt32(resetdayofthemonth);
                    }
                    else
                    {
                        UserCreditCard.ResetDayOfTheMonth = null;
                    }
                    if (setlimit != null && setlimit > 0)
                    {
                        UserCreditCard.Limit = Convert.ToDouble(setlimit);
                    }
                    else
                    {
                        UserCreditCard.Limit = null;
                    }
                    UserCreditCard.CreatedBy = CurrentUser.UserInfoID;
                    UserCreditCard.Create(conn, tran);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void UpdateUserCreditCard(ImageSolutions.User.UserCreditCard usercreditcard, int? resetdayofthemonth, double? setlimit, bool active, SqlConnection conn, SqlTransaction tran)
        {
            try
            {
                if (active)
                {
                    usercreditcard.UserInfoID = usercreditcard.UserInfoID;
                    usercreditcard.CreditCardID = mCreditCardID;
                    if (resetdayofthemonth != null && resetdayofthemonth > 0)
                    {
                        usercreditcard.ResetDayOfTheMonth = Convert.ToInt32(resetdayofthemonth);
                    }
                    else
                    {
                        usercreditcard.ResetDayOfTheMonth = null;
                    }
                    if (setlimit != null && setlimit > 0)
                    {
                        usercreditcard.Limit = Convert.ToDouble(setlimit);
                    }
                    else
                    {
                        usercreditcard.Limit = null;
                    }
                    usercreditcard.Update(conn, tran);
                }
                else
                {
                    usercreditcard.Delete(conn, tran);
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/CreditCard.aspx?id={0}", mCreditCardID));
                }
                catch(Exception ex)
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
            string fileName = "UserCreditCardTemplate.csv";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }
    }
}