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
    public partial class UserWebsiteImport : BasePageAdminUserWebSiteAuth
    {
        private string mWebsiteID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuUser.PostedFile.ContentLength > 0)
            {
                string strPath = Server.MapPath("\\Import\\UserWebsite\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");
                //string strLocalFolder = ("\\Import\\UserWebsite\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

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
            }
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
                                string strEmployeeName = Convert.ToString(objData.Tables[0].Rows[i]["Employee Name"]).Trim();
                                string strEmail = Convert.ToString(objData.Tables[0].Rows[i]["Email"]).Trim();
                                string strDivison = Convert.ToString(objData.Tables[0].Rows[i]["Divison"]).Trim();
                                string strMilitary = Convert.ToString(objData.Tables[0].Rows[i]["Military Branch"]).Trim();
                                string strEmployeeID = Convert.ToString(objData.Tables[0].Rows[i]["Employee ID"]).Trim();
                                string strBranchLocation = Convert.ToString(objData.Tables[0].Rows[i]["Branch Location"]).Trim();
                                string strCountry = Convert.ToString(objData.Tables[0].Rows[i]["Country"]).Trim();
                                string strRole = Convert.ToString(objData.Tables[0].Rows[i]["Role"]).Trim();

                                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                UserInfoFilter.EmailAddress.SearchString = strEmail;
                                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                                if (UserInfo == null)
                                {
                                    CreateUser(strEmployeeName, strEmail, strDivison, strMilitary, strEmployeeID, strBranchLocation, strRole);
                                }
                                else
                                {
                                    UpdateUser(UserInfo, strEmployeeName, strEmail, strDivison, strMilitary, strEmployeeID, strBranchLocation, strRole);
                                }
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

        protected void CreateUser(string strEmployeeName, string strEmail, string strDivison, string strMilitary, string strEmployeeID
            , string strBranchLocation, string strRole)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                if(!string.IsNullOrEmpty(strEmail))
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();

                    objTran = objConn.BeginTransaction();
                    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                    UserInfo.FirstName = strEmployeeName.Split(' ')[0];
                    UserInfo.LastName = strEmployeeName.Replace(UserInfo.FirstName, string.Empty).Trim();
                    UserInfo.EmailAddress = strEmail;
                    UserInfo.Password = "Default123!";
                    UserInfo.Create(objConn, objTran);

                    ImageSolutions.User.UserWebsite UserWebSite = new ImageSolutions.User.UserWebsite();
                    UserWebSite.UserInfoID = UserInfo.UserInfoID;
                    UserWebSite.WebsiteID = CurrentUser.CurrentUserWebSite.WebsiteID;
                    UserWebSite.Division = strDivison;
                    UserWebSite.Military = strMilitary;
                    UserWebSite.EmployeeID = strEmployeeID;
                    UserWebSite.OptInForNotification = true;
                    UserWebSite.CreatedBy = CurrentUser.UserInfoID;
                    UserWebSite.Create(objConn, objTran);

                    ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                    ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                    AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                    AccountFilter.AccountName.SearchString = strBranchLocation;
                    AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    AccountFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                    Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                    ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                    ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                    WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupFilter.GroupName.SearchString = strRole;
                    WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                    WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                    if (Account != null && !string.IsNullOrEmpty(Account.AccountID) &&
                        WebsiteGroup != null && !string.IsNullOrEmpty(WebsiteGroup.WebsiteGroupID))
                    {
                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        UserAccount.UserWebsiteID = UserWebSite.UserWebsiteID;
                        UserAccount.AccountID = Account.AccountID;
                        UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                        UserAccount.CreatedBy = CurrentUser.UserInfoID;
                        UserAccount.Create(objConn, objTran);
                    }

                    objTran.Commit();
                }                
            }
            catch(Exception ex)
            {
                objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }

        protected void UpdateUser(ImageSolutions.User.UserInfo userinfo, string strEmployeeName, string strEmail, string strDivison, string strMilitary, string strEmployeeID
            , string strBranchLocation, string strRole)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                if (!string.IsNullOrEmpty(strEmail))
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();

                    objTran = objConn.BeginTransaction();
                    userinfo.FirstName = strEmployeeName.Split(' ')[0];
                    userinfo.LastName = strEmployeeName.Replace(userinfo.FirstName, string.Empty).Trim();
                    userinfo.EmailAddress = strEmail;
                    userinfo.Password = "Default123!";
                    userinfo.Update(objConn, objTran);

                    
                    ImageSolutions.User.UserWebsite UserWebSite = userinfo.UserWebsites.Find(x => x.WebsiteID == CurrentUser.CurrentUserWebSite.WebsiteID);

                    if(UserWebSite != null && !string.IsNullOrEmpty(UserWebSite.UserWebsiteID))
                    {
                        UserWebSite.Division = strDivison;
                        UserWebSite.Military = strMilitary;
                        UserWebSite.EmployeeID = strEmployeeID;
                        UserWebSite.Update(objConn, objTran);
                    }

                    ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                    ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                    AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                    AccountFilter.AccountName.SearchString = strBranchLocation;
                    AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    AccountFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                    Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                    ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                    ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                    WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupFilter.GroupName.SearchString = strRole;
                    WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                    WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                    if (Account != null && !string.IsNullOrEmpty(Account.AccountID) &&
                        WebsiteGroup != null && !string.IsNullOrEmpty(WebsiteGroup.WebsiteGroupID))
                    {
                        List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebSite.UserWebsiteID;
                        UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.AccountID.SearchString = Account.AccountID;
                        UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter);

                        foreach (ImageSolutions.User.UserAccount _UserAccount in UserAccounts)
                        {
                            if(_UserAccount.WebsiteGroupID != WebsiteGroup.WebsiteGroupID)
                            {
                                _UserAccount.Delete(objConn, objTran);
                            }
                        }

                        if(!UserAccounts.Exists(x => x.WebsiteGroupID == WebsiteGroup.WebsiteGroupID))
                        {
                            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebSite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = CurrentUser.UserInfoID;
                            UserAccount.Create(objConn, objTran);
                        }
                    }

                    objTran.Commit();
                }
            }
            catch (Exception ex)
            {
                objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
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
            if(!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                SubmitFile(Convert.ToString(hfFilePath.Value));
                Response.Redirect("/Admin/UserWebsiteOverview.aspx");
            }
        }
    }    
}