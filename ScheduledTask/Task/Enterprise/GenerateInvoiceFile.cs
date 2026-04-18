using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ScheduledTask.Task.Enterprise
{
    public class GenerateInvoiceFile
    {
        public bool Execute()
        {
            try
            {
                string strPath = string.Format(@"C:\Import\Invoice\{0}\", DateTime.UtcNow.ToString("yyyyMM"));
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }

                CreateExportCSVBySQL(strPath, "Image Solutions Apparel Inc. (APUSA0002379499)", "APUSA0002379499", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        protected void CreateExportCSVBySQL(string filepath, string suppliername, string suppliernmber, DateTime invoicedate)
        {
            string strFilename = string.Empty;

            strFilename = string.Format("Invoice_ImageSolutions_{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

            filepath = filepath + strFilename;

            string strInvoiceNumber = string.Empty;

            StringBuilder objReturn = new StringBuilder();

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
                EnterpriseFileSubmit.Type = "Invoice";
                EnterpriseFileSubmit.Create(objConn, objTran);

                strInvoiceNumber = string.Format("INV-{0}", EnterpriseFileSubmit.EnterpriseFileSubmitID);

                objReturn.Append(string.Format("Invoice,Invoice Number*,Supplier Name,Supplier Number,Status,Invoice Date*,Submit For Approval?,Handling Amount,Misc Amount,Shipping Amount,Line Level Taxation*,Tax Amount,Tax Rate,Tax Code,Supplier Note,Payment Terms,Shipping Terms,Requester Email,Requester Name,Requester Lookup Name,Chart of Accounts*,Currency,Contract Number,Image Scan Filename,Image Scan URL,Attachment 1"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("Invoice Line,Invoice Number*,Supplier Name,Supplier Number,Line Number,Description*,Supplier Part Number,Price*,Quantity,Line Tax Amount,Line Tax Rate,Line Tax Code,Line Tax Location,Line Tax Description,Line Tax Supply Date,Unit of Measure*,PO Number,PO Line Number,Account Name,Account Code,Billing Notes,Account Segment 1,Account Segment 2,Account Segment 3,Account Segment 4,Commodity Name"));
                objReturn.AppendLine();

                objReturn.Append(string.Format("Invoice,{0},{1},{2},,{3},YES,,,,No,,,,,30,,,,,{4},USD,,,,", strInvoiceNumber, suppliername, suppliernmber, invoicedate.ToString("MM/dd/yyyy"), "US - Tulsa Shared Services"));
                objReturn.AppendLine();

                strSQL = string.Format(@"
SELECT t.EmployeeID, t.FirstName, t.LastName, t.SalesOrderID, t.NetSuiteInternalID, t.LocationPSGroup, t.LocationExternalID
	, t.TotalAmount - Deduct.Amount as Amount
	, t.TotalAmount
	, ISNULL(CreditCard.Amount,0) as CreditCardAmount
	, ISNULL(PreviousInvoice.Amount,0) as PreviousInvoiceAmount
	, Deduct.Amount as DeductAmount
FROM 
(
	SELECT ec.EmployeeID, ec.FirstName, ec.LastName, s.SalesOrderID, s.NetSuiteInternalID
		, ISNULL(Location.PSGroup,'') as LocationPSGroup, ISNULL(Location2.ExternalID,'') as LocationExternalID
		, SUM(ISNULL(ei.TotalAmount,0)) TotalAmount
	FROM EnterpriseInvoice (NOLOCK) ei
	Inner Join SalesOrder (NOLOCK) s on s.NetSuiteInternalID = ei.SalesOrderInternalID
	Inner Join UserInfo (NOLOCK) u on u.UserInfoID = s.UserInfoID
	Inner Join EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress
	Outer Apply
	(
		SELECT Top 1 ec2.PSGroup, ec2.ExternalID
		FROM EnterpriseCustomer (NOLOCK) ec2
		WHERE ec2.StoreNumber = ec.StoreNumber
		and ec2.IsIndividual = 0
	) Location
	Outer Apply
	(
		SELECT Top 1 ec2.PSGroup, ec2.ExternalID
		FROM EnterpriseCustomer (NOLOCK) ec2
		WHERE ec2.StoreNumber = SUBSTRING(ec.StoreNumber,1,2) + '99'
		and ec2.IsIndividual = 0
	) Location2
	WHERE ei.IsExported = 0
	GROUP BY ec.EmployeeID, ec.FirstName, ec.LastName, s.SalesOrderID, s.NetSuiteInternalID, ISNULL(Location.PSGroup,''), ISNULL(Location2.ExternalID,'')
) t
Outer Apply
(
	SELECT SUM(p2.AmountPaid) Amount 
	FROM Payment (NOLOCK) p2
	WHERE p2.SalesOrderID = t.SalesOrderID
	and p2.CreditCardTransactionLogID is not null
) CreditCard
Outer Apply
(
	SELECT SUM(ei2.TotalAmount) Amount
	FROM EnterpriseInvoice (NOLOCK)  ei2
	WHERE ei2.SalesOrderInternalID = t.NetSuiteInternalID
	and ei2.IsExported = 1
) PreviousInvoice
Outer Apply
(
	SELECT CASE 
		WHEN ISNULL(PreviousInvoice.Amount,0) < ISNULL(CreditCard.Amount,0) THEN ISNULL(CreditCard.Amount,0) - ISNULL(PreviousInvoice.Amount,0)
		ELSE 0 END as Amount
) Deduct
WHERE (
t.LocationPSGroup != ''
and 
t.LocationPSGroup not like 'A5%'
and 
t.LocationPSGroup != 'A0066'
)
"
                    );

                objRead = Database.GetDataReader(strSQL);

                int counter = 0;
                bool IsIncomplete = false;

                while (objRead.Read())
                {
                    decimal decAmount = Convert.ToDecimal(objRead["Amount"]);
                    string strNetSuiteInternalID = Convert.ToString(objRead["NetSuiteInternalID"]);
                    string strDescription = string.Format("{0} SO{1} {2}: {3}", Convert.ToString(objRead["EmployeeID"]), Convert.ToString(objRead["SalesOrderID"]), Convert.ToString(objRead["LastName"]), Convert.ToString(objRead["FirstName"]));

                    if (decAmount > 0)
                    {
                        counter++;

                        if (counter > 500)
                        {
                            IsIncomplete = true;
                            break;
                        }

                        if (strDescription.Length > 35)
                        {
                            strDescription = strDescription.Substring(0, 35);
                        }

                        objReturn.Append(string.Format("Invoice Line,{0},{1},{2},{3},{4},,{5},,{6},,,,,,EA,,,,,,{7},{8},{9},{10},{11}"
                            , strInvoiceNumber
                            , suppliername
                            , suppliernmber
                            , counter
                            , strDescription
                            , decAmount
                            , string.Empty //Convert.ToDecimal(objRead["TaxAmount"])
                            , "P9213"
                            , Convert.ToString(objRead["LocationPSGroup"])
                            , "631015"
                            , Convert.ToString(objRead["LocationExternalID"])
                            , "Corp DR Uniform Program-631015"
                            )
                        );
                        objReturn.AppendLine();
                    }
                   
                    List<ImageSolutions.Enterprise.EnterpriseInvoice> EnterpriseInvoices = new List<ImageSolutions.Enterprise.EnterpriseInvoice>();
                    ImageSolutions.Enterprise.EnterpriseInvoiceFilter EnterpriseInvoiceFilter = new ImageSolutions.Enterprise.EnterpriseInvoiceFilter();
                    EnterpriseInvoiceFilter.SalesOrderInternalID = new Database.Filter.StringSearch.SearchFilter();
                    EnterpriseInvoiceFilter.SalesOrderInternalID.SearchString = strNetSuiteInternalID;
                    EnterpriseInvoices = ImageSolutions.Enterprise.EnterpriseInvoice.GetEnterpriseInvoices(EnterpriseInvoiceFilter);

                    foreach(ImageSolutions.Enterprise.EnterpriseInvoice _EnterpriseInvoice in EnterpriseInvoices)
                    {
                        _EnterpriseInvoice.IsExported = true;
                        _EnterpriseInvoice.EnterpriseFileSubmitID = EnterpriseFileSubmit.EnterpriseFileSubmitID;
                        _EnterpriseInvoice.Update(objConn, objTran);
                    }
                }

                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
                }

                if (counter > 0)
                {
                    //Upload to SFTP
                    using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPHost"]), (ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPLogin"]), (ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPassword"])))
                    {
                        _SftpClient.Connect();
                        UploadFile(_SftpClient, filepath);
                    }
                }

                objTran.Commit();

                if (IsIncomplete)
                {
                    string strPath = string.Format(@"C:\Import\Invoice\{0}\", DateTime.UtcNow.ToString("yyyyMM"));
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }

                    //string strInvoiceNumber = "INV-T2";
                    CreateExportCSVBySQL(strPath, suppliername, suppliernmber, DateTime.UtcNow);
                }

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
                    string strTest = string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPath"]), strFileName);

                    sftpclient.UploadFile(filestream, string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPath"]), strFileName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
