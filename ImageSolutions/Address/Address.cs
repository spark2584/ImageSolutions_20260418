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
    public class Address : ISBase.Address
    {
        public string AddressID { get; protected set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AddressID); } }
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostialCode { get; set; }
        public string CountryCode { get; set; }
        public bool? IsResidential{ get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public DateTime CreatedOn { get; private set; }

        public Address()
        {
        }

        public Address(string AddressID)
        {
            this.AddressID = AddressID;
            Load();
        }

        public Address(DataRow objRow)
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
                         "FROM Address ab (NOLOCK) " +
                         "WHERE ab.AddressID=" + Database.HandleQuote(AddressID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AddressID=" + AddressID + " is not found");
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

                if (objColumns.Contains("AddressID")) AddressID = Convert.ToString(objRow["AddressID"]);
                if (objColumns.Contains("FullName")) FullName = Convert.ToString(objRow["FullName"]);
                if (objColumns.Contains("Address1")) Address1 = Convert.ToString(objRow["Address1"]);
                if (objColumns.Contains("City")) City = Convert.ToString(objRow["City"]);
                if (objColumns.Contains("State")) State = Convert.ToString(objRow["State"]);
                if (objColumns.Contains("PostalCode")) PostalCode = Convert.ToString(objRow["PostalCode"]);
                if (objColumns.Contains("CountryCode")) CountryCode = Convert.ToString(objRow["CountryCode"]);
                if (objColumns.Contains("IsResidential") && objRow["IsResidential"] != DBNull.Value) IsResidential = Convert.ToBoolean(objRow["IsResidential"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AddressID)) throw new Exception("Missing AddressID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, AddressID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam.Clear();
                dicParam["FullName"] = FullName;
                dicParam["Address1"] = Address1;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryCode"] = CountryCode;
                dicParam["IsResidential"] = IsResidential;
                AddressID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Address"), objConn, objTran).ToString();
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
                if (IsNew) throw new Exception("Update cannot be performed, AddressID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["FullName"] = FullName;
                dicParam["Address1"] = Address1;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryCode"] = CountryCode;
                dicParam["IsResidential"] = IsResidential;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["AddressID"] = AddressID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Address"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, CustomerID is missing");

                dicDParam["AddressID"] = AddressID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Address"), objConn, objTran);
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

        private bool ObjectAlreadyExists()
        {
            return false;
        }
        public static Address GetAddress(AddressFilter Filter)
        {
            Address objReturn = null;
            Address objNew = null;
            DataSet objData = null;
            bool blnEntityFiltered = false;
            string strSQL = string.Empty;

            try
            {

                objReturn = new Address();

                strSQL = "SELECT ab.* " +
                            "FROM Address ab (NOLOCK) " +
                            "WHERE 1=1 "; 

                if (Filter != null)
                {

                }

                if (!blnEntityFiltered) strSQL += "AND ab.CustomerID IS NULL AND ab.VendorID IS NULL ";

                if (objData != null && objData.Tables[0].Rows.Count > 1)
                {
                    throw new Exception("more than one AddressID found");
                }
                else if (objData != null && objData.Tables[0].Rows.Count == 1)
                {
                    objReturn = new Address(objData.Tables[0].Rows[0]);
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

        public static List<Address> GetAddresss()
        {
            int intTotalCount = 0;
            return GetAddresss(null, null, null, out intTotalCount);
        }

        public static List<Address> GetAddresss(AddressFilter Filter)
        {
            int intTotalCount = 0;
            return GetAddresss(Filter, null, null, out intTotalCount);
        }

        public static List<Address> GetAddresss(AddressFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAddresss(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Address> GetAddresss(AddressFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Address> objReturn = null;
            Address objNew = null;
            DataSet objData = null;
            bool blnEntityFiltered = false;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Address>();

                strSQL = "SELECT ab.* " +
                            "FROM Address ab (NOLOCK) " +
                            "WHERE 1=1 "; 

                if (Filter != null)
                {

                }

                if (!blnEntityFiltered) strSQL += "AND ab.CustomerID IS NULL AND ab.VendorID IS NULL ";

                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreatedOn" : Utility.CustomSorting.GetSortExpression(typeof(Address), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else
                    strSQL += "ORDER BY CreatedOn ";
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    if (objData.Tables[0].Rows[0].Table.Columns.Contains("TotalRecord")) TotalRecord = Convert.ToInt32(objData.Tables[0].Rows[0]["TotalRecord"]);

                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Address(objData.Tables[0].Rows[i]);
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
