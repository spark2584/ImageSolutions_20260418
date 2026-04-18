using ImageSolutions.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseIntegration.Tasks
{
    public class SyncCorporateCustomer
    {
        public bool Execute()
        {
            List<Customer> Customers = null;
            CustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<Customer>();
                CustomerFilter = new CustomerFilter();
                CustomerFilter.IsUpdated = true;
                CustomerFilter.IsIndividual = true;

                //CustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.ExternalID.SearchString = "E18BQW";

                //CustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.StoreNumber.SearchString = "1018963";

                Customers = Customer.GetCustomers(CustomerFilter);

                if (Customers != null && Customers.Count > 0)
                {
                    Parallel.ForEach(Customers, new ParallelOptions { MaxDegreeOfParallelism = 21 },
                    _Customer =>
                    {
                        try
                        {
                            //Sync to NS
                            Console.WriteLine("Syncing Customer: " + _Customer.CustomerID);
                            NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer(_Customer);
                            NSCustomer.Create();

                            if (string.IsNullOrEmpty(_Customer.ErrorMessage))
                            {
                                _Customer.IsUpdated = false;
                                _Customer.Update();
                            }
                        }
                        catch (Exception ex)
                        {
                            _Customer.ErrorMessage = ex.Message;
                            _Customer.Update();
                        }
                    });
                }

                //foreach (Customer _Customer in Customers)
                //{
                //    try
                //    {
                //        //Sync to NS
                //        Console.WriteLine("Syncing Customer: " + _Customer.CustomerID);
                //        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer(_Customer);
                //        NSCustomer.Create();

                //        if (string.IsNullOrEmpty(_Customer.ErrorMessage))
                //        {
                //            _Customer.IsUpdated = false;
                //            _Customer.Update();
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        _Customer.ErrorMessage = ex.Message;
                //        _Customer.Update();
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Customers = null;
                CustomerFilter = null;
            }
            return true;
        }
    }
}
