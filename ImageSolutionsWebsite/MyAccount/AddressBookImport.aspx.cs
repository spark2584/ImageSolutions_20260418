using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.MyAccount
{
    public partial class AddressBookImport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuAddressBook.PostedFile.ContentLength > 0)
                {
                    string strPath = Server.MapPath("\\Import\\AddressBook\\" + DateTime.UtcNow.ToString("yyyyMM") + "\\");

                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    string strFileNamExtension = System.IO.Path.GetExtension(fuAddressBook.PostedFile.FileName);
                    string strFileName = System.IO.Path.GetFileNameWithoutExtension(fuAddressBook.PostedFile.FileName) + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + strFileNamExtension;
                    string strFilePath = string.Format("{0}{1}", strPath, strFileName);
                    fuAddressBook.PostedFile.SaveAs(strFilePath);

                    UploadFile(strFilePath);
                }
                else
                {
                    gvAddressBookUpload.DataSource = null;
                    gvAddressBookUpload.DataBind();

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
                    gvAddressBookUpload.DataSource = null;
                    gvAddressBookUpload.DataBind();

                    btnSubmit.Visible = false;
                    break;
            }
        }
        public void UploadCSV(string filepath)
        {

            List<ImageSolutions.Address.AddressBook> AddressBooks = GetCSV(filepath);
            if (AddressBooks != null && AddressBooks.Count > 0)
            {
                gvAddressBookUpload.DataSource = AddressBooks;
                gvAddressBookUpload.DataBind();

                hfFilePath.Value = Convert.ToString(filepath);
                btnSubmit.Visible = true;
            }
            else
            {
                btnSubmit.Visible = false;
                throw new Exception("There is no data to import");
            }

        }
        public List<ImageSolutions.Address.AddressBook> GetCSV(string filepath)
        {
            List<ImageSolutions.Address.AddressBook> AddressBooks = null;

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

                            if (!HeaderFields.Contains("First Name")) throw new Exception("[First Name] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Last Name")) throw new Exception("[Last Name] is missing from the report, excel contact administrator");
                            if (!HeaderFields.Contains("Address Line 1")) throw new Exception("[Address Line 1] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("Address Line 2")) throw new Exception("[Address Line 2] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("City")) throw new Exception("[City] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("State")) throw new Exception("[State] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("Postal Code")) throw new Exception("[Postal Code] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("Country")) throw new Exception("[Country] is missing from the excel, please contact administrator");
                            if (!HeaderFields.Contains("Phone")) throw new Exception("[Phone] is missing from the excel, please contact administrator");

                            AddressBooks = new List<ImageSolutions.Address.AddressBook>();
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("First Name")]).Trim();
                            string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Last Name")]);
                            string strAddressLine1 = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Address Line 1")]);
                            string strAddressLine2 = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Address Line 2")]);
                            string strCity = Convert.ToString(ColumnValues[HeaderFields.IndexOf("City")]);
                            string strState = Convert.ToString(ColumnValues[HeaderFields.IndexOf("State")]);
                            string strPostalCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Postal Code")]);
                            string strCountry = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Country")]);
                            string strPhone = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Phone")]);

                            ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                            ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                            AddressCountryCodeFilter.Alpha2Code = new Database.Filter.StringSearch.SearchFilter();
                            AddressCountryCodeFilter.Alpha2Code.SearchString = strCountry;
                            AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);
                            
                            if (AddressCountryCode == null)
                            {
                                if(strCountry.Length == 3)
                                {
                                    AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                                    AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                                    AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                                    AddressCountryCodeFilter.Alpha3Code.SearchString = strCountry;
                                    AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                                    if (AddressCountryCode != null)
                                    {
                                        strCountry = AddressCountryCode.Alpha2Code;
                                    }
                                    else
                                    {
                                        throw new Exception(string.Format(@"Invalid Country: {0} - Please enter valid country code", strCountry));
                                    }
                                }
                                else
                                {
                                    throw new Exception(string.Format(@"Invalid Country: {0} - Please enter valid country code", strCountry));
                                }
                            }


                            if (!AddressExists(CurrentUser.UserInfoID
                                , strFirstName, strLastName, strAddressLine1, strAddressLine2, strCity, strState, strPostalCode, strPhone))
                            {
                                ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook();
                                AddressBook.AddressLabel = String.Format("{0} {1}", strFirstName, strLastName);
                                AddressBook.FirstName = strFirstName;
                                AddressBook.LastName = strLastName;
                                AddressBook.AddressLine1 = strAddressLine1;
                                AddressBook.AddressLine2 = strAddressLine2;
                                AddressBook.City = strCity;
                                AddressBook.State = strState;
                                AddressBook.PostalCode = strPostalCode;
                                AddressBook.CountryCode = strCountry;
                                AddressBook.PhoneNumber = strPhone;
                                AddressBook.UserInfoID = CurrentUser.UserInfoID;
                                AddressBook.CreatedBy = CurrentUser.UserInfoID;
                                AddressBooks.Add(AddressBook);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return AddressBooks;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(hfFilePath.Value)))
            {
                try
                {

                    List<ImageSolutions.Address.AddressBook> AddressBooks = GetCSV(Convert.ToString(hfFilePath.Value));

                    foreach (ImageSolutions.Address.AddressBook _AddressBook in AddressBooks)
                    {
                        _AddressBook.Create();
                    }

                    Response.Redirect(string.Format("/MyAccount/AddressBook.aspx"));
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
                            gvAddressBookUpload.DataSource = objData;
                            gvAddressBookUpload.DataBind();

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

                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                            {
                                Console.WriteLine(i);
                                
                                //Process
                            }

                            objTran.Commit();
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
                objTran.Rollback();
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

        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            string fileName = "AddressBookTemplate.csv";
            string filePath = Server.MapPath(string.Format("\\Import\\Template\\{0}", fileName));
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.WriteFile(filePath);
            Response.Flush();
            Response.End();
        }
        public bool AddressExists(string userinfoid, string firstname, string lastname, string address1, string address2, string city, string state, string zip, string phone)
        {
            bool _ret = false;
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
                    );

                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    _ret = true;
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
    }
}