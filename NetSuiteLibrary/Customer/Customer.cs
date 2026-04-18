using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;

namespace NetSuiteLibrary.Customer
{
    public class Customer : NetSuiteBase
    {
        private ImageSolutions.Customer.Customer mImageSolutionsCustomer = null;
        public ImageSolutions.Customer.Customer ImageSolutionsCustomer
        {
            get
            {
                if (mImageSolutionsCustomer == null && mNetSuiteCustomer != null && !string.IsNullOrEmpty(mNetSuiteCustomer.internalId))
                {
                    ImageSolutions.Customer.CustomerFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Customer.CustomerFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteCustomer.internalId;
                        mImageSolutionsCustomer = ImageSolutions.Customer.Customer.GetCustomer(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mImageSolutionsCustomer;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.Customer mNetSuiteCustomer = null;
        public NetSuiteLibrary.com.netsuite.webservices.Customer NetSuiteCustomer
        {
            get
            {
                if (mNetSuiteCustomer == null && mImageSolutionsCustomer != null && !string.IsNullOrEmpty(mImageSolutionsCustomer.InternalID))
                {
                    mNetSuiteCustomer = LoadNetSuiteCustomer(Service, mImageSolutionsCustomer.InternalID);
                }
                return mNetSuiteCustomer;
            }
            set
            {
                mNetSuiteCustomer = value;
            }
        }
        public Customer()
        {

        }
        public Customer(ImageSolutions.Customer.Customer ImageSolutionsCustomer)
        {
            mImageSolutionsCustomer = ImageSolutionsCustomer;
        }

        public Customer (NetSuiteLibrary.com.netsuite.webservices.Customer NetSuiteCustomer)
        {
            mNetSuiteCustomer = NetSuiteCustomer;
        }

        private static NetSuiteLibrary.com.netsuite.webservices.Customer LoadNetSuiteCustomer(NetSuiteService service, string netsuiteinternalid)
        {
            RecordRef objCustomerRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Customer objReturn = null;
            try
            {
                objCustomerRef = new RecordRef();
                objCustomerRef.internalId = netsuiteinternalid;
                objCustomerRef.type = RecordType.customer;
                objCustomerRef.typeSpecified = true;

                service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                objReadResult = service.get(objCustomerRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.Customer))
                {
                    objReturn = (com.netsuite.webservices.Customer)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Customer with Internal ID : " + netsuiteinternalid);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public bool Create()//string Currency)
        {
            WriteResponse objWriteResponse = null;
            Customer objCustomer = null;
            bool blnCurrencyFound = false;

            try
            {
                if (ImageSolutionsCustomer == null) throw new Exception("ImageSolutionsCustomer cannot be null");

                if (string.IsNullOrEmpty(ImageSolutionsCustomer.InternalID))
                {
                    objCustomer = ObjectAlreadyExists();

                    if (objCustomer != null)
                    {
                        ImageSolutionsCustomer.InternalID = objCustomer.NetSuiteCustomer.internalId;
                        ImageSolutionsCustomer.Update();
                    }
                }

                if (!string.IsNullOrEmpty(ImageSolutionsCustomer.InternalID))//objCustomer != null)
                {
                    //Console.WriteLine("Customer already exists");
                    //foreach (CustomerCurrency objCurrency in objCustomer.NetSuiteCustomer.currencyList.currency)
                    //{
                    //    if (objCurrency.currency.name == Currency)
                    //    {
                    //        blnCurrencyFound = true;
                    //        break;
                    //    }
                    //}
                    //if (!blnCurrencyFound) objWriteResponse = Service.update(UpdateNetSuiteCustomer(Currency));

                    if(ImageSolutionsCustomer.IsIndividual)
                    {
                        objWriteResponse = Service.update(UpdateNetSuiteCustomerIndividual());
                    }
                    else
                    {
                        objWriteResponse = Service.update(UpdateNetSuiteCustomerStore());// Currency));
                    }

                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Customer: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsCustomer.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        //ImageSolutionsCustomer.NetSuiteEntityID = NetSuiteCustomer.entityId;
                    }

                    //NetSuite InternalID did not get updated, auto fix
                    //ImageSolutionsCustomer.InternalID = objCustomer.NetSuiteCustomer.internalId;
                    //ImageSolutionsCustomer.NetSuiteEntityID = NetSuiteCustomer.entityId;
                }
                else
                {
                    if (ImageSolutionsCustomer.IsIndividual)
                    {
                        objWriteResponse = Service.add(CreateNetsuiteCustomerIndividual());
                    }
                    else
                    {
                        objWriteResponse = Service.add(CreateNetsuiteCustomerStore()); // Currency));
                    }

                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Customer: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsCustomer.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        //ImageSolutionsCustomer.NetSuiteEntityID = NetSuiteCustomer.entityId;
                    }
                }

                ImageSolutionsCustomer.ErrorMessage = string.Empty;
                ImageSolutionsCustomer.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsCustomer.ErrorMessage = "Customer.cs - Create() - " + ex.Message;
                ImageSolutionsCustomer.Update();
            }
            finally
            {
                objWriteResponse = null;

            }
            return true;
        }

        public string CreateEnterpriseCustomer(ImageSolutions.Enterprise.EnterpriseCustomer enterprisecustomer)
        {
            WriteResponse objWriteResponse = null;

            try
            {                
                if (enterprisecustomer.IsIndividual)
                {
                    com.netsuite.webservices.Customer Customer = CreateEnterpriseNetsuiteCustomerIndividual(enterprisecustomer);
                    objWriteResponse = Service.add(Customer);
                }
                else
                {
                    objWriteResponse = Service.add(CreateEnterpriseNetsuiteCustomerStore(enterprisecustomer));
                }

                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to create Customer: " + objWriteResponse.status.statusDetail[0].message);
                }
                else
                {
                    enterprisecustomer.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                }

                enterprisecustomer.ErrorMessage = string.Empty;
                enterprisecustomer.Update();
            }
            catch (Exception ex)
            {
                enterprisecustomer.ErrorMessage = "Customer.cs - Create() - " + ex.Message;
                enterprisecustomer.Update();
            }
            finally
            {
                objWriteResponse = null;

            }
            return enterprisecustomer.InternalID;
        }

        public string CreateEnterpriseCustomerEBA(ImageSolutions.Enterprise.EnterpriseCustomer enterprisecustomer)
        {
            WriteResponse objWriteResponse = null;

            try
            {
                com.netsuite.webservices.Customer Customer = CreateEnterpriseNetsuiteCustomerEBA(enterprisecustomer);
                objWriteResponse = Service.add(Customer);

                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to create Customer: " + objWriteResponse.status.statusDetail[0].message);
                }
                else
                {
                    enterprisecustomer.EBAInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                }

                enterprisecustomer.ErrorMessage = string.Empty;
                enterprisecustomer.Update();
            }
            catch (Exception ex)
            {
                enterprisecustomer.ErrorMessage = "Customer.cs - Create() - " + ex.Message;
                enterprisecustomer.Update();
            }
            finally
            {
                objWriteResponse = null;

            }
            return enterprisecustomer.EBAInternalID;
        }

        public bool Delete()
        {
            RecordRef objRecordRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ImageSolutionsCustomer == null) throw new Exception("ImageSolutionsCustomer cannot be null");

                if (NetSuiteCustomer != null)
                {
                    objRecordRef = new RecordRef();
                    objRecordRef.internalId = NetSuiteCustomer.internalId;
                    objRecordRef.type = RecordType.customer;
                    objRecordRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objRecordRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete customer: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteCustomer = null;
                    }
                }

                ImageSolutionsCustomer.ErrorMessage = string.Empty;
                ImageSolutionsCustomer.InternalID = string.Empty;
                //ImageSolutionsCustomer.NetSuiteEntityID = string.Empty;
                ImageSolutionsCustomer.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find Customer with Internal ID"))
                {
                    ImageSolutionsCustomer.ErrorMessage = string.Empty;
                    ImageSolutionsCustomer.InternalID = string.Empty;
                    //ImageSolutionsCustomer.NetSuiteEntityID = string.Empty;
                    ImageSolutionsCustomer.Update();
                }
                else
                {
                    ImageSolutionsCustomer.ErrorMessage = "Customer.cs - Delete() - " + ex.Message;
                    ImageSolutionsCustomer.Update();
                }
            }
            finally
            {
                objRecordRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        private com.netsuite.webservices.Customer CreateNetsuiteCustomer() //string Currency)
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;
            int intNullFieldIndex = 0;
            //ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;
            //ImageSolutions.SalesOrder.SalesOrderFilter objSalesOrderFilter = null;

            try
            {
                ImageSolutionsCustomer.Email = String.Format(@"{0}@ehi.com", Regex.Replace(ImageSolutionsCustomer.StoreNumber, @"[^0-9a-zA-Z\._]", ""));

                objReturn = new com.netsuite.webservices.Customer();
                //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerFormID, RecordType.customer);
                //objReturn.internalId = ImageSolutionsCustomer.InternalID;
                objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                    objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                    objReturn.companyName = ImageSolutionsCustomer.FirstName;
                else
                    objReturn.companyName = ImageSolutionsCustomer.Email;

                objReturn.email = ImageSolutionsCustomer.Email;
                if(ImageSolutionsCustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                }

                objReturn.isPerson = false;
                objReturn.isPersonSpecified = true;
               
                //objReturn.parent = new RecordRef();
                //objReturn.parent.internalId = ImageSolutionsCustomer.Parent.InternalID; //NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);                
                objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);                      

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.BrandName, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_emp_id");

                ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                AddressCountryCodeFilter.Alpha3Code.SearchString = ImageSolutionsCustomer.CountryCode;
                AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                if(AddressCountryCode != null && !string.IsNullOrEmpty(AddressCountryCode.Alpha2Code))
                {
                    objReturn.addressbookList = new CustomerAddressbookList();
                    objReturn.addressbookList.addressbook = new CustomerAddressbook[1];
                    objReturn.addressbookList.addressbook[0] = new CustomerAddressbook();
                    objReturn.addressbookList.addressbook[0].addressbookAddress = new Address();
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addressee = ImageSolutionsCustomer.ShipToAddress.FullName;
                    if (!string.IsNullOrEmpty(ImageSolutionsCustomer.ShipToAddress.CompanyName))
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.attention = ImageSolutionsCustomer.CompanyName;
                    }
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addr1 = ImageSolutionsCustomer.ShipToAddress.Address1;
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.addr2 = ImageSolutionsCustomer.ShipToAddress.Address2;
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.addr3 = ImageSolutionsCustomer.ShipToAddress.Address3;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.city = ImageSolutionsCustomer.ShipToAddress.City;
                    if (ImageSolutionsCustomer.ShipToAddress.State.Length > 30)
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State.Substring(0, 30);
                    else
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State;
                    //if (!string.IsNullOrEmpty(ImageSolutionsCustomer.PhoneNumber) && ImageSolutionsCustomer.PhoneNumber.Length > 5) 
                    //    objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = ImageSolutionsCustomer.PhoneNumber;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.zip = ImageSolutionsCustomer.ShipToAddress.PostalCode;

                    objReturn.addressbookList.addressbook[0].addressbookAddress.country = base.GetCountry(AddressCountryCode.Alpha2Code);
                    objReturn.addressbookList.addressbook[0].addressbookAddress.countrySpecified = true;

                    if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = ImageSolutionsCustomer.PhoneNumber;
                    }
                }
                
                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;

