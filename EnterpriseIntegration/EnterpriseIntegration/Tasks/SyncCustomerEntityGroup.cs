using ImageSolutions.Customer;
using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseIntegration.Tasks
{
    public class SyncCustomerEntityGroup
    {
        public bool Execute()
        {
            List<CustomerEntityGroup> CustomerEntityGroups = null;
            CustomerEntityGroupFilter CustomerEntityGroupFilter = null;

            try
            {
                CustomerEntityGroups = new List<CustomerEntityGroup>();
                CustomerEntityGroupFilter = new CustomerEntityGroupFilter();
                CustomerEntityGroupFilter.IsUpdated = true;

                //CustomerEntityGroupFilter.CustomerID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerEntityGroupFilter.CustomerID.SearchString = "45921";

                CustomerEntityGroups = CustomerEntityGroup.GetCustomerEntityGroups(CustomerEntityGroupFilter);

                if (CustomerEntityGroups != null && CustomerEntityGroups.Count > 0)
                {
                    Parallel.ForEach(CustomerEntityGroups, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    _CustomerEntityGroup =>
                    {
                        if (!string.IsNullOrEmpty(_CustomerEntityGroup.Customer.InternalID))
                        {
                            try
                            {
                                //Sync to NS
                                Console.WriteLine(String.Format("Syncing CustomerEntityGroups: {0} {1}", _CustomerEntityGroup.CustomerID, _CustomerEntityGroup.EntityGroupID));
                                if (_CustomerEntityGroup.Inactive)
                                {
                                    //Remove from list
                                    NetSuiteLibrary.Entity.EntityGroup NSEntityGroup = new NetSuiteLibrary.Entity.EntityGroup(_CustomerEntityGroup.EntityGroup);
                                    NSEntityGroup.Detach(_CustomerEntityGroup.Customer.InternalID);
                                }
                                else
                                {
                                    //Add to list
                                    NetSuiteLibrary.Entity.EntityGroup NSEntityGroup = new NetSuiteLibrary.Entity.EntityGroup(_CustomerEntityGroup.EntityGroup);
                                    NSEntityGroup.Attach(_CustomerEntityGroup.Customer.InternalID);
                                }
                                _CustomerEntityGroup.ErrorMessage = String.Empty;
                                _CustomerEntityGroup.IsUpdated = false;
                                _CustomerEntityGroup.Update();
                            }
                            catch (Exception ex)
                            {
                                _CustomerEntityGroup.ErrorMessage = ex.Message;
                                _CustomerEntityGroup.Update();
                            }
                        }
                    });
                }

                //foreach (CustomerEntityGroup _CustomerEntityGroup in CustomerEntityGroups)
                //{
                //    if(!string.IsNullOrEmpty(_CustomerEntityGroup.Customer.InternalID))
                //    {
                //        try
                //        {
                //            //Sync to NS
                //            Console.WriteLine(String.Format("Syncing CustomerEntityGroups: {0} {1}" ,_CustomerEntityGroup.CustomerID, _CustomerEntityGroup.EntityGroupID));

                //            if (_CustomerEntityGroup.Inactive)
                //            {
                //                //Remove from list
                //                NetSuiteLibrary.Entity.EntityGroup NSEntityGroup = new NetSuiteLibrary.Entity.EntityGroup(_CustomerEntityGroup.EntityGroup);
                //                NSEntityGroup.Detach(_CustomerEntityGroup.Customer.InternalID);
                //            }
                //            else
                //            {
                //                //Add to list
                //                NetSuiteLibrary.Entity.EntityGroup NSEntityGroup = new NetSuiteLibrary.Entity.EntityGroup(_CustomerEntityGroup.EntityGroup);
                //                NSEntityGroup.Attach(_CustomerEntityGroup.Customer.InternalID);
                //            }
                //            _CustomerEntityGroup.IsUpdated = false;
                //            _CustomerEntityGroup.Update();
                //        }
                //        catch (Exception ex)
                //        {
                //            _CustomerEntityGroup.ErrorMessage = ex.Message;
                //            _CustomerEntityGroup.Update();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CustomerEntityGroups = null;
                CustomerEntityGroupFilter = null;
            }
            return true;
        }
    }
}
