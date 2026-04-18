using ImageSolutions.User;
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

namespace ImageSolutions.RMA
{
    public class RMAReason : ISBase.BaseClass
    {
        public string RMAReasonID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(RMAReasonID); } }
        public string Reason { get; set; }
        public bool IncludeShipping { get; set; }

        public RMAReason()
        {
        }

        public RMAReason(string RMAReasonID)
        {
            this.RMAReasonID = RMAReasonID;
            Load();
        }

        public RMAReason(DataRow objRow)
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
                         "FROM RMAReason (NOLOCK) " +
                         "WHERE RMAReasonID=" + Database.HandleQuote(RMAReasonID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("RMAReasonID=" + RMAReasonID + " is not found");
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

                if (objColumns.Contains("RMAReasonID")) RMAReasonID = Convert.ToString(objRow["RMAReasonID"]);
                if (objColumns.Contains("Reason")) Reason = Convert.ToString(objRow["Reason"]);
                if (objColumns.Contains("IncludeShipping")) IncludeShipping = Convert.ToBoolean(objRow["IncludeShipping"]);

                if (string.IsNullOrEmpty(RMAReasonID)) throw new Exception("Missing RMAReasonID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, RMAReasonID already exists");

                dicParam["Reason"] = Reason;
                dicParam["IncludeShipping"] = IncludeShipping;

                RMAReasonID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "RMAReason"), objConn, objTran).ToString();

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
                if (IsNew) throw new Exception("Update cannot be performed, RMAReasonID is missing");


                dicParam["Reason"] = Reason;
                dicParam["IncludeShipping"] = IncludeShipping;

                dicWParam["RMAReasonID"] = RMAReasonID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "RMAReason"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, RMAReasonID is missing");

                dicDParam["RMAReasonID"] = RMAReasonID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "RMAReason"), objConn, objTran);
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

        public static RMAReason GetRMAReason(RMAReasonFilter Filter)
        {
            List<RMAReason> objRMAReasons = null;
            RMAReason objReturn = null;

            try
            {
                objRMAReasons = GetRMAReasons(Filter);
                if (objRMAReasons != null && objRMAReasons.Count >= 1) objReturn = objRMAReasons[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRMAReasons = null;
            }
            return objReturn;
        }

        public static List<RMAReason> GetRMAReasons()
        {
            int intTotalCount = 0;
            return GetRMAReasons(null, null, null, out intTotalCount);
        }

        public static List<RMAReason> GetRMAReasons(RMAReasonFilter Filter)
        {
            int intTotalCount = 0;
            return GetRMAReasons(Filter, null, null, out intTotalCount);
        }

        public static List<RMAReason> GetRMAReasons(RMAReasonFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetRMAReasons(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<RMAReason> GetRMAReasons(RMAReasonFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<RMAReason> objReturn = null;
            RMAReason objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<RMAReason>();

                strSQL = "SELECT * " +
                         "FROM RMAReason (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "RMAReasonID" : Utility.CustomSorting.GetSortExpression(typeof(RMAReason), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new RMAReason(objData.Tables[0].Rows[i]);
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
