using ImageSolutions.Enterprise;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScheduledTask.Task.Enterprise
{
    public class CreateNSCustomer
    {
        public bool Execute()
        {
            List<EnterpriseCustomer> Customers = null;
            EnterpriseCustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<EnterpriseCustomer>();
                CustomerFilter = new EnterpriseCustomerFilter();
                CustomerFilter.IsIndividual = true;

                //Test
                CustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                CustomerFilter.EmployeeID.SearchString = "E53B0B";

                Customers = EnterpriseCustomer.GetEnterpriseCustomers(CustomerFilter);

                foreach (EnterpriseCustomer _Customer in Customers)
                {
                    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                    UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                    UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = "53";

                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                    NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                    string CustomerInternalID = NSCustomer.CreateEnterpriseCustomerEBA(_Customer);
                    if (string.IsNullOrEmpty(CustomerInternalID))
                    {
                        throw new Exception("Unable to create customer in NS");
                    }
                    UserWebsite.CustomerInternalID = CustomerInternalID;
                    UserWebsite.Update();

                    _Customer.EBAInternalID = CustomerInternalID;
                    _Customer.Update();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
    }
}
