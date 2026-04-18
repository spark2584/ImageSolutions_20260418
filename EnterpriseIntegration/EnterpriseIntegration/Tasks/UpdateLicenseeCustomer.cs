using ImageSolutions.Address;
using ImageSolutions.Customer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace EnterpriseIntegration.Tasks
{
    public class UpdateLicenseeCustomer
    {
        public bool Execute()
        {
            List<CustomerImport> CustomerImports = null;
            CustomerImportFilter CustomerImportFilter = null;

            //List<CustomerImport> CustomerImports = new List<CustomerImport>();
            //CustomerImportFilter CustomerImportFilter = new CustomerImportFilter();
            //CustomerImportFilter.IsProcessed = false;
            //CustomerImportFilter.IsEncrypted = false;
            //CustomerImports = CustomerImport.GetCustomerImports(CustomerImportFilter);

            try
            {
                CustomerImports = new List<CustomerImport>();
                CustomerImportFilter = new CustomerImportFilter();
                CustomerImportFilter.IsProcessed = false;
                CustomerImportFilter.IsStore = true;
                CustomerImportFilter.IsEncrypted = false;
                CustomerImports = CustomerImport.GetCustomerImports(CustomerImportFilter);

                foreach (CustomerImport _CustomerImport in CustomerImports)
                {
                    try
                    {
                        string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _CustomerImport.FilePath); //Get path to saved file
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
                            case ".csv":
                                ProcessCSV(strFullPath);
                                break;
                        }

                        string strArchiveFolder = string.Format(@"{0}\Archive", Path.GetDirectoryName(strFullPath));
                        if (!Directory.Exists(strArchiveFolder))
                        {
                            Directory.CreateDirectory(strArchiveFolder);
                        }

                        string strArchivePath = string.Format(@"{0}\{1}", strArchiveFolder, Path.GetFileName(strFullPath));
                        File.Move(strFullPath, strArchivePath);

                        _CustomerImport.IsProcessed = true;
                        _CustomerImport.ErrorMessage = String.Empty;
                        _CustomerImport.Update();
                    }
                    catch (Exception ex)
                    {
                        _CustomerImport.ErrorMessage = ex.Message;
                        _CustomerImport.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CustomerImports = null;
                CustomerImportFilter = null;
            }
            return true;
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

                            if (!objColumns.Contains("group_branch")) throw new Exception("[group_branch] is missing from the report, excel contact administrator");

                            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                            {
                                Console.WriteLine(i);

                                string strGroupBranch = Convert.ToString(objData.Tables[0].Rows[i]["group_branch"]).Trim();
                                string strPeopleSoftID = Convert.ToString(objData.Tables[0].Rows[i]["peoplesoft_id"]).Trim();
                                string strArea = Convert.ToString(objData.Tables[0].Rows[i]["area"]).Trim();
                                string strRegion = Convert.ToString(objData.Tables[0].Rows[i]["region"]).Trim();
                                string strPhysicalAddress = Convert.ToString(objData.Tables[0].Rows[i]["physical_address"]).Trim();
                                string strCity = Convert.ToString(objData.Tables[0].Rows[i]["city"]).Trim();
                                string strState = Convert.ToString(objData.Tables[0].Rows[i]["state"]).Trim();
                                string strZipCode = Convert.ToString(objData.Tables[0].Rows[i]["zip_code"]).Trim();
                                string strShippingAddress = Convert.ToString(objData.Tables[0].Rows[i]["shipping_address"]).Trim();
                                string strCityS = Convert.ToString(objData.Tables[0].Rows[i]["city_s"]).Trim();
                                string strStateS = Convert.ToString(objData.Tables[0].Rows[i]["state_s"]).Trim();
                                string strZipCodeS = Convert.ToString(objData.Tables[0].Rows[i]["zip_code_s"]).Trim();
                                string strPhone = Convert.ToString(objData.Tables[0].Rows[i]["phone"]).Trim();
                                string strBrandName = Convert.ToString(objData.Tables[0].Rows[i]["brand_name"]).Trim();
                                string strBranchType = Convert.ToString(objData.Tables[0].Rows[i]["branch_type"]).Trim();
                                string strCountryCode = Convert.ToString(objData.Tables[0].Rows[i]["country_code"]).Trim();

                                //Customer Customer = new Customer(strCustomerID);
                                //CustomerFilter CustomerFilter = new CustomerFilter();
                                //CustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                                //CustomerFilter.StoreNumber.SearchString = strGroupBranch;
                                //Customer = Customer.GetCustomer(CustomerFilter);

                                string strCustomerID = GetCustomerID(strGroupBranch);

                                Customer Customer = string.IsNullOrEmpty(strCustomerID) ? new Customer() : new Customer(strCustomerID);

                                if (string.IsNullOrEmpty(strCustomerID))
                                {
                                    Customer = new Customer();
                                    Customer.ParentID = ConfigurationManager.AppSettings["LicenseeCustomerParentID"];  //Enterprise Holdings Licensee
                                    Customer.StoreNumber = strGroupBranch;

                                    string strEmail = string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));

                                    int counter = 0;
                                    while (!IsValidEmail(strEmail, string.Empty))
                                    {
                                        counter++;
                                        strEmail = string.Format("{0}.{1}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""), counter);
                                    }

                                    Customer.Email = strEmail; //Create email using Group Branch
                                    Customer.CompanyName = String.Format("Enterprise Holdings Licensee #{0}", strGroupBranch); //CompanyName based on Group Branch
                                    Customer.ExternalID = strPeopleSoftID;
                                    Customer.Area = strArea;
                                    Customer.Region = strRegion;

                                    Address PhysicalAddress = new Address();
                                    PhysicalAddress.Address1 = strPhysicalAddress;
                                    PhysicalAddress.City = strCity;
                                    PhysicalAddress.State = strState;
                                    PhysicalAddress.PostalCode = strZipCode;
                                    PhysicalAddress.Create();
                                    Customer.PhysicalAddressID = PhysicalAddress.AddressID;

                                    Address ShippingAddress = new Address();
                                    ShippingAddress.Address1 = strShippingAddress;
                                    ShippingAddress.City = strCityS;
                                    ShippingAddress.State = strStateS;
                                    ShippingAddress.PostalCode = strZipCodeS;
                                    ShippingAddress.Create();
                                    Customer.ShippingAddressID = ShippingAddress.AddressID;

                                    Customer.PhoneNumber = strPhone;
                                    Customer.BrandName = strBrandName;
                                    Customer.BranchType = strBranchType;
                                    Customer.CountryCode = strCountryCode;
                                    Customer.Password = strCountryCode + "00000"; //Password will be the CountryCode

                                    Customer.IsUpdated = true;
                                    Customer.Create();
                                }
                                else
                                {
                                    Address PhysicalAddress = new Address(Customer.PhysicalAddressID);
                                    Address ShippingAddress = new Address(Customer.ShippingAddressID);

                                    if (Customer.StoreNumber != strGroupBranch ||
                                        Customer.ExternalID != strPeopleSoftID ||
                                        Customer.Area != strArea ||
                                        Customer.Region != strRegion ||
                                        PhysicalAddress.Address1 != strPhysicalAddress ||
                                        PhysicalAddress.City != strCity ||
                                        PhysicalAddress.State != strState ||
                                        PhysicalAddress.PostalCode != strZipCode ||
                                        ShippingAddress.Address1 != strShippingAddress ||
                                        ShippingAddress.City != strCityS ||
                                        ShippingAddress.State != strStateS ||
                                        ShippingAddress.PostalCode != strZipCodeS ||
                                        Customer.PhoneNumber != strPhone ||
                                        Customer.BrandName != strBrandName ||
                                        Customer.BranchType != strBranchType ||
                                        Customer.CountryCode != strCountryCode
                                    )
                                    {
                                        if (Customer.StoreNumber != strGroupBranch)
                                        {
                                            string strTest = "test";
                                        }
                                        Customer.StoreNumber = strGroupBranch;

                                        string strEmail = string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));

                                        int counter = 0;
                                        while (!IsValidEmail(strEmail, Customer.CustomerID))
                                        {
                                            counter++;
                                            strEmail = string.Format("{0}.{1}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""), counter);
                                        }

                                        Customer.Email = strEmail; //string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));
                                        Customer.CompanyName = String.Format("Enterprise Holdings Licensee #{0}", strGroupBranch); //Store # based on Group Branch
                                        Customer.ExternalID = strPeopleSoftID;
                                        Customer.Area = strArea;
                                        Customer.Region = strRegion;

                                        PhysicalAddress.Address1 = strPhysicalAddress;
                                        PhysicalAddress.City = strCity;
                                        PhysicalAddress.State = strState;
                                        PhysicalAddress.PostalCode = strZipCode;
                                        PhysicalAddress.Update();

                                        ShippingAddress.Address1 = strShippingAddress;
                                        ShippingAddress.City = strCityS;
                                        ShippingAddress.State = strStateS;
                                        ShippingAddress.PostalCode = strZipCodeS;
                                        ShippingAddress.Update();

                                        Customer.PhoneNumber = strPhone;
                                        Customer.BrandName = strBrandName;
                                        Customer.BranchType = strBranchType;
                                        Customer.CountryCode = strCountryCode;
                                        Customer.Password = strCountryCode + "00000"; //Password will be the CountryCode

                                        Customer.IsUpdated = true;
                                        Customer.Update();
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public string GetCustomerID(string storenumber)
        {
            string strReturn = string.Empty;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"SELECT CustomerID FROM Customer WHERE ParentID = {0} and StoreNumber = {1} "
                    , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["LicenseeCustomerParentID"]))
                    , Database.HandleQuote(storenumber));
                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    strReturn = Convert.ToString(objRead["CustomerID"]);
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
            return strReturn;
        }



        public bool IsValidEmail(string email, string customerid)
        {
            bool strReturn = true;
            try
            {
                List<Customer> Customers = new List<Customer>();
                CustomerFilter CustomerFilter = new CustomerFilter();
                CustomerFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                CustomerFilter.Email.SearchString = email;
                Customers = Customer.GetCustomers(CustomerFilter);

                foreach (Customer _Customer in Customers)
                {
                    if (string.IsNullOrEmpty(customerid) ||
                        (_Customer.CustomerID != customerid)
                    )
                        strReturn = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return strReturn;
        }

        public void ProcessCSV(string filepath)
        {
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

                            if (!HeaderFields.Contains("po_number")) throw new Exception("[po_number] is missing from the report, excel contact administrator");
                        }
                        else
                        {
                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(',');

                            string strGroupBranch = Convert.ToString(ColumnValues[HeaderFields.IndexOf("group_branch")]).Trim();
                            if (string.IsNullOrEmpty(strGroupBranch)) break;

                            //Save Data
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
    }
}
