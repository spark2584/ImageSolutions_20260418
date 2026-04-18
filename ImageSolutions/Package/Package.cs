using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Package
{
    public class Package : ISBase.BaseClass
    {
        public string PackageID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(PackageID); } }
        public string WebsiteID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string OptionalFieldText { get; set; }
        public string OptionalFieldMessage { get; set; }
        public bool InActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public string DescriptionHTML
        {
            get
            {
                string strDescription = string.Empty;

                strDescription = string.Format(@"<b>{0}</b><br><b>This package includes:</b>", string.IsNullOrEmpty(DisplayName) ? Name : DisplayName);

                foreach (PackageLine _PackageLine in PackageLines)
                {
                    if (_PackageLine.IsOptional)
                    {
                        strDescription = string.Format(@"{0}<br>- {1} : qty {2} (Optional)", strDescription, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                    else
                    {
                        strDescription = string.Format(@"{0}<br>- {1} : qty {2}", strDescription, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                }

                return strDescription;
            }
        }

        public string DescriptionHTMLSelected
        {
            get
            {
                string strDescription = string.Empty;

                strDescription = string.Format(@"<b>You have selected {0}.  This includes:</b><br>", string.IsNullOrEmpty(DisplayName) ? Name : DisplayName);

                foreach (PackageLine _PackageLine in PackageLines)
                {
                    if (_PackageLine.IsOptional)
                    {
                        strDescription = string.Format(@"{0}<br>- {1} : qty {2} (Optional)", strDescription, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                    else
                    {
                        strDescription = string.Format(@"{0}<br>- {1} : qty {2}", strDescription, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                }

                return strDescription;
            }
        }

        private List<PackageLine> mPackageLines = null;
        public List<PackageLine> PackageLines
        {
            get
            {
                if (mPackageLines == null && !string.IsNullOrEmpty(PackageID))
                {
                    PackageLineFilter objFilter = null;

                    try
                    {
                        objFilter = new PackageLineFilter();
                        objFilter.PackageID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PackageID.SearchString = PackageID;

                        mPackageLines = PackageLine.GetPackageLines(objFilter);
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
                return mPackageLines;
            }
            set
            {
                mPackageLines = value;
            }
        }

        public Package()
        {
        }
        public Package(string PackageID)
        {
            this.PackageID = PackageID;
            Load();
        }
        public Package(DataRow objRow)
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
                         "FROM Package (NOLOCK) " +
                         "WHERE PackageID=" + Database.HandleQuote(PackageID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("PackageID=" + PackageID + " is not found");
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

                if (objColumns.Contains("PackageID")) PackageID = Convert.ToString(objRow["PackageID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("DisplayName")) DisplayName = Convert.ToString(objRow["DisplayName"]);
                if (objColumns.Contains("OptionalFieldText")) OptionalFieldText = Convert.ToString(objRow["OptionalFieldText"]);
                if (objColumns.Contains("OptionalFieldMessage")) OptionalFieldMessage = Convert.ToString(objRow["OptionalFieldMessage"]);
                if (objColumns.Contains("InActive")) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(PackageID)) throw new Exception("Missing PackageID in the datarow");
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
                if (!IsNew) throw new Exception("Create cannot be performed, PackageID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Name"] = Name;
                dicParam["DisplayName"] = DisplayName;
                dicParam["OptionalFieldText"] = OptionalFieldText;
                dicParam["OptionalFieldMessage"] = OptionalFieldMessage;
                dicParam["InActive"] = InActive;
 
                PackageID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Package"), objConn, objTran).ToString();

                foreach (PackageLine objPackageLine in PackageLines)
                {
                    objPackageLine.PackageID = PackageID;
                    objPackageLine.Create(objConn, objTran);
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
                if (PackageID == null) throw new Exception("PackageID is required");
                if (IsNew) throw new Exception("Update cannot be performed, PackageID is missing");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["Name"] = Name;
                dicParam["DisplayName"] = DisplayName;
                dicParam["OptionalFieldText"] = OptionalFieldText;
                dicParam["OptionalFieldMessage"] = OptionalFieldMessage;
                dicParam["InActive"] = InActive;
                dicWParam["PackageID"] = PackageID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Package"), objConn, objTran);

                foreach (PackageLine objPackageLine in PackageLines)
                {
                    objPackageLine.Update(objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, PackageID is missing");

                //Delete Lines
                foreach (PackageLine _PackageLine in PackageLines)
                {
                    _PackageLine.Delete(objConn, objTran);
                }

                dicDParam["PackageID"] = PackageID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Package"), objConn, objTran);
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

        public static Package GetPackage(PackageFilter Filter)
        {
            List<Package> objPackages = null;
            Package objReturn = null;

            try
            {
                objPackages = GetPackages(Filter);
                if (objPackages != null && objPackages.Count >= 1) objReturn = objPackages[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPackages = null;
            }
            return objReturn;
        }

        public static List<Package> GetPackages()
        {
            int intTotalCount = 0;
            return GetPackages(null, null, null, out intTotalCount);
        }

        public static List<Package> GetPackages(PackageFilter Filter)
        {
            int intTotalCount = 0;
            return GetPackages(Filter, null, null, out intTotalCount);
        }

        public static List<Package> GetPackages(PackageFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetPackages(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Package> GetPackages(PackageFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Package> objReturn = null;
            Package objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Package>();

                strSQL = "SELECT * " +
                         "FROM Package (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.PackageID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.PackageID, "PackageID");
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "CategoryID");
                    if (Filter.InActive != null) strSQL += "AND InActive=" + Database.HandleQuote(Convert.ToInt32(Filter.InActive.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "PackageID" : Utility.CustomSorting.GetSortExpression(typeof(Package), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Package(objData.Tables[0].Rows[i]);
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
