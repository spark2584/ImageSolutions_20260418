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
using static ImageSolutions.Marketing.EmailOutbox;

namespace ImageSolutions.Marketing
{
    public class EmailOutbox : ISBase.BaseClass
    {
        public string EmailOutboxID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(EmailOutboxID); } }
        public string WebsiteID { get; set; }
        public string MarketingTemplateID { get; set; }
        public string UserWebsiteID { get; set; }
        public string Subject { get; set; }
        public string HTMLContent { get; set; }
        public string ToEmail { get; set; }
        public string CCEmail { get; set; }
        public bool IsApproved { get; set; }
        public bool IsEmailed { get; set; }
        public DateTime? EmailedOn { get; set; }
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

        public EmailOutbox()
        {
        }

        public EmailOutbox(string EmailOutboxID)
        {
            this.EmailOutboxID = EmailOutboxID;
            Load();
        }

        public EmailOutbox(DataRow objRow)
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
                         "FROM EmailOutbox (NOLOCK) " +
                         "WHERE EmailOutboxID=" + Database.HandleQuote(EmailOutboxID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("EmailOutboxID=" + EmailOutboxID + " is not found");
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

                if (objColumns.Contains("EmailOutboxID")) EmailOutboxID = Convert.ToString(objRow["EmailOutboxID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("MarketingTemplateID")) MarketingTemplateID = Convert.ToString(objRow["MarketingTemplateID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                if (objColumns.Contains("Subject")) Subject = Convert.ToString(objRow["Subject"]);
                if (objColumns.Contains("HTMLContent")) HTMLContent = Convert.ToString(objRow["HTMLContent"]);
                if (objColumns.Contains("ToEmail")) ToEmail = Convert.ToString(objRow["ToEmail"]);
                if (objColumns.Contains("CCEmail")) CCEmail = Convert.ToString(objRow["CCEmail"]);
                if (objColumns.Contains("IsApproved")) IsApproved = Convert.ToBoolean(objRow["IsApproved"]);
                if (objColumns.Contains("IsEmailed")) IsEmailed = Convert.ToBoolean(objRow["IsEmailed"]);
                if (objColumns.Contains("EmailedOn") && objRow["EmailedOn"] != DBNull.Value) EmailedOn = Convert.ToDateTime(objRow["EmailedOn"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(EmailOutboxID)) throw new Exception("Missing EmailOutboxID in the datarow");
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
                if (string.IsNullOrEmpty(Subject)) throw new Exception("Subject is required");
                if (string.IsNullOrEmpty(HTMLContent)) throw new Exception("HTMLContent is required");
                if (string.IsNullOrEmpty(ToEmail)) throw new Exception("ToEmail is required");
                if (Subject.Contains("${")) throw new Exception("There are tokens that still need to be replaced from subject");
                if (Subject.Contains("{$")) throw new Exception("There are tokens that still need to be replaced from subject");
                if (HTMLContent.Contains("${")) throw new Exception("There are tokens that still need to be replaced from email content");
                if (HTMLContent.Contains("{$")) throw new Exception("There are tokens that still need to be replaced from email content");
                if (!IsNew) throw new Exception("Create cannot be performed, EmailOutboxID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingTemplateID"] = MarketingTemplateID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Subject"] = Subject;
                dicParam["HTMLContent"] = HTMLContent;
                dicParam["ToEmail"] = ToEmail;
                dicParam["CCEmail"] = CCEmail;
                dicParam["IsApproved"] = IsApproved;
                dicParam["IsEmailed"] = IsEmailed;
                dicParam["EmailedOn"] = EmailedOn;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = UpdatedOn;

                EmailOutboxID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "EmailOutbox"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(Subject)) throw new Exception("Subject is required");
                if (string.IsNullOrEmpty(HTMLContent)) throw new Exception("HTMLContent is required");
                if (string.IsNullOrEmpty(ToEmail)) throw new Exception("ToEmail is required");
                if (HTMLContent.Contains("${")) throw new Exception("There are tokens that still need to be replaced");
                if (HTMLContent.Contains("{$")) throw new Exception("There are tokens that still need to be replaced");
                if (IsNew) throw new Exception("Update cannot be performed, EmailOutboxID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingTemplateID"] = MarketingTemplateID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["Subject"] = Subject;
                dicParam["HTMLContent"] = HTMLContent;
                dicParam["ToEmail"] = ToEmail;
                dicParam["CCEmail"] = CCEmail;
                dicParam["IsApproved"] = IsApproved;
                dicParam["IsEmailed"] = IsEmailed;
                dicParam["EmailedOn"] = EmailedOn;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = UpdatedOn;
                dicWParam["EmailOutboxID"] = EmailOutboxID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "EmailOutbox"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, EmailOutboxID is missing");

                dicDParam["EmailOutboxID"] = EmailOutboxID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "EmailOutbox"), objConn, objTran);
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
            //         "FROM EmailOutbox (NOLOCK) p " +
            //         "WHERE p.MarketingTemplateID=" + Database.HandleQuote(MarketingTemplateID) +
            //         "AND p.UserWebsiteID=" + Database.HandleQuote(UserWebsiteID.ToString()) +
            //         "AND p.Subject=" + Database.HandleQuote(Subject.ToString()) +
            //         "AND p.HTMLContent=" + Database.HandleQuote(HTMLContent.ToString()) +
            //         "AND p.FromEmail=" + Database.HandleQuote(FromEmail.ToString()) +
            //         "AND p.ToEmail=" + Database.HandleQuote(ToEmail.ToString());

            //if (!string.IsNullOrEmpty(EmailOutboxID)) strSQL += "AND p.EmailOutboxID<>" + Database.HandleQuote(EmailOutboxID);
            //return Database.HasRows(strSQL);
            return false;
        }

        public static EmailOutbox GetEmailOutbox(EmailOutboxFilter Filter)
        {
            List<EmailOutbox> objEmailOutboxes = null;
            EmailOutbox objReturn = null;

            try
            {
                objEmailOutboxes = GetEmailOutboxes(Filter);
                if (objEmailOutboxes != null && objEmailOutboxes.Count >= 1) objReturn = objEmailOutboxes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmailOutboxes = null;
            }
            return objReturn;
        }

        public static List<EmailOutbox> GetEmailOutboxes()
        {
            int intTotalCount = 0;
            return GetEmailOutboxes(null, null, null, out intTotalCount);
        }

        public static List<EmailOutbox> GetEmailOutboxes(EmailOutboxFilter Filter)
        {
            int intTotalCount = 0;
            return GetEmailOutboxes(Filter, null, null, out intTotalCount);
        }

        public static List<EmailOutbox> GetEmailOutboxes(EmailOutboxFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetEmailOutboxes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<EmailOutbox> GetEmailOutboxes(EmailOutboxFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<EmailOutbox> objReturn = null;
            EmailOutbox objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<EmailOutbox>();

                strSQL = "SELECT * " +
                         "FROM EmailOutbox (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.MarketingTemplateID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.MarketingTemplateID, "MarketingTemplateID");
                    if (Filter.UserWebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserWebsiteID, "UserWebsiteID");
                    if (Filter.Subject != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Subject, "Subject");
                    if (Filter.HTMLContent != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.HTMLContent, "HTMLContent");
                    if (Filter.ToEmail != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ToEmail, "ToEmail");
                    if (Filter.IsApproved != null) strSQL += "AND IsApproved=" + Database.HandleQuote(Convert.ToInt32(Filter.IsApproved.Value).ToString());
                    if (Filter.IsEmailed != null) strSQL += "AND IsEmailed=" + Database.HandleQuote(Convert.ToInt32(Filter.IsEmailed.Value).ToString());
                    if (Filter.ErrorMessage != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ErrorMessage, "ErrorMessage");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "EmailOutboxID" : Utility.CustomSorting.GetSortExpression(typeof(EmailOutbox), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new EmailOutbox(objData.Tables[0].Rows[i]);
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
