using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class UserWebsiteBudgetProgramImport : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fileName = "UserBudgetProgramTemplate.csv";
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
                if (fuUser.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\UserWebsite\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

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
                    gvUserBudgetProgramUpload.DataSource = null;
                    gvUserBudgetProgramUpload.DataBind();

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
                    gvUserBudgetProgramUpload.DataSource = null;
                    gvUserBudgetProgramUpload.DataBind();

                    btnSubmit.Visible = false;
                    break;
            }
        }

        public void UploadCSV(string filepath)
        {
            List<UserBudgetProgram> UserBudgetPrograms = GetCSV(filepath);
            if (UserBudgetPrograms != null && UserBudgetPrograms.Count > 0)
            {
                gvUserBudgetProgramUpload.DataSource = UserBudgetPrograms;
                gvUserBudgetProgramUpload.DataBind();

                hfFilePath.Value = Convert.ToString(filepath);
                btnSubmit.Visible = true;
            }
            else
            {
                btnSubmit.Visible = false;
                throw new Exception("There is no data to import");
            }
        }

        public List<UserBudgetProgram> GetCSV(string filepath)
        {
            List<UserBudgetProgram> UserBudgetPrograms = null;

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

                            if (!HeaderFields.Contains("user")) throw new Exception("[user] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("assign (yes/no)")) throw new Exception("[assign (yes/no)] is missing from the report, excel contact administrator");

                            UserBudgetPrograms = new List<UserBudgetProgram>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strUser = Convert.ToString(ColumnValues[HeaderFields.IndexOf("user")]).Trim();
                            string strAssign = Convert.ToString(ColumnValues[HeaderFields.IndexOf("assign (yes/no)")]);

                            if(!string.IsNullOrEmpty(strUser) || !string.IsNullOrEmpty(strAssign))
                            {
                                ImageSolutions.User.UserWebsite UserWebsite = null;
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = null;
                                if (string.IsNullOrEmpty(strUser))
                                {
                                    throw new Exception(String.Format("Row {0}: Column [user] is missing", counter));
                                }
                                else
                                {
                                    UserWebsite = new ImageSolutions.User.UserWebsite();
                                    UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                    UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.EmailAddress.SearchString = strUser;
                                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                    if (UserWebsite == null)
                                    {
                                        UserWebsite = new ImageSolutions.User.UserWebsite();
                                        UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                        UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                                        UserWebsiteFilter.EmployeeID.SearchString = strUser;
                                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                        if (UserWebsite == null)
                                        {
                                            throw new Exception(String.Format("Row {0}: Invalid User", counter));
                                        }
                                    }
                                }

                                if (string.IsNullOrEmpty(strAssign))
                                {
                                    throw new Exception(String.Format("Row {0}: Column [assign (yes/no)] is missing", counter));
                                }
                                else if (strAssign.ToLower() != "yes" && strAssign.ToLower() != "no")
                                {
                                    throw new Exception(String.Format("Row {0}: Column [assign (yes/no)] must be 'yes' or 'no'", counter));
                                }

                                UserBudgetProgram UserBudgetProgram = new UserBudgetProgram();
                                UserBudgetProgram.User = strUser;
                                UserBudgetProgram.Assign = strAssign;
                                UserBudgetProgram.UserWebsiteID = UserWebsite.UserWebsiteID;
                                UserBudgetPrograms.Add(UserBudgetProgram);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return UserBudgetPrograms;
        }

        public class UserBudgetProgram
        {
            public string User { get; set; }
            public string Assign { get; set; }
            public string UserWebsiteID { get; set; }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {
                    SubmitFile(Convert.ToString(hfFilePath.Value));
                    Response.Redirect(string.Format("/Admin/UserWebsiteOverview.aspx"));
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
            catch (Exception ex)
            {
                throw ex;
                //WebUtility.DisplayJavascriptMessage(this, ex.Message);
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

                    List<UserBudgetProgram> UserBudgetPrograms = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (UserBudgetProgram _UserBudgetProgram in UserBudgetPrograms)
                    {
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(_UserBudgetProgram.UserWebsiteID);

                        UserWebsite.ApplyBudgetProgram = _UserBudgetProgram.Assign.ToLower() == "yes";
                        UserWebsite.Update(objConn, objTran);


                        if (
                            (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "20")
                            ||
                            (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "53")
                        )
                        {
                            ImageSolutions.Enterprise.EnterpriseCustomer EnterpriseCustomer = new ImageSolutions.Enterprise.EnterpriseCustomer();
                            ImageSolutions.Enterprise.EnterpriseCustomerFilter EnterpriseCustomerFilter = new ImageSolutions.Enterprise.EnterpriseCustomerFilter();
                            EnterpriseCustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                            EnterpriseCustomerFilter.EmployeeID.SearchString = UserWebsite.EmployeeID;
                            EnterpriseCustomer = ImageSolutions.Enterprise.EnterpriseCustomer.GetEnterpriseCustomer(EnterpriseCustomerFilter);

                            if (EnterpriseCustomer != null && !string.IsNullOrEmpty(EnterpriseCustomer.EnterpriseCustomerID))
                            {
                                EnterpriseCustomer.IsUpdated = true;
                                EnterpriseCustomer.Update(objConn, objTran);
                            }
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
    }
}