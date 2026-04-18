using ImageSolutions.Enterprise;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;


namespace ScheduledTask.Task.Enterprise
{
    public class UpdateLicenseeCustomer
    {
        public bool Execute()
        {
            List<EnterpriseCustomerImport> EnterpriseCustomerImports = null;
            EnterpriseCustomerImportFilter EnterpriseCustomerImportFilter = null;

            try
            {
                EnterpriseCustomerImports = new List<EnterpriseCustomerImport>();
                EnterpriseCustomerImportFilter = new EnterpriseCustomerImportFilter();
                EnterpriseCustomerImportFilter.IsProcessed = false;
                EnterpriseCustomerImportFilter.IsStore = true;
                EnterpriseCustomerImportFilter.IsEncrypted = false;

                //EnterpriseCustomerImportFilter.EnterpriseCustomerImportID = new Database.Filter.StringSearch.SearchFilter();
                //EnterpriseCustomerImportFilter.EnterpriseCustomerImportID.SearchString = "3052";

                EnterpriseCustomerImports = EnterpriseCustomerImport.GetEnterpriseCustomerImports(EnterpriseCustomerImportFilter);

                foreach (EnterpriseCustomerImport _EnterpriseCustomerImport in EnterpriseCustomerImports)
                {
                    if (string.IsNullOrEmpty(_EnterpriseCustomerImport.ErrorMessage))
                    {
                        try
                        {
                            string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _EnterpriseCustomerImport.FilePath); //Get path to saved file
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

                            _EnterpriseCustomerImport.IsProcessed = true;
                            _EnterpriseCustomerImport.ErrorMessage = String.Empty;
                            _EnterpriseCustomerImport.Update();
                        }
                        catch (Exception ex)
                        {
                            _EnterpriseCustomerImport.ErrorMessage = ex.Message;
                            _EnterpriseCustomerImport.Update();
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
                EnterpriseCustomerImports = null;
                EnterpriseCustomerImportFilter = null;
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
                            strSheetName = objSchema.Rows[i]["TABLE_NAME"].ToString() + "A1:X65535";
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

                                string strPSGroup = !objData.Tables[0].Columns.Contains("ps_group") ? String.Empty : Convert.ToString(objData.Tables[0].Rows[i]["ps_group"]).Trim();
                                string strGroupBranch = Convert.ToString(objData.Tables[0].Rows[i]["group_branch"]).Trim();
                                string strPeopleSoftID = Convert.ToString(objData.Tables[0].Rows[i]["peoplesoft_id"]).Trim();
                                string strArea = Convert.ToString(objData.Tables[0].Rows[i]["area"]).Trim();
                                string strRegion = Convert.ToString(objData.Tables[0].Rows[i]["region"]).Trim();
                                string strPhysicalAddress = Convert.ToString(objData.Tables[0].Rows[i]["physical_address"]).Trim();
                                string strPhysicalAddressLine2 = Convert.ToString(objData.Tables[0].Rows[i]["physical_address_line2"]).Trim();
                                string strCity = Convert.ToString(objData.Tables[0].Rows[i]["city"]).Trim();
                                string strState = Convert.ToString(objData.Tables[0].Rows[i]["state"]).Trim();
                                string strZipCode = Convert.ToString(objData.Tables[0].Rows[i]["zip_code"]).Trim();
                                string strShippingAddress = Convert.ToString(objData.Tables[0].Rows[i]["shipping_address"]).Trim();
                                string strShippingAddressLine2 = Convert.ToString(objData.Tables[0].Rows[i]["shipping_address_line2"]).Trim();
                                string strCityS = Convert.ToString(objData.Tables[0].Rows[i]["city_s"]).Trim();
                                string strStateS = Convert.ToString(objData.Tables[0].Rows[i]["state_s"]).Trim();
                                string strZipCodeS = Convert.ToString(objData.Tables[0].Rows[i]["zip_code_s"]).Trim();
                                string strPhone = Convert.ToString(objData.Tables[0].Rows[i]["phone"]).Trim();
                                string strBrandName = Convert.ToString(objData.Tables[0].Rows[i]["brand_name"]).Trim();
                                string strBranchType = Convert.ToString(objData.Tables[0].Rows[i]["branch_type"]).Trim();
                                string strCountryCode = Convert.ToString(objData.Tables[0].Rows[i]["country_code"]).Trim();
                                string strAirport = Convert.ToString(objData.Tables[0].Rows[i]["airport"]).Trim();
                                
                                string strBranchAdminLgcyID = !objData.Tables[0].Columns.Contains("branch_admin_lgcy_id") ? String.Empty : Convert.ToString(objData.Tables[0].Rows[i]["branch_admin_lgcy_id"]).Trim();
                                string strBranchAdminPSID = !objData.Tables[0].Columns.Contains("branch_admin_ps_id") ? String.Empty : Convert.ToString(objData.Tables[0].Rows[i]["branch_admin_ps_id"]).Trim();

                                string strRegionalized = !objData.Tables[0].Columns.Contains("regionalized") ? String.Empty : Convert.ToString(objData.Tables[0].Rows[i]["regionalized"]).Trim();
                                string strAirportCode = !objData.Tables[0].Columns.Contains("airport_code") ? String.Empty : Convert.ToString(objData.Tables[0].Rows[i]["airport_code"]).Trim();

                                string strCustomerID = GetCustomerID(strGroupBranch);

                                EnterpriseCustomer Customer = string.IsNullOrEmpty(strCustomerID) ? new EnterpriseCustomer() : new EnterpriseCustomer(strCustomerID);

                                if (string.IsNullOrEmpty(strCustomerID))
                                {
                                    Customer = new EnterpriseCustomer();
                                    Customer.ParentID = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];  //Enterprise Holdings Licensee
                                    Customer.StoreNumber = strGroupBranch;

                                    string strEmail = string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));

                                    int counter = 0;
                                    while (!IsValidEmail(strEmail, string.Empty))
                                    {
                                        counter++;
                                        strEmail = string.Format("{0}.{1}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""), counter);
                                    }

                                    Customer.Email = strEmail; //Create email using Group Branch
                                    Customer.CompanyName = String.Format("Enterprise Mobility Licensee #{0}", strGroupBranch); //CompanyName based on Group Branch
                                    Customer.ExternalID = strPeopleSoftID;
                                    Customer.Area = strArea;
                                    Customer.Region = strRegion;

                                    EnterpriseAddress PhysicalAddress = new EnterpriseAddress();
                                    PhysicalAddress.Address1 = strPhysicalAddress;
                                    PhysicalAddress.Address2 = strPhysicalAddressLine2;
                                    PhysicalAddress.City = strCity;
                                    PhysicalAddress.State = strState;
                                    PhysicalAddress.PostalCode = strZipCode;
                                    PhysicalAddress.Create();
                                    Customer.PhysicalEnterpriseAddressID = PhysicalAddress.EnterpriseAddressID;

                                    EnterpriseAddress ShippingAddress = new EnterpriseAddress();
                                    ShippingAddress.Address1 = strShippingAddress;
                                    ShippingAddress.Address2 = strShippingAddressLine2;
                                    ShippingAddress.City = strCityS;
                                    ShippingAddress.State = strStateS;
                                    ShippingAddress.PostalCode = strZipCodeS;
                                    ShippingAddress.Create();
                                    Customer.ShippingEnterpriseAddressID = ShippingAddress.EnterpriseAddressID;

                                    Customer.PhoneNumber = strPhone;
                                    Customer.BrandName = strBrandName;
                                    Customer.BranchType = strBranchType;
                                    Customer.CountryCode = strCountryCode;
                                    Customer.Password = strCountryCode + "00000"; //Password will be the CountryCode

                                    Customer.IsAirport = strAirport == "Yes";
                                    Customer.BranchAdminLgcyID = strBranchAdminLgcyID;
                                    Customer.BranchAdminPSID = strBranchAdminPSID;
                                    Customer.PSGroup = strPSGroup;

                                    Customer.IsRegionalized = Convert.ToString(strRegionalized) == "Yes";
                                    Customer.AirportCode = strAirportCode;

                                    Customer.IsUpdated = true;
                                    Customer.Create();

                                    if (Customer.IsAirport)
                                    {
                                        UpdateUserWebsiteLocationCode(Customer.StoreNumber, Customer.AirportCode);
                                    }
                                }
                                else
                                {
                                    EnterpriseAddress PhysicalAddress = new EnterpriseAddress(Customer.PhysicalEnterpriseAddressID);
                                    EnterpriseAddress ShippingAddress = new EnterpriseAddress(Customer.ShippingEnterpriseAddressID);

                                    if (Customer.StoreNumber != strGroupBranch ||
                                        Customer.ExternalID != strPeopleSoftID ||
                                        Customer.Area != strArea ||
                                        Customer.Region != strRegion ||
                                        PhysicalAddress.Address1 != strPhysicalAddress ||
                                        PhysicalAddress.Address2 != strPhysicalAddressLine2 ||
                                        PhysicalAddress.City != strCity ||
                                        PhysicalAddress.State != strState ||
                                        PhysicalAddress.PostalCode != strZipCode ||
                                        ShippingAddress.Address1 != strShippingAddress ||
                                        ShippingAddress.Address2 != strShippingAddressLine2 ||
                                        ShippingAddress.City != strCityS ||
                                        ShippingAddress.State != strStateS ||
                                        ShippingAddress.PostalCode != strZipCodeS ||
                                        Customer.PhoneNumber != strPhone ||
                                        Customer.BrandName != strBrandName ||
                                        Customer.BranchType != strBranchType ||
                                        Customer.CountryCode != strCountryCode ||
                                        Customer.BranchAdminLgcyID != strBranchAdminLgcyID ||
                                        Customer.BranchAdminPSID != strBranchAdminPSID ||
                                        Customer.PSGroup != strPSGroup ||
                                        Customer.IsAirport != (strAirport == "Yes") ||
                                        Customer.IsRegionalized != (strRegionalized == "Yes") ||
                                        Customer.AirportCode != strAirportCode
                                    )
                                    {
                                        Customer.StoreNumber = strGroupBranch;

                                        string strEmail = string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));

                                        int counter = 0;
                                        while (!IsValidEmail(strEmail, Customer.EnterpriseCustomerID))
                                        {
                                            counter++;
                                            strEmail = string.Format("{0}.{1}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""), counter);
                                        }

                                        Customer.Email = strEmail; //string.Format("{0}@ehi.com", Regex.Replace(strGroupBranch, @"[^0-9a-zA-Z\._]", ""));
                                        Customer.CompanyName = String.Format("Enterprise Mobility Licensee #{0}", strGroupBranch); //Store # based on Group Branch
                                        Customer.ExternalID = strPeopleSoftID;
                                        Customer.Area = strArea;
                                        Customer.Region = strRegion;

                                        PhysicalAddress.Address1 = strPhysicalAddress;
                                        PhysicalAddress.Address2 = strPhysicalAddressLine2;
                                        PhysicalAddress.City = strCity;
                                        PhysicalAddress.State = strState;
                                        PhysicalAddress.PostalCode = strZipCode;
                                        PhysicalAddress.Update();

                                        ShippingAddress.Address1 = strShippingAddress;
                                        ShippingAddress.Address2 = strShippingAddressLine2;
                                        ShippingAddress.City = strCityS;
                                        ShippingAddress.State = strStateS;
                                        ShippingAddress.PostalCode = strZipCodeS;
                                        ShippingAddress.Update();

                                        Customer.PhoneNumber = strPhone;
                                        Customer.BrandName = strBrandName;
                                        Customer.BranchType = strBranchType;
                                        Customer.CountryCode = strCountryCode;
                                        Customer.Password = strCountryCode + "00000"; //Password will be the CountryCode

                                        Customer.IsAirport = strAirport == "Yes";
                                        Customer.BranchAdminLgcyID = strBranchAdminLgcyID;
                                        Customer.BranchAdminPSID = strBranchAdminPSID;
                                        Customer.PSGroup = strPSGroup;

                                        Customer.IsRegionalized = Convert.ToString(strRegionalized) == "Yes";
                                        Customer.AirportCode = strAirportCode;

                                        Customer.IsUpdated = true;
                                        Customer.ErrorMessage = String.Empty;
                                        Customer.Update();

                                        if (Customer.IsAirport)
                                        {
                                            UpdateUserWebsiteLocationCode(Customer.StoreNumber, Customer.AirportCode);
                                        }
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
                strSQL = string.Format(@"SELECT EnterpriseCustomerID FROM EnterpriseCustomer (NOLOCK) WHERE ParentID = {0} and StoreNumber = {1} "
                    , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"]))
                    , Database.HandleQuote(storenumber));
                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    strReturn = Convert.ToString(objRead["EnterpriseCustomerID"]);
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
                List<EnterpriseCustomer> Customers = new List<EnterpriseCustomer>();
                EnterpriseCustomerFilter CustomerFilter = new EnterpriseCustomerFilter();
                CustomerFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                CustomerFilter.Email.SearchString = email;
                Customers = EnterpriseCustomer.GetEnterpriseCustomers(CustomerFilter);

                foreach (EnterpriseCustomer _Customer in Customers)
                {
                    if (string.IsNullOrEmpty(customerid) ||
                        (_Customer.EnterpriseCustomerID != customerid)
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

        public string UpdateUserWebsiteLocationCode(string storenumber, string locationcode)
        {
            string objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE uw SET LocationCode = {1}, IsUpdated = 1
--SELECT uw.LocationCode
FROM EnterpriseCustomer (NOLOCK) ec
Inner Join UserInfo (NOLOCK) ui on ui.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = ui.UserInfoID
WHERE ec.StoreNumber = {0}
and ISNULL(uw.LocationCode,'') != {1}
"
                        , Database.HandleQuote(storenumber)
                        , Database.HandleQuote(locationcode)
                    );

                Database.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }
    }
}
