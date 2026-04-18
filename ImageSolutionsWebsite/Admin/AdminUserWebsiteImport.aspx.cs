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
    public partial class AdminUserWebsiteImport : BasePageAdminUserWebSiteAuth
{
        private string mWebsiteID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuUser.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\UserWebsite\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    string strFileName = fuUser.PostedFile.FileName;
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
                    //UploadExcel(filepath, strConnectionString);
                    break;
                case ".xlsx": //Excel 07
                    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                    //UploadExcel(filepath, strConnectionString);
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
        public void UploadCSV(string filepath)
        {
            List<UserUpload> UserUploads = GetCSV(filepath);
            if (UserUploads != null && UserUploads.Count > 0)
            {
                gvUserUpload.DataSource = UserUploads;
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

        public List<UserUpload> GetCSV(string filepath)
        {
            List<UserUpload> UserUploads = null;

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

                            if (!HeaderFields.Contains("FirstName")) throw new Exception("[FirstName] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("LastName")) throw new Exception("[LastName] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Email")) throw new Exception("[Email] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Password")) throw new Exception("[Password] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("EmployeeID")) throw new Exception("[EmployeeID] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Account")) throw new Exception("[Account] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Group")) throw new Exception("[Group] is missing from the report, excel contact administrator");
                            //if (!HeaderFields.Contains("Username")) throw new Exception("[Username] is missing from the report, excel contact administrator");

                            UserUploads = new List<UserUpload>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("FirstName")]).Replace("|",",").Trim();
                            string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LastName")]).Replace("|", ",").Trim();
                            string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Email")]).Replace("|", ",").Trim();
                            string strPassword = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Password")]).Replace("|", ",").Trim();
                            string strEmployeeID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("EmployeeID")]).Replace("|", ",").Trim();
                            string strAccount = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Account")]).Replace("|", ",").Trim();
                            string strGroup = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Group")]).Replace("|", ",").Trim();
                            string strUsername = string.Empty;
                            if (HeaderFields.IndexOf("Username") > 0)
                            {
                                strUsername = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Username")]).Replace("|", ",").Trim();
                            }

                            if (string.IsNullOrEmpty(strFirstName))
                            {
                                throw new Exception(string.Format("line {0}: First Name is required", counter));
                            }

                            if (string.IsNullOrEmpty(strLastName))
                            {
                                throw new Exception(string.Format("line {0}: Last Name is required", counter));
                            }

                            if (string.IsNullOrEmpty(strEmail))
                            {
                                throw new Exception(string.Format("line {0}: Email is required", counter));
                            }

                            if (string.IsNullOrEmpty(strPassword))
                            {
                                throw new Exception(string.Format("line {0}: Password is required", counter));
                            }

                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmailAddress.SearchString = strEmail;
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite != null && !string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            {
                                throw new Exception(string.Format("line {0}: Email {1} already exists for {2}", counter, strEmail, CurrentWebsite.Name));
                            }

                            if (string.IsNullOrEmpty(strAccount))
                            {
                                throw new Exception(string.Format("line {0}: Account is required", counter));
                            }

                            if (!string.IsNullOrEmpty(strUsername))
                            {
                                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                                UserInfoFilter.UserName = new Database.Filter.StringSearch.SearchFilter();
                                UserInfoFilter.UserName.SearchString = strUsername;
                                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                                if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID))
                                {
                                    throw new Exception(string.Format("line {0}: Username already exists", counter));
                                }
                                else
                                {
                                    UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                                    UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                    UserInfoFilter.EmailAddress.SearchString = strUsername;
                                    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                                    if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID))
                                    {
                                        throw new Exception(string.Format("line {0}: Username already exists", counter));
                                    }
                                }
                            }

                            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                            ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                            AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                            AccountFilter.AccountName.SearchString = strAccount;
                            AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            AccountFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                            if (Account == null)
                            {
                                throw new Exception(string.Format("line {0}: Account '{1}' not found", counter, strAccount));
                            }

                            if (string.IsNullOrEmpty(strGroup))
                            {
                                throw new Exception(string.Format("line {0}: Group is required", counter));
                            }

                            ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                            ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                            WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.GroupName.SearchString = strGroup;
                            WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                            if (WebsiteGroup == null)
                            {
                                throw new Exception(string.Format("line {0}: Group '{1}' not found", counter, strGroup));
                            }

                            UserUpload UserUpload = new UserUpload();
                            UserUpload.FirstName = strFirstName;
                            UserUpload.LastName = strLastName;
                            UserUpload.Email = strEmail;
                            UserUpload.Password = strPassword;
                            UserUpload.EmployeeID = strEmployeeID;
                            UserUpload.Account = strAccount;
                            UserUpload.Group = strGroup;
                            UserUpload.UserName = strUsername;

                            UserUploads.Add(UserUpload);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return UserUploads;
        }


        protected void SubmitFile(string filepath)
        {
            string strFullPath = filepath;
            string strExtension = Path.GetExtension(strFullPath);
            string strConnectionString = String.Empty;

            switch (strExtension)
            {
                case ".xls": //Excel 97-03
                    strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;""";
                    //ProcessExcel(strConnectionString);
                    break;
                case ".xlsx": //Excel 07
                    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                    //ProcessExcel(strConnectionString);
                    break;
                case ".csv":
                    ProcessCSV(Convert.ToString(hfFilePath.Value));
                    break;
            }
        }

        public void ProcessCSV(string filepath)
        {
            List<UserUpload> UserUploads = GetCSV(filepath);
            if (UserUploads != null && UserUploads.Count > 0)
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;


                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    foreach (UserUpload _UserUpload in UserUploads)
                    {

                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                        ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                        UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        UserInfoFilter.EmailAddress.SearchString = _UserUpload.Email;
                        UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                        if (UserInfo == null)
                            {
                            UserInfo = new ImageSolutions.User.UserInfo();
                            UserInfo.FirstName = _UserUpload.FirstName;
                            UserInfo.LastName = _UserUpload.LastName;
                            UserInfo.EmailAddress = _UserUpload.Email;
                            UserInfo.Password = _UserUpload.Password;
                            if (!string.IsNullOrEmpty(_UserUpload.UserName))
                            {
                                UserInfo.UserName = _UserUpload.UserName;
                            }
                            UserInfo.Create(objConn, objTran);
                        }
                        else
                        {
                            UserInfo.FirstName = _UserUpload.FirstName;
                            UserInfo.LastName = _UserUpload.LastName;
                            UserInfo.Password = _UserUpload.Password;
                            if (!string.IsNullOrEmpty(_UserUpload.UserName))
                            {
                                UserInfo.UserName = _UserUpload.UserName;
                            }
                            UserInfo.Update(objConn, objTran);
                        }

                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                            UserWebsite.CreatedBy = CurrentUser.UserInfoID;
                            UserWebsite.EmployeeID = _UserUpload.EmployeeID;
                            UserWebsite.Create(objConn, objTran);
                        }
                        else
                        {
                            throw new Exception("User already exists");
                        }

                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                        ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                        AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.AccountName.SearchString = _UserUpload.Account;
                        AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = _UserUpload.Group;
                        WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                        UserAccount.AccountID = Account.AccountID;
                        UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                        UserAccount.CreatedBy = CurrentUser.UserInfoID;
                        UserAccount.Create(objConn, objTran);
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
                btnSubmit.Visible = false;
                throw new Exception("There is no data to import");
            }            
        }

        public void UploadExcel(string filepath, string connectionstring)
        {
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
                            objColumns = objData.Tables[0].Rows[0].Table.Columns;

                            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                            {
                                Console.WriteLine(i);
                                string strFirstName = Convert.ToString(objData.Tables[0].Rows[i]["First Name"]).Trim();                                
                            }
                        }
                        else
                        {
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
        
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect("/Admin/UserWebsiteOverview.aspx");
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "AdminUserWebsiteTemplate.csv";
                string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.WriteFile(filePath);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
        public class UserUpload
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string EmployeeID { get; set; }
            public string Account { get; set; }
            public string Group { get; set; }
            public string UserName { get; set; }
        }
    }
}