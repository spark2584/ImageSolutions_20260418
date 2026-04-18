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
    public class AddressTrans : ISBase.Address
    {
        public string AddressTransID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(AddressTransID); } }
        public string InternalID { get; set; }
        public string SalesOrderID { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; private set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; private set; }

        public AddressTrans()
        {
            IsActive = true;
        }

        public AddressTrans(string AddressTransID)
        {
            this.AddressTransID = AddressTransID;
            Load();
        }

        public AddressTrans(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Load(objConn, objTran);
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
        }

        protected override void Load(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT at.* " +
                         "FROM AddressTrans at (NOLOCK) " +
                         "WHERE at.AddressTransID=" + Database.HandleQuote(AddressTransID);
                objData = Database.GetDataSet(strSQL, objConn, objTran);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("AddressTransID=" + AddressTransID + " is not found");
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

                if (objColumns.Contains("AddressTransID")) AddressTransID = Convert.ToString(objRow["AddressTransID"]);
                if (objColumns.Contains("InternalID")) InternalID = Convert.ToString(objRow["InternalID"]);
                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("AddressLabel")) AddressLabel = Convert.ToString(objRow["AddressLabel"]);
                if (objColumns.Contains("CompanyName")) CompanyName = Convert.ToString(objRow["CompanyName"]);
                if (objColumns.Contains("FirstName")) FirstName = Convert.ToString(objRow["FirstName"]);
                if (objColumns.Contains("LastName")) LastName = Convert.ToString(objRow["LastName"]);
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
                if (objColumns.Contains("WebsiteURL")) WebsiteURL = Convert.ToString(objRow["WebsiteURL"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(AddressTransID)) throw new Exception("Missing AddressTransID in the datarow");
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
            Hashtable dicWParam = new Hashtable();

            try
            {
                //if (string.IsNullOrEmpty(BusinessID)) throw new Exception("BusinessID is required");
                //if (string.IsNullOrEmpty(SalesOrderID) && string.IsNullOrEmpty(PurchaseOrderID) && string.IsNullOrEmpty(CustInvoiceID) && string.IsNullOrEmpty(VendBillID) && string.IsNullOrEmpty(FulfillmentID)) throw new Exception("One of SalesOrderID, PurchaseOrderID, CustInvoiceID, VendBillID, FulfillmentID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, AddressTransID already exists");

                dicParam["InternalID"] = InternalID;
                dicParam["SalesOrderID"] = SalesOrderID;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["CompanyName"] = CompanyName;
                dicParam["AddressLabel"] = AddressLabel;
                dicParam["AddressLine1"] = AddressLine1;
                dicParam["AddressLine2"] = AddressLine2;
                dicParam["AddressLine3"] = AddressLine3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneExtension"] = PhoneExtension;
                dicParam["Fax"] = Fax;
                dicParam["Email"] = Email;
                dicParam["WebsiteURL"] = WebsiteURL;
                dicParam["CreatedBy"] = CreatedBy;
                AddressTransID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "AddressTrans"), objConn, objTran).ToString();
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
                //if (string.IsNullOrEmpty(BusinessID)) throw new Exception("BusinessID is required");
                //if (string.IsNullOrEmpty(SalesOrderID) && string.IsNullOrEmpty(PurchaseOrderID) && string.IsNullOrEmpty(CustInvoiceID) && string.IsNullOrEmpty(VendBillID) && string.IsNullOrEmpty(FulfillmentID)) throw new Exception("One of SalesOrderID, PurchaseOrderID, CustInvoiceID, VendBillID, FulfillmentID is required");
                if (IsNew) throw new Exception("Update cannot be performed, AddressTransID is missing");

                dicParam["InternalID"] = InternalID;
                dicParam["FirstName"] = FirstName;
                dicParam["LastName"] = LastName;
                dicParam["CompanyName"] = CompanyName;
                dicParam["AddressLabel"] = AddressLabel;
                dicParam["AddressLine1"] = AddressLine1;
                dicParam["AddressLine2"] = AddressLine2;
                dicParam["AddressLine3"] = AddressLine3;
                dicParam["City"] = City;
                dicParam["State"] = State;
                dicParam["PostalCode"] = PostalCode;
                dicParam["CountryID"] = CountryID;
                dicParam["CountryCode"] = CountryCode;
                dicParam["PhoneNumber"] = PhoneNumber;
                dicParam["PhoneExtension"] = PhoneExtension;
                dicParam["Fax"] = Fax;
                dicParam["Email"] = Email;
                dicParam["WebsiteURL"] = WebsiteURL;
                dicParam["UpdatedBy"] = UpdatedBy;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["AddressTransID"] = AddressTransID;
                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "AddressTrans"), objConn, objTran);
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
                if (IsNew) throw new Exception("Delete cannot be performed, AddressTransID is missing");

                dicDParam["AddressTransID"] = AddressTransID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "AddressTrans"), objConn, objTran);
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
    }
}
