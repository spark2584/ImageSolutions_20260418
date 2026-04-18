using ImageSolutions.Budget;
using ImageSolutions.Payment;
using ImageSolutions.SalesOrder;
using ImageSolutions.Website;
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

namespace ImageSolutions.User
{
    public class UserInfo : ISBase.BaseClass
    {
        public string UserInfoID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(UserInfoID); } }
        public string GUID { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string UserPoolUserName { get; set; }
        public bool EmailWhiteListed { get; set; }
        public string Password { get; set; }
        public string PasswordResetGUID { get; set; }
        public DateTime? PasswordResetRequestedOn { get; set; }
        public string Passcode { get; set; }
        public DateTime? PasscodeRequestedOn { get; set; }
        public string DefaultBillingAddressBookID { get; set; }
        public string DefaultShippingAddressBookID { get; set; }
        public bool? IsAdmin { get; set; }
        public string CustomerInternalID { get; set; }
        public string ImagePath { get; set; }
        public string LastVisitedUserWebsiteID { get; set; }
        public string LastVisitedUserAccountID { get; set; }
        public bool RequirePasswordReset { get; set; }
        public string ExternalID { get; set; }
        public bool IsGuest { get; set; }
        public bool AllowOnlySSO { get; set; }
        public bool InActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        private Address.AddressBook mDefaultBillingAddressBook = null;
        public Address.AddressBook DefaultBillingAddressBook
        {
            get
            {
                if (mDefaultBillingAddressBook == null && !string.IsNullOrEmpty(DefaultBillingAddressBookID))
                {
                    mDefaultBillingAddressBook = new Address.AddressBook(DefaultBillingAddressBookID);
                }
                return mDefaultBillingAddressBook;
            }
        }
        private Address.AddressBook mDefaultShippingAddressBook = null;
        public Address.AddressBook DefaultShippingAddressBook
        {
            get
            {
                if (mDefaultShippingAddressBook == null && !string.IsNullOrEmpty(DefaultShippingAddressBookID))
                {
                    mDefaultShippingAddressBook = new Address.AddressBook(DefaultShippingAddressBookID);
                }
                return mDefaultShippingAddressBook;
            }
        }

        private User.UserAccount mLastVisitedUserAccount = null;
        public User.UserAccount LastVisitedUserAccount
        {
            get
            {
                if (mLastVisitedUserAccount == null && !string.IsNullOrEmpty(LastVisitedUserAccountID))
                {
                    mLastVisitedUserAccount = new UserAccount(LastVisitedUserAccountID);
                }
                return mLastVisitedUserAccount;
            }
        }

        private User.UserWebsite mLastVisitedUserWebsite = null;
        public User.UserWebsite LastVisitedUserWebsite
        {
            get
            {
                if (mLastVisitedUserWebsite == null && !string.IsNullOrEmpty(LastVisitedUserWebsiteID))
                {
                    mLastVisitedUserWebsite = new UserWebsite(LastVisitedUserWebsiteID);
                }
                return mLastVisitedUserWebsite;
            }
        }

        private List<User.UserWebsite> mUserWebsites = null;
        public List<User.UserWebsite> UserWebsites
        {
            get
            {
                if (mUserWebsites == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    User.UserWebsiteFilter objFilter = null;
                    
                    try
                    {
                        objFilter = new UserWebsiteFilter();
                        objFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserInfoID.SearchString = UserInfoID;
                        mUserWebsites = User.UserWebsite.GetUserWebsites(objFilter);
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
                return mUserWebsites;
            }
        }

        private List<Address.AddressBook> mAddressBooks = null;
        public List<Address.AddressBook> AddressBooks
        {
            get
            {
                if (mAddressBooks == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    Address.AddressBookFilter objFilter = null;

                    try
                    {
                        objFilter = new Address.AddressBookFilter();
                        objFilter.UserInfoID = UserInfoID;
                        mAddressBooks = Address.AddressBook.GetAddressBooks(objFilter);
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
                return mAddressBooks;
            }
        }

        //private List<SalesOrder.SalesOrder> mSalesOrders = null;
        //public List<SalesOrder.SalesOrder> SalesOrders
        //{
        //    get
        //    {
        //        if (mSalesOrders == null && !string.IsNullOrEmpty(UserInfoID))
        //        {
        //            SalesOrder.SalesOrderFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new SalesOrder.SalesOrderFilter();
        //                objFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.UserInfoID.SearchString = UserInfoID;
        //                mSalesOrders = SalesOrder.SalesOrder.GetSalesOrders(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mSalesOrders;
        //    }
        //}

        private List<User.UserCreditCard> mUserCreditCards = null;
        public List<User.UserCreditCard> UserCreditCards
        {
            get
            {
                if (mUserCreditCards == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    User.UserCreditCardFilter objFilter = null;

                    try
                    {
                        objFilter = new UserCreditCardFilter();
                        objFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserInfoID.SearchString = UserInfoID;

                        mUserCreditCards = User.UserCreditCard.GetUserCreditCards(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return mUserCreditCards;
            }
        }

        public UserInfo()
        {
        }
        public UserInfo(string UserInfoID)
        {
            this.UserInfoID = UserInfoID;
            Load();
        }
        public UserInfo(DataRow objRow)
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
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE UserInfoID=" + Database.HandleQuote(UserInfoID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("UserInfoID=" + UserInfoID + " is not found");
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
        protected void Load(string GUID)
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE GUID=" + Database.HandleQuote(GUID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("GUID=" + GUID + " is not found");
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

                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("IsSuperAdmin")) IsSuperAdmin = Convert.ToBoolean(objRow["IsSuperAdmin"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]); 
                if (objColumns.Contains("DisplayName")) DisplayName = Convert.ToString(objRow["DisplayName"]);
                if (objColumns.Contains("EmailAddress")) EmailAddress = Convert.ToString(objRow["EmailAddress"]);
                if (objColumns.Contains("UserName")) UserName = Convert.ToString(objRow["UserName"]);
                if (objColumns.Contains("UserPoolUserName")) UserPoolUserName = Convert.ToString(objRow["UserPoolUserName"]);
                if (objColumns.Contains("EmailWhiteListed")) EmailWhiteListed = Convert.ToBoolean(objRow["EmailWhiteListed"]);
                if (objColumns.Contains("Password")) Password = Convert.ToString(objRow["Password"]);
                if (objColumns.Contains("PasswordResetGUID")) PasswordResetGUID = Convert.ToString(objRow["PasswordResetGUID"]);
                if (objColumns.Contains("PasswordResetRequestedOn") && objRow["PasswordResetRequestedOn"] != DBNull.Value) PasswordResetRequestedOn = Convert.ToDateTime(objRow["PasswordResetRequestedOn"]);
                if (objColumns.Contains("Passcode")) Passcode = Convert.ToString(objRow["Passcode"]);
                if (objColumns.Contains("PasscodeRequestedOn") && objRow["PasscodeRequestedOn"] != DBNull.Value) PasscodeRequestedOn = Convert.ToDateTime(objRow["PasscodeRequestedOn"]);
                if (objColumns.Contains("DefaultShippingAddressBookID")) DefaultShippingAddressBookID = Convert.ToString(objRow["DefaultShippingAddressBookID"]);
                if (objColumns.Contains("DefaultBillingAddressBookID")) DefaultBillingAddressBookID = Convert.ToString(objRow["DefaultBillingAddressBookID"]);
                if (objColumns.Contains("IsAdmin") && objRow["IsAdmin"] != DBNull.Value) IsAdmin = Convert.ToBoolean(objRow["IsAdmin"]);
                if (objColumns.Contains("CustomerInternalID")) CustomerInternalID = Convert.ToString(objRow["CustomerInternalID"]);
                if (objColumns.Contains("ImagePath")) ImagePath = Convert.ToString(objRow["ImagePath"]);
                if (objColumns.Contains("LastVisitedUserWebsiteID")) LastVisitedUserWebsiteID = Convert.ToString(objRow["LastVisitedUserWebsiteID"]);
                if (objColumns.Contains("LastVisitedUserAccountID")) LastVisitedUserAccountID = Convert.ToString(objRow["LastVisitedUserAccountID"]);
                if (objColumns.Contains("RequirePasswordReset")) RequirePasswordReset = Convert.ToBoolean(objRow["RequirePasswordReset"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("IsGuest")) IsGuest = Convert.ToBoolean(objRow["IsGuest"]);
                if (objColumns.Contains("AllowOnlySSO")) AllowOnlySSO = Convert.ToBoolean(objRow["AllowOnlySSO"]);
                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);

                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("Missing UserInfoID in the datarow");
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
        protected virtual bool LoginByUserInfoID(SqlConnection objConn, SqlTransaction objTran)
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            string strPasswordHash = string.Empty;
            string strPasswordSaltKey = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE UserInfoID=" + Database.HandleQuote(UserInfoID);
                objData = Database.GetDataSet(strSQL, objConn, objTran);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Invalid UserInfoID");
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
            return true;
        }

        protected virtual bool LoginByUserInfoID(string userinfoid)
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            string strPasswordHash = string.Empty;
            string strPasswordSaltKey = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE UserInfoID=" + Database.HandleQuote(userinfoid);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Invalid UserInfoID");
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
            return true;
        }

        protected virtual bool Login(string EmailAddress, string Password)
        {
            return Login(EmailAddress, Password, false);
        }

        protected virtual bool Login(string EmailAddress, string Password, bool LoginnOverride)
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            string strPasswordHash = string.Empty;
            string strPasswordSaltKey = string.Empty;

            try
            {
                strSQL = "SELECT * " +
         "FROM UserInfo (NOLOCK) " +
         "WHERE EmailAddress=" + Database.HandleQuote(EmailAddress);

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToBoolean(objData.Tables[0].Rows[0]["RequirePasswordReset"]) && !LoginnOverride)
                    {
                        throw new Exception("Password reset required. Email has been sent with the link to reset password.");
                    }
                    else
                    {
                        if (LoginnOverride)
                        {
                            Load(objData.Tables[0].Rows[0]);
                        }
                        else if (objData.Tables[0].Rows[0]["Password"].ToString() != Password)
                        {
                            throw new Exception("Invalid email/password");
                        }
                        else
                        {
                            Load(objData.Tables[0].Rows[0]);
                        }
                    }
                }
                else
                {
                    strSQL = "SELECT * " +
         "FROM UserInfo (NOLOCK) " +
         "WHERE ISNULL(UserName, '') != '' and UserName = " + Database.HandleQuote(EmailAddress);

                    objData = Database.GetDataSet(strSQL);
                    if (objData != null && objData.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(objData.Tables[0].Rows[0]["RequirePasswordReset"]) && !LoginnOverride)
                        {
                            throw new Exception("Password reset required. Email has been sent with the link to reset password.");
                        }
                        else
                        {
                            if (LoginnOverride)
                            {
                                Load(objData.Tables[0].Rows[0]);
                            }
                            else if (objData.Tables[0].Rows[0]["Password"].ToString() != Password)
                            {
                                throw new Exception("Invalid email/password");
                            }
                            else
                            {
                                Load(objData.Tables[0].Rows[0]);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid email/password");
                    }
                }

                //strSQL = "SELECT * " +
                //         "FROM UserInfo (NOLOCK) " +
                //         "WHERE EmailAddress=" + Database.HandleQuote(EmailAddress) + " " + 
                //         "or ( ISNULL(UserName,'') != '' and UserName =" + Database.HandleQuote(EmailAddress) + ")";
                //objData = Database.GetDataSet(strSQL);
                //if (objData != null && objData.Tables[0].Rows.Count > 0)
                //{
                //    //strPasswordHash = objData.Tables[0].Rows[0]["PasswordHash"].ToString();
                //    //strPasswordSaltKey = objData.Tables[0].Rows[0]["PasswordSaltKey"].ToString();
                //    //if (strPasswordHash != Utility.Security.CreatePasswordHash(Password, strPasswordSaltKey)) throw new Exception("Invalid password");

                //    if (Convert.ToBoolean(objData.Tables[0].Rows[0]["RequirePasswordReset"]) && !LoginnOverride)
                //    {
                //        //ResetPassword(EmailAddress);
                //        throw new Exception("Password reset required. Email has been sent with the link to reset password.");
                //    }
                //    else
                //    {
                //        if (LoginnOverride)
                //        {
                //            Load(objData.Tables[0].Rows[0]);
                //        }
                //        else if (objData.Tables[0].Rows[0]["Password"].ToString() != Password)
                //        {
                //            throw new Exception("Invalid email/password");
                //        }
                //        else
                //        {
                //            Load(objData.Tables[0].Rows[0]);
                //        }
                //    }
                //}
                //else
                //{
                //    throw new Exception("Invalid email/password");
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return true;
        }

        protected virtual bool LoginPasscode(string EmailAddress, string Passcode)
        {
            DataSet objData = null;
            string strSQL = string.Empty;
            string strPasswordHash = string.Empty;
            string strPasswordSaltKey = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE EmailAddress=" + Database.HandleQuote(EmailAddress);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0]["Passcode"].ToString() != Passcode) throw new Exception("Invalid Passcode");

                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Invalid email/passcode");
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
            return true;
        }

        protected string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }      
        protected bool ExistsEmailAddress()
        {
            bool _ret = false;

            try
            {
                UserInfo UserInfo = new UserInfo();
                UserInfoFilter UserInfoFilter = new UserInfoFilter();
                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                UserInfoFilter.EmailAddress.SearchString = EmailAddress;
                UserInfo = UserInfo.GetUserInfo(UserInfoFilter);
                if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID) && UserInfo.UserInfoID != UserInfoID)
                {
                    _ret = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _ret;
        }
        protected bool ExistsUserName()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                        "FROM UserInfo (NOLOCK) p " +
                        "WHERE " +
                        "(" +
                        "  ( ISNULL(p.UserName,'') != '' AND p.UserName=" + Database.HandleQuote(UserName) + ") )";

            if (!string.IsNullOrEmpty(UserInfoID)) strSQL += "AND p.UserInfoID <> " + Database.HandleQuote(UserInfoID);
            return Database.HasRows(strSQL);


            //bool _ret = false;

            //try
            //{
            //    if(!string.IsNullOrEmpty(UserName))
            //    {
            //        UserInfo UserInfo = new UserInfo();
            //        UserInfoFilter UserInfoFilter = new UserInfoFilter();
            //        UserInfoFilter.UserName = new Database.Filter.StringSearch.SearchFilter();
            //        UserInfoFilter.UserName.SearchString = UserName;
            //        UserInfo = UserInfo.GetUserInfo(UserInfoFilter);
            //        if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID) && UserInfo.UserInfoID != UserInfoID)
            //        {
            //            _ret = true;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            //return _ret;
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
                //if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (string.IsNullOrEmpty(EmailAddress) && !IsGuest) throw new Exception("EmailAddress is required");
                if (string.IsNullOrEmpty(Password)) throw new Exception("Password is required");

                if (!string.IsNullOrEmpty(EmailAddress) && ExistsEmailAddress()) throw new Exception("Email already exists");
                if (!string.IsNullOrEmpty(UserName) && ExistsUserName()) throw new Exception("Username already exists");

                if (!IsNew) throw new Exception("Create cannot be performed, UserInfoID already exists");

                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["DisplayName"] = DisplayName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["UserName"] = UserName;
                dicParam["EmailWhiteListed"] = EmailWhiteListed;
                dicParam["Password"] = Password;
                dicParam["PasswordResetGUID"] = PasswordResetGUID;
                dicParam["PasswordResetRequestedOn"] = PasswordResetRequestedOn;
                dicParam["Passcode"] = Passcode;
                dicParam["PasscodeRequestedOn"] = PasscodeRequestedOn;
                dicParam["IsAdmin"] = IsAdmin;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["ImagePath"] = ImagePath;
                dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBookID;
                dicParam["DefaultBillingAddressBookID"] = DefaultBillingAddressBookID;
                dicParam["LastVisitedUserWebsiteID"] = LastVisitedUserWebsiteID;
                dicParam["LastVisitedUserAccountID"] = LastVisitedUserAccountID;
                dicParam["RequirePasswordReset"] = RequirePasswordReset;
                dicParam["ExternalID"] = ExternalID;
                dicParam["IsGuest"] = IsGuest;
                dicParam["AllowOnlySSO"] = AllowOnlySSO;
                dicParam["InActive"] = InActive;

                UserInfoID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "UserInfo"), objConn, objTran).ToString();

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
                if (string.IsNullOrEmpty(UserInfoID)) throw new Exception("UserInfoID is required");
                if (string.IsNullOrEmpty(EmailAddress) && !IsGuest) throw new Exception("EmailAddress is required");
                if (string.IsNullOrEmpty(Password)) throw new Exception("Password is required");
                if (!string.IsNullOrEmpty(EmailAddress) && ExistsEmailAddress()) throw new Exception("Email already exists");
                if (!string.IsNullOrEmpty(UserName) && ExistsUserName()) throw new Exception("Username already exists");
                if (IsNew) throw new Exception("Update cannot be performed, UserInfoID is missing");

                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["DisplayName"] = DisplayName;
                dicParam["EmailAddress"] = EmailAddress;
                dicParam["UserName"] = UserName;
                dicParam["EmailWhiteListed"] = EmailWhiteListed;
                dicParam["Password"] = Password;
                dicParam["PasswordResetGUID"] = PasswordResetGUID;
                dicParam["PasswordResetRequestedOn"] = PasswordResetRequestedOn;
                dicParam["Passcode"] = Passcode;
                dicParam["PasscodeRequestedOn"] = PasscodeRequestedOn;
                dicParam["IsAdmin"] = IsAdmin;
                dicParam["CustomerInternalID"] = CustomerInternalID;
                dicParam["ImagePath"] = ImagePath;
                dicParam["DefaultShippingAddressBookID"] = DefaultShippingAddressBookID;
                dicParam["DefaultBillingAddressBookID"] = DefaultBillingAddressBookID;
                dicParam["LastVisitedUserWebsiteID"] = LastVisitedUserWebsiteID;
                dicParam["LastVisitedUserAccountID"] = LastVisitedUserAccountID;
                dicParam["RequirePasswordReset"] = RequirePasswordReset;
                dicParam["ExternalID"] = ExternalID;
                dicParam["IsGuest"] = IsGuest;
                dicParam["AllowOnlySSO"] = AllowOnlySSO;
                dicParam["InActive"] = InActive;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["UserInfoID"] = UserInfoID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "UserInfo"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, UserInfoID is missing");

                dicDParam["UserInfoID"] = UserInfoID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "UserInfo"), objConn, objTran);
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

        public static UserInfo GetUserInfo(UserInfoFilter Filter)
        {
            List<UserInfo> objUserInfos = null;
            UserInfo objReturn = null;

            try
            {
                objUserInfos = GetUserInfos(Filter);
                if (objUserInfos != null && objUserInfos.Count >= 1) objReturn = objUserInfos[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objUserInfos = null;
            }
            return objReturn;
        }

        public static List<UserInfo> GetUserInfos()
        {
            int intTotalCount = 0;
            return GetUserInfos(null, null, null, out intTotalCount);
        }

        public static List<UserInfo> GetUserInfos(UserInfoFilter Filter)
        {
            int intTotalCount = 0;
            return GetUserInfos(Filter, null, null, out intTotalCount);
        }

        public static List<UserInfo> GetUserInfos(UserInfoFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetUserInfos(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<UserInfo> GetUserInfos(UserInfoFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<UserInfo> objReturn = null;
            UserInfo objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<UserInfo>();

                strSQL = "SELECT * " +
                         "FROM UserInfo (NOLOCK) " +
                         "WHERE 1=1  ";

                if (Filter != null)
                {
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.EmailAddress != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmailAddress, "EmailAddress");
                    if (Filter.UserName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserName, "UserName");
                    if (Filter.GUID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GUID, "GUID");
                    if (Filter.PasswordResetGUID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PasswordResetGUID, "PasswordResetGUID");
                    if (Filter.EmailWhiteListed != null) strSQL += "AND EmailWhiteListed=" + Database.HandleQuote(Convert.ToInt32(Filter.EmailWhiteListed.Value).ToString());
                    if (Filter.UserPoolUserName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserPoolUserName, "PasswordResetGUID");
                    if (Filter.CustomerInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerInternalID, "CustomerInternalID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.Password != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Password, "Password");
                    if (Filter.IsGuest != null) strSQL += "AND IsGuest=" + Database.HandleQuote(Convert.ToInt32(Filter.IsGuest.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "UserInfoID" : Utility.CustomSorting.GetSortExpression(typeof(UserInfo), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new UserInfo(objData.Tables[0].Rows[i]);
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
