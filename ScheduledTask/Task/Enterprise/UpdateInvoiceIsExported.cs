using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScheduledTask.Task.Enterprise
{
    public class UpdateInvoiceIsExported
    {
        public bool Execute()
        {

            List<ImageSolutions.Enterprise.EnterpriseInvoice> EnterpriseInvoices = new List<ImageSolutions.Enterprise.EnterpriseInvoice>();
            ImageSolutions.Enterprise.EnterpriseInvoiceFilter EnterpriseInvoiceFilter = new ImageSolutions.Enterprise.EnterpriseInvoiceFilter();
            //EnterpriseInvoiceFilter.IsExported = true;
            EnterpriseInvoiceFilter.IsNSUpdated = false;
            EnterpriseInvoices = ImageSolutions.Enterprise.EnterpriseInvoice.GetEnterpriseInvoices(EnterpriseInvoiceFilter);

            foreach (ImageSolutions.Enterprise.EnterpriseInvoice _EnterpriseInvoice in EnterpriseInvoices)
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    NetSuiteLibrary.Invoice.Invoice Invoice = new NetSuiteLibrary.Invoice.Invoice();
                    Invoice.UpdateInvoiceIsExported(_EnterpriseInvoice.InvoiceInternalID);
                    //Invoice.UpdateInvoiceIsExported("50245961");

                    _EnterpriseInvoice.IsNSUpdated = true;
                    _EnterpriseInvoice.Update(objConn, objTran);

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

            return true;
        }
    }
}
