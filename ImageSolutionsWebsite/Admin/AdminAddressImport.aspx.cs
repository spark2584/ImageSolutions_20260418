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
    public partial class AdminAddressImport : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = "AdminAddressTemplate.csv";
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

                            if (!HeaderFields.Contains("Email")) throw new Exception("[Email] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Addressee")) throw new Exception("[Addressee] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("FirstName")) throw new Exception("[FirstName] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("LastName")) throw new Exception("[LastName] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("AddressLine1")) throw new Exception("[AddressLine1] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("AddressLine2")) throw new Exception("[AddressLine2] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("City")) throw new Exception("[City] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("State")) throw new Exception("[State] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("PostalCode")) throw new Exception("[PostalCode] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("CountryCode")) throw new Exception("[CountryCode] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("PhoneNumber")) throw new Exception("[PhoneNumber] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("DefaultShipping")) throw new Exception("[DefaultShipping] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("DefaultBilling")) throw new Exception("[DefaultBilling] is missing from the report, excel contact administrator");

                            UserUploads = new List<UserUpload>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Email")]).Replace("|", ",");
                            string strAddressee = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Addressee")]).Replace("|", ",");
                            string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("FirstName")]).Replace("|", ",");
                            string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LastName")]).Replace("|", ",");
                            string strAddressLine1 = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AddressLine1")]).Replace("|", ",");
                            string strAddressLine2 = Convert.ToString(ColumnValues[HeaderFields.IndexOf("AddressLine2")]).Replace("|", ",");
                            string strCity = Convert.ToString(ColumnValues[HeaderFields.IndexOf("City")]).Replace("|", ",");
                            string strState = Convert.ToString(ColumnValues[HeaderFields.IndexOf("State")]).Replace("|", ",");
                            string strPostalCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("PostalCode")]).Replace("|", ",");
                            string strCountryCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("CountryCode")]).Replace("|", ",");
                            string strPhoneNumber = Convert.ToString(ColumnValues[HeaderFields.IndexOf("PhoneNumber")]).Replace("|", ",");
                            string strDefaultShipping = Convert.ToString(ColumnValues[HeaderFields.IndexOf("DefaultShipping")]).Replace("|", ",");
                            string strDefaultBilling = Convert.ToString(ColumnValues[HeaderFields.IndexOf("DefaultBilling")]).Replace("|", ",");

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

                            if (string.IsNullOrEmpty(strAddressLine1))
                            {
                                throw new Exception(string.Format("line {0}: AddressLine1 is required", counter));
                            }

                            if (string.IsNullOrEmpty(strCity))
                            {
                                throw new Exception(string.Format("line {0}: City is required", counter));
                            }

                            if (string.IsNullOrEmpty(strState))
                            {
                                throw new Exception(string.Format("line {0}: State is required", counter));
                            }

                            if (string.IsNullOrEmpty(strPostalCode))
                            {
                                throw new Exception(string.Format("line {0}: PostalCode is required", counter));
                            }
                            if (string.IsNullOrEmpty(strCountryCode))
                            {
                                throw new Exception(string.Format("line {0}: CountryCode is required", counter));
                            }

                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmailAddress.SearchString = strEmail;
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite == null || string.IsNullOrEmpty(UserWebsite.UserWebsiteID))
                            {
                                throw new Exception(string.Format("line {0}: Invalid Email: {1}", counter, strEmail));
                            }

                            ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                            ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                            AddressCountryCodeFilter.Alpha2Code = new Database.Filter.StringSearch.SearchFilter();
                            AddressCountryCodeFilter.Alpha2Code.SearchString = strCountryCode;
                            AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                            if (AddressCountryCode == null)
                            {
                                if (strCountryCode.Length == 3)
                                {
                                    AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                                    AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                                    AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                                    AddressCountryCodeFilter.Alpha3Code.SearchString = strCountryCode;
                                    AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                                    if (AddressCountryCode != null)
                                    {
                                        strCountryCode = AddressCountryCode.Alpha2Code;
                                    }
                                    else
                                    {
                                        throw new Exception(string.Format("line {0}: Invalid Country: {1}", counter, strCountryCode));
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format("line {0}: Invalid Country: {1}", counter, strCountryCode));
                                }
                            }

                            if (!string.IsNullOrEmpty(strPhoneNumber))
                            {
                                if (strPhoneNumber.Length < 5)
                                {
                                    throw new Exception(string.Format("line {0}: Invalid Phone Number: {1}", counter, strPhoneNumber));
                                }
                            }


                            if (!string.IsNullOrEmpty(strDefaultShipping))
                            {
                                if (strDefaultShipping.ToLower() != "yes" && strDefaultShipping.ToLower() != "no")
                                {
                                    throw new Exception(string.Format("line {0}: Invalid Default Shipping: {1} - please enter 'yes' or 'no'", counter, strDefaultShipping));
                                }
                            }
                            if (!string.IsNullOrEmpty(strDefaultBilling))
                            {
                                if (strDefaultBilling.ToLower() != "yes" && strDefaultBilling.ToLower() != "no")
                                {
                                    throw new Exception(string.Format("line {0}: Invalid Default Billing: {1} - please enter 'yes' or 'no'", counter, strDefaultBilling));
                                }
                            }

                            UserUpload UserUpload = new UserUpload();
                            UserUpload.Email = strEmail;
                            UserUpload.Addressee = strAddressee;
                            UserUpload.FirstName = strFirstName;
                            UserUpload.LastName = strLastName;
                            UserUpload.AddressLine1 = strAddressLine1;
                            UserUpload.AddressLine2 = strAddressLine2;
                            UserUpload.City = strCity;
                            UserUpload.State = strState;
                            UserUpload.PostalCode = strPostalCode;
                            UserUpload.CountryCode = strCountryCode;
                            UserUpload.PhoneNumber = strPhoneNumber;
                            UserUpload.DefaultShipping = strDefaultShipping;
                            UserUpload.DefaultBilling = strDefaultBilling;

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
                            throw new Exception(String.Format("Invalid Email: {0}", _UserUpload.Email)); 
                        }

                        ImageSolutions.Address.AddressBook AddressBook = AddressExists(UserInfo.UserInfoID
                                , _UserUpload.Addressee, _UserUpload.FirstName, _UserUpload.LastName, _UserUpload.AddressLine1, _UserUpload.AddressLine2, _UserUpload.City, _UserUpload.State, _UserUpload.PostalCode, _UserUpload.PhoneNumber);

                        if (AddressBook != null && !string.IsNullOrEmpty(AddressBook.AddressBookID))
                        {
                            if (_UserUpload.DefaultShipping.ToLower() == "yes")
                            {
                                UserInfo.DefaultShippingAddressBookID = AddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);
                            }

                            if (_UserUpload.DefaultBilling.ToLower() == "yes")
                            {
                                UserInfo.DefaultBillingAddressBookID = AddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);
                            }
                        }
                        else
                        {
                            AddressBook = new ImageSolutions.Address.AddressBook();
                            AddressBook.AddressLabel = _UserUpload.Addressee;
                            AddressBook.FirstName = _UserUpload.FirstName;
                            AddressBook.LastName = _UserUpload.LastName;
                            AddressBook.AddressLine1 = _UserUpload.AddressLine1;
                            AddressBook.AddressLine2 = _UserUpload.AddressLine2;
                            AddressBook.City = _UserUpload.City;
                            AddressBook.State = _UserUpload.State;
                            AddressBook.PostalCode = _UserUpload.PostalCode;
                            AddressBook.CountryCode = _UserUpload.CountryCode;
                            AddressBook.PhoneNumber = _UserUpload.PhoneNumber;
                            AddressBook.UserInfoID = UserInfo.UserInfoID;
                            AddressBook.CreatedBy = CurrentUser.UserInfoID;
                            AddressBook.Create(objConn, objTran);

                            if (_UserUpload.DefaultShipping.ToLower() == "yes")
                            {
                                UserInfo.DefaultShippingAddressBookID = AddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);
                            }

                            if (_UserUpload.DefaultBilling.ToLower() == "yes")
                            {
                                UserInfo.DefaultBillingAddressBookID = AddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);
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
                btnSubmit.Visible = false;
                throw new Exception("There is no data to import");
            }
        }
        public ImageSolutions.Address.AddressBook AddressExists(string userinfoid, string addresseee, string firstname, string lastname, string address1, string address2, string city, string state, string zip, string phone)
        {
            ImageSolutions.Address.AddressBook _ret = null;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT AddressBookID
FROM AddressBook (NOLOCK) a
WHERE a.UserInfoID = {0}
and ISNULL(REPLACE(a.FirstName,',', ' '),'') = ISNULL({1},'')
and ISNULL(REPLACE(a.LastName,',', ' '),'')  = ISNULL({2},'')
and ISNULL(REPLACE(a.AddressLine1,',', ' '),'')  = ISNULL({3},'')
and ISNULL(REPLACE(a.AddressLine2,',', ' '),'')  = ISNULL({4},'')
and ISNULL(REPLACE(a.City,',', ' '),'')  = ISNULL({5},'')
and ISNULL(REPLACE(a.State,',', ' '),'')  = ISNULL({6},'')
and ISNULL(REPLACE(a.PostalCode,',', ' '),'')  = ISNULL({7},'')
and ISNULL(REPLACE(a.PhoneNumber,',', ' '),'')  = ISNULL({8},'')
and ISNULL(REPLACE(a.AddressLabel,',', ' '),'') = ISNULL({9},'')
"
                    , Database.HandleQuote(userinfoid)
                    , Database.HandleQuote(firstname)
                    , Database.HandleQuote(lastname)
                    , Database.HandleQuote(address1)
                    , Database.HandleQuote(address2)
                    , Database.HandleQuote(city)
                    , Database.HandleQuote(state)
                    , Database.HandleQuote(zip)
                    , Database.HandleQuote(phone)
                    , Database.HandleQuote(addresseee)
                    );

                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    _ret = new ImageSolutions.Address.AddressBook(Convert.ToString(objRead["AddressBookID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
            return _ret;
        }
        public class UserUpload
        {
            public string Email { get; set; }
            public string Addressee { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string CountryCode { get; set; }
            public string PhoneNumber { get; set; }
            public string DefaultShipping { get; set; }
            public string DefaultBilling { get; set; }
        }
    }
}