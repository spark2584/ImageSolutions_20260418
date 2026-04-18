using ImageSolutions.Sprouts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ScheduledTask.Task.Sprouts
{
    public class UpdateCustomer
    {
        public bool Execute()
        {
            List<SproutsCustomerImport> SproutsCustomerImports = null;
            SproutsCustomerImportFilter SproutsCustomerImportFilter = null;

            try
            {
                SproutsCustomerImports = new List<SproutsCustomerImport>();
                SproutsCustomerImportFilter = new SproutsCustomerImportFilter();
                SproutsCustomerImportFilter.IsProcessed = false;
                SproutsCustomerImportFilter.IsEncrypted = false;

                //SproutsCustomerImportFilter.SproutsCustomerImportID = new Database.Filter.StringSearch.SearchFilter();
                //SproutsCustomerImportFilter.SproutsCustomerImportID.SearchString = "74";

                SproutsCustomerImports = SproutsCustomerImport.GetSproutsCustomerImports(SproutsCustomerImportFilter);

                foreach (SproutsCustomerImport _SproutsCustomerImport in SproutsCustomerImports)
                {
                    if (string.IsNullOrEmpty(_SproutsCustomerImport.ErrorMessage))
                    {
                        try
                        {
                            string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _SproutsCustomerImport.FilePath); //Get path to saved file
                            string strExtension = Path.GetExtension(strFullPath);
                            //string strConnectionString = String.Empty;

                            switch (strExtension)
                            {
                                //case ".xls": //Excel 97-03
                                //    strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFullPath + @";Extended Properties=""Excel 8.0;HDR=YES;""";
                                //    ProcessExcel(strConnectionString);
                                //    break;
                                //case ".xlsx": //Excel 07
                                //    strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + strFullPath + "';Extended Properties=Excel 12.0;";
                                //    ProcessExcel(strConnectionString);
                                //    break;
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
                            if (File.Exists(strArchivePath))
                            {
                                File.Delete(strArchivePath);
                            }
                            File.Move(strFullPath, strArchivePath);

                            _SproutsCustomerImport.IsProcessed = true;
                            _SproutsCustomerImport.ErrorMessage = String.Empty;
                            _SproutsCustomerImport.Update();
                        }
                        catch (Exception ex)
                        {
                            _SproutsCustomerImport.ErrorMessage = ex.Message;
                            _SproutsCustomerImport.Update();
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
                SproutsCustomerImports = null;
                SproutsCustomerImportFilter = null;
            }
            return true;
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

                        try
                        {
                            if (counter == 1)
                            {
                                HeaderColumnValues = strCurrentLine.Split(',');
                                foreach (string _value in HeaderColumnValues)
                                {
                                    HeaderFields.Add(_value);
                                }

                                if (!HeaderFields.Contains("Team Member ID")) throw new Exception("[Team Member ID] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Cost Center - ID")) throw new Exception("[Cost Center - ID] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("CF EV Employee Status")) throw new Exception("[CF EV Employee Status] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Hire Date")) throw new Exception("[Hire Date] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("CF_EE - Termination Date")) throw new Exception("[CF_EE - Termination Date] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("CF_EE - Email Address")) throw new Exception("[CF_EE - Email Address] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Primary Work Mobile Phone")) throw new Exception("[Primary Work Mobile Phone] is missing from the file, contact administrator");
                            }
                            else
                            {
                                String[] ColumnValues = null;
                                ColumnValues = strCurrentLine.Split(',');

                                string strTeamMemberID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Team Member ID")]).Replace("\"", String.Empty).Trim();
                                string strCostCenterID = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Cost Center - ID")]).Replace("\"", String.Empty).Trim().Replace("\"", "");
                                string strEmployeeStatus = Convert.ToString(ColumnValues[HeaderFields.IndexOf("CF EV Employee Status")]).Replace("\"", String.Empty).Trim();
                                string strHireDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Hire Date")]).Replace("\"", String.Empty).Trim();
                                string strTerminationDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("CF_EE - Termination Date")]).Replace("\"", String.Empty).Trim();
                                string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("CF_EE - Email Address")]).Replace("\"", String.Empty).Trim();
                                string strMobilePhone = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Primary Work Mobile Phone")]).Replace("\"", String.Empty).Trim();

                                if (!string.IsNullOrEmpty(strTeamMemberID))
                                {
                                    SproutsCustomer SproutsCustomer = new SproutsCustomer();
                                    SproutsCustomerFilter SproutsCustomerFilter = new SproutsCustomerFilter();

                                    SproutsCustomerFilter.TeamMemberID = new Database.Filter.StringSearch.SearchFilter();
                                    SproutsCustomerFilter.TeamMemberID.SearchString = strTeamMemberID;

                                    SproutsCustomer = SproutsCustomer.GetSproutsCustomer(SproutsCustomerFilter);

                                    if (SproutsCustomer == null)
                                    {
                                        SproutsCustomer = new SproutsCustomer();
                                        SproutsCustomer.TeamMemberID = strTeamMemberID;
                                        SproutsCustomer.CostCenterID = strCostCenterID;
                                        SproutsCustomer.Status = strEmployeeStatus;
                                        SproutsCustomer.HireDate = Convert.ToDateTime(strHireDate);
                                        SproutsCustomer.TerminationDate = string.IsNullOrEmpty(strTerminationDate) ? (DateTime?)null : Convert.ToDateTime(strTerminationDate);
                                        SproutsCustomer.Email = strEmail;
                                        SproutsCustomer.MobilePhone = strMobilePhone;
                                        if (filepath.ToLower().Contains("anniversary"))
                                        {
                                            SproutsCustomer.EnableBudget = true;
                                        }
                                        SproutsCustomer.IsUpdated = true;
                                        SproutsCustomer.ErrorMessage = String.Empty;
                                        SproutsCustomer.Create();
                                    }
                                    else
                                    {

                                        SproutsCustomer.TeamMemberID = strTeamMemberID;
                                        SproutsCustomer.CostCenterID = strCostCenterID;
                                        SproutsCustomer.Status = strEmployeeStatus;
                                        SproutsCustomer.HireDate = Convert.ToDateTime(strHireDate);
                                        SproutsCustomer.TerminationDate = string.IsNullOrEmpty(strTerminationDate) ? (DateTime?)null : Convert.ToDateTime(strTerminationDate);
                                        SproutsCustomer.Email = strEmail;
                                        SproutsCustomer.MobilePhone = strMobilePhone;
                                        if (filepath.ToLower().Contains("anniversary"))
                                        {
                                            SproutsCustomer.EnableBudget = true;
                                        }
                                        SproutsCustomer.IsUpdated = true;
                                        SproutsCustomer.ErrorMessage = String.Empty;
                                        SproutsCustomer.Update();
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
