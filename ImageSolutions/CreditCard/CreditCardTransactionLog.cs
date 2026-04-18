using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.CreditCard
{
    public class CreditCardTransactionLog : ISBase.BaseClass
    {
        public string CreditCardTransactionLogID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CreditCardTransactionLogID); } }
        public string UserInfoID { get; set; }
        public CreditCard.enumTransactionType TransactionType { get; set; }
        public double Amount { get; set; }
        public string CCName { get; set; }
        public string CCLastFourNumber { get; set; }
        public string CCExpiration { get; set; }
        public string CCExpirationMonth
        {
            get
            {
                string strReturn = string.Empty;
                if (!string.IsNullOrEmpty(CCExpiration) && CCExpiration.Length == 6)
                {
                    strReturn = CCExpiration.Substring(0, 2);
                }
                return strReturn;
            }
        }
        public string CCExpirationYear
        {
            get
            {
                string strReturn = string.Empty;
                if (!string.IsNullOrEmpty(CCExpiration) && CCExpiration.Length == 6)
                {
                    strReturn = CCExpiration.Substring(2, 4);
                }
                return strReturn;
            }
        }
        public string CCCVV { get; set; }
        public CreditCard.enumCreditCardType CCType { get; set; }
        public string ApprovalCode { get; set; }
        public string AvsResultCode { get; set; }
        public string CvvResultCode { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseReasonCode { get; set; }
        public string ResponseReasonText { get; set; }
        public string ResponseSubCode { get; set; }
        public string TransactionID { get; set; }
        public string Md5Hash { get; set; }
        public string Status { get; set; }
        public bool IsCaptured
        {
            get
            {
                bool blnReturn = false;

                if (ResponseCode == "Approved")
                {
                    switch (TransactionType)
                    {
                        case CreditCard.enumTransactionType.AUTH_ONLY:
                            blnReturn = !string.IsNullOrEmpty(CapturedCreditCardTransactionLogID);
                            break;
                        default:
                            blnReturn = true;
                            break;
                    }
                }
                return blnReturn;
            }
        }
        public string CapturedCreditCardTransactionLogID { get; set; }
        public string IPAddress { get; set; }
        public DateTime CreatedOn { get; private set; }

        private ImageSolutions.Payment.Payment mPayment = null;
        public ImageSolutions.Payment.Payment Payment
        {
            get
            {
                if (mPayment == null && !string.IsNullOrEmpty(CreditCardTransactionLogID))
                {
                    ImageSolutions.Payment.PaymentFilter objFilter = null;

                    objFilter = new Payment.PaymentFilter();
                    objFilter.CreditCardTransactionLogID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.CreditCardTransactionLogID.SearchString = CreditCardTransactionLogID;

                    mPayment = ImageSolutions.Payment.Payment.GetPayment(objFilter);
                }
                return mPayment;
            }
        }
        public CreditCardTransactionLog()
        {
        }

        public CreditCardTransactionLog(string CreditCardTransactionLogID)
        {
            this.CreditCardTransactionLogID = CreditCardTransactionLogID;
            Load();
        }

        public CreditCardTransactionLog(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Load(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }

        protected override void Load(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT ccl.* " +
                         "FROM CreditCardTransactionLog ccl (NOLOCK) " +
                         "WHERE ccl.CreditCardTransactionLogID=" + Database.HandleQuote(CreditCardTransactionLogID);
                objData = Database.GetDataSet(strSQL, objConn, objTran);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CreditCardTransactionLogID=" + CreditCardTransactionLogID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("CreditCardTransactionLogID")) CreditCardTransactionLogID = Convert.ToString(objRow["CreditCardTransactionLogID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("TransactionType")) TransactionType = (CreditCard.enumTransactionType)Enum.Parse(typeof(CreditCard.enumTransactionType), Convert.ToString(objRow["TransactionType"]));
                if (objColumns.Contains("Amount")) Amount = Convert.ToDouble(objRow["Amount"]);
                if (objColumns.Contains("CCName")) CCName = Convert.ToString(objRow["CCName"]);
                if (objColumns.Contains("CCLastFourNumber")) CCLastFourNumber = Convert.ToString(objRow["CCLastFourNumber"]);
                if (objColumns.Contains("CCExpiration")) CCExpiration = Convert.ToString(objRow["CCExpiration"]);
                if (objColumns.Contains("CCCVV")) CCCVV = Convert.ToString(objRow["CCCVV"]);
                if (objColumns.Contains("CCType")) CCType = (CreditCard.enumCreditCardType)Enum.Parse(typeof(CreditCard.enumCreditCardType), Convert.ToString(objRow["CCType"]));
                if (objColumns.Contains("ApprovalCode")) ApprovalCode = Convert.ToString(objRow["ApprovalCode"]);
                if (objColumns.Contains("AvsResultCode")) AvsResultCode = Convert.ToString(objRow["AvsResultCode"]);
                if (objColumns.Contains("CvvResultCode")) CvvResultCode = Convert.ToString(objRow["CvvResultCode"]);
                if (objColumns.Contains("ResponseCode")) ResponseCode = Convert.ToString(objRow["ResponseCode"]);
                if (objColumns.Contains("ResponseReasonCode")) ResponseReasonCode = Convert.ToString(objRow["ResponseReasonCode"]);
                if (objColumns.Contains("ResponseReasonText")) ResponseReasonText = Convert.ToString(objRow["ResponseReasonText"]);
                if (objColumns.Contains("ResponseSubCode")) ResponseSubCode = Convert.ToString(objRow["ResponseSubCode"]);
                if (objColumns.Contains("TransactionID")) TransactionID = Convert.ToString(objRow["TransactionID"]);
                if (objColumns.Contains("Md5Hash")) Md5Hash = Convert.ToString(objRow["Md5Hash"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("CapturedCreditCardTransactionLogID")) CapturedCreditCardTransactionLogID = Convert.ToString(objRow["CapturedCreditCardTransactionLogID"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CreditCardTransactionLogID)) throw new Exception("Missing CreditCardTransactionLogID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
        }

        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            Hashtable dicParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CreditCardTransactionLogID already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["TransactionType"] = TransactionType.ToString();
                dicParam["Amount"] = Amount;
                dicParam["CCName"] = CCName;
                dicParam["CCLastFourNumber"] = CCLastFourNumber;
                dicParam["CCExpiration"] = CCExpiration;
                dicParam["CCCVV"] = CCCVV;
                dicParam["CCType"] = CCType.ToString();
                dicParam["ApprovalCode"] = ApprovalCode;
                dicParam["AvsResultCode"] = AvsResultCode;
                dicParam["CvvResultCode"] = CvvResultCode;
                dicParam["ResponseCode"] = ResponseCode;
                dicParam["ResponseReasonCode"] = ResponseReasonCode;
                dicParam["ResponseReasonText"] = ResponseReasonText;
                dicParam["ResponseSubCode"] = ResponseSubCode;
                dicParam["TransactionID"] = TransactionID;
                dicParam["Md5Hash"] = Md5Hash;
                dicParam["Status"] = Status;
                dicParam["CapturedCreditCardTransactionLogID"] = CapturedCreditCardTransactionLogID;
                CreditCardTransactionLogID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CreditCardTransactionLog"), objConn, objTran).ToString();
                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (IsNew) throw new Exception("Update cannot be performed, CreditCardTransactionLogID is missing");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["TransactionType"] = TransactionType.ToString();
                dicParam["Amount"] = Amount;
                dicParam["CCName"] = CCName;
                dicParam["CCLastFourNumber"] = CCLastFourNumber;
                dicParam["CCExpiration"] = CCExpiration;
                dicParam["CCCVV"] = CCCVV;
                dicParam["CCType"] = CCType.ToString();
                dicParam["ApprovalCode"] = ApprovalCode;
                dicParam["AvsResultCode"] = AvsResultCode;
                dicParam["CvvResultCode"] = CvvResultCode;
                dicParam["ResponseCode"] = ResponseCode;
                dicParam["ResponseReasonCode"] = ResponseReasonCode;
                dicParam["ResponseReasonText"] = ResponseReasonText;
                dicParam["ResponseSubCode"] = ResponseSubCode;
                dicParam["TransactionID"] = TransactionID;
                dicParam["Md5Hash"] = Md5Hash;
                dicParam["Status"] = Status;
                dicParam["CapturedCreditCardTransactionLogID"] = CapturedCreditCardTransactionLogID;
                dicWParam["CreditCardTransactionLogID"] = CreditCardTransactionLogID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditCardTransactionLog"), objConn, objTran);
                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }
            return true;
        }

        public bool Capture()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Capture(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public bool Capture(SqlConnection objConn, SqlTransaction objTran)
        {
            switch (TransactionType)
            {
                case CreditCard.enumTransactionType.AUTH_ONLY:
                    return CapturePayment(objConn, objTran);
                case CreditCard.enumTransactionType.AUTH_CAPTURE:
                    break;
                case CreditCard.enumTransactionType.PRIOR_AUTH_CAPTURE:
                    throw new Exception("Transaction type 'PRIOR_AUTH_CAPTURE' cannot be captured");
                case CreditCard.enumTransactionType.CREDIT:
                    throw new Exception("Transaction type 'CREDIT' cannot be captured");
                case CreditCard.enumTransactionType.VOID:
                    throw new Exception("Transaction type 'VOID' cannot be captured");
                default:
                    break;
            }
            return true;
        }

        private bool CapturePayment(SqlConnection objConn, SqlTransaction objTran)
        {
            //ISLibrary.CCRequest objCC = new ISLibrary.CCRequest();
            //ISLibrary.CCResponse objResponse = null;
            //CreditCardTransactionLog objLog = null;
            //Business objBusiness = null;
            //bool blnByPass = false;
            //string strSQL = string.Empty;

            //try
            //{
            //    objBusiness = new Business(BusinessID);
            //    if (objBusiness.BusinessPaymentProcessing == null) throw new Exception("Missing Authorize.NET Login");
            //    if (objBusiness.BusinessPaymentProcessing.CurrencyCode == null) throw new Exception("Missing Currency Code");

            //    objCC.CurrencyCode = objBusiness.BusinessPaymentProcessing.CurrencyCode;
            //    objCC.TransactionID = TransactionID;
            //    objCC.Amount = Amount.ToString();

            //    ISLibrary.AuthorizeNet objAuth = new ISLibrary.AuthorizeNet(
            //                                        !string.IsNullOrEmpty(SiteSetting.AuthorizeNetGatewayUrl) ? SiteSetting.AuthorizeNetGatewayUrl : "https://secure.authorize.net/gateway/transact.dll",
            //                                        !string.IsNullOrEmpty(SiteSetting.AuthorizeNetLogin) ? SiteSetting.AuthorizeNetLogin : objBusiness.BusinessPaymentProcessing.AuthorizeNetLogin,
            //                                        !string.IsNullOrEmpty(SiteSetting.AuthorizeNetTransactionID) ? SiteSetting.AuthorizeNetTransactionID : objBusiness.BusinessPaymentProcessing.AuthorizeNetTransactionID,
            //                                        SiteSetting.IsPaymentTestMode);

            //    if (TransactionType == CreditCard.enumTransactionType.AUTH_ONLY)
            //    {
            //        objResponse = objAuth.postCapture(objCC);
            //    }
            //    else
            //    {
            //        throw new Exception("Credit card transaction type must be AUTH_ONLY");
            //    }

            //    objLog = new CreditCardTransactionLog(objBusiness.BusinessID);
            //    objLog.UserInfoID = UserInfoID;
            //    objLog.PaymentID = PaymentID;
            //    objLog.TransactionType = CreditCard.enumTransactionType.PRIOR_AUTH_CAPTURE;
            //    objLog.Amount = Amount;
            //    objLog.ApprovalCode = objResponse.ApprovalCode;
            //    objLog.AvsResultCode = objResponse.AvsResultCode;
            //    objLog.CvvResultCode = objResponse.CvvResultCode;
            //    objLog.ResponseCode = objResponse.ResponseCode.ToString();
            //    objLog.ResponseReasonCode = objResponse.ResponseReasonCode;
            //    objLog.ResponseReasonText = objResponse.ResponseReasonText;
            //    objLog.ResponseSubCode = objResponse.ResponseSubCode;
            //    objLog.TransactionID = objResponse.TransactionID;
            //    objLog.Md5Hash = objResponse.Md5Hash;
            //    objLog.ResponseCode = objResponse.ResponseCode.ToString();
            //    objLog.IPAddress = WebUtility.GetClientIPAddress();
            //    objLog.Create(); //Do not use transaction

            //    //maybe responsereasoncode=307 is considered a success

            //    if (objResponse.ResponseCode == CCResponse.enumResponseCode.Error)
            //    {
            //        if (CreatedOn >= Convert.ToDateTime("2/24/2016") && CreatedOn <= Convert.ToDateTime("4/01/16") && objResponse.ResponseReasonText == "The transaction cannot be found.")
            //        {
            //            blnByPass = true;
            //        }
            //    }

            //    if (blnByPass)
            //    {
            //        //CapturedCreditCardTransactionLogID = objLog.CreditCardTransactionLogID;
            //        Update(objConn, objTran);
            //    }
            //    else if (objResponse.ResponseCode != ISLibrary.CCResponse.enumResponseCode.Approved || string.IsNullOrEmpty(objResponse.ResponseReasonCode) || objResponse.ResponseReasonCode != "1")
            //    {
            //        throw new Exception(objResponse.ResponseReasonText);
            //    }
            //    else
            //    {
            //        CapturedCreditCardTransactionLogID = objLog.CreditCardTransactionLogID;
            //        Update(objConn, objTran);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    objCC = null;
            //    objResponse = null;
            //    objBusiness = null;
            //    objLog = null;
            //}
            return true;
        }

        public static List<CreditCardTransactionLog> GetCustomerCreditCardTransactionLogs(CreditCardTransactionLogFilter Filter)
        {
            int intTotalCount = 0;
            return GetCustomerCreditCardTransactionLogs(Filter, null, null, out intTotalCount);
        }

        public static List<CreditCardTransactionLog> GetCustomerCreditCardTransactionLogs(CreditCardTransactionLogFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CreditCardTransactionLog> objReturn = null;
            CreditCardTransactionLog objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CreditCardTransactionLog>();

                strSQL = "SELECT ccl.* " +
                            "FROM CreditCardTransactionLog ccl (NOLOCK) " +
                            "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.UserInfoID)) strSQL += "AND ccl.UserInfoID=" + Database.HandleQuote(Filter.UserInfoID);
                    if (!string.IsNullOrEmpty(Filter.CCName)) strSQL += "AND ccl.CCName=" + Database.HandleQuote(Filter.CCName);
                    if (!string.IsNullOrEmpty(Filter.CCLastFourNumber)) strSQL += "AND ccl.CCLastFourNumber=" + Database.HandleQuote(Filter.CCLastFourNumber);
                    if (Filter.TransactionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransactionID, "ccl.TransactionID");
                    if (Filter.Amount != null) strSQL += "AND cc.Amount=" + Database.HandleQuote(Filter.Amount.Value.ToString());
                    if (Filter.CCType != null) strSQL += "AND ccl.CCType=" + Database.HandleQuote(Filter.CCType.Value.ToString());
                    if (Filter.CreatedOnFrom != null) strSQL += "AND ccl.CreatedOn>=" + Database.HandleQuote(Filter.CreatedOnFrom.Value.ToString());
                    if (Filter.CreatedOnTo != null) strSQL += "AND ccl.CreatedOn<=" + Database.HandleQuote(Filter.CreatedOnTo.Value.ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, "CreditCardTransactionLogID DESC", PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CreditCardTransactionLog(objData.Tables[0].Rows[i]);
                        objReturn.Add(objNew);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
                objNew = null;
            }
            return objReturn;
        }

        public static CreditCardTransactionLog GetCustomerCreditCardTransactionLog(CreditCardTransactionLogFilter Filter)
        {
            CreditCardTransactionLog objReturn = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {

                objReturn = new CreditCardTransactionLog();

                strSQL = "SELECT top 1 ccl.* " +
                            "FROM CreditCardTransactionLog ccl (NOLOCK) " +
                            "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.UserInfoID)) strSQL += "AND ccl.UserInfoID=" + Database.HandleQuote(Filter.UserInfoID);
                    if (!string.IsNullOrEmpty(Filter.CCName)) strSQL += "AND ccl.CCName=" + Database.HandleQuote(Filter.CCName);
                    if (!string.IsNullOrEmpty(Filter.CCLastFourNumber)) strSQL += "AND ccl.CCLastFourNumber=" + Database.HandleQuote(Filter.CCLastFourNumber);
                    if (Filter.TransactionID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.TransactionID, "ccl.TransactionID");
                    if (Filter.Amount != null) strSQL += "AND cc.Amount=" + Database.HandleQuote(Filter.Amount.Value.ToString());
                    if (Filter.CCType != null) strSQL += "AND ccl.CCType=" + Database.HandleQuote(Filter.CCType.Value.ToString());
                    if (Filter.CreatedOnFrom != null) strSQL += "AND ccl.CreatedOn>=" + Database.HandleQuote(Filter.CreatedOnFrom.Value.ToString());
                    if (Filter.CreatedOnTo != null) strSQL += "AND ccl.CreatedOn<=" + Database.HandleQuote(Filter.CreatedOnTo.Value.ToString());
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objReturn = new CreditCardTransactionLog(objData.Tables[0].Rows[i]);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
