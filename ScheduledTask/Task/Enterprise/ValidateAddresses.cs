using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class ValidateAddresses
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;
            try
            {
                int counter = 0;

                List<ImageSolutions.Account.Account> Accounts = new List<ImageSolutions.Account.Account>();
                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                Accounts = ImageSolutions.Account.Account.GetAccounts(AccountFilter);

                foreach(ImageSolutions.Account.Account _Account in Accounts)
                {
                    counter++;
                    Console.WriteLine(string.Format("{0}. {1}", counter, _Account.AccountName));
                    if (!string.IsNullOrEmpty(_Account.DefaultShippingAddressBookID))
                    {
                        Console.WriteLine(string.Format("{0}", _Account.DefaultShippingAddressBook.CountryCode));
                        if (_Account.DefaultShippingAddressBook.CountryCode == "US" 
                            && _Account.DefaultShippingAddressBook.State.Length > 2
                        )
                        {
                            ValidateAddress(_Account);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }
        }

        protected bool ValidateAddress(ImageSolutions.Account.Account account)
        {
            bool _ret = false;

            ImageSolutions.Address.AddressTrans AddressTrans = new ImageSolutions.Address.AddressTrans();
            AddressTrans.AddressLine1 = account.DefaultShippingAddressBook.AddressLine1;
            AddressTrans.AddressLine2 = account.DefaultShippingAddressBook.AddressLine2;
            AddressTrans.City = account.DefaultShippingAddressBook.City;
            AddressTrans.State = account.DefaultShippingAddressBook.State;
            AddressTrans.PostalCode = account.DefaultShippingAddressBook.PostalCode;
            AddressTrans.CountryCode = string.IsNullOrEmpty(account.DefaultShippingAddressBook.CountryCode) ? "US" : account.DefaultShippingAddressBook.CountryCode;

            Shippo.Address ShippoAddress = ImageSolutions.Shipping.ShippoShipping.GetAddress(AddressTrans);

            if (Convert.ToBoolean(ShippoAddress.ValidationResults.IsValid))
            {

                // Update to suggested?
                if (Convert.ToString(AddressTrans.AddressLine1) != Convert.ToString(ShippoAddress.Street1)
                    || Convert.ToString(AddressTrans.AddressLine2) != Convert.ToString(ShippoAddress.Street2)
                    || Convert.ToString(AddressTrans.City) != Convert.ToString(ShippoAddress.City)
                    || Convert.ToString(AddressTrans.State) != Convert.ToString(ShippoAddress.State)
                    || Convert.ToString(AddressTrans.PostalCode) != Convert.ToString(ShippoAddress.Zip)
                )
                {

                    if (Convert.ToString(AddressTrans.AddressLine1) != Convert.ToString(ShippoAddress.Street1)
                        || Convert.ToString(AddressTrans.AddressLine2) != Convert.ToString(ShippoAddress.Street2)
                        )
                    {
                        string strTest = "test";

                        if (!string.IsNullOrEmpty(AddressTrans.AddressLine2))
                        {
                            strTest = "update";
                        }
                    }

                    account.DefaultShippingAddressBook.AddressLine1 = Convert.ToString(ShippoAddress.Street1);
                    account.DefaultShippingAddressBook.AddressLine2 = Convert.ToString(ShippoAddress.Street2);
                    account.DefaultShippingAddressBook.City = Convert.ToString(ShippoAddress.City);
                    account.DefaultShippingAddressBook.State = Convert.ToString(ShippoAddress.State);
                    account.DefaultShippingAddressBook.PostalCode = Convert.ToString(ShippoAddress.Zip);

                    account.DefaultShippingAddressBook.IsUpdated = true;
                    account.DefaultShippingAddressBook.Update();

                    _ret = false;
                }
            }
            else
            {
                //Invalid Address
                
                account.DefaultShippingAddressBook.IsInvalid = true;
                account.DefaultShippingAddressBook.Update();
            }

            return _ret;
        }
    }
}
