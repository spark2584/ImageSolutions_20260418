using ImageSolutions.Item;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.User
{
    public class UserCreditCard : ISBase.BaseClass
    {
        public string UserCreditCardID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserCreditCardID); } }
        public string UserInfoID { get; set; }
        public string CreditCardID { get; set; }
        public int? ResetDayOfTheMonth{ get; set; }
        public double? Limit { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string Description
        {
            get
            {
                if (ResetDayOfTheMonth != null && ResetDayOfTheMonth.Value > 0
                    && Limit != null && Limit > 0)
                    return CreditCard == null ? string.Empty : String.Format("{0} - {1} {2}", CreditCard.Nickname, string.Format("exp: {0}/{1}", CreditCard.ExpirationMonth, CreditCard.ExpirationYear), string.Format("( Remaining: ${0} )", Convert.ToString(RemainingBalance)) );
                else
                    return CreditCard == null ? string.Empty : String.Format("{0} - {1}", CreditCard.Nickname, string.Format("exp: {0}/{1}", CreditCard.ExpirationMonth, CreditCard.ExpirationYear));

                //if (ResetDayOfTheMonth != null && ResetDayOfTheMonth.Value > 0
                //    && Limit != null && Limit > 0)
                //    return CreditCard == null ? string.Empty : CreditCard.CreditCardType + " " + CreditCard.LastFourDigit + "( Remaining: "+ Convert.ToString(RemainingBalance) + " )";
                //else
                //    return CreditCard == null ? string.Empty : CreditCard.CreditCardType + " " + CreditCard.LastFourDigit;
            }
        }
        public double? RemainingBalance
        {
            get
            {
                if(ResetDayOfTheMonth != null && ResetDayOfTheMonth.Value > 0
                    && Limit != null && Limit > 0)
                {
                    SqlDataReader objRead = null;
                    string strSQL = string.Empty;

                    try
                    {
                        strSQL = string.Format(@"
DECLARE @day int
SET @day = {0}

DECLARE @currentmonthday int
DECLARE @previousmonthday int
DECLARE @nextmonthday int
SELECT 
	@currentmonthday = CASE WHEN @day > DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1)))) 
		THEN DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1))))
		ELSE @day END,
	@previousmonthday = CASE WHEN @day > DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), MONTH(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), 1)))) 
		THEN DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), MONTH(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), 1))))
		ELSE @day END,
	@nextmonthday = CASE WHEN @day > DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), MONTH(MONTH(DATEADD(MONTH,-1,GETUTCDATE()))), 1)))) 
		THEN DAY(DATEADD(DAY, -1,DATEADD(MONTH, +1, DATEFROMPARTS(YEAR(MONTH(DATEADD(MONTH,1,GETUTCDATE()))), MONTH(MONTH(DATEADD(MONTH,1,GETUTCDATE()))), 1))))
		ELSE @day END

