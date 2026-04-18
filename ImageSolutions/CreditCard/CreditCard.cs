using ImageSolutions.Account;
using ImageSolutions.Item;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.CreditCard
{
    public class CreditCard : ISBase.BaseClass
    {
        public enum enumCreditCardType
        {
            [Description("Visa")]
            Visa,
            [Description("MasterCard")]
            Mastercard,
            [Description("American Express")]
            AmericanExpress,
            [Description("Discover")]
            Discover
        }
        public enum enumTransactionType { AUTH_CAPTURE, AUTH_ONLY, PRIOR_AUTH_CAPTURE, CREDIT, VOID }

        public string CreditCardID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(CreditCardID); } }
        public string GUID { get; set; }
        public string Data { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string LastFourDigit { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CVV { get; set; }
        public string CreditCardType { get; set; }
        public string BillingAddressBookID { get; set; }
        public string PayerExternalID { get; set; }
        public string CardExternalID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string ExpirationYear {
            get
            {
                return Convert.ToString(ExpirationDate.Year);
            }
        }
        public string ExpirationMonth
        {
            get
            {
                return Convert.ToString(ExpirationDate.Month);
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

        private List<UserCreditCard> mUserCreditCards = null;
        public List<UserCreditCard> UserCreditCards
        {
            get
            {
                if (mUserCreditCards == null && !string.IsNullOrEmpty(CreditCardID))
                {
                    UserCreditCardFilter objFilter = null;

                    try
                    {
                        objFilter = new UserCreditCardFilter();
                        objFilter.CreditCardID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.CreditCardID.SearchString = CreditCardID;
                        mUserCreditCards = UserCreditCard.GetUserCreditCards(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mUserCreditCards;
            }
        }

        private Address.AddressBook mBillingAddressBook = null;
        public Address.AddressBook BillingAddressBook
        {
            get
            {
                if (mBillingAddressBook == null && !string.IsNullOrEmpty(BillingAddressBookID))
                {
                    mBillingAddressBook = new Address.AddressBook(BillingAddressBookID);
                }
                return mBillingAddressBook;
            }
            set
            {
                mBillingAddressBook = value;
            }
        }

        public CreditCard()
        {
        }
        public CreditCard(string CreditCardID)
        {
            this.CreditCardID = CreditCardID;
            Load();
        }
        public CreditCard(DataRow objRow)
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
                         "FROM CreditCard (NOLOCK) " +
                         "WHERE CreditCardID=" + Database.HandleQuote(CreditCardID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CreditCardID=" + CreditCardID + " is not found");
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
                if (objColumns.Contains("CreditCardID")) CreditCardID = Convert.ToString(objRow["CreditCardID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("Data")) Data = Convert.ToString(objRow["Data"]);
                if (objColumns.Contains("Nickname")) Nickname = Convert.ToString(objRow["Nickname"]);
                if (objColumns.Contains("FullName")) FullName = Convert.ToString(objRow["FullName"]);
                if (objColumns.Contains("LastFourDigit")) LastFourDigit = Convert.ToString(objRow["LastFourDigit"]);
                if (objColumns.Contains("ExpirationDate") && objRow["ExpirationDate"] != DBNull.Value) ExpirationDate = Convert.ToDateTime(objRow["ExpirationDate"]);
                if (objColumns.Contains("CVV")) CVV = Convert.ToString(objRow["CVV"]);
                if (objColumns.Contains("CreditCardType")) CreditCardType = Convert.ToString(objRow["CreditCardType"]);
                if (objColumns.Contains("BillingAddressBookID")) BillingAddressBookID = Convert.ToString(objRow["BillingAddressBookID"]);
                if (objColumns.Contains("PayerExternalID")) PayerExternalID = Convert.ToString(objRow["PayerExternalID"]);
                if (objColumns.Contains("CardExternalID")) CardExternalID = Convert.ToString(objRow["CardExternalID"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(CreditCardID)) throw new Exception("Missing CreditCardID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(FullName)) throw new Exception("FullName is required");
                if (string.IsNullOrEmpty(LastFourDigit)) throw new Exception("LastFourDigit is required");
                if (ExpirationDate == null) throw new Exception("ExpirationDate is required");
                if (string.IsNullOrEmpty(CVV)) throw new Exception("CVV is required");
                if (string.IsNullOrEmpty(CreditCardType)) throw new Exception("CreditCardType is required");
                if (!IsNew) throw new Exception("Create cannot be performed, CreditCardID already exists");

                dicParam["GUID"] = GUID;
                dicParam["Data"] = Data;
                dicParam["Nickname"] = Nickname;
                dicParam["FullName"] = FullName;
                dicParam["LastFourDigit"] = LastFourDigit;
                dicParam["ExpirationDate"] = ExpirationDate;
                dicParam["CVV"] = CVV;
                dicParam["CreditCardType"] = CreditCardType;
                dicParam["BillingAddressBookID"] = BillingAddressBookID;
                dicParam["PayerExternalID"] = PayerExternalID;
                dicParam["CardExternalID"] = CardExternalID;
                dicParam["CreatedBy"] = CreatedBy;
                CreditCardID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "CreditCard"), objConn, objTran).ToString();

                if (BillingAddressBook != null)
                {
                    if (BillingAddressBook.IsNew)
                    {
                        BillingAddressBook.CreditCardID = CreditCardID;
                        BillingAddressBook.Create(objConn, objTran);

                        dicParam = new Hashtable();
                        dicWParam = new Hashtable();
                        dicParam["BillingAddressBookID"] = BillingAddressBook.AddressBookID;
                        dicWParam["CreditCardID"] = CreditCardID;
                        Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditCard"), objConn, objTran);
                    }
                }

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
                if (CreditCardID == null) throw new Exception("CreditCardID is required");
                if (string.IsNullOrEmpty(FullName)) throw new Exception("FullName is required");
                if (string.IsNullOrEmpty(LastFourDigit)) throw new Exception("LastFourDigit is required");
                if (ExpirationDate == null) throw new Exception("ExpirationDate is required");
                if (string.IsNullOrEmpty(CVV)) throw new Exception("CVV is required");
                if (string.IsNullOrEmpty(CreditCardType)) throw new Exception("CreditCardType is required");
                if (IsNew) throw new Exception("Update cannot be performed, CreditCardID is missing");

                dicParam["Data"] = Data;
                dicParam["Nickname"] = Nickname;
                dicParam["FullName"] = FullName;
                dicParam["LastFourDigit"] = LastFourDigit;
                dicParam["ExpirationDate"] = ExpirationDate;
                dicParam["CVV"] = CVV;
                dicParam["CreditCardType"] = CreditCardType;
                dicParam["BillingAddressBookID"] = BillingAddressBookID;
                dicParam["PayerExternalID"] = PayerExternalID;
                dicParam["CardExternalID"] = CardExternalID;
                dicWParam["CreditCardID"] = CreditCardID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditCard"), objConn, objTran);


                if (BillingAddressBook != null)
                {
                    if (BillingAddressBook.IsNew)
                    {
                        BillingAddressBook.CreditCardID = CreditCardID;
                        BillingAddressBook.Create(objConn, objTran);

                        dicParam = new Hashtable();
                        dicWParam = new Hashtable();
                        dicParam["BillingAddressBookID"] = BillingAddressBook.AddressBookID;
                        dicWParam["CreditCardID"] = CreditCardID;
                        Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "CreditCard"), objConn, objTran);
                    }
                    else
                    {
                        BillingAddressBook.Update(objConn, objTran);
                    }
                }
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
                if (IsNew) throw new Exception("Delete cannot be performed, CreditCardID is missing");

                foreach (ImageSolutions.User.UserCreditCard _UserCreditCard in UserCreditCards)
                {
                    _UserCreditCard.Delete(objConn, objTran);
                }

                dicDParam["CreditCardID"] = CreditCardID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "CreditCard"), objConn, objTran);
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

        public static CreditCard GetCreditCard(CreditCardFilter Filter)
        {
            List<CreditCard> objCreditCards = null;
            CreditCard objReturn = null;

            try
            {
                objCreditCards = GetCreditCards(Filter);
                if (objCreditCards != null && objCreditCards.Count >= 1) objReturn = objCreditCards[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCreditCards = null;
            }
            return objReturn;
        }

        public static List<CreditCard> GetCreditCards()
        {
            int intTotalCount = 0;
            return GetCreditCards(null, null, null, out intTotalCount);
        }

        public static List<CreditCard> GetCreditCards(CreditCardFilter Filter)
        {
            int intTotalCount = 0;
            return GetCreditCards(Filter, null, null, out intTotalCount);
        }

        public static List<CreditCard> GetCreditCards(CreditCardFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetCreditCards(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<CreditCard> GetCreditCards(CreditCardFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<CreditCard> objReturn = null;
            CreditCard objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<CreditCard>();

                strSQL = "SELECT * " +
                         "FROM CreditCard (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.LastFiveDigit != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.LastFiveDigit, "LastFiveDigit");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreditCardID" : Utility.CustomSorting.GetSortExpression(typeof(CreditCard), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new CreditCard(objData.Tables[0].Rows[i]);
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
