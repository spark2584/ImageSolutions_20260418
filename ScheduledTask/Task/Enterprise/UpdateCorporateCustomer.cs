using ImageSolutions.Enterprise;
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
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class UpdateCorporateCustomer
    {
        public class FileRow
        {
            public string Email { get; set; }
            public string EID { get; set; }
            public string WorkdayID { get; set; }
            public string Location { get; set; }
            public string JobCode { get; set; }
            public string HireDate { get; set; }
            public string ActionDate { get; set; }
            public string TermDate { get; set; }
            public string Status { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string NotificationEmail { get; set; }
            public string Title { get; set; }
            public string RegTemp { get; set; }
            public string Brand { get; set; }
            public string GPBR { get; set; }

        }

        public bool Execute()
        {
            List<EnterpriseCustomerImport> EnterpriseCustomerImports = null;
            EnterpriseCustomerImportFilter EnterpriseCustomerImportFilter = null;

            try
            {
                EnterpriseCustomerImports = new List<EnterpriseCustomerImport>();
                EnterpriseCustomerImportFilter = new EnterpriseCustomerImportFilter();
                EnterpriseCustomerImportFilter.IsProcessed = false;
                EnterpriseCustomerImportFilter.IsStore = false;
                EnterpriseCustomerImportFilter.IsEncrypted = false;
                EnterpriseCustomerImportFilter.IsPreEmployee = false;

                //EnterpriseCustomerImportFilter.EnterpriseCustomerImportID = new Database.Filter.StringSearch.SearchFilter();
                //EnterpriseCustomerImportFilter.EnterpriseCustomerImportID.SearchString = "3071";

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

                                EnterpriseCustomer Customer = string.IsNullOrEmpty(strCustomerID) ? new EnterpriseCustomer() : new EnterpriseCustomer(strCustomerID);

                                if (string.IsNullOrEmpty(strCustomerID))
                                {
                                    Customer = new EnterpriseCustomer();
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

                                    Customer.CompanyName = String.Format("Enterprise Mobility Corporate");

                                    Customer.IsIndividual = true;
                                    Customer.Password = "Im@geSolutions$1";

                                    Customer.IsUpdated = true;
                                    Customer.Create();

                                    List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                    ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                    EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                    EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                                    List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();
                                    foreach (string _entitygroupid in EntityGroupIDs)
                                    {
                                        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                        CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                                        CustomerEntityGroup.EnterpriseEntityGroupID = _entitygroupid;
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

                                        List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                        ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                        EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                        EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                                        List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();
                                        foreach (string _entitygroupid in EntityGroupIDs)
                                        {
                                            ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                            ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter CustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                                            CustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                                            CustomerEntityGroupFilter.EnterpriseEntityGroupID = new Database.Filter.StringSearch.SearchFilter();
                                            CustomerEntityGroupFilter.EnterpriseEntityGroupID.SearchString = _entitygroupid;
                                            CustomerEntityGroup = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroup(CustomerEntityGroupFilter);

                                            if (CustomerEntityGroup == null)
                                            {
                                                CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                                CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                                                CustomerEntityGroup.EnterpriseEntityGroupID = _entitygroupid;
                                                CustomerEntityGroup.Create();
                                            }
                                        }

                                        List<ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup> ExistCustomerEntityGroups = new List<EnterpriseCustomerEnterpriseEntityGroup>();
                                        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter ExistCustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                                        ExistCustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                                        ExistCustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                                        ExistCustomerEntityGroups = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroups(ExistCustomerEntityGroupFilter);

                                        foreach (EnterpriseCustomerEnterpriseEntityGroup _CustomerEntityGroup in ExistCustomerEntityGroups)
                                        {
                                            if (!EntityGroupIDs.Contains(_CustomerEntityGroup.EnterpriseEntityGroupID))
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
                strSQL = string.Format(@"SELECT EnterpriseCustomerID FROM EnterpriseCustomer (NOLOCK) WHERE Email = {0}", Database.HandleQuote(email));
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
        public void ProcessTSV(string filepath)
        {
            try
            {
                string strSyncID = String.Format("{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"));

                using (StreamReader _StreamReader = new StreamReader(filepath))
                {
                    int counter = 0;
                    String[] HeaderColumnValues = null;
                    List<string> HeaderFields = new List<string>();
                    string strCurrentLine = string.Empty;
                    string strSep = "\t";

                    List<FileRow> FileRows = new List<FileRow>();

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
                                string strWorkdayID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("WORKDAYID")]).Trim();
                                string strLocation = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LOCATION")]).Trim();
                                string strJobCode = Convert.ToString(ColumnValues[HeaderFields.IndexOf("JOBCODE")]).Trim();
                                string strHireDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HIRE DATE")]).Trim();
                                string strActionDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ACTION DATE")]).Trim();
                                string strTermDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("TERM DATE")]).Trim();
                                string strStatus = Convert.ToString(ColumnValues[HeaderFields.IndexOf("STATUS")]).Trim();
                                string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("FIRST")]).Trim();
                                string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LAST")]).Trim();
                                string strNotificationEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("EMAIL")]).Trim();
                                string strTitle = Convert.ToString(ColumnValues[HeaderFields.IndexOf("TITLE")]).Trim();
                                string strRegTemp = Convert.ToString(ColumnValues[HeaderFields.IndexOf("REG/TEMP")]).Trim();
                                string strBrand = Convert.ToString(ColumnValues[HeaderFields.IndexOf("BRAND")]).Trim();
                                string strGPBR = Convert.ToString(ColumnValues[HeaderFields.IndexOf("GP/BR")]).Trim();


                                FileRow FileRow = new FileRow();
                                FileRow.Email = strEmail;
                                FileRow.EID = strEID;
                                FileRow.WorkdayID = strWorkdayID;
                                FileRow.Location = strLocation;
                                FileRow.JobCode = strJobCode;
                                FileRow.HireDate = strHireDate;
                                FileRow.ActionDate = strActionDate;
                                FileRow.TermDate = strTermDate;
                                FileRow.Status = strStatus;
                                FileRow.FirstName = strFirstName;
                                FileRow.LastName = strLastName;
                                FileRow.NotificationEmail = strNotificationEmail;
                                FileRow.Title = strTitle;
                                FileRow.RegTemp = strRegTemp;
                                FileRow.Brand = strBrand;
                                FileRow.GPBR = strGPBR;
                                FileRows.Add(FileRow);


                                ////Save Data
                                //string strCustomerID = GetCustomerID(strEmail);

                                //EnterpriseCustomer Customer = string.IsNullOrEmpty(strCustomerID) ? new EnterpriseCustomer() : new EnterpriseCustomer(strCustomerID);

                                //EnterpriseCustomer StoreCustomer = new EnterpriseCustomer();
                                //EnterpriseCustomerFilter StoreCustomerFilter = new EnterpriseCustomerFilter();
                                //StoreCustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                                //StoreCustomerFilter.ExternalID.SearchString = strLocation;
                                //StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                //StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];
                                //StoreCustomer = EnterpriseCustomer.GetEnterpriseCustomer(StoreCustomerFilter);

                                //if (string.IsNullOrEmpty(strCustomerID))
                                //{
                                //    Customer = new EnterpriseCustomer();
                                //    Customer.Email = strEmail; //Create email using Group Branch                                    
                                //    Customer.ParentID = ConfigurationManager.AppSettings["EnterpriseCorporateCustomerParentID"];  //Enterprise Holdings Corporate
                                //    Customer.FirstName = strFirstName;
                                //    Customer.LastName = strLastName;

                                //    Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : strGPBR; //strLocation;
                                //    Customer.EmployeeID = strEID;
                                //    Customer.ExternalID = strEID;
                                //    Customer.HireDate = Convert.ToDateTime(strHireDate);
                                //    Customer.ActionDate = Convert.ToDateTime(strActionDate);

                                //    if (!string.IsNullOrEmpty(strTermDate))
                                //    {
                                //        Customer.TermDate = Convert.ToDateTime(strTermDate);
                                //    }
                                //    else
                                //    {
                                //        Customer.TermDate = null;
                                //    }
                                //    Customer.CustomerStatus = strStatus;
                                //    Customer.CustomerRegTemp = strRegTemp;
                                //    Customer.CustomerBrand = strBrand;
                                //    Customer.CustomerGPBR = strGPBR;
                                //    Customer.NotificationEmail = strNotificationEmail;

                                //    Customer.Job = strJobCode;
                                //    Customer.Title = strTitle;

                                //    Customer.CompanyName = String.Format("Enterprise Holdings Corporate #{0}", strEID);

                                //    Customer.IsIndividual = true;
                                //    Customer.Password = "Im@geSolutions$1";

                                //    Customer.IsPreEmployee = false;

                                //    Customer.IsUpdated = true;
                                //    Customer.SyncID = strSyncID;
                                //    Customer.Create();

                                //    List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                //    ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                //    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                //    EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                //    EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                                //    if (EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                                //    {
                                //        EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                //        EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                //        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                //        EntityGroupMapFilter.Code.SearchString = "ADMIN";
                                //        EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);
                                //    }

                                //    List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();

                                //    foreach (string _Entitygroupid in EntityGroupIDs)
                                //    {
                                //        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                //        CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                                //        CustomerEntityGroup.EnterpriseEntityGroupID = _Entitygroupid;
                                //        CustomerEntityGroup.IsUpdated = true;
                                //        CustomerEntityGroup.Create();
                                //    }
                                //}
                                //else
                                //{
                                //    if (Customer.StoreNumber != (StoreCustomer != null ? StoreCustomer.StoreNumber : strGPBR) ||
                                //        Customer.FirstName != strFirstName ||
                                //        Customer.LastName != strLastName ||
                                //        Customer.ExternalID != strEID ||
                                //        Customer.HireDate != Convert.ToDateTime(strActionDate) ||
                                //        Customer.ActionDate != Convert.ToDateTime(strActionDate) ||
                                //        Customer.Job != strJobCode ||
                                //        Customer.Title != strTitle ||

                                //        (Customer.TermDate == null) != string.IsNullOrEmpty(strTermDate) ||
                                //        Customer.CustomerStatus != strStatus ||
                                //        Customer.CustomerRegTemp != strRegTemp ||
                                //        Customer.CustomerBrand != strBrand ||
                                //        Customer.CustomerGPBR != strGPBR ||
                                //        Customer.NotificationEmail != strNotificationEmail ||
                                //        Customer.IsPreEmployee == true
                                //    )
                                //    {
                                //        Customer.FirstName = strFirstName;
                                //        Customer.LastName = strLastName;
                                //        Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : strGPBR; //strLocation;
                                //        Customer.EmployeeID = strEID;
                                //        Customer.ExternalID = strEID;
                                //        Customer.HireDate = Convert.ToDateTime(strHireDate);
                                //        Customer.ActionDate = Convert.ToDateTime(strActionDate);
                                //        if (!string.IsNullOrEmpty(strTermDate))
                                //        {
                                //            Customer.TermDate = Convert.ToDateTime(strTermDate);
                                //        }
                                //        else
                                //        {
                                //            Customer.TermDate = null;
                                //        }
                                //        Customer.CustomerStatus = strStatus;
                                //        Customer.CustomerRegTemp = strRegTemp;
                                //        Customer.CustomerBrand = strBrand;
                                //        Customer.CustomerGPBR = strGPBR;
                                //        Customer.NotificationEmail = strNotificationEmail;

                                //        Customer.Job = strJobCode;
                                //        Customer.Title = strTitle;

                                //        Customer.CompanyName = String.Format("Enterprise Holdings Corporate #{0}", strEID);

                                //        Customer.IsPreEmployee = false;

                                //        Customer.IsUpdated = true;
                                //        Customer.SyncID = strSyncID;
                                //        Customer.Update();

                                //        List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                //        ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                //        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                //        EntityGroupMapFilter.Code.SearchString = Customer.Job;
                                //        EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                                //        if (EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                                //        {
                                //            EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                                //            EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                                //            EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                                //            EntityGroupMapFilter.Code.SearchString = "ADMIN";
                                //            EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);
                                //        }

                                //        List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();
                                //        foreach (string _entitygroupid in EntityGroupIDs)
                                //        {
                                //            ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                //            ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter CustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                                //            CustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                                //            CustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                                //            CustomerEntityGroupFilter.EnterpriseEntityGroupID = new Database.Filter.StringSearch.SearchFilter();
                                //            CustomerEntityGroupFilter.EnterpriseEntityGroupID.SearchString = _entitygroupid;
                                //            CustomerEntityGroup = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroup(CustomerEntityGroupFilter);

                                //            if (CustomerEntityGroup == null)
                                //            {
                                //                CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                                //                CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                                //                CustomerEntityGroup.EnterpriseEntityGroupID = _entitygroupid;
                                //                CustomerEntityGroup.IsUpdated = true;
                                //                CustomerEntityGroup.Create();
                                //            }
                                //            else
                                //            {
                                //                if (CustomerEntityGroup.Inactive)
                                //                {
                                //                    CustomerEntityGroup.Inactive = false;
                                //                    CustomerEntityGroup.IsUpdated = true;
                                //                    CustomerEntityGroup.Update();
                                //                }
                                //            }
                                //        }

                                //        List<ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup> ExistCustomerEntityGroups = new List<EnterpriseCustomerEnterpriseEntityGroup>();
                                //        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter ExistCustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                                //        ExistCustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                                //        ExistCustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                                //        ExistCustomerEntityGroups = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroups(ExistCustomerEntityGroupFilter);

                                //        foreach (EnterpriseCustomerEnterpriseEntityGroup _CustomerEntityGroup in ExistCustomerEntityGroups)
                                //        {
                                //            if (!EntityGroupIDs.Contains(_CustomerEntityGroup.EnterpriseEntityGroupID))
                                //            {
                                //                _CustomerEntityGroup.Inactive = true;
                                //                _CustomerEntityGroup.IsUpdated = true;
                                //                _CustomerEntityGroup.Update();
                                //            }
                                //        }
                                //    }
                                //    else
                                //    {
                                //        Customer.SyncID = strSyncID;
                                //        Customer.Update();
                                //    }
                                //}
                            }
                        }
                    }

                    int intCounter = 0;

                    //Parallel.ForEach(FileRows, new ParallelOptions { MaxDegreeOfParallelism = 20 }, filerow =>
                    //    {
                    //        intCounter++;
                    //        Console.WriteLine(String.Format("{0}", intCounter));
                    //        ProcessFileRow(filerow, strSyncID);
                    //    }
                    //);

                    foreach (FileRow _FileRow in FileRows)
                    {
                        intCounter++;
                        Console.WriteLine(String.Format("{0}", intCounter));
                        ProcessFileRow(_FileRow, strSyncID);
                    }

                    UpdateInActiveCustomer(strSyncID);
                    UpdateInActivePreEmployeeCustomer(strSyncID);
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

        public void ProcessFileRow(FileRow filerow, string syncid)
        {
            //Save Data
            string strCustomerID = GetCustomerID(filerow.Email);

            EnterpriseCustomer Customer = string.IsNullOrEmpty(strCustomerID) ? new EnterpriseCustomer() : new EnterpriseCustomer(strCustomerID);

            //if(Customer != null && !Customer.ExternalID.ToLower().Contains("update"))
            //{
            //    return;
            //}

            EnterpriseCustomer StoreCustomer = new EnterpriseCustomer();
            EnterpriseCustomerFilter StoreCustomerFilter = new EnterpriseCustomerFilter();
            StoreCustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
            StoreCustomerFilter.ExternalID.SearchString = filerow.Location;
            StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
            StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];
            StoreCustomer = EnterpriseCustomer.GetEnterpriseCustomer(StoreCustomerFilter);

            if (string.IsNullOrEmpty(strCustomerID))
            {
                Customer = new EnterpriseCustomer();
                Customer.Email = filerow.Email; //Create email using Group Branch                                    
                Customer.ParentID = ConfigurationManager.AppSettings["EnterpriseCorporateCustomerParentID"];  //Enterprise Holdings Corporate
                Customer.FirstName = filerow.FirstName;
                Customer.LastName = filerow.LastName;

                Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : filerow.GPBR; //strLocation;
                Customer.EmployeeID = filerow.EID;
                Customer.ExternalID = filerow.EID;
                
                //Customer.HireDate = Convert.ToDateTime(filerow.HireDate);
                if (!string.IsNullOrEmpty(filerow.HireDate))
                {
                    Customer.HireDate = Convert.ToDateTime(filerow.HireDate);
                    Customer.BudgetRefreshedOn = Customer.HireDate;
                    //if (Convert.ToDateTime(Customer.HireDate).Year < DateTime.UtcNow.Year)
                    //{
                    //    DateTime dtRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(Customer.HireDate).Month, Convert.ToDateTime(Customer.HireDate).Day);
                    //    if (dtRefreshedOn > DateTime.UtcNow)
                    //    {
                    //        Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(Customer.HireDate).Month, Convert.ToDateTime(Customer.HireDate).Day);
                    //    }
                    //}
                }
                else
                {
                    Customer.HireDate = null;
                }

                Customer.ActionDate = Convert.ToDateTime(filerow.ActionDate);

                if (!string.IsNullOrEmpty(filerow.TermDate))
                {
                    Customer.TermDate = Convert.ToDateTime(filerow.TermDate);
                    //Customer.BudgetEndDate = Convert.ToDateTime(filerow.TermDate);
                }
                else
                {
                    Customer.TermDate = null;
                }
                Customer.CustomerStatus = filerow.Status;
                Customer.CustomerRegTemp = filerow.RegTemp;
                Customer.CustomerBrand = filerow.Brand;
                Customer.CustomerGPBR = filerow.GPBR;
                Customer.NotificationEmail = filerow.NotificationEmail;

                Customer.Job = filerow.JobCode;
                Customer.Title = filerow.Title;

                Customer.CompanyName = String.Format("Enterprise Mobility Corporate #{0}", filerow.EID);
                Customer.WorkdayID = filerow.WorkdayID;

                Customer.IsIndividual = true;
                Customer.Password = "Im@geSolutions$1";

                Customer.WorkdayID = filerow.WorkdayID;
                Customer.IsPreEmployee = false;

                Customer.IsUpdated = true;
                Customer.SyncID = syncid;
                Customer.InActive = false;
                Customer.Create();

                List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                EntityGroupMapFilter.Code.SearchString = Customer.Job;
                EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                if (EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                {
                    EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                    EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                    EntityGroupMapFilter.Code.SearchString = "ADMIN";
                    EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);
                }

                List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();

                foreach (string _Entitygroupid in EntityGroupIDs)
                {
                    ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                    CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                    CustomerEntityGroup.EnterpriseEntityGroupID = _Entitygroupid;
                    CustomerEntityGroup.IsUpdated = true;
                    CustomerEntityGroup.Create();
                }
            }
            else
            {
                if (Customer.StoreNumber != (StoreCustomer != null ? StoreCustomer.StoreNumber : filerow.GPBR) ||
                    Customer.FirstName != filerow.FirstName ||
                    Customer.LastName != filerow.LastName||
                    Customer.ExternalID != filerow.EID ||
                    Customer.HireDate != Convert.ToDateTime(filerow.HireDate) ||
                    Customer.ActionDate != Convert.ToDateTime(filerow.ActionDate) ||
                    Customer.Job != filerow.JobCode ||
                    Customer.Title != filerow.Title ||

                    (Customer.TermDate == null) != string.IsNullOrEmpty(filerow.TermDate) ||
                    (Customer.TermDate != null && !string.IsNullOrEmpty(filerow.TermDate) && Customer.TermDate != Convert.ToDateTime(filerow.TermDate)) ||

                    Customer.CustomerStatus != filerow.Status ||
                    Customer.CustomerRegTemp != filerow.RegTemp ||
                    Customer.CustomerBrand != filerow.Brand ||
                    Customer.CustomerGPBR != filerow.GPBR ||
                    Customer.WorkdayID != filerow.WorkdayID ||
                    Customer.NotificationEmail != filerow.NotificationEmail ||
                    Customer.IsPreEmployee == true ||
                    Customer.InActivePreEmployee == true ||
                    Customer.InActive == true
                )
                {
                    Customer.FirstName = filerow.FirstName;
                    Customer.LastName = filerow.LastName;
                    Customer.StoreNumber = StoreCustomer != null ? StoreCustomer.StoreNumber : filerow.GPBR; //strLocation;
                    Customer.EmployeeID = filerow.EID;
                    Customer.ExternalID = filerow.EID;
                    Customer.HireDate = Convert.ToDateTime(filerow.HireDate);
                    Customer.ActionDate = Convert.ToDateTime(filerow.ActionDate);
                    if (!string.IsNullOrEmpty(filerow.TermDate))
                    {
                        Customer.TermDate = Convert.ToDateTime(filerow.TermDate);
                        if (Customer.BudgetEndDate == null && Convert.ToDateTime(Customer.TermDate).AddYears(1) > DateTime.UtcNow)
                        {
                            Customer.BudgetEndDate = Customer.TermDate;
                        }
                    }
                    else
                    {
                        Customer.TermDate = null;
                    }
                    Customer.CustomerStatus = filerow.Status;
                    Customer.CustomerRegTemp = filerow.RegTemp;
                    Customer.CustomerBrand = filerow.Brand;
                    Customer.CustomerGPBR = filerow.GPBR;
                    Customer.NotificationEmail = filerow.NotificationEmail;

                    Customer.Job = filerow.JobCode;
                    Customer.Title = filerow.Title;

                    Customer.CompanyName = String.Format("Enterprise Mobility Corporate #{0}", filerow.EID);
                    Customer.WorkdayID = filerow.WorkdayID;

                    Customer.IsPreEmployee = false;
                    Customer.InActivePreEmployee = false;

                    Customer.IsUpdated = true;
                    Customer.SyncID = syncid;
                    Customer.InActive = false;
                    Customer.Update();

                    List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap> EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                    ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                    EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                    EntityGroupMapFilter.Code.SearchString = Customer.Job;
                    EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);

                    if (EntityGroupMaps == null || EntityGroupMaps.Count == 0)
                    {
                        EntityGroupMaps = new List<ImageSolutions.Enterprise.EnterpriseEntityGroupMap>();
                        EntityGroupMapFilter = new ImageSolutions.Enterprise.EnterpriseEntityGroupMapFilter();
                        EntityGroupMapFilter.Code = new Database.Filter.StringSearch.SearchFilter();
                        EntityGroupMapFilter.Code.SearchString = "ADMIN";
                        EntityGroupMaps = ImageSolutions.Enterprise.EnterpriseEntityGroupMap.GetEnterpriseEntityGroupMaps(EntityGroupMapFilter);
                    }

                    List<string> EntityGroupIDs = EntityGroupMaps.Select(x => x.EnterpriseEntityGroupID).Distinct().ToList();
                    foreach (string _entitygroupid in EntityGroupIDs)
                    {
                        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                        ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter CustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                        CustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                        CustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                        CustomerEntityGroupFilter.EnterpriseEntityGroupID = new Database.Filter.StringSearch.SearchFilter();
                        CustomerEntityGroupFilter.EnterpriseEntityGroupID.SearchString = _entitygroupid;
                        CustomerEntityGroup = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroup(CustomerEntityGroupFilter);

                        if (CustomerEntityGroup == null)
                        {
                            CustomerEntityGroup = new EnterpriseCustomerEnterpriseEntityGroup();
                            CustomerEntityGroup.EnterpriseCustomerID = Customer.EnterpriseCustomerID;
                            CustomerEntityGroup.EnterpriseEntityGroupID = _entitygroupid;
                            CustomerEntityGroup.IsUpdated = true;
                            CustomerEntityGroup.Create();
                        }
                        else
                        {
                            if (CustomerEntityGroup.Inactive)
                            {
                                CustomerEntityGroup.Inactive = false;
                                CustomerEntityGroup.IsUpdated = true;
                                CustomerEntityGroup.Update();
                            }
                        }
                    }

                    List<ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroup> ExistCustomerEntityGroups = new List<EnterpriseCustomerEnterpriseEntityGroup>();
                    ImageSolutions.Enterprise.EnterpriseCustomerEnterpriseEntityGroupFilter ExistCustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                    ExistCustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                    ExistCustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = Customer.EnterpriseCustomerID;
                    ExistCustomerEntityGroups = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroups(ExistCustomerEntityGroupFilter);

                    foreach (EnterpriseCustomerEnterpriseEntityGroup _CustomerEntityGroup in ExistCustomerEntityGroups)
                    {
                        if (!EntityGroupIDs.Contains(_CustomerEntityGroup.EnterpriseEntityGroupID))
                        {
                            _CustomerEntityGroup.Inactive = true;
                            _CustomerEntityGroup.IsUpdated = true;
                            _CustomerEntityGroup.Update();
                        }
                    }
                }
                else
                {
                    Customer.SyncID = syncid;
                    Customer.InActive = false;
                    Customer.Update();
                }
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

        public string UpdateInActiveCustomer(string syncid)
        {
            string objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE ec SET InActive = 1, IsUpdated = 1, IsPreEmployee = 1
--SELECT ec.InActive
FROM EnterpriseCustomer (NOLOCK) ec
Inner Join UserInfo (NOLOCK) ui on ui.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = ui.UserInfoID
WHERE ec.IsIndividual = 1
and ec.IsPreEmployee = 0
and uw.WebsiteID = {1}
and ISNULL(ec.SyncID,'') != {0}
and ec.InActive = 0
and ISNULL(ec.EmployeeID,'') not in ('E888KJ','E36CPR','E989XS','E371S4','E1649G','E621WM', 'E938N1')
"
                        , Database.HandleQuote(syncid)
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]))
                    );

                Database.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }

        public string UpdateInActivePreEmployeeCustomer(string syncid)
        {
            string objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE ec SET InActive = 1, IsUpdated = 1
--SELECT ec.InActive
FROM EnterpriseCustomer (NOLOCK) ec
Inner Join UserInfo (NOLOCK) ui on ui.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = ui.UserInfoID
Outer Apply
(
	SELECT Max(SyncID) LatestSyncID
	FROM EnterpriseCustomer (NOLOCK) ec2
	WHERE ec2.IsPreEmployee = 1
	and ec2.IsIndividual = 1
) sync
WHERE ec.IsIndividual = 1
and ec.IsPreEmployee = 1
and uw.WebsiteID = {1}
--and ISNULL(ec.SyncID,'') != sync.LatestSyncID
and ISNULL(ec.SyncID,'') < {0}
and ec.InActive = 0
"
                        , Database.HandleQuote(syncid)
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]))
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
