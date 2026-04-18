using ImageSolutions.SalesOrder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using static ImageSolutions.Marketing.SMSOutbox;

namespace ImageSolutions.Marketing
{
    public class SMSOutbox : ISBase.BaseClass
    {
        public string SMSOutboxID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SMSOutboxID); } }
        public string WebsiteID { get; set; }
        public string MarketingTemplateID { get; set; }
        public string UserWebsiteID { get; set; }
        public string Message { get; set; }
        public string SMSMobileNumber { get; set; }
        public bool IsApproved { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentOn { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private MarketingTemplate mMarketingTemplate = null;
        public MarketingTemplate MarketingTemplate
        {
            get
            {
                if (mMarketingTemplate == null && !string.IsNullOrEmpty(MarketingTemplateID))
                {
                    mMarketingTemplate = new MarketingTemplate(MarketingTemplateID);
                }
                return mMarketingTemplate;
            }
        }

        public SMSOutbox()
        {
        }

        public SMSOutbox(string SMSOutboxID)
        {
            this.SMSOutboxID = SMSOutboxID;
            Load();
        }

        public SMSOutbox(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM SMSOutbox (NOLOCK) " +
                         "WHERE SMSOutboxID=" + Database.HandleQuote(SMSOutboxID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SMSOutboxID=" + SMSOutboxID + " is not found");
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

                if (objColumns.Contains("SMSOutboxID")) SMSOutboxID = Convert.ToString(objRow["SMSOutboxID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("MarketingTemplateID")) MarketingTemplateID = Convert.ToString(objRow["MarketingTemplateID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("Message")) Message = Convert.ToString(objRow["Message"]);
                if (objColumns.Contains("SMSMobileNumber")) SMSMobileNumber = Convert.ToString(objRow["SMSMobileNumber"]);
                if (objColumns.Contains("IsApproved")) IsApproved = Convert.ToBoolean(objRow["IsApproved"]);
                if (objColumns.Contains("IsSent")) IsSent = Convert.ToBoolean(objRow["IsSent"]);
                if (objColumns.Contains("SentOn") && objRow["SentOn"] != DBNull.Value) SentOn = Convert.ToDateTime(objRow["SentOn"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(SMSOutboxID)) throw new Exception("Missing SMSOutboxID in the datarow");
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
                if (string.IsNullOrEmpty(MarketingTemplateID)) throw new Exception("MarketingTemplateID is required");
                if (string.IsNullOrEmpty(UserWebsiteID)) throw new Exception("UserWebsiteID is required");
                if (string.IsNullOrEmpty(Message)) throw new Exception("Message is required");
                if (string.IsNullOrEmpty(SMSMobileNumber)) throw new Exception("SMSMobileNumber is required");
                if (Message.Contains("${")) throw new Exception("There are tokens that still need to be replaced from Message");
                if (Message.Contains("{$")) throw new Exception("There are tokens that still need to be replaced from Message");
                if (SMSMobileNumber.Contains("${")) throw new Exception("There are tokens that still need to be replaced from SMS content");
                if (SMSMobileNumber.Contains("{$")) throw new Exception("There are tokens that still need to be replaced from SMS content");
                if (!IsNew) throw new Exception("Create cannot be performed, SMSOutboxID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingTemplateID"] = MarketingTemplateID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Message"] = Message;
                dicParam["SMSMobileNumber"] = SMSMobileNumber;
                dicParam["IsApproved"] = IsApproved;
                dicParam["IsSent"] = IsSent;
                dicParam["SentOn"] = SentOn;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = UpdatedOn;

                SMSOutboxID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SMSOutbox"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(MarketingTemplateID)) throw new Exception("MarketingTemplateID is required");
                if (string.IsNullOrEmpty(UserWebsiteID)) throw new Exception("UserWebsiteID is required");
                if (string.IsNullOrEmpty(Message)) throw new Exception("Message is required");
                if (string.IsNullOrEmpty(SMSMobileNumber)) throw new Exception("SMSMobileNumber is required");
                if (SMSMobileNumber.Contains("${")) throw new Exception("There are tokens that still need to be replaced");
                if (SMSMobileNumber.Contains("{$")) throw new Exception("There are tokens that still need to be replaced");
                if (IsNew) throw new Exception("Update cannot be performed, SMSOutboxID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingTemplateID"] = MarketingTemplateID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Message"] = Message;
                dicParam["SMSMobileNumber"] = SMSMobileNumber;
                dicParam["IsApproved"] = IsApproved;
                dicParam["IsSent"] = IsSent;
                dicParam["SentOn"] = SentOn;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = UpdatedOn;
                dicWParam["SMSOutboxID"] = SMSOutboxID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SMSOutbox"), objConn, objTran);

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

        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
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

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, SMSOutboxID is missing");

                dicDParam["SMSOutboxID"] = SMSOutboxID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SMSOutbox"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        public bool ObjectAlreadyExists()
        {
            //string strSQL = string.Empty;

            //strSQL = "SELECT TOP 1 p.* " +
            //         "FROM SMSOutbox (NOLOCK) p " +
            //         "WHERE p.MarketingTemplateID=" + Database.HandleQuote(MarketingTemplateID) +
            //         "AND p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID.ToString()) +
            //         "AND p.Message=" + Database.HandleQuote(Message.ToString()) +
            //         "AND p.SMSMobileNumber=" + Database.HandleQuote(SMSMobileNumber.ToString()) +
            //         "AND p.FromSMS=" + Database.HandleQuote(FromSMS.ToString()) +
            //         "AND p.ToSMS=" + Database.HandleQuote(ToSMS.ToString());

            //if (!string.IsNullOrEmpty(SMSOutboxID)) strSQL += "AND p.SMSOutboxID<>" + Database.HandleQuote(SMSOutboxID);
            //return Database.HasRows(strSQL);
            return false;
        }

        public static SMSOutbox GetSMSOutbox(SMSOutboxFilter Filter)
        {
            List<SMSOutbox> objSMSOutboxes = null;
            SMSOutbox objReturn = null;

            try
            {
                objSMSOutboxes = GetSMSOutboxes(Filter);
                if (objSMSOutboxes != null && objSMSOutboxes.Count >= 1) objReturn = objSMSOutboxes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSMSOutboxes = null;
            }
            return objReturn;
        }

        public static List<SMSOutbox> GetSMSOutboxes()
        {
            int intTotalCount = 0;
            return GetSMSOutboxes(null, null, null, out intTotalCount);
        }

        public static List<SMSOutbox> GetSMSOutboxes(SMSOutboxFilter Filter)
        {
            int intTotalCount = 0;
            return GetSMSOutboxes(Filter, null, null, out intTotalCount);
        }

        public static List<SMSOutbox> GetSMSOutboxes(SMSOutboxFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSMSOutboxes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SMSOutbox> GetSMSOutboxes(SMSOutboxFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SMSOutbox> objReturn = null;
            SMSOutbox objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SMSOutbox>();

                strSQL = "SELECT * " +
                         "FROM SMSOutbox (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.MarketingTemplateID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.MarketingTemplateID, "MarketingTemplateID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                    if (Filter.Message != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Message, "Message");
                    if (Filter.SMSMobileNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SMSMobileNumber, "SMSMobileNumber");
                    if (Filter.IsApproved != null) strSQL += "AND IsApproved=" + Database.HandleQuote(Convert.ToInt32(Filter.IsApproved.Value).ToString());
                    if (Filter.IsSent != null) strSQL += "AND IsSent=" + Database.HandleQuote(Convert.ToInt32(Filter.IsSent.Value).ToString());
                    if (Filter.ErrorMessage != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ErrorMessage, "ErrorMessage");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SMSOutboxID" : Utility.CustomSorting.GetSortExpression(typeof(SMSOutbox), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SMSOutbox(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
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
            }
            return objReturn;
        }
    }
}
