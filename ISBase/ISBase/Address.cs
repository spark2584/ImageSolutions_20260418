using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ISBase
{
    [Serializable]
    public class Address : BaseClass
    {
        public string AddressLabel { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set;  }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneExtension { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebsiteURL { get; set; }

        public string GetHTML
        {
            get
            {
                string strReturn = (string.IsNullOrEmpty(CompanyName) ? "" : (CompanyName + (true ? "<br />" : Environment.NewLine))) +
                               (string.IsNullOrEmpty(FirstName) ? "" : FirstName + " ") +
                               (string.IsNullOrEmpty(LastName) ? "" : LastName + " ") +
                               (!string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? (true ? "<br />" : Environment.NewLine) : "") +
                               (string.IsNullOrEmpty(AddressLine1) ? "" : AddressLine1 + (true ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(AddressLine2) ? "" : AddressLine2 + (true ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(AddressLine3) ? "" : AddressLine3 + (true ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(City) ? "" : City + ", ") +
                               (string.IsNullOrEmpty(State) ? "" : State + " ") +
                               (string.IsNullOrEmpty(PostalCode) ? "" : PostalCode + (true ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(CountryName) ? "" : CountryName + (true ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(CountryCode) ? "" : CountryCode + (true ? "<br />" : Environment.NewLine)) +
                               GetPhoneNumberFormat(PhoneNumber, PhoneExtension);


                return strReturn;
            }
        }

        public Address()
        {
        }

        public string GetDisplayFormat(bool HTMLFormat)
        {
            string strReturn = (string.IsNullOrEmpty(AddressLabel) || Convert.ToString(AddressLabel) == String.Format("{0} {1}", FirstName, LastName) ? "" : (AddressLabel + (HTMLFormat ? "<br />" : Environment.NewLine))) + 
                               (string.IsNullOrEmpty(CompanyName) ? "" : (CompanyName + (HTMLFormat ? "<br />" : Environment.NewLine))) +
                               (string.IsNullOrEmpty(FirstName) ? "" : FirstName + " ") +
                               (string.IsNullOrEmpty(LastName) ? "" : LastName + " ") +
                               (!string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName) ? (HTMLFormat ? "<br />" : Environment.NewLine) : "") +
                               (string.IsNullOrEmpty(AddressLine1) ? "" : AddressLine1 + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(AddressLine2) ? "" : AddressLine2 + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(AddressLine3) ? "" : AddressLine3 + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(City) ? "" : City + ", ") +
                               (string.IsNullOrEmpty(State) ? "" : State + " ") +
                               (string.IsNullOrEmpty(PostalCode) ? "" : PostalCode + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(CountryName) ? "" : CountryName + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               (string.IsNullOrEmpty(CountryCode) ? "" : CountryCode + (HTMLFormat ? "<br />" : Environment.NewLine)) +
                               GetPhoneNumberFormat(PhoneNumber, PhoneExtension);
                               

            return strReturn;
        }

        public string GetPhoneNumberFormat(string PhoneNumber, string Extension)
        {
            if (string.IsNullOrEmpty(PhoneNumber)) return string.Empty;
            PhoneNumber = Regex.Replace(PhoneNumber, "[^a-zA-Z0-9]", "");
            switch (PhoneNumber.Length)
            {
                case 7: PhoneNumber = Regex.Replace(PhoneNumber, "([a-zA-Z0-9]{3})([a-zA-Z0-9]{4})", "$1-$2"); break;
                case 10: PhoneNumber = Regex.Replace(PhoneNumber, "([a-zA-Z0-9]{3})([a-zA-Z0-9]{3})([a-zA-Z0-9]{4})", "($1)$2-$3"); break;
                case 11: PhoneNumber = Regex.Replace(PhoneNumber, "([a-zA-Z0-9]{1})([a-zA-Z0-9]{3})([a-zA-Z0-9]{3})([a-zA-Z0-9]{4})", "$1($2)$3-$4"); break;
            }
            return string.IsNullOrEmpty(Extension) ? PhoneNumber : PhoneNumber + " EXT " + Extension;
        }

        public virtual bool IsValid()
        {
            return true;
        }
    }
}
