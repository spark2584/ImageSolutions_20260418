using ImageSolutions.MavisTire;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace ScheduledTask.Task.MavisTire
{
    public class UpdateCustomer
    {
        public bool Execute()
        {
            List<MavisTireCustomerImport> MavisTireCustomerImports = null;
            MavisTireCustomerImportFilter MavisTireCustomerImportFilter = null;

            try
            {
                MavisTireCustomerImports = new List<MavisTireCustomerImport>();
                MavisTireCustomerImportFilter = new MavisTireCustomerImportFilter();
                MavisTireCustomerImportFilter.IsProcessed = false;
                MavisTireCustomerImportFilter.IsEncrypted = false;

                //MavisTireCustomerImportFilter.MavisTireCustomerImportID = new Database.Filter.StringSearch.SearchFilter();
                //MavisTireCustomerImportFilter.MavisTireCustomerImportID.SearchString = "74";

                MavisTireCustomerImports = MavisTireCustomerImport.GetMavisTireCustomerImports(MavisTireCustomerImportFilter);

                foreach (MavisTireCustomerImport _MavisTireCustomerImport in MavisTireCustomerImports)
                {
                    if (string.IsNullOrEmpty(_MavisTireCustomerImport.ErrorMessage))
                    {
                        try
                        {
                            string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _MavisTireCustomerImport.FilePath); //Get path to saved file
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
                                case ".txt":
                                    ProcessTxt(strFullPath);
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

                            _MavisTireCustomerImport.IsProcessed = true;
                            _MavisTireCustomerImport.ErrorMessage = String.Empty;
                            _MavisTireCustomerImport.Update();
                        }
                        catch (Exception ex)
                        {
                            _MavisTireCustomerImport.ErrorMessage = ex.Message;
                            _MavisTireCustomerImport.Update();
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
                MavisTireCustomerImports = null;
                MavisTireCustomerImportFilter = null;
            }
            return true;
        }

        public void ProcessTxt(string filepath)
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
                                HeaderColumnValues = strCurrentLine.Split('\t');
                                foreach (string _value in HeaderColumnValues)
                                {
                                    HeaderFields.Add(_value);
                                }

                                if (!HeaderFields.Contains("EmployeeNumber")) throw new Exception("[EmployeeNumber] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("EmployeeName")) throw new Exception("[EmployeeName] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("StoreNumber")) throw new Exception("[StoreNumber] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("MailingName")) throw new Exception("[MailingName] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("HireDate")) throw new Exception("[HireDate] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("TerminationDate")) throw new Exception("[TerminationDate] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("DaysEmployed")) throw new Exception("[DaysEmployed] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Position")) throw new Exception("[Position] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("PositionStatus")) throw new Exception("[PositionStatus] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("PositionArea")) throw new Exception("[PositionArea] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Email")) throw new Exception("[Email] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("WorkEmail")) throw new Exception("[WorkEmail] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("HomePhone")) throw new Exception("[HomePhone] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("MobilePhone")) throw new Exception("[MobilePhone] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Territory")) throw new Exception("[Territory] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("Brand")) throw new Exception("[Brand] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("UniformBrand")) throw new Exception("[UniformBrand] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("LocationAddress")) throw new Exception("[LocationAddress] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("LocationCity")) throw new Exception("[LocationCity] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("LocationState")) throw new Exception("[LocationState] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("LocationZip")) throw new Exception("[LocationZip] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("RVP")) throw new Exception("[RVP] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("RD")) throw new Exception("[RD] is missing from the file, contact administrator");
                                if (!HeaderFields.Contains("RTM")) throw new Exception("[RTM] is missing from the file, contact administrator");
                            }
                            else
                            {
                                String[] ColumnValues = null;
                                ColumnValues = strCurrentLine.Split('\t');

                                string strEmployeeNumber = Convert.ToString(ColumnValues[HeaderFields.IndexOf("EmployeeNumber")]).Replace("\"", String.Empty).Trim();
                                string strEmployeeName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("EmployeeName")]).Replace("\"", String.Empty).Trim().Replace("\"","");
                                string strStoreNumber = Convert.ToString(ColumnValues[HeaderFields.IndexOf("StoreNumber")]).Replace("\"", String.Empty).Trim();
                                string strMailingName = Convert.ToString(ColumnValues[HeaderFields.IndexOf("MailingName")]).Replace("\"", String.Empty).Trim();
                                string strHireDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HireDate")]).Replace("\"", String.Empty).Trim();
                                string strTerminationDate = Convert.ToString(ColumnValues[HeaderFields.IndexOf("TerminationDate")]).Replace("\"", String.Empty).Trim();
                                string strDaysEmployed = Convert.ToString(ColumnValues[HeaderFields.IndexOf("DaysEmployed")]).Replace("\"", String.Empty).Trim();
                                string strPosition = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Position")]).Replace("\"", String.Empty).Trim();
                                string strPositionStatus = Convert.ToString(ColumnValues[HeaderFields.IndexOf("PositionStatus")]).Replace("\"", String.Empty).Trim();
                                string strPositionArea = Convert.ToString(ColumnValues[HeaderFields.IndexOf("PositionArea")]).Replace("\"", String.Empty).Trim();
                                string strEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Email")]).Replace("\"", String.Empty).Trim();
                                string strWorkEmail = Convert.ToString(ColumnValues[HeaderFields.IndexOf("WorkEmail")]).Replace("\"", String.Empty).Trim();
                                string strHomePhone = Convert.ToString(ColumnValues[HeaderFields.IndexOf("HomePhone")]).Replace("\"", String.Empty).Trim();
                                string strMobilePhone = Convert.ToString(ColumnValues[HeaderFields.IndexOf("MobilePhone")]).Replace("\"", String.Empty).Trim();
                                string strTerritory = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Territory")]).Replace("\"", String.Empty).Trim();
                                string strBrand = Convert.ToString(ColumnValues[HeaderFields.IndexOf("Brand")]).Replace("\"", String.Empty).Trim();
                                string strUniformBrand = Convert.ToString(ColumnValues[HeaderFields.IndexOf("UniformBrand")]).Replace("\"", String.Empty).Trim();
                                string strLocationAddress = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LocationAddress")]).Replace("\"", String.Empty).Trim();
                                string strLocationCity = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LocationCity")]).Replace("\"", String.Empty).Trim();
                                string strLocationState = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LocationState")]).Replace("\"", String.Empty).Trim();
                                string strLocationZip = Convert.ToString(ColumnValues[HeaderFields.IndexOf("LocationZip")]).Replace("\"", String.Empty).Trim();
                                string strRVP = Convert.ToString(ColumnValues[HeaderFields.IndexOf("RVP")]).Replace("\"", String.Empty).Trim();
                                string strRD = Convert.ToString(ColumnValues[HeaderFields.IndexOf("RD")]).Replace("\"", String.Empty).Trim();
                                string strRTM = Convert.ToString(ColumnValues[HeaderFields.IndexOf("RTM")]).Replace("\"", String.Empty).Trim();

                                if (!string.IsNullOrEmpty(strEmployeeNumber))
                                {
                                    MavisTireCustomer MavisTireCustomer = new MavisTireCustomer();
                                    MavisTireCustomerFilter MavisTireCustomerFilter = new MavisTireCustomerFilter();

                                    MavisTireCustomerFilter.EmployeeNumber = new Database.Filter.StringSearch.SearchFilter();
                                    MavisTireCustomerFilter.EmployeeNumber.SearchString = strEmployeeNumber;

                                    MavisTireCustomer = MavisTireCustomer.GetMavisTireCustomer(MavisTireCustomerFilter);

                                    if (MavisTireCustomer == null)
                                    {
                                        MavisTireCustomer = new MavisTireCustomer();
                                        MavisTireCustomer.EmployeeNumber = strEmployeeNumber;
                                        MavisTireCustomer.EmployeeName = strEmployeeName;
                                        MavisTireCustomer.StoreNumber = strStoreNumber;
                                        MavisTireCustomer.MailingName = strMailingName;
                                        MavisTireCustomer.HireDate = Convert.ToDateTime(strHireDate);
                                        MavisTireCustomer.TerminationDate = string.IsNullOrEmpty(strTerminationDate) ? (DateTime?)null : Convert.ToDateTime(strTerminationDate);
                                        MavisTireCustomer.DaysEmployed = string.IsNullOrEmpty(strDaysEmployed) ? 0 : Convert.ToInt32(strDaysEmployed);
                                        MavisTireCustomer.Position = strPosition;
                                        MavisTireCustomer.PositionStatus = strPositionStatus;
                                        MavisTireCustomer.PositionArea = strPositionArea;
                                        MavisTireCustomer.Email = strEmail;
                                        MavisTireCustomer.WorkEmail = strWorkEmail;
                                        MavisTireCustomer.HomePhone = strHomePhone;
                                        MavisTireCustomer.MobilePhone = strMobilePhone;
                                        MavisTireCustomer.Territory = strTerritory;
                                        MavisTireCustomer.Brand = strBrand;
                                        MavisTireCustomer.UniformBrand = strUniformBrand;
                                        MavisTireCustomer.LocationAddress = strLocationAddress;
                                        MavisTireCustomer.LocationCity = strLocationCity;
                                        MavisTireCustomer.LocationState = strLocationState;
                                        MavisTireCustomer.LocationZip = strLocationZip;
                                        MavisTireCustomer.RVP = strRVP;
                                        MavisTireCustomer.RD = strRD;
                                        MavisTireCustomer.RTM = strRTM;
                                        MavisTireCustomer.IsUpdated = true;
                                        MavisTireCustomer.ErrorMessage = String.Empty;

                                        MavisTireCustomer.SendNewHireEmail = true;

                                        MavisTireCustomer.Create();
                                    }
                                    else
                                    {
                                        string strCurrentGroup = string.Format("{0} - {1} {2}"
                                            , MavisTireCustomer.UniformBrand
                                            , MavisTireCustomer.Territory
                                            , MavisTireCustomer.PositionArea);


                                        string strNewGroup = string.Format("{0} - {1} {2}"
                                            , strUniformBrand
                                            , strTerritory
                                            , strPositionArea);

                                        if (strCurrentGroup != strNewGroup)
                                        {
                                            MavisTireCustomer.ResetPackage = true;
                                            MavisTireCustomer.SendRegionChangeEmail = true;
                                        }


                                        MavisTireCustomer.EmployeeNumber = strEmployeeNumber;
                                        MavisTireCustomer.EmployeeName = strEmployeeName;
                                        MavisTireCustomer.StoreNumber = strStoreNumber;
                                        MavisTireCustomer.MailingName = strMailingName;
                                        MavisTireCustomer.HireDate = Convert.ToDateTime(strHireDate);
                                        MavisTireCustomer.TerminationDate = string.IsNullOrEmpty(strTerminationDate) ? (DateTime?)null : Convert.ToDateTime(strTerminationDate);
                                        MavisTireCustomer.DaysEmployed = string.IsNullOrEmpty(strDaysEmployed) ? 0 : Convert.ToInt32(strDaysEmployed);
                                        MavisTireCustomer.Position = strPosition;

                                        if (
                                            (MavisTireCustomer.PositionStatus == "Terminated" || MavisTireCustomer.PositionStatus == "Promoted")
                                            && strPositionStatus == "Active"
                                        )
                                        {
                                            MavisTireCustomer.ResetPackage = true;
                                            MavisTireCustomer.PositionStatus = strPositionStatus;
                                            MavisTireCustomer.SendRegionChangeEmail = false;
                                            MavisTireCustomer.SendPositionChangeEmail = true;
                                        }
                                        else
                                        {
                                            MavisTireCustomer.PositionStatus = strPositionStatus;
                                        }

                                        MavisTireCustomer.PositionArea = strPositionArea;
                                        MavisTireCustomer.Email = strEmail;
                                        MavisTireCustomer.WorkEmail = strWorkEmail;
                                        MavisTireCustomer.HomePhone = strHomePhone;
                                        MavisTireCustomer.MobilePhone = strMobilePhone;
                                        MavisTireCustomer.Territory = strTerritory;
                                        MavisTireCustomer.Brand = strBrand;
                                        MavisTireCustomer.UniformBrand = strUniformBrand;
                                        MavisTireCustomer.LocationAddress = strLocationAddress;
                                        MavisTireCustomer.LocationCity = strLocationCity;
                                        MavisTireCustomer.LocationState = strLocationState;
                                        MavisTireCustomer.LocationZip = strLocationZip;
                                        MavisTireCustomer.RVP = strRVP;
                                        MavisTireCustomer.RD = strRD;
                                        MavisTireCustomer.RTM = strRTM;
                                        MavisTireCustomer.IsUpdated = true;
                                        MavisTireCustomer.ErrorMessage = String.Empty;
                                        MavisTireCustomer.Update();
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
