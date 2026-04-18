using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Package
{
    public class PackageLine : ISBase.BaseClass
    {
        public string PackageLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PackageLineID); } }
        public string PackageID { get; set; }
        public string PackageGroupID { get; set; }
        public int Quantity { get; set; }
        public bool IsOptional { get; set; }
        public bool ValidateSingleSize { get; set; }
        public int Sort { get; set; }
        public DateTime CreatedOn { get; private set; }

        private Package mPackage = null;
        public Package Package
        {
            get
            {
                if (mPackage == null && !string.IsNullOrEmpty(PackageID))
                {
                    mPackage = new Package(PackageID);
                }
                return mPackage;
            }
        }
        private PackageGroup mPackageGroup = null;
        public PackageGroup PackageGroup
        {
            get
            {
                if (mPackageGroup == null && !string.IsNullOrEmpty(PackageGroupID))
                {
                    mPackageGroup = new PackageGroup(PackageGroupID);
                }
                return mPackageGroup;
            }
        }
        public PackageLine()
        {
        }

        public PackageLine(string PackageLineID)
        {
            this.PackageLineID = PackageLineID;
            Load();
        }

        public PackageLine(DataRow objRow)
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
                         "FROM PackageLine (NOLOCK) " +
                         "WHERE PackageLineID=" + Database.HandleQuote(PackageLineID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PackageLineID=" + PackageLineID + " is not found");
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

                if (objColumns.Contains("PackageLineID")) PackageLineID = Convert.ToString(objRow["PackageLineID"]);
                if (objColumns.Contains("PackageID")) PackageID = Convert.ToString(objRow["PackageID"]);
                if (objColumns.Contains("PackageGroupID")) PackageGroupID = Convert.ToString(objRow["PackageGroupID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("IsOptional")) IsOptional = Convert.ToBoolean(objRow["IsOptional"]);
                if (objColumns.Contains("ValidateSingleSize")) ValidateSingleSize = Convert.ToBoolean(objRow["ValidateSingleSize"]);
                if (objColumns.Contains("Sort")) Sort = Convert.ToInt32(objRow["Sort"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PackageLineID)) throw new Exception("Missing PackageLineID in the datarow");
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
                if (string.IsNullOrEmpty(PackageID)) throw new Exception("PackageID is required");
                if (string.IsNullOrEmpty(PackageGroupID)) throw new Exception("PackageGroupID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, PackageLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["PackageID"] = PackageID;
                dicParam["PackageGroupID"] = PackageGroupID;
                dicParam["Quantity"] = Quantity;
                dicParam["IsOptional"] = IsOptional;
                dicParam["ValidateSingleSize"] = ValidateSingleSize;
                dicParam["Sort"] = Sort;
                PackageLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "PackageLine"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(PackageID)) throw new Exception("PackageID is required");
                if (string.IsNullOrEmpty(PackageGroupID)) throw new Exception("PackageGroupID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PackageLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");


                dicParam["PackageID"] = PackageID;
                dicParam["PackageGroupID"] = PackageGroupID;
                dicParam["Quantity"] = Quantity;
                dicParam["IsOptional"] = IsOptional;
                dicParam["ValidateSingleSize"] = ValidateSingleSize;
                dicWParam["PackageLineID"] = PackageLineID;
                dicParam["Sort"] = Sort;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "PackageLine"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, PackageLinesID is missing");

                dicDParam["PackageLineID"] = PackageLineID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "PackageLine"), objConn, objTran);
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

        public static PackageLine GetPackageLine(PackageLineFilter Filter)
        {
            List<PackageLine> objPackageLines = null;
            PackageLine objReturn = null;

            try
            {
                objPackageLines = GetPackageLines(Filter);
                if (objPackageLines != null && objPackageLines.Count >= 1) objReturn = objPackageLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPackageLines = null;
            }
            return objReturn;

        }
        public static List<PackageLine> GetPackageLines()
        {
            int intTotalCount = 0;
            return GetPackageLines(null, null, null, out intTotalCount);
        }

        public static List<PackageLine> GetPackageLines(PackageLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetPackageLines(Filter, null, null, out intTotalCount);
        }

        public static List<PackageLine> GetPackageLines(PackageLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPackageLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<PackageLine> GetPackageLines(PackageLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<PackageLine> objReturn = null;
            PackageLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<PackageLine>();

                strSQL = "SELECT * " +
                         "FROM PackageLine (NOLOCK) pl " +
                         "WHERE 1=1 ";


                if (Filter != null)
                {
                    if (Filter.PackageLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageLineID, "pl.PackageLineID");
                    if (Filter.PackageID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageID, "pl.PackageID");
                    if (Filter.PackageGroupID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageGroupID, "pl.PackageGroupID");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PackageLineID" : Utility.CustomSorting.GetSortExpression(typeof(PackageLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                else strSQL += " ORDER BY ISNULL(Sort,9999) ASC";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new PackageLine(objData.Tables[0].Rows[i]);
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
