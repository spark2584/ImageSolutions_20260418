using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Vendor
{
    public class Vendor : ISBase.BaseClass
    {
        public string VendorID { get; private set; }
        public string ExternalID { get; set; }
        public string InternalID { get; set; }
        public string NetSuiteEntityID { get; set; }
        public string NetSuiteLeadInternalID { get; set; }
        public string NetSuiteLeadEntityID { get; set; }
        public string MerchantID { get; set; }
        public string CompanyName { get; set; }
        public string DefaultBillingAddressID { get; set; }
        public string DefaultShippingAddressID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal? CommissionPercentage { get; set; }
        public int? FreeStorageMonths { get; set; }
        public DateTime? ServiceStartDate { get; set; }
        public string ShopTitle { get; set; }

        public string MemberID { get; set; }

        public string AdjustmentInternalID { get; set; }
        public string AdjustmentNetSuiteDocumentNumber { get; set; }

        public string IsB2B { get; set; }
        public string Area { get; set; }
        public bool BrandRegistry { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsNew { get { return string.IsNullOrEmpty(VendorID); } }

        public Vendor()
        {
        }

        public Vendor(string VendorID)
        {
            this.VendorID = VendorID;
            Load();
        }

        public Vendor(DataRow objRow)
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
                         "FROM Vendor (NOLOCK) " +
                         "WHERE VendorID=" + Database.HandleQuote(VendorID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("VendorID=" + VendorID + " is not found");
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

                if (objColumns.Contains("VendorID")) VendorID = Convert.ToString(objRow["VendorID"]);
                if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("NetSuiteEntityID")) NetSuiteEntityID = Convert.ToString(objRow["NetSuiteEntityID"]);
                if (objColumns.Contains("NetSuiteLeadInternalID")) NetSuiteLeadInternalID = Convert.ToString(objRow["NetSuiteLeadInternalID"]);
                if (objColumns.Contains("NetSuiteLeadEntityID")) NetSuiteLeadEntityID = Convert.ToString(objRow["NetSuiteLeadEntityID"]);
                if (objColumns.Contains("MerchantID")) MerchantID = Convert.ToString(objRow["MerchantID"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("DefaultBillingAddressID")) DefaultBillingAddressID = Convert.ToString(objRow["DefaultBillingAddressID"]);
                if (objColumns.Contains("DefaultShippingAddressID")) DefaultShippingAddressID = Convert.ToString(objRow["DefaultShippingAddressID"]);
                if (objColumns.Contains("Phone")) Phone = Convert.ToString(objRow["Phone"]);
                if (objColumns.Contains("Email")) Email = Convert.ToString(objRow["Email"]);
                if (objColumns.Contains("CommissionPercentage") && objRow["CommissionPercentage"] != DBNull.Value) CommissionPercentage = Convert.ToDecimal(objRow["CommissionPercentage"]);
                if (objColumns.Contains("FreeStorageMonths") && objRow["FreeStorageMonths"] != DBNull.Value) FreeStorageMonths = Convert.ToInt32(objRow["FreeStorageMonths"]);
                if (objColumns.Contains("ServiceStartDate") && objRow["ServiceStartDate"] != DBNull.Value) ServiceStartDate = Convert.ToDateTime(objRow["ServiceStartDate"]);
                if (objColumns.Contains("ShopTitle")) ShopTitle = Convert.ToString(objRow["ShopTitle"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("BrandRegistry")) BrandRegistry = Convert.ToBoolean(objRow["BrandRegistry"]);
                if (objColumns.Contains("MemberID")) MemberID = Convert.ToString(objRow["MemberID"]);
                if (objColumns.Contains("Area")) Area = Convert.ToString(objRow["Area"]);
                if (objColumns.Contains("AdjustmentInternalID")) AdjustmentInternalID = Convert.ToString(objRow["AdjustmentInternalID"]);
                if (objColumns.Contains("AdjustmentNetSuiteDocumentNumber")) AdjustmentNetSuiteDocumentNumber = Convert.ToString(objRow["AdjustmentNetSuiteDocumentNumber"]);

                if (objColumns.Contains("IsB2B")) IsB2B = Convert.ToString(objRow["IsB2B"]);

                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);

                if (string.IsNullOrEmpty(VendorID)) throw new Exception("Missing VendorID in the datarow");
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
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(MerchantID)) throw new Exception("MerchantID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, VendorID already exists");

                dicParam["ExternalID"] = ExternalID;
                dicParam["InternalID"] = InternalID;
                dicParam["NetSuiteEntityID"] = NetSuiteEntityID;
                dicParam["NetSuiteLeadInternalID"] = NetSuiteLeadInternalID;
                dicParam["NetSuiteLeadEntityID"] = NetSuiteLeadEntityID;
                dicParam["MerchantID"] = MerchantID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["DefaultBillingAddressID"] = DefaultBillingAddressID;
                dicParam["DefaultShippingAddressID"] = DefaultShippingAddressID;
                dicParam["Phone"] = Phone;
                dicParam["Email"] = Email;
                dicParam["CommissionPercentage"] = CommissionPercentage;
                dicParam["FreeStorageMonths"] = FreeStorageMonths;
                dicParam["ServiceStartDate"] = ServiceStartDate;
                dicParam["ShopTitle"] = ShopTitle;

                dicParam["MemberID"] = MemberID;
                dicParam["AdjustmentInternalID"] = AdjustmentInternalID;
                dicParam["AdjustmentNetSuiteDocumentNumber"] = AdjustmentNetSuiteDocumentNumber;


                dicParam["IsB2B"] = IsB2B;
                dicParam["Area"] = Area;

                dicParam["ErrorMessage"] = ErrorMessage;

                VendorID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Vendor"), objConn, objTran).ToString();
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
                if (string.IsNullOrEmpty(CompanyName)) throw new Exception("CompanyName is required");
                if (string.IsNullOrEmpty(MerchantID)) throw new Exception("MerchantID is required");
                if (IsNew) throw new Exception("Update cannot be performed, VendorID is missing");

                //dicParam["ExternalID"] = ExternalID;
                //dicParam["InternalID"] = InternalID;
                //dicParam["NetSuiteEntityID"] = NetSuiteEntityID;
                //dicParam["NetSuiteLeadInternalID"] = NetSuiteLeadInternalID;
                //dicParam["NetSuiteLeadEntityID"] = NetSuiteLeadEntityID;
                //dicParam["MerchantID"] = MerchantID;
                dicParam["CompanyName"] = CompanyName;
                dicParam["DefaultBillingAddressID"] = DefaultBillingAddressID;
                dicParam["DefaultShippingAddressID"] = DefaultShippingAddressID;
                dicParam["Phone"] = Phone;
                dicParam["Email"] = Email;
                dicParam["CommissionPercentage"] = CommissionPercentage;
                dicParam["FreeStorageMonths"] = FreeStorageMonths;
                dicParam["ServiceStartDate"] = ServiceStartDate;
                dicParam["ShopTitle"] = ShopTitle;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicParam["BrandRegistry"] = BrandRegistry;
                dicParam["MemberID"] = MemberID;
                dicParam["AdjustmentInternalID"] = AdjustmentInternalID;
                dicParam["AdjustmentNetSuiteDocumentNumber"] = AdjustmentNetSuiteDocumentNumber;


                dicParam["IsB2B"] = IsB2B;
                dicParam["Area"] = Area;

                dicWParam["VendorID"] = VendorID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Vendor"), objConn, objTran);

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
                if (IsNew) throw new Exception("Delete cannot be performed, VendorID is missing");

                dicDParam["VendorID"] = VendorID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Vendor"), objConn, objTran);
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

        public static Vendor GetVendor(VendorFilter Filter)
        {
            List<Vendor> objSalesOrders = null;
            Vendor objReturn = null;

            try
            {
                objSalesOrders = GetVendors(Filter);
                if (objSalesOrders != null && objSalesOrders.Count >= 1) objReturn = objSalesOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
            }
            return objReturn;
        }

        public static bool Create(List<Vendor> Vendors)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                foreach (Vendor objVendor in Vendors)
                {
                    objVendor.Create(objConn, objTran);
                }

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

        public static List<Vendor> GetVendors()
        {
            int intTotalCount = 0;
            return GetVendors(null, null, null, out intTotalCount);
        }

        public static List<Vendor> GetVendors(VendorFilter Filter)
        {
            int intTotalCount = 0;
            return GetVendors(Filter, null, null, out intTotalCount);
        }

        public static List<Vendor> GetVendors(VendorFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetVendors(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Vendor> GetVendors(VendorFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Vendor> objReturn = null;
            Vendor objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Vendor>();

                strSQL = "SELECT * " +
                         "FROM Vendor (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.VendorID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.VendorID, "VendorID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.NetSuiteEntityID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteEntityID, "NetSuiteEntityID");
                    if (Filter.NetSuiteLeadInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteLeadInternalID, "NetSuiteLeadInternalID");
                    if (Filter.NetSuiteLeadEntityID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteLeadEntityID, "NetSuiteLeadEntityID");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.MerchantID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.MerchantID, "MerchantID");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                    if (Filter.MemberID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.MemberID, "MemberID");

                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "VendorID" : Utility.CustomSorting.GetSortExpression(typeof(Vendor), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Vendor(objData.Tables[0].Rows[i]);
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

        //public static Vendor IfExternalIDExists(string ExternalID)
        //{
        //    VendorFilter objFilter = null;
        //    Vendor objVendor = null;
        //    try
        //    {
        //        objFilter = new Toolots.Vendor.VendorFilter();
        //        objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
        //        objFilter.ExternalID.SearchString = ExternalID;
        //        objVendor = Toolots.Vendor.Vendor.GetVendor(objFilter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objFilter = null;
        //    }
        //    return objVendor;
        //}
    }
}
