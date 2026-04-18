using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ImageSolutions.Shipping
{
    public class ShippingService : ISBase.BaseClass
    {
        public string ShippingServiceID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ShippingServiceID); } }
        public string Carrier { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public ShippingService()
        {
        }
        public ShippingService(string ShippingServiceID)
        {
            this.ShippingServiceID = ShippingServiceID;
            Load();
        }
        public ShippingService(DataRow objRow)
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
                         "FROM ShippingService (NOLOCK) " +
                         "WHERE ShippingServiceID=" + Database.HandleQuote(ShippingServiceID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ShippingServiceID=" + ShippingServiceID + " is not found");
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

                if (objColumns.Contains("ShippingServiceID")) ShippingServiceID = Convert.ToString(objRow["ShippingServiceID"]);
                if (objColumns.Contains("Carrier")) Carrier = Convert.ToString(objRow["Carrier"]);
                if (objColumns.Contains("ServiceCode")) ServiceCode = Convert.ToString(objRow["ServiceCode"]);
                if (objColumns.Contains("ServiceName")) ServiceName = Convert.ToString(objRow["ServiceName"]);
                if (objColumns.Contains("Description")) Description = Convert.ToString(objRow["Description"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ShippingServiceID)) throw new Exception("Missing ShippingServiceID in the datarow");
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

        public static ShippingService GetShippingService(ShippingServiceFilter Filter)
        {
            List<ShippingService> objShippingServices = null;
            ShippingService objReturn = null;

            try
            {
                objShippingServices = GetShippingServices(Filter);
                if (objShippingServices != null && objShippingServices.Count >= 1) objReturn = objShippingServices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShippingServices = null;
            }
            return objReturn;
        }

        public static List<ShippingService> GetShippingServices()
        {
            int intTotalCount = 0;
            return GetShippingServices(null, null, null, out intTotalCount);
        }

        public static List<ShippingService> GetShippingServices(ShippingServiceFilter Filter)
        {
            int intTotalCount = 0;
            return GetShippingServices(Filter, null, null, out intTotalCount);
        }

        public static List<ShippingService> GetShippingServices(ShippingServiceFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShippingServices(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShippingService> GetShippingServices(ShippingServiceFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShippingService> objReturn = null;
            ShippingService objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShippingService>();

                strSQL = "SELECT * " +
                         "FROM ShippingService (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.Carrier != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Carrier, "Carrier");
                    if (Filter.ServiceCode != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ServiceCode, "ServiceCode");
                    if (Filter.ServiceName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ServiceName, "ServiceName");
                    if (Filter.Description != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Description, "Description");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShippingServiceID" : Utility.CustomSorting.GetSortExpression(typeof(ShippingService), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShippingService(objData.Tables[0].Rows[i]);
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
