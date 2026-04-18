using ImageSolutions.SalesOrder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImageSolutions.Marketing.MarketingTemplate;

namespace ImageSolutions.Marketing
{
    public class MarketingTemplate : ISBase.BaseClass
    {
        public enum enumMarketingCampaign
        {
            Welcome,
            OrderConfirmation,
            Outreach,
            OrderTracking
        }

        public string MarketingTemplateID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(MarketingTemplateID); } }
        public string WebsiteID { get; set; }
        public enumMarketingCampaign? MarketingCampaign { get; set; }
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
        public string SMSContent { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
        }


        public string MarketingCampaignName
        {
            get
            {
                string strReturn = string.Empty;
                if (Website != null)
                {
                    strReturn = Website.Name + " - " + MarketingCampaign.ToString();
                }
                return strReturn;
            }
        }

        public MarketingTemplate()
        {
        }

        public MarketingTemplate(string MarketingTemplateID)
        {
            this.MarketingTemplateID = MarketingTemplateID;
            Load();
        }

        public MarketingTemplate(DataRow objRow)
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
                         "FROM MarketingTemplate (NOLOCK) " +
                         "WHERE MarketingTemplateID=" + Database.HandleQuote(MarketingTemplateID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("MarketingTemplateID=" + MarketingTemplateID + " is not found");
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

                if (objColumns.Contains("MarketingTemplateID")) MarketingTemplateID = Convert.ToString(objRow["MarketingTemplateID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("MarketingCampaign")) MarketingCampaign = (enumMarketingCampaign)Enum.Parse(typeof(enumMarketingCampaign), Convert.ToString(objRow["MarketingCampaign"]), true);
                if (objColumns.Contains("EmailSubject")) EmailSubject = Convert.ToString(objRow["EmailSubject"]);
                if (objColumns.Contains("EmailContent")) EmailContent = Convert.ToString(objRow["EmailContent"]);
                if (objColumns.Contains("SMSContent")) SMSContent = Convert.ToString(objRow["SMSContent"]);
                if (objColumns.Contains("IsEnabled")) IsEnabled = Convert.ToBoolean(objRow["IsEnabled"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(MarketingTemplateID)) throw new Exception("Missing MarketingTemplateID in the datarow");
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (MarketingCampaign == null) throw new Exception("MarketingCampaign is required");
                if (string.IsNullOrEmpty(EmailSubject)) throw new Exception("EmailSubject is required");
                if (string.IsNullOrEmpty(EmailContent)) throw new Exception("EmailContent is required");
                if (string.IsNullOrEmpty(SMSContent)) throw new Exception("SMSContent is required");
                if (!IsNew) throw new Exception("Create cannot be performed, MarketingTemplateID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingCampaign"] = MarketingCampaign;
                dicParam["EmailSubject"] = EmailSubject;
                dicParam["EmailContent"] = EmailContent;
                dicParam["SMSContent"] = SMSContent;
                dicParam["IsEnabled"] = IsEnabled;

                MarketingTemplateID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "MarketingTemplate"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("WebsiteID is required");
                if (MarketingCampaign == null) throw new Exception("MarketingCampaign is required");
                if (string.IsNullOrEmpty(EmailSubject)) throw new Exception("EmailSubject is required");
                if (string.IsNullOrEmpty(EmailContent)) throw new Exception("EmailContent is required");
                if (string.IsNullOrEmpty(SMSContent)) throw new Exception("SMSContent is required");
                if (IsNew) throw new Exception("Update cannot be performed, MarketingTemplateID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["MarketingCampaign"] = MarketingCampaign;
                dicParam["EmailSubject"] = EmailSubject;
                dicParam["EmailContent"] = EmailContent;
                dicParam["SMSContent"] = SMSContent;
                dicParam["IsEnabled"] = IsEnabled;
                dicWParam["MarketingTemplateID"] = MarketingTemplateID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "MarketingTemplate"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, MarketingTemplateID is missing");

                dicDParam["MarketingTemplateID"] = MarketingTemplateID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "MarketingTemplate"), objConn, objTran);
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
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM MarketingTemplate (NOLOCK) p " +
                     "WHERE p.WebsiteID=" + Database.HandleQuote(WebsiteID) +
                     "AND p.MarketingCampaign=" + Database.HandleQuote(MarketingCampaign.ToString());

            if (!string.IsNullOrEmpty(MarketingTemplateID)) strSQL += "AND p.MarketingTemplateID<>" + Database.HandleQuote(MarketingTemplateID);
            return Database.HasRows(strSQL);
        }

        public static MarketingTemplate GetMarketingTemplate(MarketingTemplateFilter Filter)
        {
            List<MarketingTemplate> objSalesOrders = null;
            MarketingTemplate objReturn = null;

            try
            {
                objSalesOrders = GetMarketingTemplates(Filter);
                if (objSalesOrders != null && objSalesOrders.Count >= 1) objReturn = objSalesOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
            }
            return objReturn;
        }

        public static List<MarketingTemplate> GetMarketingTemplates()
        {
            int intTotalCount = 0;
            return GetMarketingTemplates(null, null, null, out intTotalCount);
        }

        public static List<MarketingTemplate> GetMarketingTemplates(MarketingTemplateFilter Filter)
        {
            int intTotalCount = 0;
            return GetMarketingTemplates(Filter, null, null, out intTotalCount);
        }

        public static List<MarketingTemplate> GetMarketingTemplates(MarketingTemplateFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetMarketingTemplates(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<MarketingTemplate> GetMarketingTemplates(MarketingTemplateFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<MarketingTemplate> objReturn = null;
            MarketingTemplate objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<MarketingTemplate>();

                strSQL = "SELECT * " +
                         "FROM MarketingTemplate (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.EmailSubject != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmailSubject, "EmailSubject");
                    if (Filter.EmailContent != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmailContent, "EmailContent");
                    if (Filter.SMSContent != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SMSContent, "SMSContent");
                    if (Filter.MarketingCampaigns != null && Filter.MarketingCampaigns.Count > 0) strSQL += "AND MarketingCampaign IN (" + String.Join(",", Filter.MarketingCampaigns.Select(m => Database.HandleQuote(m.ToString().Replace("_", ""))).ToArray()) + ") ";
                    if (Filter.IsEnabled != null) strSQL += "AND IsEnabled=" + Database.HandleQuote(Convert.ToInt32(Filter.IsEnabled.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "MarketingTemplateID" : Utility.CustomSorting.GetSortExpression(typeof(MarketingTemplate), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new MarketingTemplate(objData.Tables[0].Rows[i]);
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