SELECT ISNULL(SUM(p.AmountPaid),0) As Amount
FROM Payment (NOLOCK) p
Inner Join CreditCard (NOLOCK) c on c.CreditCardID = p.CreditCardID
Outer Apply
(
	SELECT CASE 
		WHEN DAY(GETUTCDATE()) < @currentmonthday 
			THEN DATEFROMPARTS(YEAR(DATEADD(MONTH,-1,GETUTCDATE())), MONTH(DATEADD(MONTH,-1,GETUTCDATE())), @previousmonthday)
		ELSE 
			DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), @currentmonthday) END as Value
) FromDate
Outer Apply
(
	SELECT CASE 
		WHEN DAY(GETUTCDATE()) < @currentmonthday
			THEN DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), @currentmonthday) 
		ELSE
			DATEFROMPARTS(YEAR(DATEADD(MONTH,1,GETUTCDATE())), MONTH(DATEADD(MONTH,1,GETUTCDATE())), @nextmonthday) END as Value
) ToDate
WHERE p.UserInfoID = {1}
and p.CreditCardID = {2}
and p.CreatedOn between FromDate.Value and ToDate.Value
"
                            , Database.HandleQuote(Convert.ToString(ResetDayOfTheMonth))
                            , Database.HandleQuote(Convert.ToString(UserInfoID))
                            , Database.HandleQuote(Convert.ToString(CreditCardID))
                            );
                        objRead = Database.GetDataReader(strSQL);
                        if(objRead.Read())
                        {
                            return Limit - Convert.ToDouble(Convert.ToString(objRead["Amount"]));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (objRead != null) objRead.Dispose();
                        objRead = null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

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

        private ImageSolutions.CreditCard.CreditCard mCreditCard = null;
        public ImageSolutions.CreditCard.CreditCard CreditCard
        {
            get
            {
                if (mCreditCard == null && !string.IsNullOrEmpty(CreditCardID))
                {
                    mCreditCard = new ImageSolutions.CreditCard.CreditCard(CreditCardID);
                }
                return mCreditCard;
            }
        }
        private UserInfo mUserInfo = null;
        public UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }
        public UserCreditCard()
        {
        }
        public UserCreditCard(string UserCreditCardID)
        {
            this.UserCreditCardID = UserCreditCardID;
            Load();
        }
        public UserCreditCard(DataRow objRow)
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
                         "FROM UserCreditCard (NOLOCK) " +
                         "WHERE UserCreditCardID=" + Database.HandleQuote(UserCreditCardID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserCreditCardID=" + UserCreditCardID + " is not found");
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
                if (objColumns.Contains("UserCreditCardID")) UserCreditCardID = Convert.ToString(objRow["UserCreditCardID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("CreditCardID")) CreditCardID = Convert.ToString(objRow["CreditCardID"]);
                if (objColumns.Contains("ResetDayOfTheMonth") && objRow["ResetDayOfTheMonth"] != DBNull.Value) ResetDayOfTheMonth = Convert.ToInt32(objRow["ResetDayOfTheMonth"]);
                if (objColumns.Contains("Limit") && objRow["Limit"] != DBNull.Value) Limit = Convert.ToDouble(objRow["Limit"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserCreditCardID)) throw new Exception("Missing UserCreditCardID in the datarow");
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
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (string.IsNullOrEmpty(CreditCardID)) throw new Exception("CreditCardID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, UserCreditCardID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["ResetDayOfTheMonth"] = ResetDayOfTheMonth;
                dicParam["Limit"] = Limit;
                dicParam["CreatedBy"] = CreatedBy;
                UserCreditCardID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "UserCreditCard"), objConn, objTran).ToString();

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
                if (UserCreditCardID == null) throw new Exception("UserCreditCardID is required");
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (string.IsNullOrEmpty(CreditCardID)) throw new Exception("CreditCardID is required");
                if (IsNew) throw new Exception("Update cannot be performed, UserCreditCardID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["ResetDayOfTheMonth"] = ResetDayOfTheMonth;
                dicParam["Limit"] = Limit;
                dicWParam["UserCreditCardID"] = UserCreditCardID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "UserCreditCard"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, UserCreditCardID is missing");

                //Delete Lines
                //List<UserCreditCardLine> lstUserCreditCardLine;
                //lstUserCreditCardLine = UserCreditCardLines;
                //foreach (UserCreditCardLine _UserCreditCardLine in lstUserCreditCardLine)
                //{
                //    _UserCreditCardLine.Delete(objConn, objTran);
                //}

                dicDParam["UserCreditCardID"] = UserCreditCardID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "UserCreditCard"), objConn, objTran);
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
                    "FROM UserCreditCard (NOLOCK) p " +
                    "WHERE " +
                    "(" +
                    "  (p.UserInfoID=" + Database.HandleQuote(UserInfoID) + " AND p.CreditCardID=" + Database.HandleQuote(CreditCardID) + ")" +
                    ") ";


            if (!string.IsNullOrEmpty(UserCreditCardID)) strSQL += "AND p.UserCreditCardID<>" + Database.HandleQuote(UserCreditCardID);
            return Database.HasRows(strSQL);
        }

        public static UserCreditCard GetUserCreditCard(UserCreditCardFilter Filter)
        {
            List<UserCreditCard> objUserCreditCards = null;
            UserCreditCard objReturn = null;

            try
            {
                objUserCreditCards = GetUserCreditCards(Filter);
                if (objUserCreditCards != null && objUserCreditCards.Count >= 1) objReturn = objUserCreditCards[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objUserCreditCards = null;
            }
            return objReturn;
        }

        public static List<UserCreditCard> GetUserCreditCards()
        {
            int intTotalCount = 0;
            return GetUserCreditCards(null, null, null, out intTotalCount);
        }

        public static List<UserCreditCard> GetUserCreditCards(UserCreditCardFilter Filter)
        {
            int intTotalCount = 0;
            return GetUserCreditCards(Filter, null, null, out intTotalCount);
        }

        public static List<UserCreditCard> GetUserCreditCards(UserCreditCardFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUserCreditCards(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<UserCreditCard> GetUserCreditCards(UserCreditCardFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<UserCreditCard> objReturn = null;
            UserCreditCard objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<UserCreditCard>();

                strSQL = "SELECT * " +
                         "FROM UserCreditCard (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.CreditCardID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CreditCardID, "CreditCardID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "UserCreditCardID" : Utility.CustomSorting.GetSortExpression(typeof(UserCreditCard), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new UserCreditCard(objData.Tables[0].Rows[i]);
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
