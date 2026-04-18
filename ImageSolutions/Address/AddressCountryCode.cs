using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Address
{
    public class AddressCountryCode : ISBase.Address
    {
        public string AddressCountryCodeID { get; protected set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AddressCountryCodeID); } }
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        public string Alpha3Code { get; set; }
        public string Numeric { get; set; }

        public AddressCountryCode()
        {
        }

        public AddressCountryCode(string AddressCountryCodeID)
        {
            this.AddressCountryCodeID = AddressCountryCodeID;
            Load();
        }

        public AddressCountryCode(DataRow objRow)
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
                         "FROM AddressCountryCode ab (NOLOCK) " +
                         "WHERE ab.AddressCountryCodeID=" + Database.HandleQuote(AddressCountryCodeID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AddressCountryCodeID=" + AddressCountryCodeID + " is not found");
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

                if (objColumns.Contains("AddressCountryCodeID")) AddressCountryCodeID = Convert.ToString(objRow["AddressCountryCodeID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Alpha2Code")) Alpha2Code = Convert.ToString(objRow["Alpha2Code"]);
                if (objColumns.Contains("Alpha3Code")) Alpha3Code = Convert.ToString(objRow["Alpha3Code"]);
                if (objColumns.Contains("Numeric")) Numeric = Convert.ToString(objRow["Numeric"]);

                if (string.IsNullOrEmpty(AddressCountryCodeID)) throw new Exception("Missing AddressCountryCodeID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, AddressCountryCodeID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam.Clear();
                dicParam["Name"] = Name;
                dicParam["Alpha2Code"] = Alpha2Code;
                dicParam["Alpha3Code"] = Alpha3Code;
                dicParam["Numeric"] = Numeric;
                AddressCountryCodeID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AddressCountryCode"), objConn, objTran).ToString();
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
                if (IsNew) throw new Exception("Update cannot be performed, AddressCountryCodeID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;
                dicParam["Alpha2Code"] = Alpha2Code;
                dicParam["Alpha3Code"] = Alpha3Code;
                dicWParam["AddressCountryCodeID"] = AddressCountryCodeID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AddressCountryCode"), objConn, objTran);
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

                dicDParam["AddressCountryCodeID"] = AddressCountryCodeID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AddressCountryCode"), objConn, objTran);
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
        public static AddressCountryCode GetAddressCountryCode(AddressCountryCodeFilter Filter)
        {
            List<AddressCountryCode> objAddressCountryCodes = null;
            AddressCountryCode objReturn = null;

            try
            {
                objAddressCountryCodes = GetAddressCountryCodes(Filter);
                if (objAddressCountryCodes != null && objAddressCountryCodes.Count >= 1) objReturn = objAddressCountryCodes[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objAddressCountryCodes = null;
            }
            return objReturn;
        }

        public static List<AddressCountryCode> GetAddressCountryCodes()
        {
            int intTotalCount = 0;
            return GetAddressCountryCodes(null, null, null, out intTotalCount);
        }

        public static List<AddressCountryCode> GetAddressCountryCodes(AddressCountryCodeFilter Filter)
        {
            int intTotalCount = 0;
            return GetAddressCountryCodes(Filter, null, null, out intTotalCount);
        }

        public static List<AddressCountryCode> GetAddressCountryCodes(AddressCountryCodeFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetAddressCountryCodes(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<AddressCountryCode> GetAddressCountryCodes(AddressCountryCodeFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<AddressCountryCode> objReturn = null;
            AddressCountryCode objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<AddressCountryCode>();

                strSQL = "SELECT ab.* " +
                            "FROM AddressCountryCode ab (NOLOCK) " +
                            "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Alpha2Code != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Alpha2Code, "Alpha2Code");
                    if (Filter.Alpha3Code != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Alpha3Code, "Alpha3Code");
                    if (Filter.Numeric != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Numeric, "Numeric");
                }


                if (PageSize != null && PageNumber != null)
                    strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "CreatedOn" : Utility.CustomSorting.GetSortExpression(typeof(AddressCountryCode), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);


                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "AddressCountryCodeID" : Utility.CustomSorting.GetSortExpression(typeof(AddressCountryCode), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AddressCountryCode(objData.Tables[0].Rows[i]);
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
                objNew = null;
            }
            return objReturn;
        }
    }
}
