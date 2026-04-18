using ImageSolutions.Brightspeed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ScheduledTask.Task.Brightspeed
{
    public class UpdateCustomer
    {
        public bool Execute()
        {
            List<BrightspeedCustomerImport> BrightspeedCustomerImports = null;
            BrightspeedCustomerImportFilter BrightspeedCustomerImportFilter = null;

            try
            {
                BrightspeedCustomerImports = new List<BrightspeedCustomerImport>();
                BrightspeedCustomerImportFilter = new BrightspeedCustomerImportFilter();
                BrightspeedCustomerImportFilter.IsProcessed = false;
                BrightspeedCustomerImportFilter.IsStore = false;
                BrightspeedCustomerImportFilter.IsEncrypted = false;

                //BrightspeedCustomerImportFilter.BrightspeedCustomerImportID = new Database.Filter.StringSearch.SearchFilter();
                //BrightspeedCustomerImportFilter.BrightspeedCustomerImportID.SearchString = "1";

                BrightspeedCustomerImports = BrightspeedCustomerImport.GetBrightspeedCustomerImports(BrightspeedCustomerImportFilter);

                foreach (BrightspeedCustomerImport _BrightspeedCustomerImport in BrightspeedCustomerImports)
                {
                    try
                    {
                        string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _BrightspeedCustomerImport.FilePath); //Get path to saved file
                        string strExtension = Path.GetExtension(strFullPath);
                        string strConnectionString = String.Empty;

                        switch (strExtension)
                        {
                            case ".xls": //Excel 97-03
                                strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;""";
                                ProcessExcel(strConnectionString);
                                break;
                            //case ".xlsx": //Excel 07
                            //    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                            //    ProcessExcel(strConnectionString);
                            //    break;
                            //case ".csv":
                            //    ProcessCSV(strFullPath);
                            //    break;
                        }

                        string strArchiveFolder = string.Format(@"{0}\Archive", Path.GetDirectoryName(strFullPath));
                        if (!Directory.Exists(strArchiveFolder))
                        {
                            Directory.CreateDirectory(strArchiveFolder);
                        }

                        string strArchivePath = string.Format(@"{0}\{1}", strArchiveFolder, Path.GetFileName(strFullPath));
                        File.Move(strFullPath, strArchivePath);

                        _BrightspeedCustomerImport.IsProcessed = true;
                        _BrightspeedCustomerImport.ErrorMessage = String.Empty;
                        _BrightspeedCustomerImport.Update();
                    }
                    catch (Exception ex)
                    {
                        _BrightspeedCustomerImport.ErrorMessage = ex.Message;
                        _BrightspeedCustomerImport.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                BrightspeedCustomerImports = null;
                BrightspeedCustomerImportFilter = null;
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
                        //for (int i = 0; i < objSchema.Rows.Count; i++)
                        //{
                        //    strSheetName = objSchema.Rows[i]["TABLE_NAME"].ToString() + "A1:U65535";
                        //}

                        strSheetName = objSchema.Rows[0]["TABLE_NAME"].ToString() + "A1:U65535";
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

                            //if (!objColumns.Contains("group_branch")) throw new Exception("[group_branch] is missing from the report, excel contact administrator");

                            string strSyncID = String.Format("{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"));

                            for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                            {
                                Console.WriteLine(i);

                                string strEmployeeID = Convert.ToString(objData.Tables[0].Rows[i]["User/Employee ID"]).Trim();
                                string strDisplayName = Convert.ToString(objData.Tables[0].Rows[i]["Display Name"]).Trim();
                                string strEmail = Convert.ToString(objData.Tables[0].Rows[i]["Company  Email Information Email Address"]).Trim();
                                string strTitle = Convert.ToString(objData.Tables[0].Rows[i]["Title"]).Trim();
                                string strManager = Convert.ToString(objData.Tables[0].Rows[i]["Manager"]).Trim();
                                string strBusinessUnit = Convert.ToString(objData.Tables[0].Rows[i]["Business Unit Business Unit Name"]).Trim();
                                string strDivisionName = Convert.ToString(objData.Tables[0].Rows[i]["Division Name"]).Trim();
                                string strDepartmentName = Convert.ToString(objData.Tables[0].Rows[i]["Department Name"]).Trim();
                                string strCostCenterCode = Convert.ToString(objData.Tables[0].Rows[i]["Cost Center Code"]).Trim();
                                string strLocation = Convert.ToString(objData.Tables[0].Rows[i]["Location"]).Trim();
                                string strEmployeeStatus = Convert.ToString(objData.Tables[0].Rows[i]["Employee Status"]).Trim();
                                string strPersonnelSubArea = Convert.ToString(objData.Tables[0].Rows[i]["Personnel Sub Area"]).Trim();
                                string strJobCode = Convert.ToString(objData.Tables[0].Rows[i]["Job Classification Job Code"]).Trim();
                                string strEmployeeHireDate = Convert.ToString(objData.Tables[0].Rows[i]["Employment Details Hire Date"]).Trim();

                                string strCustomerID = GetCustomerID(strEmployeeID);

                                BrightspeedCustomer Customer = string.IsNullOrEmpty(strCustomerID) ? new BrightspeedCustomer() : new BrightspeedCustomer(strCustomerID);

                                if (!string.IsNullOrEmpty(strEmail))
                                {
                                    if (string.IsNullOrEmpty(strCustomerID))
                                    {
                                        Customer = new BrightspeedCustomer();
                                        Customer.EmployeeID = strEmployeeID;
                                        Customer.DisplayName = strDisplayName;

                                        if (!IsValidEmail(strEmail, string.Empty))
                                        {
                                            throw new Exception(String.Format(@"Invalid Email: {0}", strEmail));
                                        }

                                        Customer.Email = strEmail; //Create email using Group Branch
                                        Customer.Title = strTitle;
                                        Customer.Manager = strManager;
                                        Customer.BusinessUnit = strBusinessUnit;
                                        Customer.DivisionName = strDivisionName;
                                        Customer.DepartmentName = strDepartmentName;
                                        Customer.CostCenterCode = strCostCenterCode;
                                        Customer.Location = strLocation;
                                        Customer.EmployeeStatus = strEmployeeStatus;
                                        Customer.PersonnelSubArea = strPersonnelSubArea;
                                        Customer.JobCode = strJobCode;

                                        Customer.HireDate = Convert.ToDateTime(strEmployeeHireDate);

                                        Customer.IsUpdated = true;
                                        Customer.SyncID = strSyncID;
                                        Customer.Create();
                                    }
                                    else
                                    {
                                        Customer.SyncID = strSyncID;
                                        Customer.Update();

                                        if (Customer.DisplayName != strDisplayName ||
                                            Customer.Email != strEmail ||
                                            Customer.Title != strTitle ||
                                            Customer.Manager != strManager ||
                                            Customer.BusinessUnit != strBusinessUnit ||
                                            Customer.DivisionName != strDivisionName ||
                                            Customer.DepartmentName != strDepartmentName ||
                                            Customer.CostCenterCode != strCostCenterCode ||
                                            Customer.Location != strLocation ||
                                            Customer.EmployeeStatus != strEmployeeStatus ||
                                            Customer.PersonnelSubArea != strPersonnelSubArea ||
                                            Customer.JobCode != strJobCode ||
                                            Customer.HireDate != Convert.ToDateTime(strEmployeeHireDate) ||
                                            Customer.InActive
                                        )
                                        {
                                            Customer.DisplayName = strDisplayName;

                                            if (!IsValidEmail(strEmail, Customer.BrightspeedCustomerID))
                                            {
                                                throw new Exception(String.Format(@"Invalid Email: {0}", strEmail));
                                            }

                                            Customer.Email = strEmail; //Create email using Group Branch
                                            Customer.Title = strTitle;
                                            Customer.Manager = strManager;
                                            Customer.BusinessUnit = strBusinessUnit;
                                            Customer.DivisionName = strDivisionName;
                                            Customer.DepartmentName = strDepartmentName;
                                            Customer.CostCenterCode = strCostCenterCode;
                                            Customer.Location = strLocation;
                                            Customer.EmployeeStatus = strEmployeeStatus;
                                            Customer.PersonnelSubArea = strPersonnelSubArea;
                                            Customer.JobCode = strJobCode;

                                            Customer.HireDate = Convert.ToDateTime(strEmployeeHireDate);
                                            Customer.InActive = false;

                                            Customer.IsUpdated = true;
                                            Customer.ErrorMessage = String.Empty;
                                            Customer.Update();
                                        }
                                    }
                                }
                            }

                            UpdateInActiveCustomer(strSyncID);
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

        public string GetCustomerID(string employeeid)
        {
            string strReturn = string.Empty;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"SELECT BrightspeedCustomerID FROM BrightspeedCustomer (NOLOCK) WHERE EmployeeID = {0} "
                    , Database.HandleQuote(employeeid));
                objRead = Database.GetDataReader(strSQL);

                if (objRead.Read())
                {
                    strReturn = Convert.ToString(objRead["BrightspeedCustomerID"]);
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
                List<BrightspeedCustomer> Customers = new List<BrightspeedCustomer>();
                BrightspeedCustomerFilter CustomerFilter = new BrightspeedCustomerFilter();
                CustomerFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                CustomerFilter.Email.SearchString = email;
                Customers = BrightspeedCustomer.GetBrightspeedCustomers(CustomerFilter);

                foreach (BrightspeedCustomer _Customer in Customers)
                {
                    if (string.IsNullOrEmpty(customerid) ||
                        (_Customer.BrightspeedCustomerID != customerid)
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

        public string UpdateInActiveCustomer(string syncid)
        {
            string objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE ec SET InActive = 1, IsUpdated = 1
FROM BrightspeedCustomer (NOLOCK) ec
Inner Join UserInfo (NOLOCK) ui on ui.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = ui.UserInfoID
WHERE uw.WebsiteID = {1}
and ISNULL(ec.SyncID,'') != {0}
and ec.InActive = 0
"
                        , Database.HandleQuote(syncid)
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["BrightspeedWebsiteID"]))
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
