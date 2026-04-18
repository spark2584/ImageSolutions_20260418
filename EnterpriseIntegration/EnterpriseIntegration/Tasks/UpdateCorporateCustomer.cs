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
using System.Linq;
using System.Text.RegularExpressions;

namespace EnterpriseIntegration.Tasks
{
    public class UpdateCorporateCustomer
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
                CustomerImportFilter.IsStore = false;
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
                            case ".tsv":
                                ProcessTSV(strFullPath);
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

                                string strEmail = Convert.ToString(objData.Tables[0].Rows[i]["Email"]).Trim();

                                string strEID = Convert.ToString(objData.Tables[0].Rows[i]["EID"]).Trim();
                                string strLocation = Convert.ToString(objData.Tables[0].Rows[i]["LOCATION "]).Trim();
                                string strJob = Convert.ToString(objData.Tables[0].Rows[i]["JOB "]).Trim();
                                string strHireDate = Convert.ToString(objData.Tables[0].Rows[i]["HIRE DATE "]).Trim();
                                string strActionDate = Convert.ToString(objData.Tables[0].Rows[i]["ACTION DATE "]).Trim();
                                string strFullPartTime = Convert.ToString(objData.Tables[0].Rows[i]["FULL_PART_TIME"]).Trim();
                                string strFirstNameMiddle = Convert.ToString(objData.Tables[0].Rows[i]["FIRST NAME + MIDDLE"]).Trim();
                                string strLastName = Convert.ToString(objData.Tables[0].Rows[i]["LAST NAME "]).Trim();
                                string strTitle = Convert.ToString(objData.Tables[0].Rows[i]["Title"]).Trim();
                                string strRegTemp = Convert.ToString(objData.Tables[0].Rows[i]["REG_TEMP"]).Trim();
                                string strBrand = Convert.ToString(objData.Tables[0].Rows[i]["BRAND "]).Trim();
                                string strGR_BR = Convert.ToString(objData.Tables[0].Rows[i]["GR_BR"]).Trim();

                                string strCustomerID = GetCustomerID(strEmail);

                                Customer Customer = string.IsNullOrEmpty(strCustomerID) ? new Customer() : new Customer(strCustomerID);

                                if (string.IsNullOrEmpty(strCustomerID))
                                {
                                    Customer = new Customer();
                                    Customer.Email = strEmail; //Create email using Group Branch                                    
                                    Customer.ParentID = ConfigurationManager.AppSettings["CorporateCustomerParentID"];  //Enterprise Holdings Corporate
                                    Customer.FirstName = strFirstNameMiddle;
                                    Customer.LastName = strLastName;
                                    Customer.StoreNumber = strLocation;
                                    Customer.ExternalID = strEID;
                                    Customer.HireDate = Convert.ToDateTime(strHireDate);
                                    Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                    Customer.Job = strJob;
                                    Customer.Title = strTitle;

                                    Customer.CompanyName = String.Format("Enterprise Holdings Corporate");

                                    Customer.IsIndividual = true;
                                    Customer.Password = "Im@geSolutions$1";

                                    Customer.IsUpdated = true;
                                    Customer.Create();

                                    List<ImageSolutions.Entity.EntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                    ImageSolutions.Entity.EntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                    EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                    EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);

