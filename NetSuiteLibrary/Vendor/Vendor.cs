using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace NetSuiteLibrary.Vendor
{
    public class Vendor : NetSuiteBase
    {
        private static string NetSuiteVendorFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteVendorFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteVendorFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string NetSuiteLeadQualified
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteLeadQualified"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteLeadQualified"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string NetSuiteLeadFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteLeadFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteLeadFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string NetSuiteConsignedMerchantTermID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteConsignedMerchantTermID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteConsignedMerchantTermID"].ToString();
                else
                    throw new Exception(" Missing NetSuiteConsignedMerchantTermID in config");
            }
        }

        private static string NetSuiteDropshipMerchantTermID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteDropshipMerchantTermID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteDropshipMerchantTermID"].ToString();
                else
                    throw new Exception(" Missing NetSuiteDropshipMerchantTermID in config");
            }
        }

        private static string ImageSolutionsSubsidiaryID
        {
            get
            {
                if (ConfigurationManager.AppSettings["ImageSolutionsSubsidiaryID"] != null)
                    return ConfigurationManager.AppSettings["ImageSolutionsSubsidiaryID"].ToString();
                else
                    return string.Empty;
            }
        }

        private Customer.Customer mLead = null;
        public Customer.Customer Lead
        {
            get
            {
                if (ImageSolutionsVendor != null && mLead == null && !string.IsNullOrEmpty(ImageSolutionsVendor.NetSuiteLeadInternalID))
                {
                    mLead = LeadObjectAlreadyExists();
                }
                return mLead;
            }
        }

        public int? CommissionPercent
        {
            get
            {
                long? lgReturn = null;

                if (NetSuiteVendor != null)
                {
                    lgReturn = NetSuiteHelper.GetLongCustomFieldValue(NetSuiteVendor, "custentity_merchant_commission_percent");
                }

                return lgReturn == null ? (Int32?)null : Convert.ToInt32(lgReturn);
            }
        }

        public int? FreeStorageMonths
        {
            get
            {
                long? lgReturn = null;

                if (NetSuiteVendor != null)
                {
                    lgReturn = NetSuiteHelper.GetLongCustomFieldValue(NetSuiteVendor, "custentity_merchant_free_storage_months");
                }

                return lgReturn == null ? (Int32?)null : Convert.ToInt32(lgReturn);
            }
        }

        public bool WarrantyServiceFee
        {
            get
            {
                bool blnReturn = false;

                if (NetSuiteVendor != null)
                {
                    blnReturn = NetSuiteHelper.GetBoolCustomFieldValue(NetSuiteVendor, "custentity_merchant_warranty_service_fee");
                }

                return blnReturn;
            }
        }
        public bool OrderProcessingFee
        {
            get
            {
                bool blnReturn = false;

                if (NetSuiteVendor != null)
                {
                    blnReturn = NetSuiteHelper.GetBoolCustomFieldValue(NetSuiteVendor, "custentity_merchant_order_processing_fee");
                }

                return blnReturn;
            }
        }
        public bool TransactionFee
        {
            get
            {
                bool blnReturn = false;

                if (NetSuiteVendor != null)
                {
                    blnReturn = NetSuiteHelper.GetBoolCustomFieldValue(NetSuiteVendor, "custentity_merchant_transaction_fee");
                }

                return blnReturn;
            }
        }

        private ImageSolutions.Vendor.Vendor mImageSolutionsVendor = null;
        public ImageSolutions.Vendor.Vendor ImageSolutionsVendor
        {
            get
            {
                if (mImageSolutionsVendor == null && mNetSuiteVendor != null && !string.IsNullOrEmpty(mNetSuiteVendor.internalId))
                {
                    ImageSolutions.Vendor.VendorFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Vendor.VendorFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteVendor.internalId;
                        mImageSolutionsVendor = ImageSolutions.Vendor.Vendor.GetVendor(objFilter);
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
                return mImageSolutionsVendor;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.Vendor mNetSuiteVendor = null;
        public NetSuiteLibrary.com.netsuite.webservices.Vendor NetSuiteVendor
        {
            get
            {
                if (mNetSuiteVendor == null && mImageSolutionsVendor != null && !string.IsNullOrEmpty(mImageSolutionsVendor.InternalID))
                {
                    mNetSuiteVendor = LoadNetSuiteVendor(mImageSolutionsVendor.InternalID);
                }
                return mNetSuiteVendor;
            }
            set
            {
                mNetSuiteVendor = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.Customer mNetSuiteCustomer = null;
        public NetSuiteLibrary.com.netsuite.webservices.Customer NetSuiteCustomer
        {
            get
            {
                if (mNetSuiteCustomer == null && mImageSolutionsVendor != null && !string.IsNullOrEmpty(mImageSolutionsVendor.NetSuiteLeadInternalID))
                {
                    mNetSuiteCustomer = LoadNetSuiteCustomer(mImageSolutionsVendor.NetSuiteLeadInternalID);
                }
                return mNetSuiteCustomer;
            }
            set
            {
                mNetSuiteCustomer = value;
            }
        }

        public Vendor(ImageSolutions.Vendor.Vendor ImageSolutionsVendor)
        {
            mImageSolutionsVendor = ImageSolutionsVendor;
        }

        public Vendor(NetSuiteLibrary.com.netsuite.webservices.Vendor NetSuiteVendor)
        {
            mNetSuiteVendor = NetSuiteVendor;
        }

        private NetSuiteLibrary.com.netsuite.webservices.Vendor LoadNetSuiteVendor(string NetSuiteInternalID)
        {
            RecordRef objVendorRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Vendor objReturn = null;
            try
            {
                objVendorRef = new RecordRef();
                objVendorRef.internalId = NetSuiteInternalID;
                objVendorRef.type = RecordType.vendor;
                objVendorRef.typeSpecified = true;

                objReadResult = Service.get(objVendorRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.Vendor))
                {
                    objReturn = (com.netsuite.webservices.Vendor)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Vendor with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objVendorRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.Customer LoadNetSuiteCustomer(string NetSuiteInternalID)
        {
            RecordRef objCustomerRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Customer objReturn = null;
            try
            {
                objCustomerRef = new RecordRef();
                objCustomerRef.internalId = NetSuiteInternalID;
                objCustomerRef.type = RecordType.customer;
                objCustomerRef.typeSpecified = true;

                objReadResult = Service.get(objCustomerRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.Customer))
                {
                    objReturn = (com.netsuite.webservices.Customer)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Customer with Internal ID : " + NetSuiteInternalID);
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

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            Vendor objVendor = null;

            try
            {
                if (ImageSolutionsVendor == null) throw new Exception("ImageSolutionsVendor cannot be null");

                objVendor = ObjectAlreadyExists();
                if (objVendor != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    UpdateCustomer(objVendor);
                    ImageSolutionsVendor.InternalID = objVendor.NetSuiteVendor.internalId;
                    ImageSolutionsVendor.NetSuiteEntityID = objVendor.NetSuiteVendor.entityId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetsuiteVendor());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Vendor: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        objVendor = ObjectAlreadyExists();
                        UpdateCustomer(objVendor);
                        ImageSolutionsVendor.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ImageSolutionsVendor.NetSuiteEntityID = NetSuiteVendor.entityId;
                    }
                }

                ImageSolutionsVendor.ErrorMessage = string.Empty;
                ImageSolutionsVendor.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsVendor.ErrorMessage = "Vendor.cs - Create() - " + ex.Message;
                ImageSolutionsVendor.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private com.netsuite.webservices.Vendor CreateNetsuiteVendor()
        {
            com.netsuite.webservices.Vendor objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                if (NetSuiteCustomer == null) throw new Exception("Unable to find NetSuiteCustomer: Lead InternalID: " + ImageSolutionsVendor.NetSuiteLeadInternalID);

                objReturn = new com.netsuite.webservices.Vendor();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteVendorFormID, RecordType.vendor);
                objReturn.internalId = ImageSolutionsVendor.InternalID;
                objReturn.entityId = ImageSolutionsVendor.NetSuiteEntityID;
                objReturn.category = NetSuiteHelper.GetRecordRef("4", RecordType.vendorCategory);

                objReturn.companyName = ImageSolutionsVendor.CompanyName;
                objReturn.entityId = ImageSolutionsVendor.CompanyName;
                objReturn.email = ImageSolutionsVendor.Email;
                if (!string.IsNullOrEmpty(ImageSolutionsVendor.Phone)) objReturn.phone = Regex.Replace(ImageSolutionsVendor.Phone.Trim(), "[^0-9]", "");

                objReturn.isPerson = false;
                objReturn.isPersonSpecified = true;
                objReturn.terms = NetSuiteHelper.GetRecordRef("7", RecordType.term); //NET 45

                objReturn.subsidiary = NetSuiteHelper.GetRecordRef(ImageSolutionsSubsidiaryID, RecordType.subsidiary);

                objReturn.customFieldList = new CustomFieldRef[99];
                if (NetSuiteCustomer.customFieldList != null)
                {
                    foreach (CustomFieldRef objCustomField in NetSuiteCustomer.customFieldList)
                    {
                        objReturn.customFieldList[intCustomFieldIndex++] = objCustomField;
                    }
                }
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsVendor.VendorID, "custentity_api_external_id");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsVendor.NetSuiteLeadInternalID, "custentity_vendor_lead_source");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsVendor.MerchantID, "custentity_merchant_id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public bool CreateLead()
        {
            WriteResponse objWriteResponse = null;
            NetSuiteLibrary.Customer.Customer objCustomer = null;

            try
            {
                if (ImageSolutionsVendor == null) throw new Exception("ImageSolutionsVendor cannot be null");

                objCustomer = LeadObjectAlreadyExists();
                if (objCustomer != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    //UpdateCustomer(objVendor);
                    ImageSolutionsVendor.NetSuiteLeadInternalID = objCustomer.NetSuiteCustomer.internalId;
                    ImageSolutionsVendor.NetSuiteLeadEntityID = objCustomer.NetSuiteCustomer.entityId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetsuiteLead());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Lead: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        //UpdateCustomer(objVendor);
                        ImageSolutionsVendor.NetSuiteLeadInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ImageSolutionsVendor.NetSuiteLeadEntityID = NetSuiteCustomer.entityId;
                    }
                }

                ImageSolutionsVendor.ErrorMessage = string.Empty;
                ImageSolutionsVendor.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsVendor.ErrorMessage = "Vendor.cs - CreateNetSuiteLead() - " + ex.Message;
                ImageSolutionsVendor.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public bool UpdateLead()
        {
            WriteResponse objWriteResponse = null;
            try
            {
                if (ImageSolutionsVendor == null) throw new Exception("ImageSolutionsVendor cannot be null");


                objWriteResponse = Service.update(UpdateNetsuiteLead());

                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Lead Update : Lead can not be updated " + objWriteResponse.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteCustomer = null;
                }
                ImageSolutionsVendor.ErrorMessage = string.Empty;
                ImageSolutionsVendor.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsVendor.ErrorMessage = "Vendor.cs - UpdateNetSuiteLead() - " + ex.Message;
                ImageSolutionsVendor.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private com.netsuite.webservices.Customer CreateNetsuiteLead()
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;
            int intNullFieldIndex = 0;

            try
            {
                if (NetSuiteCustomer != null) throw new Exception("NetSuiteCustomer already exists");
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteLeadFormID, RecordType.customer);

                //objReturn.internalId = ImageSolutionsVendor.NetSuiteInternalID;
                //objReturn.entityId = ImageSolutionsVendor.NetSuiteEntityID;

                objReturn.companyName = ImageSolutionsVendor.CompanyName;
                objReturn.email = ImageSolutionsVendor.Email;
                if (!string.IsNullOrEmpty(ImageSolutionsVendor.Phone)) objReturn.phone = Regex.Replace(ImageSolutionsVendor.Phone.Trim(), "[^0-9]", "");

                objReturn.isPerson = false;
                objReturn.isPersonSpecified = true;
                objReturn.entityStatus = NetSuiteHelper.GetRecordRef(NetSuiteLeadQualified, RecordType.customerStatus);

                objReturn.subsidiary = NetSuiteHelper.GetRecordRef(ImageSolutionsSubsidiaryID, RecordType.subsidiary);

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsVendor.VendorID, "custentity_api_external_id");
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField("11", "custentity_lead_status");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsVendor.MerchantID, "custentity_merchant_id");

                objReturn.nullFieldList = new string[99];
                objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        private com.netsuite.webservices.Customer UpdateNetsuiteLead()
        {
            com.netsuite.webservices.Customer objReturn = null;
            int intCustomFieldIndex = 0;
            int intNullFieldIndex = 0;

            try
            {
                if (NetSuiteCustomer == null) throw new Exception("NetSuiteCustomer does not exist to update");
                if (ImageSolutionsVendor == null) throw new Exception("ImageSolutions vendor is required");
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteLeadFormID, RecordType.customer);

                objReturn.internalId = ImageSolutionsVendor.NetSuiteLeadInternalID;
                //objReturn.entityId = ImageSolutionsVendor.NetSuiteLeadEntityID;

                //objReturn.companyName = ImageSolutionsVendor.CompanyName;
                //objReturn.email = ImageSolutionsVendor.Email;
                //if (!string.IsNullOrEmpty(ImageSolutionsVendor.Phone)) objReturn.phone = Regex.Replace(ImageSolutionsVendor.Phone.Trim(), "[^0-9]", "");

                objReturn.customFieldList = new CustomFieldRef[99];


                //objReturn.nullFieldList = new string[99];
                //objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public bool UpdateCustomer(NetSuiteLibrary.Vendor.Vendor Vendor)
        {
            WriteResponse objWriteResponse = null;

            try
            {
                if (NetSuiteCustomer == null) throw new Exception("Customer record is not found");

                objWriteResponse = Service.update(UpdateNetsuiteCustomer(Vendor));
                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to update Customer: " + objWriteResponse.status.statusDetail[0].message);
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

        private com.netsuite.webservices.Customer UpdateNetsuiteCustomer(NetSuiteLibrary.Vendor.Vendor Vendor)
        {
            com.netsuite.webservices.Customer objReturn = null;

            try
            {
                objReturn = new com.netsuite.webservices.Customer();
                objReturn.internalId = NetSuiteCustomer.internalId;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteLeadFormID, RecordType.customer);

                objReturn.customFieldList = new CustomFieldRef[2];
                objReturn.customFieldList[0] = NetSuiteHelper.CreateSelectCustomField(Vendor.NetSuiteVendor.internalId, "custentity_lead_vendor_source");
                objReturn.customFieldList[1] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsVendor.VendorID, "custentity_api_external_id");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public Vendor ObjectAlreadyExists()
        {
            List<Vendor> objVendors = null;
            VendorFilter objFilter = null;
            Vendor objReturn = null;

            try
            {
                objFilter = new NetSuiteLibrary.Vendor.VendorFilter();
                objFilter.APIExternalID = ImageSolutionsVendor.VendorID;

                objVendors = GetVendors(Service, objFilter);
                if (objVendors != null && objVendors.Count() > 0)
                {
                    if (objVendors.Count > 1) throw new Exception("More than one Vendors with API External ID:" + ImageSolutionsVendor.VendorID + " found in Netsuite with InternalIDs " + string.Join(", ", objVendors.Select(m => m.NetSuiteVendor.internalId)));
                    objReturn = objVendors[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objVendors = null;
                objFilter = null;
            }
            return objReturn;
        }

        public NetSuiteLibrary.Customer.Customer LeadObjectAlreadyExists()
        {
            List<Customer.Customer> objCustomers = null;
            Customer.CustomerFilter objFilter = null;
            Customer.Customer objReturn = null;

            try
            {
                objFilter = new Customer.CustomerFilter();
                objFilter.APIExternalID = ImageSolutionsVendor.VendorID;
                objFilter.IsPerson = false;

                objCustomers = Customer.Customer.GetCustomers(Service, objFilter);
                if (objCustomers != null && objCustomers.Count() > 0)
                {
                    if (objCustomers.Count > 1) throw new Exception("More than one Leads with API External ID:" + ImageSolutionsVendor.VendorID + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomers.Select(m => m.NetSuiteCustomer.internalId)));
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

        public List<Vendor> GetVendors(VendorFilter Filter)
        {
            return GetVendors(Service, Filter);
        }

        public static Vendor GetVendor(NetSuiteService Service, VendorFilter Filter)
        {
            List<Vendor> objVendors = null;
            Vendor objReturn = null;

            try
            {
                objVendors = GetVendors(Service, Filter);
                if (objVendors != null && objVendors.Count >= 1) objReturn = objVendors[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objVendors = null;
            }
            return objReturn;
        }

        public static List<Vendor> GetVendors(NetSuiteService Service, VendorFilter Filter)
        {
            List<Vendor> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Vendor>();
                objSearchResult = GetNetSuiteVendors(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNSVendor in objSearchResult.recordList)
                        {
                            if (objNSVendor is NetSuiteLibrary.com.netsuite.webservices.Vendor)
                            {
                                objReturn.Add(new Vendor((NetSuiteLibrary.com.netsuite.webservices.Vendor)objNSVendor));
                            }
                        }
                        Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }
                    while (objSearchResult.pageSizeSpecified = true && objSearchResult.totalPages >= objSearchResult.pageIndex);
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

        private static SearchResult GetNetSuiteVendors(NetSuiteService Service, VendorFilter Filter)
        {
            SearchResult objSearchResult = null;
            VendorSearch objVendorSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objVendorSearch = new com.netsuite.webservices.VendorSearch();
                objVendorSearch.basic = new com.netsuite.webservices.VendorSearchBasic();

                if (Filter != null)
                {
                    if (Filter.VendorInternalIDs != null)
                    {
                        objVendorSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.VendorInternalIDs);
                    }

                    if (Filter.LastModified != null)
                    {
                        objVendorSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        objVendorSearch.basic.customFieldList = new SearchCustomField[1];

                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custentity_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objVendorSearch.basic.customFieldList[0] = objAPIExternalID;
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objVendorSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Vendor - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objVendorSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
}
