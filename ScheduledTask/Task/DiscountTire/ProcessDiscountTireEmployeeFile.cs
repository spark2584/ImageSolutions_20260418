using ImageSolutions.Customer;
using NetSuiteLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.DiscountTire
{
    public class ProcessDiscountTireEmployeeFile : NetSuiteBase
    {
        public bool Execute()
        {
            List<CustomerImport> CustomerImports = null;
            CustomerImportFilter CustomerImportFilter = null;
            int counter = 0;

            try
            {
                CustomerImportFilter = new CustomerImportFilter();
                CustomerImportFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                CustomerImportFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                CustomerImportFilter.IsProcessed = false;
                CustomerImportFilter.IsStore = false;
                CustomerImportFilter.IsEncrypted = false;
                CustomerImports = CustomerImport.GetCustomerImports(CustomerImportFilter);

                foreach (CustomerImport _CustomerImport in CustomerImports.OrderBy(x => x.FileDate))
                {
                    try
                    {
                        counter++;

                        string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _CustomerImport.FilePath); //Get path to saved file
                        string strExtension = Path.GetExtension(strFullPath);
                        string strConnectionString = String.Empty;

                        switch (strExtension)
                        {
                            case ".csv":
                                ProcessCSV(strFullPath);
                                break;
                            default:
                                throw new Exception("File Type not supported");
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
                Console.Write(string.Format(@"Error - Process Discount Tire Employee File: {0}", ex.Message));
            }
            finally
            {
                CustomerImports = null;
                CustomerImportFilter = null;
            }
            return true;
        }

        public void ProcessCSV(string filepath)
        {
            try
            {
                List<string> UserWebsiteIDs = new List<string>();

                using (StreamReader _StreamReader = new StreamReader(filepath))
                {
                    int counter = 0;
                    String[] HeaderColumnValues = null;
                    List<string> HeaderFields = new List<string>();
                    string strCurrentLine = string.Empty;

                    UpdateEmployeeFileImportStatus(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]), "Processing");

                    while (!string.IsNullOrEmpty(strCurrentLine = _StreamReader.ReadLine()))
                    {
                        //bool blnIsNewEmployee = false;
                        counter++;

                        ImageSolutions.Employee.EmployeeFileImport EmployeeFileImport = null;
                        ImageSolutions.Employee.EmployeeFileImportFilter EmployeeFileImportFilter = null;

                        try
                        {
                            if (counter == 1)
                            {
                                HeaderColumnValues = strCurrentLine.Split(',');
                                foreach (string _value in HeaderColumnValues)
                                {
                                    HeaderFields.Add(_value);
                                }

                                if (!HeaderFields.Contains("region")) throw new Exception("[region] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("cost-center")) throw new Exception("[cost-center] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("employee-id")) throw new Exception("[employee-id] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("last-name")) throw new Exception("[last-name] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("first-name")) throw new Exception("[first-name] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("middle-name")) throw new Exception("[middle-name] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("time-type")) throw new Exception("[time-type] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("service-date")) throw new Exception("[service-date] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("doh")) throw new Exception("[doh] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("new-hire")) throw new Exception("[new-hire] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("ee-password")) throw new Exception("[ee-password] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("company-brand")) throw new Exception("[company-brand] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("preferred-first")) throw new Exception("[preferred-first] is missing from the file, contact administrator");
                            }
                            else
                            {
                                String[] ColumnValues = null;
                                ColumnValues = strCurrentLine.Split(',');

                                string strRegion = Convert.ToString(ColumnValues[HeaderFields.IndexOf("region")]).Replace("\"", String.Empty).Trim();
                                string strCostCenter = Convert.ToString(ColumnValues[HeaderFields.IndexOf("cost-center")]).Replace("\"", String.Empty).Trim();
                                string strEmployeeID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("employee-id")]).Replace("\"", String.Empty).Trim();
                                string strLastName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("last-name")]).Replace("\"", String.Empty).Trim();
                                string strFirstName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("first-name")]).Replace("\"", String.Empty).Trim();
                                string strMiddleName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("middle-name")]).Replace("\"", String.Empty).Trim();
                                string strTimeType = Convert.ToString(ColumnValues[HeaderFields.IndexOf("time-type")]).Replace("\"", String.Empty).Trim();
                                string strServiceDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("service-date")]).Replace("\"", String.Empty).Trim();
                                string strDOH = Convert.ToString(ColumnValues[HeaderFields.IndexOf("doh")]).Replace("\"", String.Empty).Trim();
                                string strNewHire = Convert.ToString(ColumnValues[HeaderFields.IndexOf("new-hire")]).Replace("\"", String.Empty).Trim();
                                string strEEPassword = Convert.ToString(ColumnValues[HeaderFields.IndexOf("ee-password")]).Replace("\"", String.Empty).Trim();
                                string strCompanyBrand = Convert.ToString(ColumnValues[HeaderFields.IndexOf("company-brand")]).Replace("\"", String.Empty).Trim();
                                string strPreferredFirst = Convert.ToString(ColumnValues[HeaderFields.IndexOf("preferred-first")]).Replace("\"", String.Empty).Trim();

                                if (string.IsNullOrEmpty(strEmployeeID))
                                {
                                    throw new Exception("EmployeeID is required.");
                                }

                                EmployeeFileImport = new ImageSolutions.Employee.EmployeeFileImport();
                                EmployeeFileImportFilter = new ImageSolutions.Employee.EmployeeFileImportFilter();

                                EmployeeFileImportFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                EmployeeFileImportFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                EmployeeFileImportFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                                EmployeeFileImportFilter.EmployeeID.SearchString = strEmployeeID;

                                EmployeeFileImport = ImageSolutions.Employee.EmployeeFileImport.GetEmployeeFileImport(EmployeeFileImportFilter);

                                if(EmployeeFileImport == null)
                                {
                                    EmployeeFileImport = new ImageSolutions.Employee.EmployeeFileImport();
                                    EmployeeFileImport.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                    EmployeeFileImport.Region = strRegion;
                                    EmployeeFileImport.CostCenter = strCostCenter;
                                    EmployeeFileImport.EmployeeID = strEmployeeID;
                                    EmployeeFileImport.LastName = strLastName;
                                    EmployeeFileImport.FirstName = strFirstName;
                                    EmployeeFileImport.MiddleName = strMiddleName;
                                    EmployeeFileImport.TimeType = strTimeType;
                                    EmployeeFileImport.ServiceDate = Convert.ToDateTime(strServiceDate);
                                    EmployeeFileImport.DateOfHire = Convert.ToDateTime(strDOH);
                                    EmployeeFileImport.NewHire = strNewHire;
                                    EmployeeFileImport.Password = strEEPassword;
                                    EmployeeFileImport.Status = "New";
                                    EmployeeFileImport.Create();
                                }
                                else
                                {
                                    EmployeeFileImport.Region = strRegion;
                                    EmployeeFileImport.CostCenter = strCostCenter;
                                    EmployeeFileImport.EmployeeID = strEmployeeID;
                                    EmployeeFileImport.LastName = strLastName;
                                    EmployeeFileImport.FirstName = strFirstName;
                                    EmployeeFileImport.MiddleName = strMiddleName;

                                    if (EmployeeFileImport.TimeType != strTimeType)
                                    {
                                        EmployeeFileImport.Status = "Renew";
                                    }

                                    EmployeeFileImport.TimeType = strTimeType;
                                    EmployeeFileImport.ServiceDate = Convert.ToDateTime(strServiceDate);
                                    EmployeeFileImport.DateOfHire = Convert.ToDateTime(strDOH);
                                    EmployeeFileImport.NewHire = strNewHire;
                                    EmployeeFileImport.Password = strEEPassword;
                                    EmployeeFileImport.Update();
                                }

                                try
                                {
                                    //Check User - create/update user
                                    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                                    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                                    //UserInfoFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                                    //UserInfoFilter.ExternalID.SearchString = strEmployeeID;
                                    UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                    UserInfoFilter.EmailAddress.SearchString = String.Format(@"{0}@dt.imageinc.com", strEmployeeID);
                                    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                                    if (UserInfo == null)
                                    {
                                        UserInfo = new ImageSolutions.User.UserInfo();

                                        UserInfo.FirstName = strFirstName;
                                        UserInfo.LastName = strLastName;
                                        UserInfo.UserName = strEmployeeID;
                                        UserInfo.EmailAddress = String.Format(@"{0}@dt.imageinc.com", strEmployeeID);
                                        UserInfo.Password = strEEPassword;
                                        UserInfo.ExternalID = strEmployeeID;
                                        UserInfo.DisplayName = strPreferredFirst;
                                        UserInfo.Create();
                                    }
                                    else
                                    {
                                        UserInfo.FirstName = strFirstName;
                                        UserInfo.LastName = strLastName;
                                        UserInfo.UserName = strEmployeeID;
                                        UserInfo.Password = strEEPassword;
                                        UserInfo.ExternalID = strEmployeeID;
                                        UserInfo.DisplayName = strPreferredFirst;
                                        UserInfo.Update();
                                    }

                                    //UserWebsite
                                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                    UserWebsite = CreateUserWebsiteUserAccount(UserInfo, UserWebsite, strRegion, strCostCenter, strEmployeeID, strTimeType, strDOH, strCompanyBrand);

                                    //if (UserWebsite == null)
                                    //{
                                    //    //blnIsNewEmployee = true;
                                    //    UserWebsite = CreateUserWebsiteUserAccount(UserInfo, UserWebsite, strRegion, strCostCenter, strEmployeeID, strTimeType, strDOH);
                                    //}
                                    //else
                                    //{
                                    //List<ImageSolutions.User.UserAccount> UserAccounts = new List<ImageSolutions.User.UserAccount>();
                                    //ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                    //UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    //UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                    //UserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(UserAccountFilter);

                                    //if (UserAccounts == null || UserAccounts.Count == 0)
                                    //{
                                    //    //blnIsNewEmployee = true;
                                    //    UserWebsite = CreateUserWebsiteUserAccount(UserInfo, UserWebsite, strRegion, strCostCenter, strEmployeeID, strTimeType, strDOH);
                                    //}

                                    //UserWebsite.IsPartTime = strTimeType == "P";

                                    //if (strRegion == "AZO")
                                    //{
                                    //    UserWebsite.BudgetSetting = "No Budget";
                                    //}
                                    //else if (UserWebsite.BudgetSetting == "Part Time" || UserWebsite.BudgetSetting == "Full Time")
                                    //{
                                    //    UserWebsite.BudgetSetting = strTimeType == "P" ? "Part Time" : "Full Time";
                                    //}

                                    //UserWebsite.HiredDate = Convert.ToDateTime(strDOH);
                                    //UserWebsite.Update();
                                    //}

                                    //bool AutoAssignBudget = UserWebsite.AutoAssignBudget;
                                    //if (!AutoAssignBudget)
                                    //{
                                    //    foreach (ImageSolutions.User.UserAccount _UserAccount in UserWebsite.UserAccounts)
                                    //    {
                                    //        if (_UserAccount.Account.AutoAssignBudget)
                                    //        {
                                    //            AutoAssignBudget = true;
                                    //        }
                                    //    }
                                    //}

                                    //if (AutoAssignBudget)
                                    UserWebsite = new ImageSolutions.User.UserWebsite(UserWebsite.UserWebsiteID);

                                    if (UserWebsite.Accounts.Exists(x => !string.IsNullOrEmpty(x.BudgetSetting)) || !string.IsNullOrEmpty(UserWebsite.BudgetSetting))
                                    {
                                        string strBudgetSetting = string.Empty;

                                        if (!string.IsNullOrEmpty(UserWebsite.BudgetSetting))
                                        {
                                            strBudgetSetting = UserWebsite.BudgetSetting;
                                        }
                                        else
                                        {
                                            foreach (ImageSolutions.Account.Account _Account in UserWebsite.Accounts.FindAll(x => !string.IsNullOrEmpty(x.BudgetSetting)))
                                            {
                                                string strAccountBudgetSetting = string.Empty;

                                                if (_Account.BudgetSetting == "Full Time/Part Time")
                                                {
                                                    strAccountBudgetSetting = strTimeType == "P" ? "Part Time" : "Full Time";
                                                }
                                                else
                                                {
                                                    strAccountBudgetSetting = _Account.BudgetSetting;
                                                }

                                                if (!string.IsNullOrEmpty(strBudgetSetting) && strBudgetSetting != strAccountBudgetSetting)
                                                {
                                                    throw new Exception("Multiple Budget Setting Detected");
                                                }

                                                strBudgetSetting = strAccountBudgetSetting;
                                            }
                                        }

                                        //Find Budget
                                        string strFullTimeBudgetName = string.Format("{0} - Full Time", DateTime.UtcNow.Year);
                                        ImageSolutions.Budget.Budget FullTimeBudget = new ImageSolutions.Budget.Budget();
                                        ImageSolutions.Budget.BudgetFilter FullTiemBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                                        FullTiemBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        FullTiemBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        FullTiemBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                                        FullTiemBudgetFilter.BudgetName.SearchString = strFullTimeBudgetName;
                                        FullTimeBudget = ImageSolutions.Budget.Budget.GetBudget(FullTiemBudgetFilter);
                                        if (FullTimeBudget == null)
                                        {
                                            FullTimeBudget = new ImageSolutions.Budget.Budget();
                                            FullTimeBudget.BudgetName = strFullTimeBudgetName;
                                            FullTimeBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                            FullTimeBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                                            FullTimeBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                                            FullTimeBudget.BudgetAmount = Convert.ToDouble(275);
                                            FullTimeBudget.AllowOverBudget = true;
                                            FullTimeBudget.IncludeShippingAndTaxes = false;
                                            FullTimeBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                            FullTimeBudget.Create();
                                        }

                                        string strPartTimeBudgetName = string.Format("{0} - Part Time", DateTime.UtcNow.Year);
                                        ImageSolutions.Budget.Budget PartTimeBudget = new ImageSolutions.Budget.Budget();
                                        ImageSolutions.Budget.BudgetFilter PartTimeBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                                        PartTimeBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        PartTimeBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        PartTimeBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                                        PartTimeBudgetFilter.BudgetName.SearchString = strPartTimeBudgetName;
                                        PartTimeBudget = ImageSolutions.Budget.Budget.GetBudget(PartTimeBudgetFilter);
                                        if (PartTimeBudget == null)
                                        {
                                            PartTimeBudget = new ImageSolutions.Budget.Budget();
                                            PartTimeBudget.BudgetName = strPartTimeBudgetName;
                                            PartTimeBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                            PartTimeBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                                            PartTimeBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                                            PartTimeBudget.BudgetAmount = Convert.ToDouble(225);
                                            PartTimeBudget.AllowOverBudget = true;
                                            FullTimeBudget.IncludeShippingAndTaxes = false;
                                            PartTimeBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                            PartTimeBudget.Create();
                                        }

                                        string strWarehouseFacilitiesBudgetName = string.Format("{0} - Warehouse Facilities", DateTime.UtcNow.Year);
                                        ImageSolutions.Budget.Budget WarehouseFacilitiesBudget = new ImageSolutions.Budget.Budget();
                                        ImageSolutions.Budget.BudgetFilter WarehouseFacilitiesBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                                        WarehouseFacilitiesBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        WarehouseFacilitiesBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        WarehouseFacilitiesBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                                        WarehouseFacilitiesBudgetFilter.BudgetName.SearchString = strWarehouseFacilitiesBudgetName;
                                        WarehouseFacilitiesBudget = ImageSolutions.Budget.Budget.GetBudget(WarehouseFacilitiesBudgetFilter);
                                        if (WarehouseFacilitiesBudget == null)
                                        {
                                            WarehouseFacilitiesBudget = new ImageSolutions.Budget.Budget();
                                            WarehouseFacilitiesBudget.BudgetName = strWarehouseFacilitiesBudgetName;
                                            WarehouseFacilitiesBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                            WarehouseFacilitiesBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                                            WarehouseFacilitiesBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                                            WarehouseFacilitiesBudget.BudgetAmount = Convert.ToDouble(400);
                                            WarehouseFacilitiesBudget.AllowOverBudget = true;
                                            WarehouseFacilitiesBudget.IncludeShippingAndTaxes = false;
                                            WarehouseFacilitiesBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                            WarehouseFacilitiesBudget.Create();
                                        }

                                        string strFleetBudgetName = string.Format("{0} - Fleet Budget", DateTime.UtcNow.Year);
                                        ImageSolutions.Budget.Budget FleetBudget = new ImageSolutions.Budget.Budget();
                                        ImageSolutions.Budget.BudgetFilter FleetBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                                        FleetBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        FleetBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        FleetBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                                        FleetBudgetFilter.BudgetName.SearchString = strFleetBudgetName;
                                        FleetBudget = ImageSolutions.Budget.Budget.GetBudget(FleetBudgetFilter);
                                        if (FleetBudget == null)
                                        {
                                            FleetBudget = new ImageSolutions.Budget.Budget();
                                            FleetBudget.BudgetName = strFleetBudgetName;
                                            FleetBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                            FleetBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                                            FleetBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                                            FleetBudget.BudgetAmount = Convert.ToDouble(400);
                                            FleetBudget.AllowOverBudget = true;
                                            FleetBudget.IncludeShippingAndTaxes = false;
                                            FleetBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                            FleetBudget.Create();
                                        }


                                        ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                        ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                                        BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                                        BudgetAssignmentFilter.BudgetID.SearchString = FullTimeBudget.BudgetID;
                                        BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                        BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                        BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                                        if (BudgetAssignment == null)
                                        {
                                            BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                                            BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                                            BudgetAssignmentFilter.BudgetID.SearchString = PartTimeBudget.BudgetID;
                                            BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                            BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                            BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                                        }
                                        if (BudgetAssignment == null)
                                        {
                                            BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                                            BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                                            BudgetAssignmentFilter.BudgetID.SearchString = WarehouseFacilitiesBudget.BudgetID;
                                            BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                            BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                            BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                                        }

                                        //No Budget - Inactivate current budget
                                        if (strBudgetSetting == "No Budget")
                                        {
                                            if (BudgetAssignment != null)
                                            {
                                                BudgetAssignment.InActive = true;
                                                BudgetAssignment.Update();
                                            }
                                        }
                                        else
                                        {
                                            if (BudgetAssignment != null)
                                            {
                                                BudgetAssignment.InActive = false;
                                                BudgetAssignment.Update();
                                            }
                                        }
                                        
                                        if (strBudgetSetting == "Part Time")
                                        {
                                            if (BudgetAssignment == null)
                                            {
                                                BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                                BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                                                BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                                BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                BudgetAssignment.Create();
                                            }
                                            else
                                            {
                                                if (BudgetAssignment.BudgetID != PartTimeBudget.BudgetID)
                                                {
                                                    BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                                                    BudgetAssignment.Update();
                                                }

                                                if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                                {
                                                    if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                                    {
                                                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                                        ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                                        BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                                        BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                                        BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                                        BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                        BudgetAssignmentAdjustment.Create();
                                                    }
                                                }
                                            }
                                        }

                                        if (strBudgetSetting == "Full Time")
                                        {
                                            if (BudgetAssignment == null)
                                            {
                                                BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                                BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                                                BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                                BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                BudgetAssignment.Create();
                                            }
                                            else
                                            {

                                                if (BudgetAssignment.BudgetID != FullTimeBudget.BudgetID)
                                                {
                                                    BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                                                    BudgetAssignment.Update();
                                                }

                                                if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                                {
                                                    if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                                    {
                                                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                                        ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                                        BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                                        BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                                        BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                                        BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                        BudgetAssignmentAdjustment.Create();
                                                    }
                                                }
                                            }
                                        }

                                        if (strBudgetSetting == "Warehouse/Facilities")
                                        {
                                            if (BudgetAssignment == null)
                                            {
                                                BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                                BudgetAssignment.BudgetID = WarehouseFacilitiesBudget.BudgetID;
                                                BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                                BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                BudgetAssignment.Create();
                                            }
                                            else
                                            {

                                                if (BudgetAssignment.BudgetID != WarehouseFacilitiesBudget.BudgetID)
                                                {
                                                    BudgetAssignment.BudgetID = WarehouseFacilitiesBudget.BudgetID;
                                                    BudgetAssignment.Update();
                                                }

                                                if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                                {
                                                    if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                                    {
                                                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                                        ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                                        BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                                        BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                                        BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                                        BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                        BudgetAssignmentAdjustment.Create();
                                                    }
                                                }
                                            }
                                        }

                                        if (strBudgetSetting == "Fleet")
                                        {
                                            if (BudgetAssignment == null)
                                            {
                                                BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                                BudgetAssignment.BudgetID = FleetBudget.BudgetID;
                                                BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                                BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                BudgetAssignment.Create();
                                            }
                                            else
                                            {

                                                if (BudgetAssignment.BudgetID != FleetBudget.BudgetID)
                                                {
                                                    BudgetAssignment.BudgetID = FleetBudget.BudgetID;
                                                    BudgetAssignment.Update();
                                                }

                                                if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                                {
                                                    if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                                    {
                                                        ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                                        ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                                        BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                                        BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                                        BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                                        BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                                        BudgetAssignmentAdjustment.Create();
                                                    }
                                                }
                                            }
                                        }


                                        //if (UserWebsite.IsPartTime)
                                        //{
                                        //    if (BudgetAssignment == null)
                                        //    {
                                        //        if (strRegion != "AZO")
                                        //        {
                                        //            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                        //            BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                                        //            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        //            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                        //            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        //            BudgetAssignment.Create();
                                        //        }
                                        //    }
                                        //    else
                                        //    {

                                        //        if (BudgetAssignment.BudgetID != PartTimeBudget.BudgetID)
                                        //        {
                                        //            BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                                        //            //blnIsNewEmployee = true;
                                        //        }

                                        //        BudgetAssignment.Update();

                                        //        //if (blnIsNewEmployee)
                                        //        if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                        //        {
                                        //            if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                        //            {
                                        //                ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                        //                ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                        //                BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                        //                BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                        //                BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                        //                BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        //            }
                                        //        }

                                        //        if (strRegion == "AZO")
                                        //        {
                                        //            BudgetAssignment.InActive = true;
                                        //            BudgetAssignment.Update();
                                        //        }
                                        //    }
                                        //}
                                        //else
                                        //{

                                        //    if (BudgetAssignment == null)
                                        //    {
                                        //        if (strRegion != "AZO")
                                        //        {
                                        //            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                        //            BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                                        //            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                        //            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                        //            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        //            BudgetAssignment.Create();
                                        //        }
                                        //    }
                                        //    else
                                        //    {

                                        //        if (BudgetAssignment.BudgetID != FullTimeBudget.BudgetID)
                                        //        {
                                        //            BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                                        //            //blnIsNewEmployee = true;
                                        //        }
                                        //        BudgetAssignment.Update();

                                        //        //if (blnIsNewEmployee)
                                        //        if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                                        //        {
                                        //            if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                        //            {
                                        //                ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                        //                ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                        //                BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                        //                BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                        //                BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                        //                BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        //            }
                                        //        }

                                        //        if (strRegion == "AZO")
                                        //        {
                                        //            BudgetAssignment.InActive = true;
                                        //            BudgetAssignment.Update();
                                        //        }
                                        //    }
                                        //}
                                    }

                                    UserWebsiteIDs.Add(UserWebsite.UserWebsiteID);

                                    EmployeeFileImport.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    EmployeeFileImport.Status = "Completed";
                                    EmployeeFileImport.ErrorMessage = String.Empty;
                                    EmployeeFileImport.Update();
                                }
                                catch (Exception ex)
                                {
                                    EmployeeFileImport.ErrorMessage = ex.Message;
                                    EmployeeFileImport.Status = "Failed";
                                    EmployeeFileImport.Update();
                                    Console.WriteLine(String.Format("{0}", ex.Message));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("{0}", ex.Message));
                        }                        
                    }

                    //Remove Users not on file
                    List<ImageSolutions.Employee.EmployeeFileImport> ProcessingEmployeeFileImports = new List<ImageSolutions.Employee.EmployeeFileImport>();
                    ImageSolutions.Employee.EmployeeFileImportFilter ProcessingEmployeeFileImportFilter = new ImageSolutions.Employee.EmployeeFileImportFilter();
                    ProcessingEmployeeFileImportFilter.Status = new Database.Filter.StringSearch.SearchFilter();
                    ProcessingEmployeeFileImportFilter.Status.SearchString = "Processing";
                    ProcessingEmployeeFileImports = ImageSolutions.Employee.EmployeeFileImport.GetEmployeeFileImports(ProcessingEmployeeFileImportFilter);

                    foreach(ImageSolutions.Employee.EmployeeFileImport _EmployeeFileImport in ProcessingEmployeeFileImports)
                    {
                        if(!string.IsNullOrEmpty(_EmployeeFileImport.UserWebsiteID))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(_EmployeeFileImport.UserWebsiteID);
                            foreach (ImageSolutions.User.UserAccount _UserAccount in UserWebsite.UserAccounts)
                            {
                                UserWebsite.InActive = true;
                                UserWebsite.Update();
                                //if (_UserAccount.UserWebsite.WebsiteID == Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]))
                                //{
                                //    _UserAccount.Delete();
                                //}
                            }
                        }
                    }

                    //foreach (string _UserWebsiteID in GetCurrentActiveEmployees().Where(x => !UserWebsiteIDs.All(x2 => x2 != x)))
                    //{
                    //    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite(_UserWebsiteID);
                    //    foreach(ImageSolutions.User.UserAccount _UserAccount in UserWebsite.UserAccounts)
                    //    {
                    //        if(_UserAccount.UserWebsite.WebsiteID == Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]))
                    //        {
                    //            _UserAccount.Delete();
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}"), ex.Message);
                //throw ex;
            }
            finally
            {
            }
        }

        public List<string> GetCurrentActiveEmployees()
        {
            List<string> objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                objReturn = new List<string>();
                strSQL = string.Format(@"
SELECT distinct ua.UserWebsiteID
FROM UserAccount (NOLOCK) ua
Inner Join UserWebsite (NOLOCK) uw on uw.UserWebsiteID = ua.UserWebsiteID
Inner Join Website (NOLOCK) w on w.WebsiteID = uw.WebsiteID
Inner Join WebsiteGroup (NOLOCK) wg on wg.WebsiteGroupID = ua.WebsiteGroupID
WHERE w.WebsiteID = {0}
and wg.GroupName in (
'Discount Tire Technicians'
,'America''s Tire Technicians'
,'DT - CAS Region Technicians'
,'AT - CAS Region Technicians'
,'Pit Pass Technicians'
)"
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"])));
                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    objReturn.Add(Convert.ToString(objRead["UserWebsiteID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }

        public string FindStore(string strStoreNumber)
        {
            string objReturn = null;
            int counter = 0;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT a.AccountID
FROM Account a
WHERE a.WebsiteID = {0}
--and a.AccountName like '%{1}'
and a.StoreNumber = '{1}'
"
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]))
                        , strStoreNumber);

                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    counter++;
                    objReturn = Convert.ToString(objRead["AccountID"]);
                    
                    if(counter > 1)
                    {
                        throw new Exception(string.Format("Multiple accounts found: {0}", strStoreNumber));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }

        public void UpdateEmployeeFileImportStatus(string websiteid, string status)
        {
            int counter = 0;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE EmployeeFileImport SET Status = {0} WHERE WebsiteID = {1}
"
                            , Database.HandleQuote(status)
                            , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]))
                        );

                counter = Database.ExecuteSQL(strSQL);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ImageSolutions.User.UserWebsite CreateUserWebsiteUserAccount(ImageSolutions.User.UserInfo UserInfo, ImageSolutions.User.UserWebsite UserWebsite
            , string strRegion, string strCostCenter, string strEmployeeID, string strTimeType, string strDOH, string strCompanyBrand)
        {
            //Get Account from Region/Cost Center
            //ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
            //ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
            //AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
            //AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
            //AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
            //AccountFilter.AccountName.SearchString = String.Format("{0} - {1}", strRegion, strCostCenter);
            //Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

            string strAccountID = FindStore(strCostCenter);
            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(strAccountID);

            if (Account == null)
            {
                throw new Exception(String.Format("Missing Account: {0}", String.Format("{0} - {1}", strRegion, strCostCenter)));
            }

            if (Account != null)
            {
                //Check NS for existing Customer
                //NetSuiteLibrary.Customer.Customer Customer = new NetSuiteLibrary.Customer.Customer();
                //NetSuiteLibrary.Customer.CustomerFilter CustomerFilter = new NetSuiteLibrary.Customer.CustomerFilter();
                //CustomerFilter.ParentInternalIDs = new List<string>();
                //CustomerFilter.ParentInternalIDs.Add(Account.CustomerInternalID);
                //CustomerFilter.StoreOrgNumber = strCostCenter;
                //CustomerFilter.StoreEmpID = strEmployeeID;
                //Customer = NetSuiteLibrary.Customer.Customer.GetCustomer(Service, CustomerFilter);

                if (UserWebsite == null)
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite();
                    UserWebsite.UserInfoID = UserInfo.UserInfoID;
                    UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    UserWebsite.IsPartTime = strTimeType == "P";
                    //if (strRegion == "AZO")
                    //{
                    //    UserWebsite.BudgetSetting = "No Budget";
                    //}
                    //else
                    //{
                    //    UserWebsite.BudgetSetting = strTimeType == "P" ? "Part Time" : "Full Time";
                    //}
                    UserWebsite.HiredDate = Convert.ToDateTime(strDOH);
                    UserWebsite.EmployeeID = strEmployeeID;
                    UserWebsite.PaymentTermID = "4"; //Payroll Deduction

                    UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);

                    UserWebsite.AddressPermission = "Account";
                    UserWebsite.AutoAssignGroup = true;
                    UserWebsite.AutoAssignBudget = true;
                    UserWebsite.Create();
                }
                else
                {
                    UserWebsite.IsPartTime = strTimeType == "P";
                    //if (strRegion == "AZO")
                    //{
                    //    UserWebsite.BudgetSetting = "No Budget";
                    //}
                    //else
                    //{
                    //    UserWebsite.BudgetSetting = strTimeType == "P" ? "Part Time" : "Full Time";
                    //}

                    UserWebsite.HiredDate = Convert.ToDateTime(strDOH);
                    UserWebsite.EmployeeID = strEmployeeID;
                    //UserWebsite.AddressPermission = "Account";
                    UserWebsite.InActive = false;
                    UserWebsite.Update();

                    foreach(ImageSolutions.User.UserAccount _UserAccount in UserWebsite.UserAccounts)
                    {
                        if(_UserAccount.AccountID != strAccountID)
                        {
                            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                            UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.WebsiteGroupID.SearchString = _UserAccount.WebsiteGroupID;
                            UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.AccountID.SearchString = strAccountID;
                            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);

                            if (UserAccount == null)
                            {
                                _UserAccount.AccountID = strAccountID;
                                _UserAccount.Update();

                                UserWebsite.IsUpdated = true;
                                UserWebsite.Update();
                            }
                            else
                            {
                                _UserAccount.Delete();
                            }
                        }
                    }
                }

                if(UserWebsite.AutoAssignGroup)
                {
                    if (strCostCenter.StartsWith("ZCL") || strCostCenter.StartsWith("ZCN"))
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "Americas Tire Fleet";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: Americas Tire Fleet");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }
                    else if (strCostCenter.StartsWith("Z"))
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "Discount Tire Fleet";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: Discount Tire Fleet");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }
                    else

                    //Identify and Assign Group
                    if (Account.ParentAccount.AccountName == "Discount Tire Company"
                        && strRegion != "CAS"
                        && strCostCenter != "GAA043" && strCostCenter != "GAA000")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "DT - Discount Tire Technicians";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: DT - Discount Tire Technicians");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }
                    else if (Account.ParentAccount.AccountName == "America's Tire Company" && strRegion != "CAS")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "DT - AT Technicians Search";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: DT - AT Technicians Search");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }
                    else if (Account.ParentAccount.AccountName.Contains("Tire Company") && strRegion == "CAS")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "DT - CAS Region Technicians";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: DT - CAS Region Technicians");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }

                        ImageSolutions.Website.WebsiteGroup WebsiteGroup2 = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter2 = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter2.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter2.GroupName.SearchString = "AT - CAS Region Technicians";
                        WebsiteGroup2 = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter2);

                        if (WebsiteGroup2 == null)
                        {
                            throw new Exception("Missing Group: AT - CAS Region Technicians");
                        }

                        ImageSolutions.User.UserAccount UserAccount2 = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter2 = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter2.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter2.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter2.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter2.WebsiteGroupID.SearchString = WebsiteGroup2.WebsiteGroupID;
                        UserAccount2 = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter2);
                        if (UserAccount2 == null)
                        {
                            UserAccount2 = new ImageSolutions.User.UserAccount();
                            UserAccount2.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount2.AccountID = Account.AccountID;
                            UserAccount2.WebsiteGroupID = WebsiteGroup2.WebsiteGroupID;
                            UserAccount2.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount2.Create();
                        }

                    }
                    //else if (Account.ParentAccount.AccountName.Contains("Tire Company") && strRegion == "CAS")
                    //{
                    //    ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                    //    ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                    //    WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                    //    WebsiteGroupFilter.GroupName.SearchString = "AT - CAS Region Technicians";
                    //    WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                    //    if (WebsiteGroup == null)
                    //    {
                    //        throw new Exception("Missing Group: AT - CAS Region Technicians");
                    //    }

                    //    ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                    //    ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                    //    UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    //    UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                    //    UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                    //    UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                    //    UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                    //    if (UserAccount == null)
                    //    {
                    //        UserAccount = new ImageSolutions.User.UserAccount();
                    //        UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                    //        UserAccount.AccountID = Account.AccountID;
                    //        UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                    //        UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                    //        UserAccount.Create();
                    //    }
                    //}
                    else if (strCostCenter == "GAA043" || strCostCenter == "GAA000")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "DT - Pit Pass Technicians";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: DT - Pit Pass Technicians");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }

                    else if (strCompanyBrand == "Dunn Tire")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "Dunn Tire";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: Dunn Tire");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }
                   
                    else if (strCompanyBrand == "Suburban Tire")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "Suburban Tire";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: Suburban Tire");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }

                    else if (strCompanyBrand == "Ellisville Tire and Service")
                    {
                        ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                        ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                        WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                        WebsiteGroupFilter.GroupName.SearchString = "Ellisville Tire";
                        WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                        if (WebsiteGroup == null)
                        {
                            throw new Exception("Missing Group: Ellisville Tire");
                        }

                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create();
                        }
                    }

                    else
                    {
                        throw new Exception("No Group Found");
                    }

                    
                }               
            }

            return UserWebsite;
        }

        public void TestProcessEmployee(string EmployeeFileImportID, string CompanyBrand)
        {
            ImageSolutions.Employee.EmployeeFileImport EmployeeFileImport = new ImageSolutions.Employee.EmployeeFileImport(EmployeeFileImportID);

            try
            {
                //Check User - create/update user
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = String.Format(@"{0}@dt.imageinc.com", EmployeeFileImport.EmployeeID);
                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                if (UserInfo == null)
                {
                    UserInfo = new ImageSolutions.User.UserInfo();

                    UserInfo.FirstName = EmployeeFileImport.FirstName;
                    UserInfo.LastName = EmployeeFileImport.LastName;
                    UserInfo.UserName = EmployeeFileImport.EmployeeID;
                    UserInfo.EmailAddress = String.Format(@"{0}@dt.imageinc.com", EmployeeFileImport.EmployeeID);
                    UserInfo.Password = EmployeeFileImport.Password;
                    UserInfo.ExternalID = EmployeeFileImport.EmployeeID;
                    UserInfo.DisplayName = EmployeeFileImport.FirstName;
                    UserInfo.Create();
                }
                else
                {
                    UserInfo.FirstName = EmployeeFileImport.FirstName;
                    UserInfo.LastName = EmployeeFileImport.LastName;
                    UserInfo.UserName = EmployeeFileImport.EmployeeID;
                    UserInfo.Password = EmployeeFileImport.Password;
                    UserInfo.ExternalID = EmployeeFileImport.EmployeeID;
                    UserInfo.DisplayName = EmployeeFileImport.FirstName;
                    UserInfo.Update();
                }

                //UserWebsite
                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                UserWebsite = CreateUserWebsiteUserAccount(UserInfo, UserWebsite, EmployeeFileImport.Region, EmployeeFileImport.CostCenter, EmployeeFileImport.EmployeeID, EmployeeFileImport.TimeType, Convert.ToString(EmployeeFileImport.DateOfHire), CompanyBrand);

                UserWebsite = new ImageSolutions.User.UserWebsite(UserWebsite.UserWebsiteID);

                if (UserWebsite.Accounts.Exists(x => !string.IsNullOrEmpty(x.BudgetSetting)) || !string.IsNullOrEmpty(UserWebsite.BudgetSetting))
                {
                    string strBudgetSetting = string.Empty;

                    if (!string.IsNullOrEmpty(UserWebsite.BudgetSetting))
                    {
                        strBudgetSetting = UserWebsite.BudgetSetting;
                    }
                    else
                    {
                        foreach (ImageSolutions.Account.Account _Account in UserWebsite.Accounts.FindAll(x => !string.IsNullOrEmpty(x.BudgetSetting)))
                        {
                            string strAccountBudgetSetting = string.Empty;

                            if (_Account.BudgetSetting == "Full Time/Part Time")
                            {
                                strAccountBudgetSetting = EmployeeFileImport.TimeType == "P" ? "Part Time" : "Full Time";
                            }
                            else
                            {
                                strAccountBudgetSetting = _Account.BudgetSetting;
                            }

                            if (!string.IsNullOrEmpty(strBudgetSetting) && strBudgetSetting != strAccountBudgetSetting)
                            {
                                throw new Exception("Multiple Budget Setting Detected");
                            }

                            strBudgetSetting = strAccountBudgetSetting;
                        }
                    }

                    //Find Budget
                    string strFullTimeBudgetName = string.Format("{0} - Full Time", DateTime.UtcNow.Year);
                    ImageSolutions.Budget.Budget FullTimeBudget = new ImageSolutions.Budget.Budget();
                    ImageSolutions.Budget.BudgetFilter FullTiemBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                    FullTiemBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    FullTiemBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    FullTiemBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    FullTiemBudgetFilter.BudgetName.SearchString = strFullTimeBudgetName;
                    FullTimeBudget = ImageSolutions.Budget.Budget.GetBudget(FullTiemBudgetFilter);
                    if (FullTimeBudget == null)
                    {
                        FullTimeBudget = new ImageSolutions.Budget.Budget();
                        FullTimeBudget.BudgetName = strFullTimeBudgetName;
                        FullTimeBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                        FullTimeBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                        FullTimeBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                        FullTimeBudget.BudgetAmount = Convert.ToDouble(275);
                        FullTimeBudget.AllowOverBudget = true;
                        FullTimeBudget.IncludeShippingAndTaxes = false;
                        FullTimeBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                        FullTimeBudget.Create();
                    }

                    string strPartTimeBudgetName = string.Format("{0} - Part Time", DateTime.UtcNow.Year);
                    ImageSolutions.Budget.Budget PartTimeBudget = new ImageSolutions.Budget.Budget();
                    ImageSolutions.Budget.BudgetFilter PartTimeBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                    PartTimeBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    PartTimeBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    PartTimeBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    PartTimeBudgetFilter.BudgetName.SearchString = strPartTimeBudgetName;
                    PartTimeBudget = ImageSolutions.Budget.Budget.GetBudget(PartTimeBudgetFilter);
                    if (PartTimeBudget == null)
                    {
                        PartTimeBudget = new ImageSolutions.Budget.Budget();
                        PartTimeBudget.BudgetName = strPartTimeBudgetName;
                        PartTimeBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                        PartTimeBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                        PartTimeBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                        PartTimeBudget.BudgetAmount = Convert.ToDouble(225);
                        PartTimeBudget.AllowOverBudget = true;
                        FullTimeBudget.IncludeShippingAndTaxes = false;
                        PartTimeBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                        PartTimeBudget.Create();
                    }

                    string strWarehouseFacilitiesBudgetName = string.Format("{0} - Warehouse Facilities", DateTime.UtcNow.Year);
                    ImageSolutions.Budget.Budget WarehouseFacilitiesBudget = new ImageSolutions.Budget.Budget();
                    ImageSolutions.Budget.BudgetFilter WarehouseFacilitiesBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                    WarehouseFacilitiesBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WarehouseFacilitiesBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    WarehouseFacilitiesBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    WarehouseFacilitiesBudgetFilter.BudgetName.SearchString = strWarehouseFacilitiesBudgetName;
                    WarehouseFacilitiesBudget = ImageSolutions.Budget.Budget.GetBudget(WarehouseFacilitiesBudgetFilter);
                    if (WarehouseFacilitiesBudget == null)
                    {
                        WarehouseFacilitiesBudget = new ImageSolutions.Budget.Budget();
                        WarehouseFacilitiesBudget.BudgetName = strWarehouseFacilitiesBudgetName;
                        WarehouseFacilitiesBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                        WarehouseFacilitiesBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                        WarehouseFacilitiesBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                        WarehouseFacilitiesBudget.BudgetAmount = Convert.ToDouble(400);
                        WarehouseFacilitiesBudget.AllowOverBudget = true;
                        WarehouseFacilitiesBudget.IncludeShippingAndTaxes = false;
                        WarehouseFacilitiesBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                        WarehouseFacilitiesBudget.Create();
                    }

                    string strFleetBudgetName = string.Format("{0} - Fleet Budget", DateTime.UtcNow.Year);
                    ImageSolutions.Budget.Budget FleetBudget = new ImageSolutions.Budget.Budget();
                    ImageSolutions.Budget.BudgetFilter FleetBudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                    FleetBudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    FleetBudgetFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                    FleetBudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                    FleetBudgetFilter.BudgetName.SearchString = strFleetBudgetName;
                    FleetBudget = ImageSolutions.Budget.Budget.GetBudget(FleetBudgetFilter);
                    if (FleetBudget == null)
                    {
                        FleetBudget = new ImageSolutions.Budget.Budget();
                        FleetBudget.BudgetName = strFleetBudgetName;
                        FleetBudget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                        FleetBudget.StartDate = Convert.ToDateTime(String.Format("1/1/{0}", DateTime.UtcNow.Year));
                        FleetBudget.EndDate = Convert.ToDateTime(String.Format("12/31/{0}", DateTime.UtcNow.Year));
                        FleetBudget.BudgetAmount = Convert.ToDouble(400);
                        FleetBudget.AllowOverBudget = true;
                        FleetBudget.IncludeShippingAndTaxes = false;
                        FleetBudget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                        FleetBudget.Create();
                    }


                    ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                    ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                    BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetAssignmentFilter.BudgetID.SearchString = FullTimeBudget.BudgetID;
                    BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                    BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                    if (BudgetAssignment == null)
                    {
                        BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                        BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.BudgetID.SearchString = PartTimeBudget.BudgetID;
                        BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                    }
                    if (BudgetAssignment == null)
                    {
                        BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                        BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.BudgetID.SearchString = WarehouseFacilitiesBudget.BudgetID;
                        BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        BudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);
                    }

                    //No Budget - Inactivate current budget
                    if (strBudgetSetting == "No Budget")
                    {
                        if (BudgetAssignment != null)
                        {
                            BudgetAssignment.InActive = true;
                            BudgetAssignment.Update();
                        }
                    }
                    else
                    {
                        if (BudgetAssignment != null)
                        {
                            BudgetAssignment.InActive = false;
                            BudgetAssignment.Update();
                        }
                    }

                    if (strBudgetSetting == "Part Time")
                    {
                        if (BudgetAssignment == null)
                        {
                            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            BudgetAssignment.Create();
                        }
                        else
                        {
                            if (BudgetAssignment.BudgetID != PartTimeBudget.BudgetID)
                            {
                                BudgetAssignment.BudgetID = PartTimeBudget.BudgetID;
                                BudgetAssignment.Update();
                            }

                            if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                            {
                                if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                {
                                    ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                    ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                    BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                    BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                    BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                    BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BudgetAssignmentAdjustment.Create();
                                }
                            }
                        }
                    }

                    if (strBudgetSetting == "Full Time")
                    {
                        if (BudgetAssignment == null)
                        {
                            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            BudgetAssignment.Create();
                        }
                        else
                        {

                            if (BudgetAssignment.BudgetID != FullTimeBudget.BudgetID)
                            {
                                BudgetAssignment.BudgetID = FullTimeBudget.BudgetID;
                                BudgetAssignment.Update();
                            }

                            if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                            {
                                if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                {
                                    ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                    ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                    BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                    BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                    BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                    BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BudgetAssignmentAdjustment.Create();
                                }
                            }
                        }
                    }

                    if (strBudgetSetting == "Warehouse/Facilities")
                    {
                        if (BudgetAssignment == null)
                        {
                            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            BudgetAssignment.BudgetID = WarehouseFacilitiesBudget.BudgetID;
                            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            BudgetAssignment.Create();
                        }
                        else
                        {

                            if (BudgetAssignment.BudgetID != WarehouseFacilitiesBudget.BudgetID)
                            {
                                BudgetAssignment.BudgetID = WarehouseFacilitiesBudget.BudgetID;
                                BudgetAssignment.Update();
                            }

                            if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                            {
                                if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                {
                                    ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                    ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                    BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                    BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                    BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                    BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BudgetAssignmentAdjustment.Create();
                                }
                            }
                        }
                    }

                    if (strBudgetSetting == "Fleet")
                    {
                        if (BudgetAssignment == null)
                        {
                            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            BudgetAssignment.BudgetID = FleetBudget.BudgetID;
                            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            BudgetAssignment.Create();
                        }
                        else
                        {

                            if (BudgetAssignment.BudgetID != FleetBudget.BudgetID)
                            {
                                BudgetAssignment.BudgetID = FleetBudget.BudgetID;
                                BudgetAssignment.Update();
                            }

                            if (EmployeeFileImport.Status == "New" || EmployeeFileImport.Status == "Renew")
                            {
                                if (BudgetAssignment.Payments != null && BudgetAssignment.Payments.Count > 0)
                                {
                                    ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(UserInfo.UserInfoID, BudgetAssignment.BudgetAssignmentID);

                                    ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                                    BudgetAssignmentAdjustment.BudgetAssignmentID = BudgetAssignment.BudgetAssignmentID;
                                    BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);
                                    BudgetAssignmentAdjustment.Reason = "Renew Budget";
                                    BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BudgetAssignmentAdjustment.Create();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.Message));
            }

        }
    }
}