                                    List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EntityGroupID).Distinct().ToList();
                                    foreach(string _entitygroupid in EntityGroupIDs)
                                    {
                                        ImageSolutions.Customer.CustomerEntityGroup CustomerEntityGroup = new CustomerEntityGroup();
                                        CustomerEntityGroup.CustomerID = Customer.CustomerID;
                                        CustomerEntityGroup.EntityGroupID = _entitygroupid;
                                        CustomerEntityGroup.Create();
                                    }
                                }
                                else
                                {
                                    if (Customer.StoreNumber != strLocation ||
                                        Customer.FirstName != strFirstNameMiddle ||
                                        Customer.LastName != strLastName ||
                                        Customer.ExternalID != strEID ||
                                        Customer.ActionDate != Convert.ToDateTime(strActionDate) ||
                                        Customer.Job != strJob ||
                                        Customer.Title != strTitle
                                    )
                                    {
                                        Customer.FirstName = strFirstNameMiddle;
                                        Customer.LastName = strLastName;
                                        Customer.StoreNumber = strLocation;
                                        Customer.ExternalID = strEID;
                                        Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                        Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                        Customer.Job = strJob;
                                        Customer.Title = strTitle;

                                        Customer.IsUpdated = true;
                                        Customer.Update();

                                        List<ImageSolutions.Entity.EntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                        ImageSolutions.Entity.EntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                        EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                        EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);

                                        List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EntityGroupID).Distinct().ToList();
                                        foreach (string _entitygroupid in EntityGroupIDs)
                                        {                                           
                                            ImageSolutions.Customer.CustomerEntityGroup CustomerEntityGroup = new CustomerEntityGroup();
                                            ImageSolutions.Customer.CustomerEntityGroupFilter CustomerEntityGroupFilter = new CustomerEntityGroupFilter();
                                            CustomerEntityGroupFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.CustomerID.SearchString = Customer.CustomerID;
                                            CustomerEntityGroupFilter.EntityGroupID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.EntityGroupID.SearchString = _entitygroupid;
                                            CustomerEntityGroup = CustomerEntityGroup.GetCustomerEntityGroup(CustomerEntityGroupFilter);

                                            if(CustomerEntityGroup == null)
                                            {
                                                CustomerEntityGroup = new CustomerEntityGroup();
                                                CustomerEntityGroup.CustomerID = Customer.CustomerID;
                                                CustomerEntityGroup.EntityGroupID = _entitygroupid;
                                                CustomerEntityGroup.Create();
                                            }
                                        }

                                        List<ImageSolutions.Customer.CustomerEntityGroup> ExistCustomerEntityGroups = new List<CustomerEntityGroup>();
                                        ImageSolutions.Customer.CustomerEntityGroupFilter ExistCustomerEntityGroupFilter = new CustomerEntityGroupFilter();
                                        ExistCustomerEntityGroupFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                                        ExistCustomerEntityGroupFilter.CustomerID.SearchString = Customer.CustomerID;
                                        ExistCustomerEntityGroups = CustomerEntityGroup.GetCustomerEntityGroups(ExistCustomerEntityGroupFilter);

                                        foreach(CustomerEntityGroup _CustomerEntityGroup in ExistCustomerEntityGroups)
                                        {
                                            if(!EntityGroupIDs.Contains(_CustomerEntityGroup.EntityGroupID))
                                            {
                                                _CustomerEntityGroup.Inactive = true;
                                                _CustomerEntityGroup.IsUpdated = true;
                                                _CustomerEntityGroup.Update();
                                            }
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

        public string GetCustomerID(string email)
        {
            string strReturn = string.Empty;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"SELECT CustomerID FROM Customer WHERE Email = {0}", Database.HandleQuote(email));
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
        public void ProcessTSV(string filepath)
        {
            try
            {
                using (StreamReader _StreamReader = new StreamReader(filepath))
                {
                    int counter = 0;
                    String[] HeaderColumnValues = null;
                    List<string> HeaderFields = new List<string>();
                    string strCurrentLine = string.Empty;
                    string strSep = "\t";

                    while (!string.IsNullOrEmpty(strCurrentLine = _StreamReader.ReadLine()))
                    {
                        counter++;
                        if (counter == 1)
                        {
                            HeaderColumnValues = strCurrentLine.Split(strSep.ToArray());
                            foreach (string _value in HeaderColumnValues)
                            {
                                HeaderFields.Add(_value.Trim());
                            }

                            if (!HeaderFields.Contains("EID")) throw new Exception("[EID] is missing, please contact administrator");
                            if (!HeaderFields.Contains("LOCATION")) throw new Exception("[LOCATION] is missing, please contact administrator");
                            if (!HeaderFields.Contains("JOBCODE")) throw new Exception("[JOBCODE] is missing, please contact administrator");
                        }
                        else
                        {
                            Console.WriteLine(counter);

                            String[] ColumnValues = null;
                            ColumnValues = strCurrentLine.Split(strSep.ToArray());

                            if (ColumnValues.Length > 3)  //Do not include last line
                            {
                                string strEmail = string.Format("{0}{1}"
                                    , Convert.ToString(ColumnValues[HeaderFields.IndexOf("EID")]).Trim()
                                    , "@ehi.com");
                                string strEID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("EID")]).Trim();
                                string strLocation = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LOCATION")]).Trim();
                                string strJobCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("JOBCODE")]).Trim();
                                string strHireDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HIRE DATE")]).Trim();
                                string strActionDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ACTION DATE")]).Trim();
                                string strStatus = Convert.ToString(ColumnValues[HeaderFields.IndexOf("STATUS")]).Trim();
                                string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("FIRST")]).Trim();
                                string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LAST")]).Trim();
                                string strTitle = Convert.ToString(ColumnValues[HeaderFields.IndexOf("TITLE")]).Trim();
                                string strRegTemp = Convert.ToString(ColumnValues[HeaderFields.IndexOf("REG/TEMP")]).Trim();
                                string strBrand = Convert.ToString(ColumnValues[HeaderFields.IndexOf("BRAND")]).Trim();
                                string strGR_BR = Convert.ToString(ColumnValues[HeaderFields.IndexOf("GP/BR")]).Trim();

                                //Save Data
                                string strCustomerID = GetCustomerID(strEmail);

                                Customer Customer = string.IsNullOrEmpty(strCustomerID) ? new Customer() : new Customer(strCustomerID);

                                Customer StoreCustomer = new Customer();
                                CustomerFilter StoreCustomerFilter = new CustomerFilter();
                                StoreCustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                                StoreCustomerFilter.ExternalID.SearchString = strLocation;
                                StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["LicenseeCustomerParentID"];
                                StoreCustomer = Customer.GetCustomer(StoreCustomerFilter);

                                if (string.IsNullOrEmpty(strCustomerID))
                                {
                                    Customer = new Customer();
                                    Customer.Email = strEmail; //Create email using Group Branch                                    
                                    Customer.ParentID = ConfigurationManager.AppSettings["CorporateCustomerParentID"];  //Enterprise Holdings Corporate
                                    Customer.FirstName = strFirstName;
                                    Customer.LastName = strLastName;                                    

                                    Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : strLocation;
                                    Customer.EmployeeID = strEID;
                                    Customer.ExternalID = strEID;
                                    Customer.HireDate = Convert.ToDateTime(strHireDate);
                                    Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                    Customer.Job = strJobCode;
                                    Customer.Title = strTitle;

                                    Customer.CompanyName = String.Format("Enterprise Holdings Corporate #{0}", strEID);

                                    Customer.IsIndividual = true;
                                    Customer.Password = "Im@geSolutions$1";

                                    Customer.IsUpdated = true;
                                    Customer.Create();

                                    List<ImageSolutions.Entity.EntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                    ImageSolutions.Entity.EntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                    EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                    EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);

                                    if(EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                                    {
                                        EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                        EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                        EntityGroupMapFilter.Code.SearchString = "ADMIN";
                                        EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);
                                    }

                                    List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EntityGroupID).Distinct().ToList();

                                    foreach (string _Entitygroupid in EntityGroupIDs)
                                    {
                                        ImageSolutions.Customer.CustomerEntityGroup CustomerEntityGroup = new CustomerEntityGroup();
                                        CustomerEntityGroup.CustomerID = Customer.CustomerID;
                                        CustomerEntityGroup.EntityGroupID = _Entitygroupid;
                                        CustomerEntityGroup.IsUpdated = true;
                                        CustomerEntityGroup.Create();
                                    }
                                }
                                else
                                {
                                    if (Customer.StoreNumber != (StoreCustomer != null ? StoreCustomer.StoreNumber : strLocation) ||
                                        Customer.FirstName != strFirstName ||
                                        Customer.LastName != strLastName ||
                                        Customer.ExternalID != strEID ||
                                        Customer.ActionDate != Convert.ToDateTime(strActionDate) ||
                                        Customer.Job != strJobCode ||
                                        Customer.Title != strTitle
                                    )
                                    {
                                        Customer.FirstName = strFirstName;
                                        Customer.LastName = strLastName;
                                        Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : strLocation;
                                        Customer.EmployeeID = strEID;
                                        Customer.ExternalID = strEID;
                                        Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                        Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                        Customer.Job = strJobCode;
                                        Customer.Title = strTitle;

                                        Customer.CompanyName = String.Format("Enterprise Holdings Corporate #{0}", strEID);

                                        Customer.IsUpdated = true;
                                        Customer.Update();

                                        List<ImageSolutions.Entity.EntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                        ImageSolutions.Entity.EntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                        EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                        EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);

                                        if (EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                                        {
                                            EntityGroupMaps = new List<ImageSolutions.Entity.EntityGroupMap>();
                                            EntityGroupMapFilter = new ImageSolutions.Entity.EntityGroupMapFilter();
                                            EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                            EntityGroupMapFilter.Code.SearchString = "ADMIN";
                                            EntityGroupMaps = ImageSolutions.Entity.EntityGroupMap.GetEntityGroupMaps(EntityGroupMapFilter);
                                        }

                                        List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EntityGroupID).Distinct().ToList();
                                        foreach (string _entitygroupid in EntityGroupIDs)
                                        {
                                            ImageSolutions.Customer.CustomerEntityGroup CustomerEntityGroup = new CustomerEntityGroup();
                                            ImageSolutions.Customer.CustomerEntityGroupFilter CustomerEntityGroupFilter = new CustomerEntityGroupFilter();
                                            CustomerEntityGroupFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.CustomerID.SearchString = Customer.CustomerID;
                                            CustomerEntityGroupFilter.EntityGroupID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.EntityGroupID.SearchString = _entitygroupid;
                                            CustomerEntityGroup = CustomerEntityGroup.GetCustomerEntityGroup(CustomerEntityGroupFilter);

                                            if (CustomerEntityGroup == null)
                                            {
                                                CustomerEntityGroup = new CustomerEntityGroup();
                                                CustomerEntityGroup.CustomerID = Customer.CustomerID;
                                                CustomerEntityGroup.EntityGroupID = _entitygroupid;
                                                CustomerEntityGroup.IsUpdated = true;
                                                CustomerEntityGroup.Create();
                                            }
                                            else
                                            {
                                                if(CustomerEntityGroup.Inactive)
                                                {
                                                    CustomerEntityGroup.Inactive = false;
                                                    CustomerEntityGroup.IsUpdated = true;
                                                    CustomerEntityGroup.Update();
                                                }
                                            }
                                        }

                                        List<ImageSolutions.Customer.CustomerEntityGroup> ExistCustomerEntityGroups = new List<CustomerEntityGroup>();
                                        ImageSolutions.Customer.CustomerEntityGroupFilter ExistCustomerEntityGroupFilter = new CustomerEntityGroupFilter();
                                        ExistCustomerEntityGroupFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                                        ExistCustomerEntityGroupFilter.CustomerID.SearchString = Customer.CustomerID;
                                        ExistCustomerEntityGroups = CustomerEntityGroup.GetCustomerEntityGroups(ExistCustomerEntityGroupFilter);

                                        foreach (CustomerEntityGroup _CustomerEntityGroup in ExistCustomerEntityGroups)
                                        {
                                            if (!EntityGroupIDs.Contains(_CustomerEntityGroup.EntityGroupID))
                                            {
                                                _CustomerEntityGroup.Inactive = true;
                                                _CustomerEntityGroup.IsUpdated = true;
                                                _CustomerEntityGroup.Update();
                                            }
                                        }
                                    }
                                }
                            }
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