                objReturn.password = ImageSolutionsCustomer.Password;
                objReturn.password2 = ImageSolutionsCustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private com.netsuite.webservices.Customer CreateNetsuiteCustomerStore()
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                ImageSolutionsCustomer.Email = String.Format(@"{0}@ehi.com", Regex.Replace(ImageSolutionsCustomer.StoreNumber, @"[^0-9a-zA-Z\._]", ""));

                objReturn = new com.netsuite.webservices.Customer();
                objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                    objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                    objReturn.companyName = ImageSolutionsCustomer.FirstName;
                else
                    objReturn.companyName = ImageSolutionsCustomer.Email;

                objReturn.email = ImageSolutionsCustomer.Email;
                if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                }

                objReturn.isPerson = false;
                objReturn.isPersonSpecified = true;

                objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.BrandName, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_emp_id");

                ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                AddressCountryCodeFilter.Alpha3Code.SearchString = ImageSolutionsCustomer.CountryCode;
                AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                if (AddressCountryCode != null && !string.IsNullOrEmpty(AddressCountryCode.Alpha2Code))
                {
                    objReturn.addressbookList = new CustomerAddressbookList();
                    objReturn.addressbookList.addressbook = new CustomerAddressbook[1];
                    objReturn.addressbookList.addressbook[0] = new CustomerAddressbook();
                    objReturn.addressbookList.addressbook[0].addressbookAddress = new Address();
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addressee = ImageSolutionsCustomer.ShipToAddress.FullName;
                    if (!string.IsNullOrEmpty(ImageSolutionsCustomer.ShipToAddress.CompanyName))
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.attention = ImageSolutionsCustomer.CompanyName;
                    }
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addr1 = ImageSolutionsCustomer.ShipToAddress.Address1;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.city = ImageSolutionsCustomer.ShipToAddress.City;
                    if (ImageSolutionsCustomer.ShipToAddress.State.Length > 30)
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State.Substring(0, 30);
                    else
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.zip = ImageSolutionsCustomer.ShipToAddress.PostalCode;

                    objReturn.addressbookList.addressbook[0].addressbookAddress.country = base.GetCountry(AddressCountryCode.Alpha2Code);
                    objReturn.addressbookList.addressbook[0].addressbookAddress.countrySpecified = true;

                    if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = ImageSolutionsCustomer.PhoneNumber;
                    }
                }
                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;
                objReturn.password = ImageSolutionsCustomer.Password;
                objReturn.password2 = ImageSolutionsCustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }
        private com.netsuite.webservices.Customer CreateEnterpriseNetsuiteCustomerStore(ImageSolutions.Enterprise.EnterpriseCustomer enterprisecustomer)
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                enterprisecustomer.Email = String.Format(@"{0}@ehi.com", Regex.Replace(enterprisecustomer.StoreNumber, @"[^0-9a-zA-Z\._]", ""));

                objReturn = new com.netsuite.webservices.Customer();
                objReturn.firstName = enterprisecustomer.FirstName.Length > 32 ? enterprisecustomer.FirstName.Substring(0, 32) : enterprisecustomer.FirstName;
                objReturn.lastName = enterprisecustomer.LastName.Length > 32 ? enterprisecustomer.LastName.Substring(0, 32) : enterprisecustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(enterprisecustomer.CompanyName) ? enterprisecustomer.CompanyName : enterprisecustomer.FirstName + " " + enterprisecustomer.LastName;
                if (!string.IsNullOrEmpty(enterprisecustomer.CompanyName))
                    objReturn.companyName = enterprisecustomer.CompanyName;
                else if (!string.IsNullOrEmpty(enterprisecustomer.FirstName))
                    objReturn.companyName = enterprisecustomer.FirstName;
                else
                    objReturn.companyName = enterprisecustomer.Email;

                objReturn.email = enterprisecustomer.Email;
                if (enterprisecustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = enterprisecustomer.PhoneNumber;
                }

                objReturn.isPerson = false;
                objReturn.isPersonSpecified = true;

                objReturn.parent = NetSuiteHelper.GetRecordRef(enterprisecustomer.Parent.InternalID, RecordType.customer);
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.BrandName, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.StoreNumber, "custentity_store_emp_id");

                ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                AddressCountryCodeFilter.Alpha3Code.SearchString = enterprisecustomer.CountryCode;
                AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                if (AddressCountryCode != null && !string.IsNullOrEmpty(AddressCountryCode.Alpha2Code))
                {
                    objReturn.addressbookList = new CustomerAddressbookList();
                    objReturn.addressbookList.addressbook = new CustomerAddressbook[1];
                    objReturn.addressbookList.addressbook[0] = new CustomerAddressbook();
                    objReturn.addressbookList.addressbook[0].addressbookAddress = new Address();
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addressee = enterprisecustomer.ShipToAddress.FullName;
                    if (!string.IsNullOrEmpty(enterprisecustomer.ShipToAddress.CompanyName))
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.attention = enterprisecustomer.CompanyName;
                    }
                    objReturn.addressbookList.addressbook[0].addressbookAddress.addr1 = enterprisecustomer.ShipToAddress.Address1;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.city = enterprisecustomer.ShipToAddress.City;
                    if (enterprisecustomer.ShipToAddress.State.Length > 30)
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = enterprisecustomer.ShipToAddress.State.Substring(0, 30);
                    else
                        objReturn.addressbookList.addressbook[0].addressbookAddress.state = enterprisecustomer.ShipToAddress.State;
                    objReturn.addressbookList.addressbook[0].addressbookAddress.zip = enterprisecustomer.ShipToAddress.PostalCode;

                    objReturn.addressbookList.addressbook[0].addressbookAddress.country = base.GetCountry(AddressCountryCode.Alpha2Code);
                    objReturn.addressbookList.addressbook[0].addressbookAddress.countrySpecified = true;

                    if (enterprisecustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = enterprisecustomer.PhoneNumber;
                    }
                }
                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;
                objReturn.password = enterprisecustomer.Password;
                objReturn.password2 = enterprisecustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }


        private com.netsuite.webservices.Customer CreateNetsuiteCustomerIndividual()
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                    objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                    objReturn.companyName = ImageSolutionsCustomer.FirstName;
                else
                    objReturn.companyName = ImageSolutionsCustomer.Email;

                objReturn.entityId = String.Format("{0} {1}", objReturn.lastName, ImageSolutionsCustomer.EmployeeID);
                objReturn.email = ImageSolutionsCustomer.Email;
                if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                }

                objReturn.isPerson = true;
                objReturn.isPersonSpecified = true;

                objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.EmployeeID, "custentity_store_emp_id");
                //if(ImageSolutionsCustomer.HireDate != null)
                //{
                //    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(Convert.ToDateTime(ImageSolutionsCustomer.HireDate), "custentity_store_emp_hire_date");
                //}

                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;
                objReturn.password = ImageSolutionsCustomer.Password;
                objReturn.password2 = ImageSolutionsCustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private com.netsuite.webservices.Customer CreateEnterpriseNetsuiteCustomerIndividual(ImageSolutions.Enterprise.EnterpriseCustomer enterprisecustomer)
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.firstName = enterprisecustomer.FirstName.Length > 32 ? enterprisecustomer.FirstName.Substring(0, 32) : enterprisecustomer.FirstName;
                objReturn.lastName = enterprisecustomer.LastName.Length > 32 ? enterprisecustomer.LastName.Substring(0, 32) : enterprisecustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(enterprisecustomer.CompanyName) ? enterprisecustomer.CompanyName : enterprisecustomer.FirstName + " " + enterprisecustomer.LastName;
                if (!string.IsNullOrEmpty(enterprisecustomer.CompanyName))
                    objReturn.companyName = enterprisecustomer.CompanyName;
                else if (!string.IsNullOrEmpty(enterprisecustomer.FirstName))
                    objReturn.companyName = enterprisecustomer.FirstName;
                else
                    objReturn.companyName = enterprisecustomer.Email;

                objReturn.entityId = String.Format("{0} {1}", objReturn.lastName, enterprisecustomer.EmployeeID);
                objReturn.email = enterprisecustomer.Email;
                if (enterprisecustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = enterprisecustomer.PhoneNumber;
                }

                objReturn.isPerson = true;
                objReturn.isPersonSpecified = true;

                objReturn.parent = NetSuiteHelper.GetRecordRef(enterprisecustomer.Parent.InternalID, RecordType.customer);
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.StoreNumber, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.EmployeeID, "custentity_store_emp_id");

                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;
                objReturn.password = enterprisecustomer.Password;
                objReturn.password2 = enterprisecustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private com.netsuite.webservices.Customer CreateEnterpriseNetsuiteCustomerEBA(ImageSolutions.Enterprise.EnterpriseCustomer enterprisecustomer)
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.firstName = enterprisecustomer.FirstName.Length > 32 ? enterprisecustomer.FirstName.Substring(0, 32) : enterprisecustomer.FirstName;
                objReturn.lastName = enterprisecustomer.LastName.Length > 32 ? enterprisecustomer.LastName.Substring(0, 32) : enterprisecustomer.LastName;
                objReturn.companyName = !string.IsNullOrEmpty(enterprisecustomer.CompanyName) ? enterprisecustomer.CompanyName : enterprisecustomer.FirstName + " " + enterprisecustomer.LastName;
                if (!string.IsNullOrEmpty(enterprisecustomer.CompanyName))
                    objReturn.companyName = enterprisecustomer.CompanyName;
                else if (!string.IsNullOrEmpty(enterprisecustomer.FirstName))
                    objReturn.companyName = enterprisecustomer.FirstName;
                else
                    objReturn.companyName = enterprisecustomer.Email;

                objReturn.entityId = String.Format("EBA {0} {1}", objReturn.lastName, enterprisecustomer.EmployeeID);
                objReturn.email = enterprisecustomer.Email;
                if (enterprisecustomer.PhoneNumber.Length > 6)
                {
                    objReturn.phone = enterprisecustomer.PhoneNumber;
                }

                objReturn.isPerson = true;
                objReturn.isPersonSpecified = true;

                //objReturn.parent = NetSuiteHelper.GetRecordRef(enterprisecustomer.Parent.InternalID, RecordType.customer);
                objReturn.parent = NetSuiteHelper.GetRecordRef(ConfigurationManager.AppSettings["EnterpriseEBAInternalID"], RecordType.customer);
                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.StoreNumber, "custentity_store_org_number");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(enterprisecustomer.EmployeeID, "custentity_store_emp_id");

                objReturn.giveAccess = true;
                objReturn.giveAccessSpecified = true;
                objReturn.password = enterprisecustomer.Password;
                objReturn.password2 = enterprisecustomer.Password;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //public static string GetCustomerSuffix(string CustomerPrefix)
        //{
        //    List<Customer> objCustomers = null;
        //    CustomerFilter objFilter = null;
        //    int intReturn = 1;

        //    try
        //    {
        //        objFilter = new CustomerFilter();
        //        objFilter.ExternalID = CustomerPrefix;
        //        objCustomers = GetCustomers(objFilter);
        //        if (objCustomers != null)
        //        {
        //            foreach (Customer objCustomer in objCustomers)
        //            {
        //                string strCustomerNumber = objCustomer.NetSuiteCustomer.entityId.Replace(CustomerPrefix, string.Empty);
        //                if (Utility.IsInteger(strCustomerNumber))
        //                {
        //                    if (Convert.ToInt32(strCustomerNumber) > intReturn)
        //                    {
        //                        intReturn = Convert.ToInt32(strCustomerNumber);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objCustomers = null;
        //        objFilter = null;
        //    }

        //    if ((intReturn + 1) > 99) throw new Exception("Customer Suffix is > 99");

        //    return ((intReturn + 1)).ToString().PadLeft(2, '0');
        //}

        public bool UpdateNetSuiteCustomerID_New(string InternalID, string ExternalID, string CustomerID)
        {
            WriteResponse objWriteResponse = null;
            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                objWriteResponse = Service.update(UpdateNetSuiteCustomerID(InternalID, ExternalID));
                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to create Customer: " + objWriteResponse.status.statusDetail[0].message);
                }
                else
                {
                    dicParam["NetSuiteEntityID"] = ExternalID;
                    dicWParam["CustomerID"] = CustomerID;
                    Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Customer"));
                    //ImageSolutionsCustomer.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                    //ImageSolutionsCustomer.NetSuiteEntityID = NetSuiteCustomer.entityId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWriteResponse = null;

            }
            return true;
        }

        private static com.netsuite.webservices.Customer UpdateNetSuiteCustomerID(string InternalID, string ExternalID)
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerFormID, RecordType.customer);
                objReturn.internalId = InternalID;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ExternalID, "custentity_external_id");
                objReturn.entityId = ExternalID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private com.netsuite.webservices.Customer UpdateNetSuiteCustomer()//string Currency)
        {
            com.netsuite.webservices.Customer objReturn = null;

            try
            {
                //objReturn = new com.netsuite.webservices.Customer();
                //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerFormID, RecordType.customer);
                //objReturn.internalId = ImageSolutionsCustomer.InternalID;

                //objReturn.currencyList = new CustomerCurrencyList();
                //objReturn.currencyList.replaceAll = false;
                //objReturn.currencyList.currency = new CustomerCurrency[1];
                //objReturn.currencyList.currency[0] = new CustomerCurrency();
                //objReturn.currencyList.currency[0].currency = new RecordRef();
                //objReturn.currencyList.currency[0].currency.internalId = GetCurrencyInternalID(Currency);


                int intCustomFieldIndex = 0;
                int intNullFieldIndex = 0;

                try
                {
                    objReturn = new com.netsuite.webservices.Customer();
                    //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerFormID, RecordType.customer);
                    objReturn.internalId = ImageSolutionsCustomer.InternalID;
                    objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                    objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                    objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                    if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                        objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                    else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                        objReturn.companyName = ImageSolutionsCustomer.FirstName;
                    else
                        objReturn.companyName = ImageSolutionsCustomer.Email;
                    objReturn.email = ImageSolutionsCustomer.Email;
                    if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                    }

                    objReturn.isPerson = false;
                    objReturn.isPersonSpecified = true;

                    objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);

                    objReturn.customFieldList = new CustomFieldRef[99];
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.BrandName, "custentity_store_org_number");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_emp_id");

                    //objReturn.addressbookList = new CustomerAddressbookList();
                    //objReturn.addressbookList.addressbook = new CustomerAddressbook[1];
                    //objReturn.addressbookList.addressbook[0] = new CustomerAddressbook();
                    //objReturn.addressbookList.addressbook[0].addressbookAddress = new Address();
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.addressee = ImageSolutionsCustomer.ShipToAddress.FullName;
                    //if (!string.IsNullOrEmpty(ImageSolutionsCustomer.ShipToAddress.CompanyName))
                    //{
                    //    objReturn.addressbookList.addressbook[0].addressbookAddress.attention = ImageSolutionsCustomer.ShipToAddress.CompanyName;
                    //}
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.addr1 = ImageSolutionsCustomer.ShipToAddress.Address1;
                    ////objReturn.addressbookList.addressbook[0].addressbookAddress.addr2 = ImageSolutionsCustomer.ShipToAddress.Address2;
                    ////objReturn.addressbookList.addressbook[0].addressbookAddress.addr3 = ImageSolutionsCustomer.ShipToAddress.Address3;
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.city = ImageSolutionsCustomer.ShipToAddress.City;
                    //if (ImageSolutionsCustomer.ShipToAddress.State.Length > 30)
                    //    objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State.Substring(0, 30);
                    //else
                    //    objReturn.addressbookList.addressbook[0].addressbookAddress.state = ImageSolutionsCustomer.ShipToAddress.State;
                    //if (!string.IsNullOrEmpty(ImageSolutionsCustomer.ShipToAddress.PhoneNumber) && ImageSolutionsCustomer.ShipToAddress.PhoneNumber.Length > 5) objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = ImageSolutionsCustomer.ShipToAddress.PhoneNumber;
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.zip = ImageSolutionsCustomer.ShipToAddress.PostalCode;
                    ////objReturn.addressbookList.addressbook[0].addressbookAddress.country = base.GetCountry(ImageSolutionsCustomer.ShipToAddress.CountryCode);
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.countrySpecified = true;
                    //objReturn.addressbookList.addressbook[0].addressbookAddress.addrPhone = ImageSolutionsCustomer.ShipToAddress.PhoneNumber;

                    objReturn.giveAccess = true;
                    objReturn.giveAccessSpecified = true;

                    objReturn.password = ImageSolutionsCustomer.Password;
                    objReturn.password2 = ImageSolutionsCustomer.Password;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }
        private com.netsuite.webservices.Customer UpdateNetSuiteCustomerStore()
        {
            com.netsuite.webservices.Customer objReturn = null;

            try
            {
                int intCustomFieldIndex = 0;
                int intNullFieldIndex = 0;

                try
                {
                    objReturn = new com.netsuite.webservices.Customer();
                    objReturn.internalId = ImageSolutionsCustomer.InternalID;
                    objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                    objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                    objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                    if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                        objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                    else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                        objReturn.companyName = ImageSolutionsCustomer.FirstName;
                    else
                        objReturn.companyName = ImageSolutionsCustomer.Email;
                    objReturn.email = ImageSolutionsCustomer.Email;
                    if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                    }

                    objReturn.isPerson = false;
                    objReturn.isPersonSpecified = true;

                    objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);

                    objReturn.customFieldList = new CustomFieldRef[99];
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.BrandName, "custentity_store_org_number");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_emp_id");                   

                    objReturn.giveAccess = true;
                    objReturn.giveAccessSpecified = true;

                    objReturn.password = ImageSolutionsCustomer.Password;
                    objReturn.password2 = ImageSolutionsCustomer.Password;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }
        private com.netsuite.webservices.Customer UpdateNetSuiteCustomerIndividual()
        {
            com.netsuite.webservices.Customer objReturn = null;

            try
            {
                int intCustomFieldIndex = 0;
                int intNullFieldIndex = 0;

                try
                {
                    objReturn = new com.netsuite.webservices.Customer();
                    objReturn.internalId = ImageSolutionsCustomer.InternalID;
                    objReturn.firstName = ImageSolutionsCustomer.FirstName.Length > 32 ? ImageSolutionsCustomer.FirstName.Substring(0, 32) : ImageSolutionsCustomer.FirstName;
                    objReturn.lastName = ImageSolutionsCustomer.LastName.Length > 32 ? ImageSolutionsCustomer.LastName.Substring(0, 32) : ImageSolutionsCustomer.LastName;
                    
                    objReturn.companyName = !string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName) ? ImageSolutionsCustomer.CompanyName : ImageSolutionsCustomer.FirstName + " " + ImageSolutionsCustomer.LastName;
                    if (!string.IsNullOrEmpty(ImageSolutionsCustomer.CompanyName))
                        objReturn.companyName = ImageSolutionsCustomer.CompanyName;
                    else if (!string.IsNullOrEmpty(ImageSolutionsCustomer.FirstName))
                        objReturn.companyName = ImageSolutionsCustomer.FirstName;
                    else
                        objReturn.companyName = ImageSolutionsCustomer.Email;

                    objReturn.entityId = String.Format("{0} {1}", objReturn.lastName, ImageSolutionsCustomer.EmployeeID);
                    objReturn.email = ImageSolutionsCustomer.Email;
                    if (ImageSolutionsCustomer.PhoneNumber.Length > 6)
                    {
                        objReturn.phone = ImageSolutionsCustomer.PhoneNumber;
                    }

                    objReturn.isPerson = true;
                    objReturn.isPersonSpecified = true;
                    objReturn.parent = NetSuiteHelper.GetRecordRef(ImageSolutionsCustomer.Parent.InternalID, RecordType.customer);
                    objReturn.customFieldList = new CustomFieldRef[99];
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.StoreNumber, "custentity_store_org_number");
                    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsCustomer.EmployeeID, "custentity_store_emp_id");
                    if (ImageSolutionsCustomer.HireDate != null)
                    {
                        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(Convert.ToDateTime(ImageSolutionsCustomer.HireDate), "custentity_store_emp_hire_date");
                    }

                    objReturn.giveAccess = true;
                    objReturn.giveAccessSpecified = true;

                    objReturn.password = ImageSolutionsCustomer.Password;
                    objReturn.password2 = ImageSolutionsCustomer.Password;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { }
                return objReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public Customer ObjectAlreadyExists()
        {
            List<Customer> objCustomers = null;
            CustomerFilter objFilter = null;
            Customer objReturn = null;

            try
            {
                objFilter = new NetSuiteLibrary.Customer.CustomerFilter();
                objFilter.Email = ImageSolutionsCustomer.Email;
                objCustomers = GetCustomers(objFilter);
                if (objCustomers != null && objCustomers.Count() > 0)
                {
                    if (objCustomers.Count > 1) throw new Exception("More than one Customers with Email:" + ImageSolutionsCustomer.Email + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomers.Select(m => m.NetSuiteCustomer.internalId)));
                    objReturn = objCustomers[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomers = null;
                objFilter = null;
            }
            return objReturn;
        }

        //public Customer ObjectAlreadyExists_Old()
        //{
        //    List<Customer> objCustomers = null;
        //    CustomerFilter objFilter = null;
        //    Customer objReturn = null;

        //    try
        //    {
        //        objFilter = new NetSuiteLibrary.Customer.CustomerFilter();
        //        //objFilter.APIExternalID = ImageSolutionsCustomer.CustomerID;
        //        //objFilter.IsPerson = true;

        //        objCustomers = GetCustomers(BusinessID, objFilter);
        //        if (objCustomers != null && objCustomers.Count() > 0)
        //        {
        //            if (objCustomers.Count > 1) throw new Exception("More than one Customers with API External ID:" + ImageSolutionsCustomer.CustomerID + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomers.Select(m => m.NetSuiteCustomer.internalId)));
        //            objReturn = objCustomers[0];
        //        }
        //        else
        //        {
        //            objFilter = new NetSuiteLibrary.Customer.CustomerFilter();
        //            objFilter.Email = ImageSolutionsCustomer.EmailAddress;
        //            objCustomers = GetCustomers(BusinessID, objFilter);
        //            if (objCustomers != null && objCustomers.Count() > 0)
        //            {
        //                //if (objCustomers.Count > 1) throw new Exception("More than one Customers with Email:" + ImageSolutionsCustomer.EmailAddress + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomers.Select(m => m.NetSuiteCustomer.internalId)));
        //                objReturn = objCustomers[0];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objCustomers = null;
        //        objFilter = null;
        //    }
        //    return objReturn;
        //}
        public List<Customer> GetCustomers(CustomerFilter Filter)
        {
            return GetCustomers(Service, Filter);
        }
        public Customer GetCustomer(CustomerFilter Filter)
        {
            return GetCustomer(Service, Filter);
        }
        public static Customer GetCustomer(NetSuiteService Service, CustomerFilter Filter)
        {
            List<Customer> objCustomers = null;
            Customer objReturn = null;

            try
            {
                objCustomers = GetCustomers(Service, Filter);
                if (objCustomers != null && objCustomers.Count >= 1) objReturn = objCustomers[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomers = null;
            }
            return objReturn;
        }

        public static List<Customer> GetCustomers(NetSuiteService Service, CustomerFilter Filter)
        {
            List<Customer> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Customer>();
                objSearchResult = GetNetSuiteCustomers(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNSCustomer in objSearchResult.recordList)
                        {
                            if (objNSCustomer is NetSuiteLibrary.com.netsuite.webservices.Customer)
                            {
                                objReturn.Add(new Customer((NetSuiteLibrary.com.netsuite.webservices.Customer)objNSCustomer));
                            }
                        }
                        Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }
                    while (objSearchResult.pageSizeSpecified == true && objSearchResult.totalPages >= objSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSearchResult = null;
            }
            return objReturn;
        }

        private static SearchResult GetNetSuiteCustomers(NetSuiteService Service, CustomerFilter Filter)
        {
            SearchResult objSearchResult = null;
            CustomerSearch objCustomerSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objCustomerSearch = new com.netsuite.webservices.CustomerSearch();
                objCustomerSearch.basic = new com.netsuite.webservices.CustomerSearchBasic();

                if (Filter != null)
                {
                    int counter = 0;

                    if (Filter.CustomerInternalIDs != null)
                    {
                        objCustomerSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.CustomerInternalIDs);
                    }

                    if (Filter.ParentInternalIDs != null)
                    {
                        objCustomerSearch.basic.parent = NetSuiteHelper.LoadMultiSearchField(Filter.ParentInternalIDs);
                    }
                    if (Filter.LastModified != null)
                    {
                        objCustomerSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    if (Filter.Status != null)
                    {
                        objCustomerSearch.basic.entityStatus = NetSuiteHelper.LoadMultiSearchField(Filter.Status);
                    }

                    objCustomerSearch.basic.customFieldList = new SearchCustomField[99];

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custentity_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objCustomerSearch.basic.customFieldList[counter] = objAPIExternalID;
                        counter++;
                    }

                    if (!string.IsNullOrEmpty(Filter.ExternalID))
                    {
                        SearchStringCustomField objExternalID = new SearchStringCustomField();
                        objExternalID.scriptId = "custentity_external_id";
                        objExternalID.searchValue = Filter.ExternalID;
                        objExternalID.@operator = SearchStringFieldOperator.startsWith;
                        objExternalID.operatorSpecified = true;
                        objCustomerSearch.basic.customFieldList[counter] = objExternalID;
                        counter++;
                    }


                    if (!string.IsNullOrEmpty(Filter.StoreOrgNumber))
                    {
                        SearchStringCustomField objStoreOrgNumber = new SearchStringCustomField();
                        objStoreOrgNumber.scriptId = "custentity_store_org_number";
                        objStoreOrgNumber.searchValue = Filter.StoreOrgNumber;
                        objStoreOrgNumber.@operator = SearchStringFieldOperator.startsWith;
                        objStoreOrgNumber.operatorSpecified = true;
                        objCustomerSearch.basic.customFieldList[counter] = objStoreOrgNumber;
                        counter++;
                    }


                    if (!string.IsNullOrEmpty(Filter.StoreEmpID))
                    {
                        SearchStringCustomField objStoreEmpID = new SearchStringCustomField();
                        objStoreEmpID.scriptId = "custentity_store_emp_id";
                        objStoreEmpID.searchValue = Filter.StoreEmpID;
                        objStoreEmpID.@operator = SearchStringFieldOperator.startsWith;
                        objStoreEmpID.operatorSpecified = true;
                        objCustomerSearch.basic.customFieldList[counter] = objStoreEmpID;
                        counter++;
                    }

                    if (!string.IsNullOrEmpty(Filter.Email))
                    {
                        objCustomerSearch.basic.email = new SearchStringField();
                        objCustomerSearch.basic.email.@operator = SearchStringFieldOperator.@is;
                        objCustomerSearch.basic.email.operatorSpecified = true;
                        objCustomerSearch.basic.email.searchValue = Filter.Email;
                    }

                    if (Filter.IsPerson != null)
                    {
                        objCustomerSearch.basic.isPerson = NetSuiteHelper.LoadBooleanSearchField(Filter.IsPerson.Value);
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objCustomerSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Customer - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }

        public static List<Customer> GetCustomerSavedSearch(NetSuiteService service, string savedsearchid)
        {
            List<Customer> Customers = null;
            SearchResult SearchResult = null;
            Dictionary<string, string> dicInternalIds = new Dictionary<string, string>();

            try
            {
                Customers = new List<Customer>();
                SearchResult = GetSavedSearch(service, savedsearchid);

                if (SearchResult != null && SearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (CustomerSearchRow _CustomerSearchRow in SearchResult.searchRowList)
                        {
                            if (_CustomerSearchRow is CustomerSearchRow && !dicInternalIds.ContainsKey(_CustomerSearchRow.basic.internalId[0].searchValue.internalId))
                            {
                                string strInternalID = _CustomerSearchRow.basic.internalId[0].searchValue.internalId;

                                dicInternalIds.Add(strInternalID, strInternalID);

                                Customers.Add(new Customer(LoadNetSuiteCustomer(service, strInternalID)));
                            }
                        }
                        service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        SearchResult = service.searchMoreWithId(SearchResult.searchId, SearchResult.pageIndex + 1);
                    }
                    while ( SearchResult.pageSizeSpecified = true && SearchResult.totalPages >= SearchResult.pageIndex );
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Customers;
        }

        public static SearchResult GetSavedSearch(NetSuiteService service, string savedsearchid)
        {
            CustomerSearchAdvanced CustomerSearchAdvanced = null;
            SearchResult SearchResult = null;
            CustomerSearch CustomerSearch = null;
            SearchPreferences SearchPreferences = null;

            try
            {
                CustomerSearchAdvanced = new CustomerSearchAdvanced();
                CustomerSearchAdvanced.savedSearchId = savedsearchid;

                service.searchPreferences = SearchPreferences;
                SearchResult = service.search(CustomerSearchAdvanced);

                if (SearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find Saved Search - {0}: {1}", savedsearchid, SearchResult.status.statusDetail[0].message));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return SearchResult;
        }
    }
}
