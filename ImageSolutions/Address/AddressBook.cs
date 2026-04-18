using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Address
{
    public class AddressBook : ISBase.Address
    {
        public string AddressBookID { get; protected set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AddressBookID); } }
        public string UserInfoID { get; set; }
        public string AccountID { get; set; }
        public string CreditCardID { get; set; }

        public bool IsInvalid { get; set; }
        public bool IsUpdated { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public AddressBook()
        {
        }

        public AddressBook(string AddressBookID)
        {
            this.AddressBookID = AddressBookID;
            Load();
        }

        public AddressBook(DataRow objRow)
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
                strSQL = "SELECT ab.* " +
                         "FROM AddressBook ab (NOLOCK) " +
                         "WHERE ab.AddressBookID=" + Database.HandleQuote(AddressBookID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AddressBookID=" + AddressBookID + " is not found");
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

                if (objColumns.Contains("AddressBookID")) AddressBookID = Convert.ToString(objRow["AddressBookID"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("AccountID")) AccountID = Convert.ToString(objRow["AccountID"]);
                if (objColumns.Contains("CreditCardID")) CreditCardID = Convert.ToString(objRow["CreditCardID"]);
                if (objColumns.Contains("AddressLabel")) AddressLabel = Convert.ToString(objRow["AddressLabel"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("AddressLine1")) AddressLine1 = Convert.ToString(objRow["AddressLine1"]);
                if (objColumns.Contains("AddressLine2")) AddressLine2 = Convert.ToString(objRow["AddressLine2"]);
                if (objColumns.Contains("AddressLine3")) AddressLine3 = Convert.ToString(objRow["AddressLine3"]);
                if (objColumns.Contains("City")) City = Convert.ToString(objRow["City"]);
                if (objColumns.Contains("State")) State = Convert.ToString(objRow["State"]);
                if (objColumns.Contains("PostalCode")) PostalCode = Convert.ToString(objRow["PostalCode"]);
                if (objColumns.Contains("CountryID")) CountryID = Convert.ToString(objRow["CountryID"]);
                if (objColumns.Contains("CountryCode")) CountryCode = Convert.ToString(objRow["CountryCode"]);
                if (objColumns.Contains("CountryName")) CountryName = Convert.ToString(objRow["CountryName"]);
                if (objColumns.Contains("PhoneNumber")) PhoneNumber = Convert.ToString(objRow["PhoneNumber"]);
                if (objColumns.Contains("PhoneExtension")) PhoneExtension = Convert.ToString(objRow["PhoneExtension"]);
                if (objColumns.Contains("Fax")) Fax = Convert.ToString(objRow["Fax"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);

                if (objColumns.Contains("IsInvalid")) IsInvalid = Convert.ToBoolean(objRow["IsInvalid"]);
                if (objColumns.Contains("IsUpdated")) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);

                if (objColumns.Contains("WebsiteURL")) WebsiteURL = Convert.ToString(objRow["WebsiteURL"]);
                if (objColumns.Contains("UpdatedBy")) UpdatedBy = Convert.ToString(objRow["UpdatedBy"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AddressBookID)) throw new Exception("Missing AddressBookID in the datarow");
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
                if (string.IsNullOrEmpty(UserInfoID) && string.IsNullOrEmpty(AccountID) && string.IsNullOrEmpty(CreditCardID)) throw new Exception("UserInfoID or AccountID or CreditCardID is required");
                //if (string.IsNullOrEmpty(AddressLabel)) throw new Exception("AddressLabel is required");
                if (string.IsNullOrEmpty(AddressLine1) && string.IsNullOrEmpty(AddressLine2)) throw new Exception("AddressLine1 or AddressLine2 is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AddressBookID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam.Clear();
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["AccountID"] = AccountID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["AddressLabel"] = AddressLabel;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["CompanyName"] = CompanyName;
                dicParam["AddressLine1"] = AddressLine1;
                dicParam["AddressLine2"] = AddressLine2;
                dicParam["AddressLine3"] = AddressLine3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = CountryCode == "CA" ? PostalCode.Replace("-"," ") : PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneExtension"] = PhoneExtension;
                dicParam["Fax"] = Fax;
                dicParam["Email"] = Email;
                dicParam["WebsiteURL"] = WebsiteURL;

                dicParam["IsInvalid"] = IsInvalid;
                dicParam["IsUpdated"] = IsUpdated;

                dicParam["CreatedBy"] = CreatedBy;
                AddressBookID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AddressBook"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(UserInfoID) && string.IsNullOrEmpty(AccountID) && string.IsNullOrEmpty(CreditCardID)) throw new Exception("UserInfoID or AccountID or CreditCardID is required");
                //if (string.IsNullOrEmpty(AddressLabel)) throw new Exception("AddressLabel is required");
                if (string.IsNullOrEmpty(AddressLine1) && string.IsNullOrEmpty(AddressLine2)) throw new Exception("AddressLine1 or AddressLine2 is required");
                if (string.IsNullOrEmpty(CreatedBy)) throw new Exception("CreatedBy is required");
                if (IsNew) throw new Exception("Update cannot be performed, AddressBookID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["UserInfoID"] = UserInfoID;
                dicParam["AccountID"] = AccountID;
                dicParam["CreditCardID"] = CreditCardID;
                dicParam["AddressLabel"] = AddressLabel;
                dicParam["CompanyName"] = CompanyName;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["AddressLine1"] = AddressLine1;
                dicParam["AddressLine2"] = AddressLine2;
                dicParam["AddressLine3"] = AddressLine3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = CountryCode == "CA" ? PostalCode.Replace("-", " ") : PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneExtension"] = PhoneExtension;
                dicParam["Fax"] = Fax;
                dicParam["Email"] = Email;
                dicParam["WebsiteURL"] = WebsiteURL;

                dicParam["IsInvalid"] = IsInvalid;
                dicParam["IsUpdated"] = IsUpdated;

                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicWParam["AddressBookID"] = AddressBookID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AddressBook"), objConn, objTran);
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

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();
            ArrayList arylQuery = new ArrayList();

            try
            {
                if (string.IsNullOrEmpty(AddressBookID)) throw new Exception("Missing AddressBookID");

                if (!string.IsNullOrEmpty(UserInfoID))
                {
                    dicParam.Clear();
                    dicWParam.Clear();
                    dicParam["DefaultBillingAddressBookID"] = DBNull.Value;
                    dicWParam["DefaultBillingAddressBookID"] = AddressBookID;
                    dicWParam["UserInfoID"] = UserInfoID;
                    arylQuery.Add(Database.GetUpdateSQL(dicParam, dicWParam, "UserInfo"));

                    dicParam.Clear();
                    dicWParam.Clear();
                    dicParam["DefaultShippingAddressBookID"] = DBNull.Value;
                    dicWParam["DefaultShippingAddressBookID"] = AddressBookID;
                    dicWParam["UserInfoID"] = UserInfoID;
                    arylQuery.Add(Database.GetUpdateSQL(dicParam, dicWParam, "UserInfo"));
                }
                dicParam.Clear();
                dicWParam.Clear();
                dicWParam["AddressBookID"] = AddressBookID;
                arylQuery.Add(Database.GetDeleteSQL(dicWParam, "AddressBook"));

                Database.ExecuteList(arylQuery, objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            return false;
        }
        public static AddressBook GetAddressBook(AddressBookFilter Filter)
        {
            AddressBook objReturn = null;
            AddressBook objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                //if (!string.IsNullOrEmpty(BusinessID))
                //{
                objReturn = new AddressBook();

                strSQL = "SELECT ab.* " +
                            "FROM AddressBook ab (NOLOCK) " +
                            "WHERE 1=1 "; //ab.BusinessID=" + Database.HandleQuote(BusinessID);

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.AddressBookID)) strSQL += "AND ab.AddressBookID = " + Database.HandleQuote(Filter.AddressLabel.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.UserInfoID)) { strSQL += "AND ab.UserInfoID=" + Database.HandleQuote(Filter.UserInfoID); }
                    if (!string.IsNullOrEmpty(Filter.AccountID)) { strSQL += "AND ab.AccountID=" + Database.HandleQuote(Filter.AccountID); }
                    if (!string.IsNullOrEmpty(Filter.AddressLabel)) strSQL += "AND ab.AddressLabel LIKE " + Database.HandleQuote(Filter.AddressLabel.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.AddressLine1)) strSQL += "AND ab.AddressLine1 LIKE " + Database.HandleQuote(Filter.AddressLine1.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.AddressLine2)) strSQL += "AND ab.AddressLine2 LIKE " + Database.HandleQuote(Filter.AddressLine2.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.City)) strSQL += "AND ab.City LIKE " + Database.HandleQuote(Filter.City.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.State)) strSQL += "AND ab.State LIKE " + Database.HandleQuote(Filter.State.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.PostalCode)) strSQL += "AND ab.PostalCode LIKE " + Database.HandleQuote(Filter.PostalCode.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.CountryID)) strSQL += "AND ab.CountryID LIKE " + Database.HandleQuote(Filter.CountryID.Replace("*", "%"));
                }

                if (objData != null && objData.Tables[0].Rows.Count > 1)
                {
                    throw new Exception("more than one AddressBookID found");
                }
                else if(objData != null && objData.Tables[0].Rows.Count == 1)
                {
                    objReturn = new AddressBook(objData.Tables[0].Rows[0]);
                }
                else
                {
                    objReturn = null;
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

        public static List<AddressBook> GetAddressBooks()
        {
            int intTotalCount = 0;
            return GetAddressBooks(null, null, null, out intTotalCount);
        }

        public static List<AddressBook> GetAddressBooks(AddressBookFilter Filter)
        {
            int intTotalCount = 0;
            return GetAddressBooks(Filter, null, null, out intTotalCount);
        }

        public static List<AddressBook> GetAddressBooks(AddressBookFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAddressBooks(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AddressBook> GetAddressBooks(AddressBookFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AddressBook> objReturn = null;
            AddressBook objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                //if (!string.IsNullOrEmpty(BusinessID))
                //{
                objReturn = new List<AddressBook>();

                strSQL = "SELECT ab.* " +
                            "FROM AddressBook ab (NOLOCK) " +
                            "WHERE 1=1 "; //ab.BusinessID=" + Database.HandleQuote(BusinessID);

                if (Filter != null)
                {
                    if (!string.IsNullOrEmpty(Filter.AddressBookID)) strSQL += "AND ab.AddressBookID = " + Database.HandleQuote(Filter.AddressLabel.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.UserInfoID)) { strSQL += "AND ab.UserInfoID=" + Database.HandleQuote(Filter.UserInfoID); }
                    if (!string.IsNullOrEmpty(Filter.AccountID)) { strSQL += "AND ab.AccountID=" + Database.HandleQuote(Filter.AccountID); }
                    if (!string.IsNullOrEmpty(Filter.AddressLabel)) strSQL += "AND ab.AddressLabel LIKE " + Database.HandleQuote(Filter.AddressLabel.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.AddressLine1)) strSQL += "AND ab.AddressLine1 LIKE " + Database.HandleQuote(Filter.AddressLine1.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.AddressLine2)) strSQL += "AND ab.AddressLine2 LIKE " + Database.HandleQuote(Filter.AddressLine2.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.City)) strSQL += "AND ab.City LIKE " + Database.HandleQuote(Filter.City.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.State)) strSQL += "AND ab.State LIKE " + Database.HandleQuote(Filter.State.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.PostalCode)) strSQL += "AND ab.PostalCode LIKE " + Database.HandleQuote(Filter.PostalCode.Replace("*", "%"));
                    if (!string.IsNullOrEmpty(Filter.CountryID)) strSQL += "AND ab.CountryID LIKE " + Database.HandleQuote(Filter.CountryID.Replace("*", "%"));
                }

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreatedOn" : Utility.CustomSorting.GetSortExpression(typeof(AddressBook), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                    strSQL += "ORDER BY CreatedOn ";
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AddressBook(objData.Tables[0].Rows[i]);
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
    }
}
