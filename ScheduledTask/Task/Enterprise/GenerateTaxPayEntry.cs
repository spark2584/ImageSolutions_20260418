using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronXL;
using Renci.SshNet;

namespace ScheduledTask.Task.Enterprise
{
    public class GenerateTaxPayEntry
    {
        public bool Execute()
        {
            ImageSolutions.Task.Task Task = null;
            ImageSolutions.Task.TaskFilter TaskFilter = null;

            try
            {

                //Get Task for Enterprise Budget Pay Sync
                Task = new ImageSolutions.Task.Task();
                TaskFilter = new ImageSolutions.Task.TaskFilter();
                TaskFilter.TaskName = new Database.Filter.StringSearch.SearchFilter();
                TaskFilter.TaskName.SearchString = "Submit Budget Pay Entry";
                Task = ImageSolutions.Task.Task.GetTask(TaskFilter);

                string strPath = string.Format(@"C:\Import\Payroll\{0}\", DateTime.UtcNow.ToString("yyyyMM"));
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                //CreateExportCSVBySQL(strPath, "US", Convert.ToDateTime("7/17/2024"), Convert.ToDateTime("7/17/2024"), Convert.ToDateTime("7/17/2024"), "329", "IMGSOL_BATCH_USA");
                //CreateExportCSVBySQL(strPath, "CA", Convert.ToDateTime("7/17/2024"), Convert.ToDateTime("7/17/2024"), Convert.ToDateTime("7/17/2024"), "85GU", "IMGSOL_BATCH_CAN");

                //CreateExportExcelBySQL(strPath, "US", DateTime.UtcNow, Convert.ToDateTime("7/31/2024"), Convert.ToDateTime("7/31/2024"), "329", "IMGSOL_BATCH_USA");
                //CreateExportExcelBySQL(strPath, "CA", Convert.ToDateTime("7/31/2024"), Convert.ToDateTime("7/31/2024"), Convert.ToDateTime("7/31/2024"), "EN85GU", "IMGSOL_BATCH_CAN");

                CreateExportExcelBySQL(strPath, "US", "329", "IMGSOL_BATCH_USA");
                //CreateExportExcelBySQL(strPath, "CA", "EN85GU", "IMGSOL_BATCH_CAN");

                Task.LastExecutedOn = DateTime.UtcNow;
                Task.Update();
            }
            catch (Exception ex)
            {
                if (Task != null)
                {
                    Task.ErrorMessage = ex.Message;
                    Task.Update();
                }

                throw ex;
            }

            return true;
        }

        //employeexrefcode -> Employee ID
        //prearningxrefcode -> US: 329 / CA: 85GU
        //SourceSystem = IMGSOL_BATCH_USA
        protected void CreateExportCSVBySQL(string filepath, string countrycode, DateTime fromdate, DateTime todate
            , DateTime date, string prearningxrefcode, string sourcesystem)
        {
            string strFilename = string.Empty;

            if (countrycode == "US")
            {
                strFilename = string.Format("USA_PayEntryImport_ImageSolutions_{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            }
            else if (countrycode == "CA")
            {
                strFilename = string.Format("CAN_PayEntryImport_ImageSolutions_{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            }
            filepath = filepath + strFilename;

            StringBuilder objReturn = new StringBuilder();

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                objReturn.Append(string.Format("PayEntryImport,,,,,,,,,,,,"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("H,PayEntryImportSetting,PayEntryImportSetting,AutoDetectPayDataPayGroup,,,,,,,,,"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("H,PayData,PayData,RegularPayPeriodStart,,,,,,,,,"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("H,Batch,PayData,BatchName,DefaultSourceSystem,,,,,,,,"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("H,Earnings,Batch,ActionType,EmployeeXrefCode,PREarningXrefCode,Amount,BusinessDate,FLSAAdjustPeriodStart,FLSAAdjustPeriodEnd,SourceSystem,ImportIdentifier,Comment"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("D,PayEntryImportSetting,PayEntryImportSetting,1,,,,,,,,,"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("D,PayData,PayData,{0},,,,,,,,,", date.ToString("yyyy-MM-dd")));
                objReturn.AppendLine();

                objReturn.Append(string.Format("D,Batch,PayData,IMGSOL_{0}{1}{2}_{3},ImageSolutions,,,,,,,,"
                    , date.Year
                    , date.Month < 10 ? string.Format("0{0}", date.Month) : Convert.ToString(date.Month)
                    , date.Day < 10 ? string.Format("0{0}", date.Day) : Convert.ToString(date.Day)
                    , countrycode == "CA" ? "CAN" : "USA"
                    ) 
                );
                objReturn.AppendLine();

                strSQL = string.Format(@"
SELECT uw.EmployeeID, Name.Value as Name, Comment.Value as Comment, SUM(ISNULL(s.BudgetTaxAmount,0) * -1) as Amount
FROM SalesOrder (NOLOCK) s
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = u.UserInfoID
Inner Join EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress
inner join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID
Outer APply
(
	SELECT CASE WHEN LEN(ec.FirstName + ' ' + ec.LastName) > 100 THEN SUBSTRING(ec.FirstName + ' ' + ec.LastName, 0, 100) ELSE ec.FirstName + ' ' + ec.LastName END Value
) Name
Outer Apply
(
	SELECT 
		CASE WHEN ec.HireDate > GETUTCDATE() 
			THEN Name.Value + ' - Pre: ' + convert(varchar, ec.HireDate, 101)
		ELSE Name.Value END as Value
) Comment
WHERE s.WebsiteID = 53
AND ISNULL(s.BudgetTaxAmount,0) != 0
AND at.CountryCode = {0}
GROUP BY uw.EmployeeID, Name.Value, Comment.Value
"
                    , Database.HandleQuote(countrycode));

                objRead = Database.GetDataReader(strSQL);

                int counter = 0;
                while (objRead.Read())
                {
                    counter++;
                    objReturn.Append(string.Format("D,Earnings,Batch,append,{0},{1},{2},,,,{3},{4},{5}"
                        , Convert.ToString(objRead["EmployeeID"])
                        , prearningxrefcode
                        , Convert.ToDecimal(objRead["Amount"])
                        , sourcesystem
                        , counter
                        , Convert.ToString(objRead["Comment"]))
                    );
                    objReturn.AppendLine();
                }

                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //protected void CreateExportExcelBySQL(string filepath, string countrycode, DateTime fromdate, DateTime todate, DateTime date, string prearningxrefcode, string sourcesystem)
        protected void CreateExportExcelBySQL(string filepath, string countrycode, string prearningxrefcode, string sourcesystem)
        {
            WorkBook WorkBook = WorkBook.Create(ExcelFileFormat.XLSX);
            WorkSheet WorkSheet = WorkBook.CreateWorkSheet("Result Sheet");

            string strFilename = string.Empty;

            string strEncryptFilePath = string.Empty;
            if (!Directory.Exists(filepath + "Encrypt\\"))
            {
                Directory.CreateDirectory(filepath + "Encrypt\\");
            }
            strEncryptFilePath = filepath + "Encrypt\\" + strFilename;

            if (countrycode == "US")
            {
                strFilename = string.Format("USA_PayEntryImport_ImageSolutions_{0}.xlsx", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            }
            else if (countrycode == "CA")
            {
                strFilename = string.Format("CAN_PayEntryImport_ImageSolutions_{0}.xlsx", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            }
            filepath = filepath + strFilename;

            strEncryptFilePath = strEncryptFilePath + strFilename + ".pgp";
            //StringBuilder objReturn = new StringBuilder();


            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                ImageSolutions.Enterprise.EnterpriseFileSubmit EnterpriseFileSubmit = new ImageSolutions.Enterprise.EnterpriseFileSubmit();
                EnterpriseFileSubmit.FilePath = filepath;
                EnterpriseFileSubmit.Type = "Payroll Entry";
                EnterpriseFileSubmit.Create(objConn, objTran);

                WorkSheet["A1"].Value = "PayEntryImport";
                //objReturn.Append(string.Format("PayEntryImport,,,,,,,,,,,,"));
                //objReturn.AppendLine();

                WorkSheet["A2"].Value = "H";
                WorkSheet["B2"].Value = "PayEntryImportSetting";
                WorkSheet["C2"].Value = "PayEntryImportSetting";
                WorkSheet["D2"].Value = "AutoDetectPayDataPayGroup";
                //objReturn.Append(string.Format("H,PayEntryImportSetting,PayEntryImportSetting,AutoDetectPayDataPayGroup,,,,,,,,,"));
                //objReturn.AppendLine();

                WorkSheet["A3"].Value = "H";
                WorkSheet["B3"].Value = "PayData";
                WorkSheet["C3"].Value = "PayData";
                WorkSheet["D3"].Value = "RegularPayPeriodStart";
                //objReturn.Append(string.Format("H,PayData,PayData,RegularPayPeriodStart,,,,,,,,,"));
                //objReturn.AppendLine();

                WorkSheet["A4"].Value = "H";
                WorkSheet["B4"].Value = "Batch";
                WorkSheet["C4"].Value = "PayData";
                WorkSheet["D4"].Value = "BatchName";
                WorkSheet["E4"].Value = "DefaultSourceSystem";
                //objReturn.Append(string.Format("H,Batch,PayData,BatchName,DefaultSourceSystem,,,,,,,,"));
                //objReturn.AppendLine();

                WorkSheet["A5"].Value = "H";
                WorkSheet["B5"].Value = "Earnings";
                WorkSheet["C5"].Value = "Batch";
                WorkSheet["D5"].Value = "ActionType";
                WorkSheet["E5"].Value = "EmployeeXrefCode";
                WorkSheet["F5"].Value = "PREarningXrefCode";
                WorkSheet["G5"].Value = "Amount";
                WorkSheet["H5"].Value = "BusinessDate";
                WorkSheet["I5"].Value = "FLSAAdjustPeriodStart";
                WorkSheet["J5"].Value = "FLSAAdjustPeriodEnd";
                WorkSheet["K5"].Value = "SourceSystem";
                WorkSheet["L5"].Value = "ImportIdentifier";
                WorkSheet["M5"].Value = "Comment";
                //objReturn.Append(string.Format("H,Earnings,Batch,ActionType,EmployeeXrefCode,PREarningXrefCode,Amount,BusinessDate,FLSAAdjustPeriodStart,FLSAAdjustPeriodEnd,SourceSystem,ImportIdentifier,Comment"));
                //objReturn.AppendLine();

                WorkSheet["A6"].Value = "D";
                WorkSheet["B6"].Value = "PayEntryImportSetting";
                WorkSheet["C6"].Value = "PayEntryImportSetting";
                WorkSheet["D6"].Value = 1;
                //objReturn.Append(string.Format("D,PayEntryImportSetting,PayEntryImportSetting,true,,,,,,,,,"));
                //objReturn.AppendLine();

                WorkSheet["A7"].Value = "D";
                WorkSheet["B7"].Value = "PayData";
                WorkSheet["C7"].Value = "PayData";
                WorkSheet["D7"].Value = DateTime.UtcNow.ToString("yyyy-MM-dd");
                WorkSheet["D7"].FormatString = "yyyy-MM-dd";
                //objReturn.Append(string.Format("D,PayData,PayData,{0},,,,,,,,,", date.ToString("yyyy-MM-dd")));
                //objReturn.AppendLine();

                WorkSheet["A8"].Value = "D";
                WorkSheet["B8"].Value = "Batch";
                WorkSheet["C8"].Value = "PayData";
                WorkSheet["D8"].Value = string.Format("IMGSOL_{0}{1}{2}_{3}", DateTime.UtcNow.Year, DateTime.UtcNow.Month < 10 ? string.Format("0{0}", DateTime.UtcNow.Month) : Convert.ToString(DateTime.UtcNow.Month), DateTime.UtcNow.Day < 10 ? string.Format("0{0}", DateTime.UtcNow.Day) : Convert.ToString(DateTime.UtcNow.Day), countrycode == "CA" ? "CAN" : "USA");
                WorkSheet["E8"].Value = "ImageSolutions";
                //objReturn.Append(string.Format("D,Batch,PayData,IMGSOL_{0}{1}{2}_{3},ImageSolutions,,,,,,,,"
                //    , date.Year
                //    , date.Month < 10 ? string.Format("0{0}", date.Month) : Convert.ToString(date.Month)
                //    , date.Day < 10 ? string.Format("0{0}", date.Day) : Convert.ToString(date.Day)
                //    , countrycode == "CA" ? "CAN" : "USA"
                //    )
                //);
                //objReturn.AppendLine();


                strSQL = string.Format(@"
SELECT s.SalesOrderID, ec.WorkdayID as EmployeeID, Name.Value as Name, Comment.Value as Comment, ISNULL(p.AmountPaid,0) as Amount
FROM SalesOrder (NOLOCK) s
Inner Join Payment (NOLOCK) p on p.SalesOrderID = s.SalesOrderID and p.BudgetAssignmentID is not null
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = u.UserInfoID and uw.WebsiteID = s.WebsiteID
Inner Join EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress and ec.IsIndividual = 1
inner join AddressTrans (NOLOCK) at on at.AddressTransID = s.DeliveryAddressTransID
Outer Apply
(
	SELECT CASE WHEN LEN(ec.FirstName + ' ' + ec.LastName) > 100 THEN SUBSTRING(ec.FirstName + ' ' + ec.LastName, 0, 100) ELSE ec.FirstName + ' ' + ec.LastName END Value
) Name
Outer Apply
(
	SELECT 
		CASE WHEN ec.HireDate > GETUTCDATE() 
			THEN Name.Value + ' - Pre: ' + convert(varchar, ec.HireDate, 101)
		ELSE Name.Value END as Value
) Comment
WHERE s.WebsiteID = {0}
AND ISNULL(p.AmountPaid,0) != 0
AND ISNULL(at.CountryCode,'US') = {1}
and S.Status in (
'Billed',
'Cancelled',
'Closed'
)
AND s.IsBudgetPayEntrySubmitted = 0
ORDER BY uw.EmployeeID


"
                    , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"])) //WebsiteID
                    , Database.HandleQuote(countrycode)
                    );

                objRead = Database.GetDataReader(strSQL);

                string strPreviousEmployeeID = string.Empty;
                string strCurrentEmployeeID = string.Empty;
                decimal decCurrentAmount = 0;
                int counter = 0;
                int intLine = 8;

                bool IsIncomplete = false;

                while (objRead.Read())
                {
                    string strSalesOrderID = Convert.ToString(objRead["SalesOrderID"]);
                    ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(strSalesOrderID);

                    SalesOrder.IsBudgetPayEntrySubmitted = true;
                    SalesOrder.BudgetPayEntryReference = EnterpriseFileSubmit.EnterpriseFileSubmitID;
                    SalesOrder.BudgetPayEntrySubmittedOn = DateTime.UtcNow;
                    SalesOrder.Update(objConn, objTran);

                    strCurrentEmployeeID = Convert.ToString(objRead["EmployeeID"]);

                    if (strCurrentEmployeeID != strPreviousEmployeeID)
                    {
                        counter++;

                        //if (counter > 500)
                        //{
                        //    IsIncomplete = true;
                        //    break;
                        //}

                        intLine = 8 + counter;
                        decCurrentAmount = Convert.ToDecimal(objRead["Amount"]);

                        WorkSheet["A" + intLine].Value = "D";
                        WorkSheet["B" + intLine].Value = "Earnings";
                        WorkSheet["C" + intLine].Value = "Batch";
                        WorkSheet["D" + intLine].Value = "append";
                        WorkSheet["E" + intLine].Value = strCurrentEmployeeID; //Convert.ToString(objRead["EmployeeID"]);
                        WorkSheet["F" + intLine].Value = prearningxrefcode;
                        WorkSheet["G" + intLine].Value = decCurrentAmount; //Convert.ToDecimal(objRead["Amount"]);
                        WorkSheet["H" + intLine].Value = string.Empty;
                        WorkSheet["I" + intLine].Value = string.Empty; ;
                        WorkSheet["J" + intLine].Value = string.Empty; ;
                        WorkSheet["K" + intLine].Value = sourcesystem;
                        WorkSheet["L" + intLine].Value = counter;
                        WorkSheet["M" + intLine].Value = Convert.ToString(objRead["Comment"]);

                        strPreviousEmployeeID = Convert.ToString(objRead["EmployeeID"]);
                    }
                    else
                    {
                        decCurrentAmount = decCurrentAmount + Convert.ToDecimal(objRead["Amount"]);
                        WorkSheet["G" + intLine].Value = decCurrentAmount;
                    }
                }

                WorkBook.SaveAs(filepath);

                //PGPEncryptDecrypt.EncryptFile(filepath, strEncryptFilePath, @"C:\Import\0x266A2EAA-pub.asc", true, true);

                string strPubKey = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseTaxPGPPublicKeyPath"]);
                PGPEncryptDecrypt.EncryptFile(filepath, strEncryptFilePath, strPubKey, true, true);

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["EnterprisePayrollSFTPHost"]), (ConfigurationManager.AppSettings["EnterprisePayrollSFTPLogin"]), (ConfigurationManager.AppSettings["EnterprisePayrollSFTPPassword"])))
                {
                    _SftpClient.Connect();
                    UploadFile(_SftpClient, strEncryptFilePath);
                }

                objTran.Commit();               

                //if (IsIncomplete)
                //{
                //    string strPath = string.Format(@"C:\Import\Staging\TaxFile\{0}\", DateTime.UtcNow.ToString("yyyyMM"));
                //    if (!Directory.Exists(strPath))
                //    {
                //        Directory.CreateDirectory(strPath);
                //    }

                //    CreateExportExcelBySQL(strPath, countrycode, prearningxrefcode, sourcesystem);
                //}
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();

                throw ex;
            }
        }

        protected void UploadFile(SftpClient sftpclient, string filepath)
        {
            try
            {
                using (var filestream = System.IO.File.OpenRead(filepath))
                {
                    string strFileName = Path.GetFileName(filepath);
                    string strTest = string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterprisePayrollSFTPPath"]), strFileName);

                    sftpclient.UploadFile(filestream, string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterprisePayrollSFTPPath"]), strFileName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
