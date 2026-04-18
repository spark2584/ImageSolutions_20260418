using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.MavisTire
{
    public class MavisTireCustomer : ISBase.BaseClass
    {
        public string MavisTireCustomerID { get; private set; }
        public string InternalID { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string StoreNumber { get; set; }
        public string MailingName { get; set; }
        public DateTime? HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? DaysEmployed { get; set; }
        public string Position { get; set; }
        public string PositionStatus { get; set; }
        public string PositionArea { get; set; }
        public string Email { get; set; }
        public string WorkEmail { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Territory { get; set; }
        public string Brand { get; set; }
        public string UniformBrand { get; set; }
        public string LocationAddress { get; set; }
        public string LocationCity { get; set; }
        public string LocationState { get; set; }
        public string LocationZip { get; set; }
        public string RVP { get; set; }
        public string RD { get; set; }
        public string RTM { get; set; }
        public bool ResetPackage { get; set; }
        public bool SendNewHireEmail { get; set; }
        public bool SendRegionChangeEmail { get; set; }
        public bool SendPositionChangeEmail { get; set; }
        public bool InActive { get; set; }
        public bool IsUpdated { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(MavisTireCustomerID); } }

        public MavisTireCustomer()
        {
        }
        public MavisTireCustomer(string MavisTireCustomerID)
        {
            this.MavisTireCustomerID = MavisTireCustomerID;
            Load();
        }
        public MavisTireCustomer(DataRow objRow)
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
                         "FROM MavisTireCustomer (NOLOCK) " +
                         "WHERE MavisTireCustomerID=" + Database.HandleQuote(MavisTireCustomerID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("MavisTireCustomerID=" + MavisTireCustomerID + " is not found");
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

                if (objColumns.Contains("MavisTireCustomerID")) MavisTireCustomerID = Convert.ToString(objRow["MavisTireCustomerID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("EmployeeNumber")) EmployeeNumber = Convert.ToString(objRow["EmployeeNumber"]);
                if (objColumns.Contains("EmployeeName")) EmployeeName = Convert.ToString(objRow["EmployeeName"]);
                if (objColumns.Contains("StoreNumber")) StoreNumber = Convert.ToString(objRow["StoreNumber"]);
                if (objColumns.Contains("MailingName")) MailingName = Convert.ToString(objRow["MailingName"]);
                if (objColumns.Contains("HireDate") && objRow["HireDate"] != DBNull.Value) HireDate = Convert.ToDateTime(objRow["HireDate"]);
                if (objColumns.Contains("TerminationDate") && objRow["TerminationDate"] != DBNull.Value) TerminationDate = Convert.ToDateTime(objRow["TerminationDate"]);
                if (objColumns.Contains("DaysEmployed") && objRow["DaysEmployed"] != DBNull.Value) DaysEmployed = Convert.ToInt32(objRow["DaysEmployed"]);
                if (objColumns.Contains("Position")) Position = Convert.ToString(objRow["Position"]);
                if (objColumns.Contains("PositionStatus")) PositionStatus = Convert.ToString(objRow["PositionStatus"]);
                if (objColumns.Contains("PositionArea")) PositionArea = Convert.ToString(objRow["PositionArea"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("WorkEmail")) WorkEmail = Convert.ToString(objRow["WorkEmail"]);
                if (objColumns.Contains("HomePhone")) HomePhone = Convert.ToString(objRow["HomePhone"]);
                if (objColumns.Contains("MobilePhone")) MobilePhone = Convert.ToString(objRow["MobilePhone"]);
                if (objColumns.Contains("Territory")) Territory = Convert.ToString(objRow["Territory"]);
                if (objColumns.Contains("Brand")) Brand = Convert.ToString(objRow["Brand"]);
                if (objColumns.Contains("UniformBrand")) UniformBrand = Convert.ToString(objRow["UniformBrand"]);
                if (objColumns.Contains("LocationAddress")) LocationAddress = Convert.ToString(objRow["LocationAddress"]);
                if (objColumns.Contains("LocationCity")) LocationCity = Convert.ToString(objRow["LocationCity"]);
                if (objColumns.Contains("LocationState")) LocationState = Convert.ToString(objRow["LocationState"]);
                if (objColumns.Contains("LocationZip")) LocationZip = Convert.ToString(objRow["LocationZip"]);
                if (objColumns.Contains("RVP")) RVP = Convert.ToString(objRow["RVP"]);
                if (objColumns.Contains("RD")) RD = Convert.ToString(objRow["RD"]);
                if (objColumns.Contains("RTM")) RTM = Convert.ToString(objRow["RTM"]);
                if (objColumns.Contains("ResetPackage") && objRow["ResetPackage"] != DBNull.Value) ResetPackage = Convert.ToBoolean(objRow["ResetPackage"]);
                if (objColumns.Contains("SendNewHireEmail") && objRow["SendNewHireEmail"] != DBNull.Value) SendNewHireEmail = Convert.ToBoolean(objRow["SendNewHireEmail"]);
                if (objColumns.Contains("SendRegionChangeEmail") && objRow["SendRegionChangeEmail"] != DBNull.Value) SendRegionChangeEmail = Convert.ToBoolean(objRow["SendRegionChangeEmail"]);
                if (objColumns.Contains("SendPositionChangeEmail") && objRow["SendPositionChangeEmail"] != DBNull.Value) SendPositionChangeEmail = Convert.ToBoolean(objRow["SendPositionChangeEmail"]);
                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("IsUpdated") && objRow["IsUpdated"] != DBNull.Value) IsUpdated = Convert.ToBoolean(objRow["IsUpdated"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(MavisTireCustomerID)) throw new Exception("Missing MavisTireCustomerID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, MavisTireCustomerID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["EmployeeNumber"] = EmployeeNumber;
                dicParam["EmployeeName"] = EmployeeName;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["MailingName"] = MailingName;
                dicParam["HireDate"] = HireDate;
                dicParam["TerminationDate"] = TerminationDate;
                dicParam["DaysEmployed"] = DaysEmployed;
                dicParam["Position"] = Position;
                dicParam["PositionStatus"] = PositionStatus;
                dicParam["PositionArea"] = PositionArea;
                dicParam["Email"] = Email;
                dicParam["WorkEmail"] = WorkEmail;
                dicParam["HomePhone"] = HomePhone;
                dicParam["MobilePhone"] = MobilePhone;
                dicParam["Territory"] = Territory;
                dicParam["Brand"] = Brand;
                dicParam["UniformBrand"] = UniformBrand;
                dicParam["LocationAddress"] = LocationAddress;
                dicParam["LocationCity"] = LocationCity;
                dicParam["LocationState"] = LocationState;
                dicParam["LocationZip"] = LocationZip;
                dicParam["RVP"] = RVP;
                dicParam["RD"] = RD;
                dicParam["RTM"] = RTM;
                dicParam["ResetPackage"] = ResetPackage;
                dicParam["SendNewHireEmail"] = SendNewHireEmail;
                dicParam["SendRegionChangeEmail"] = SendRegionChangeEmail;
                dicParam["SendPositionChangeEmail"] = SendPositionChangeEmail;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;

                MavisTireCustomerID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "MavisTireCustomer"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, MavisTireCustomerID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["EmployeeNumber"] = EmployeeNumber;
                dicParam["EmployeeName"] = EmployeeName;
                dicParam["StoreNumber"] = StoreNumber;
                dicParam["MailingName"] = MailingName;
                dicParam["HireDate"] = HireDate;
                dicParam["TerminationDate"] = TerminationDate;
                dicParam["DaysEmployed"] = DaysEmployed;
                dicParam["Position"] = Position;
                dicParam["PositionStatus"] = PositionStatus;
                dicParam["PositionArea"] = PositionArea;
                dicParam["Email"] = Email;
                dicParam["WorkEmail"] = WorkEmail;
                dicParam["HomePhone"] = HomePhone;
                dicParam["MobilePhone"] = MobilePhone;
                dicParam["Territory"] = Territory;
                dicParam["Brand"] = Brand;
                dicParam["UniformBrand"] = UniformBrand;
                dicParam["LocationAddress"] = LocationAddress;
                dicParam["LocationCity"] = LocationCity;
                dicParam["LocationState"] = LocationState;
                dicParam["LocationZip"] = LocationZip;
                dicParam["RVP"] = RVP;
                dicParam["RD"] = RD;
                dicParam["RTM"] = RTM;
                dicParam["ResetPackage"] = ResetPackage;
                dicParam["SendNewHireEmail"] = SendNewHireEmail;
                dicParam["SendRegionChangeEmail"] = SendRegionChangeEmail;
                dicParam["SendPositionChangeEmail"] = SendPositionChangeEmail;
                dicParam["InActive"] = InActive;
                dicParam["IsUpdated"] = IsUpdated;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                dicWParam["MavisTireCustomerID"] = MavisTireCustomerID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "MavisTireCustomer"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, MavisTireCustomerID is missing");

                dicDParam["MavisTireCustomerID"] = MavisTireCustomerID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "MavisTireCustomer"), objConn, objTran);
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

        public static MavisTireCustomer GetMavisTireCustomer(MavisTireCustomerFilter Filter)
        {
            List<MavisTireCustomer> objMavisTireCustomers = null;
            MavisTireCustomer objReturn = null;

            try
            {
                objMavisTireCustomers = GetMavisTireCustomers(Filter);
                if (objMavisTireCustomers != null && objMavisTireCustomers.Count >= 1) objReturn = objMavisTireCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objMavisTireCustomers = null;
            }
            return objReturn;
        }

        public static List<MavisTireCustomer> GetMavisTireCustomers()
        {
            int intTotalCount = 0;
            return GetMavisTireCustomers(null, null, null, out intTotalCount);
        }

        public static List<MavisTireCustomer> GetMavisTireCustomers(MavisTireCustomerFilter Filter)
        {
            int intTotalCount = 0;
            return GetMavisTireCustomers(Filter, null, null, out intTotalCount);
        }

        public static List<MavisTireCustomer> GetMavisTireCustomers(MavisTireCustomerFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetMavisTireCustomers(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<MavisTireCustomer> GetMavisTireCustomers(MavisTireCustomerFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<MavisTireCustomer> objReturn = null;
            MavisTireCustomer objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<MavisTireCustomer>();

                strSQL = "SELECT * " +
                         "FROM MavisTireCustomer (NOLOCK) " +
                         "where 1=1 ";

                if (Filter != null)
                {
                    if (Filter.EmployeeNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.EmployeeNumber, "EmployeeNumber");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.Email != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Email, "Email");
                    if (Filter.IsUpdated != null) strSQL += "AND IsUpdated=" + Database.HandleQuote(Convert.ToInt32(Filter.IsUpdated.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "MavisTireCustomerID" : Utility.CustomSorting.GetSortExpression(typeof(MavisTireCustomer), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                {
                    strSQL += " ORDER BY MavisTireCustomerID DESC";
                }

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new MavisTireCustomer(objData.Tables[0].Rows[i]);
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
