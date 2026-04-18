using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.DiscountTire
{
    public class SyncCustomer : NetSuiteBase
    {
        public bool Execute()
        {
            NetSuiteLibrary.Customer.Customer Customer = new NetSuiteLibrary.Customer.Customer();

            List<NetSuiteLibrary.Customer.Customer> NSCustomers = NetSuiteLibrary.Customer.Customer.GetCustomerSavedSearch(Service, "239790");

            return true;
        }

        //public List<NetSuiteLibrary.com.netsuite.webservices.Customer> GetCustomerSavedSearch(string savedsearchid)
        //{
        //    List<Customer> Customers = null;
        //    SearchResult SearchResult = null;
        //    Dictionary<string, string> dicInternalIds = new Dictionary<string, string>();

        //    try
        //    {
        //        Customers = new List<NetSuiteLibrary.com.netsuite.webservices.Customer>();
        //        SearchResult = GetSavedSearch(savedsearchid);

        //        int count = 0;

        //        if (SearchResult != null && SearchResult.totalRecords > 0)
        //        {
        //            do
        //            {
        //                if (SearchResult.searchRowList != null)
        //                {
        //                    foreach (CustomerSearchRow _CustomerSearchRow in SearchResult.searchRowList)
        //                    {
        //                        count++;
        //                        Console.WriteLine(string.Format("{0}/{1}", count, SearchResult.totalRecords));

        //                        if (_CustomerSearchRow is CustomerSearchRow && !dicInternalIds.ContainsKey(_CustomerSearchRow.basic.internalId[0].searchValue.internalId))
        //                        {                                   
        //                            string strInternalID = _CustomerSearchRow.basic.internalId[0].searchValue.internalId;
        //                            //Customers.Add(LoadNetSuiteCustomer(strInternalID));

        //                            dicInternalIds.Add(strInternalID, strInternalID);
        //                            Customer Customer = LoadNetSuiteCustomer(strInternalID);
        //                            if (_CustomerSearchRow.basic.terms != null)
        //                            {
        //                                Customer.terms = _CustomerSearchRow.basic.terms[0].searchValue;
        //                            }
        //                            Customers.Add(Customer);
        //                        }
        //                    }
        //                    Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
        //                    SearchResult = Service.searchMoreWithId(SearchResult.searchId, SearchResult.pageIndex + 1);
        //                }
        //            }
        //            while (SearchResult.searchRowList != null && SearchResult.pageSizeSpecified == true && SearchResult.totalPages >= SearchResult.pageIndex);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Customers;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.Customer LoadNetSuiteCustomer(string NetSuiteInternalID)
        //{
        //    RecordRef objCustomerRef = null;
        //    ReadResponse objReadResult = null;
        //    NetSuiteLibrary.com.netsuite.webservices.Customer objReturn = null;
        //    try
        //    {
        //        objCustomerRef = new RecordRef();
        //        objCustomerRef.internalId = NetSuiteInternalID;
        //        objCustomerRef.type = RecordType.customer;
        //        objCustomerRef.typeSpecified = true;

        //        objReadResult = Service.get(objCustomerRef);

        //        if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.Customer))
        //        {
        //            objReturn = (NetSuiteLibrary.com.netsuite.webservices.Customer)objReadResult.record;
        //        }
        //        else
        //        {
        //            throw new Exception("Can not find Customer with Internal ID : " + NetSuiteInternalID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objCustomerRef = null;
        //        objReadResult = null;
        //    }
        //    return objReturn;
        //}
    }
}
