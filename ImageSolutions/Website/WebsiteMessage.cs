using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class WebsiteMessage : ISBase.BaseClass
    {
        public string WebsiteMessageID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteMessageID); } }
        public string WebsiteID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsAnnouncement { get; set; }
        public bool IsNotification { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }


        private Website mWebsite = null;
        public Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website(WebsiteID);
                }
                return mWebsite;
            }
        }
        public WebsiteMessage()
        {
        }
        public WebsiteMessage(string WebsiteMessageID)
        {
            this.WebsiteMessageID = WebsiteMessageID;
            Load();
        }
        public WebsiteMessage(DataRow objRow)
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
                         "FROM WebsiteMessage (NOLOCK) " +
                         "WHERE WebsiteMessageID=" + Database.HandleQuote(WebsiteMessageID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteMessageID=" + WebsiteMessageID + " is not found");
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
                if (objColumns.Contains("WebsiteMessageID")) WebsiteMessageID = Convert.ToString(objRow["WebsiteMessageID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("StartDate")) StartDate = Convert.ToDateTime(objRow["StartDate"]);
                if (objColumns.Contains("EndDate")) EndDate = Convert.ToDateTime(objRow["EndDate"]);
                if (objColumns.Contains("Subject")) Subject = Convert.ToString(objRow["Subject"]);
                if (objColumns.Contains("Message")) Message = Convert.ToString(objRow["Message"]);
                if (objColumns.Contains("IsAnnouncement")) IsAnnouncement = Convert.ToBoolean(objRow["IsAnnouncement"]);
                if (objColumns.Contains("IsNotification")) IsNotification = Convert.ToBoolean(objRow["IsNotification"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteMessageID)) throw new Exception("Missing WebsiteMessageID in the datarow");
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

            if (!IsActive) return true;

            Hashtable dicParam = new Hashtable();
            try
            {
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (string.IsNullOrEmpty(Subject)) throw new Exception("Subject is required");
                if (string.IsNullOrEmpty(Message)) throw new Exception("Message is required");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteMessageID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["StartDate"] = StartDate;
                dicParam["EndDate"] = EndDate;
                dicParam["Subject"] = Subject;
                dicParam["Message"] = Message;
                dicParam["IsAnnouncement"] = IsAnnouncement;
                dicParam["IsNotification"] = IsNotification;
                dicParam["CreatedBy"] = CreatedBy;
                WebsiteMessageID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "WebsiteMessage"), objConn, objTran).ToString();

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

            if (!IsActive) return Delete(objConn, objTran);

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (WebsiteMessageID == null) throw new Exception("WebsiteMessageID is required");
                if (string.IsNullOrEmpty(Subject)) throw new Exception("Subject is required");
                if (Message == null) throw new Exception("Message is required");
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteMessageID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["StartDate"] = StartDate;
                dicParam["EndDate"] = EndDate;
                dicParam["Subject"] = Subject;
                dicParam["Message"] = Message;
                dicParam["IsAnnouncement"] = IsAnnouncement;
                dicParam["IsNotification"] = IsNotification;
                dicWParam["WebsiteMessageID"] = WebsiteMessageID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "WebsiteMessage"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteMessageID is missing");

                dicDParam["WebsiteMessageID"] = WebsiteMessageID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "WebsiteMessage"), objConn, objTran);
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
            return false;
        }

        public static WebsiteMessage GetWebsiteMessage(WebsiteMessageFilter Filter)
        {
            List<WebsiteMessage> objWebsiteMessages = null;
            WebsiteMessage objReturn = null;

            try
            {
                objWebsiteMessages = GetWebsiteMessages(Filter);
                if (objWebsiteMessages != null && objWebsiteMessages.Count >= 1) objReturn = objWebsiteMessages[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteMessages = null;
            }
            return objReturn;
        }

        public static List<WebsiteMessage> GetWebsiteMessages()
        {
            int intTotalCount = 0;
            return GetWebsiteMessages(null, null, null, out intTotalCount);
        }

        public static List<WebsiteMessage> GetWebsiteMessages(WebsiteMessageFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsiteMessages(Filter, null, null, out intTotalCount);
        }

        public static List<WebsiteMessage> GetWebsiteMessages(WebsiteMessageFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsiteMessages(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<WebsiteMessage> GetWebsiteMessages(WebsiteMessageFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<WebsiteMessage> objReturn = null;
            WebsiteMessage objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<WebsiteMessage>();

                strSQL = "SELECT * " +
                         "FROM WebsiteMessage (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Subject != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Subject, "Subject");
                    if (Filter.Message != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Message, "Message");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.IsAnnouncment != null) strSQL += "AND IsAnnouncment=" + Database.HandleQuote(Convert.ToInt32(Filter.IsAnnouncment).ToString());
                    if (Filter.IsNotification != null) strSQL += "AND IsNotification=" + Database.HandleQuote(Convert.ToInt32(Filter.IsNotification).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteMessageID" : Utility.CustomSorting.GetSortExpression(typeof(WebsiteMessage), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new WebsiteMessage(objData.Tables[0].Rows[i]);
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
