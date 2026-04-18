using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace NetSuiteLibrary.SupportCase
{
    public class SupportCase
    {
        public enum enumPreferredContactMethod
        {
            Email = 1,
            Telephone = 2
        }
        public enum enumCaseType
        {
            OrderInquiry = 1,
            ProductInquiry = 2,
            Returns = 3
        }

        private static NetSuiteService Service
        {
            get
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                NetSuiteService mNetSuiteService = new NetSuiteService();
                NetSuiteLibrary.User objUser = new NetSuiteLibrary.User();
                mNetSuiteService.tokenPassport = objUser.TokenPassport();
                mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.Passport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

                return mNetSuiteService;
            }
        }
        
        private static string NetSuiteSupportCaseFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteSupportCaseFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteSupportCaseFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string PreferredContactMethodEmail
        {
            get
            {
                if (ConfigurationManager.AppSettings["PreferredContactMethodEmail"] != null)
                    return ConfigurationManager.AppSettings["PreferredContactMethodEmail"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string PreferredContactMethodTelephone
        {
            get
            {
                if (ConfigurationManager.AppSettings["PreferredContactMethodTelephone"] != null)
                    return ConfigurationManager.AppSettings["PreferredContactMethodTelephone"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string ToolotsSubsidiaryID
        {
            get
            {
                if (ConfigurationManager.AppSettings["ToolotsSubsidiaryID"] != null)
                    return ConfigurationManager.AppSettings["ToolotsSubsidiaryID"].ToString();
                else
                    return string.Empty;
            }
        }

        private Toolots.SupportCase.SupportCase mToolotsSupportCase = null;
        public Toolots.SupportCase.SupportCase ToolotsSupportCase
        {
            get
            {
                if (mToolotsSupportCase == null && mNetSuiteSupportCase != null && !string.IsNullOrEmpty(mNetSuiteSupportCase.internalId))
                {
                    Toolots.SupportCase.SupportCaseFilter objFilter = null;

                    try
                    {
                        objFilter = new Toolots.SupportCase.SupportCaseFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteSupportCase.internalId;
                        mToolotsSupportCase = Toolots.SupportCase.SupportCase.GetSupportCase(objFilter);
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
                return mToolotsSupportCase;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.SupportCase mNetSuiteSupportCase = null;
        public NetSuiteLibrary.com.netsuite.webservices.SupportCase NetSuiteSupportCase
        {
            get
            {
                if (mNetSuiteSupportCase == null && mToolotsSupportCase != null && !string.IsNullOrEmpty(mToolotsSupportCase.NetSuiteInternalID))
                {
                    mNetSuiteSupportCase = LoadNetSuiteSupportCase(mToolotsSupportCase.NetSuiteInternalID);
                }
                return mNetSuiteSupportCase;
            }
            set
            {
                mNetSuiteSupportCase = value;
            }
        }

        private Toolots.Customer.Customer mToolotsCustomer = null;
        public Toolots.Customer.Customer ToolotsCustomer
        {
            get
            {
                if (mToolotsCustomer == null && !string.IsNullOrEmpty(ToolotsSupportCase.Email))
                {
                    Toolots.Customer.CustomerFilter objFilter = null;

                    try
                    {
                        objFilter = new Toolots.Customer.CustomerFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;
                        objFilter.Email = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.Email.SearchString = ToolotsSupportCase.Email;
                        mToolotsCustomer = Toolots.Customer.Customer.GetCustomer(objFilter);
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
                return mToolotsCustomer;
            }
        }

        public SupportCase(Toolots.SupportCase.SupportCase ToolotsSupportCase)
        {
            mToolotsSupportCase = ToolotsSupportCase;
        }

        public SupportCase(NetSuiteLibrary.com.netsuite.webservices.SupportCase NetSuiteSupportCase)
        {
            mNetSuiteSupportCase = NetSuiteSupportCase;
        }

        private NetSuiteLibrary.com.netsuite.webservices.SupportCase LoadNetSuiteSupportCase(string NetSuiteInternalID)
        {
            RecordRef objSupportCaseRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.SupportCase objReturn = null;
            try
            {
                objSupportCaseRef = new RecordRef();
                objSupportCaseRef.internalId = NetSuiteInternalID;
                objSupportCaseRef.type = RecordType.supportCase;
                objSupportCaseRef.typeSpecified = true;

                objReadResult = Service.get(objSupportCaseRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.SupportCase))
                {
                    objReturn = (com.netsuite.webservices.SupportCase)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find SupportCase with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSupportCaseRef = null;
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
            SupportCase objSupportCase = null;

            try
            {
                if (ToolotsSupportCase == null) throw new Exception("ToolotsSupportCase cannot be null");

                objSupportCase = ObjectAlreadyExists();
                if (objSupportCase != null)
                {
                    ToolotsSupportCase.NetSuiteInternalID = objSupportCase.NetSuiteSupportCase.internalId;
                    ToolotsSupportCase.NetSuiteDocumentNumber = objSupportCase.NetSuiteSupportCase.caseNumber;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetsuiteSupportCase());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create SupportCase: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ToolotsSupportCase.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ToolotsSupportCase.NetSuiteDocumentNumber = NetSuiteSupportCase.caseNumber;
                    }
                }

                ToolotsSupportCase.ErrorMessage = string.Empty;
                ToolotsSupportCase.Update();
            }
            catch (Exception ex)
            {
                ToolotsSupportCase.ErrorMessage = "SupportCase.cs - Create() - " + ex.Message;
                ToolotsSupportCase.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        private com.netsuite.webservices.SupportCase CreateNetsuiteSupportCase()
        {
            com.netsuite.webservices.SupportCase objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.SupportCase();
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteSupportCaseFormID, RecordType.supportCase);
                objReturn.profile = new RecordRef();
                objReturn.profile.internalId = "1";
                objReturn.company = new RecordRef();
                if (ToolotsCustomer != null)
                    objReturn.company.internalId = ToolotsCustomer.NetSuiteInternalID;
                else
                    objReturn.company.internalId = "4"; //Anonymous Customer
                objReturn.title = ToolotsSupportCase.CaseType;
                objReturn.email = ToolotsSupportCase.Email;
                objReturn.incomingMessage = ToolotsSupportCase.EmailContent;
                if (!string.IsNullOrEmpty(ToolotsSupportCase.Phone)) objReturn.phone = Regex.Replace(ToolotsSupportCase.Phone.Trim(), "[^0-9]", "");

                //objReturn.subsidiary = NetSuiteHelper.GetRecordRef(ToolotsSubsidiaryID, RecordType.subsidiary);

                switch (ToolotsSupportCase.CaseType.ToLower())
                {
                    case "order":
                        objReturn.category = NetSuiteHelper.GetRecordRef(Convert.ToInt32(enumCaseType.OrderInquiry).ToString(), RecordType.supportCaseType);
                        break;
                    case "product":
                        objReturn.category = NetSuiteHelper.GetRecordRef(Convert.ToInt32(enumCaseType.ProductInquiry).ToString(), RecordType.supportCaseType);
                        break;
                    case "return":
                        objReturn.category = NetSuiteHelper.GetRecordRef(Convert.ToInt32(enumCaseType.Returns).ToString(), RecordType.supportCaseType);
                        break;
                    default:
                        break;
                }

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsSupportCase.SupportCaseID, "custevent_api_external_id");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsSupportCase.Name, "custevent_contact_name");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsSupportCase.Company, "custevent_company");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsSupportCase.Industry, "custevent_industry");
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsSupportCase.Position, "custevent_position");

                switch (ToolotsSupportCase.PreferredContactMethod.ToLower())
                {
                    case "email":
                        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(Convert.ToInt32(enumPreferredContactMethod.Email).ToString(), "custevent_preferred_contact_method");
                        break;
                    case "telephone":
                        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(Convert.ToInt32(enumPreferredContactMethod.Telephone).ToString(), "custevent_preferred_contact_method");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public SupportCase ObjectAlreadyExists()
        {
            List<SupportCase> objSupportCases = null;
            SupportCaseFilter objFilter = null;
            SupportCase objReturn = null;

            try
            {
                objFilter = new NetSuiteLibrary.SupportCase.SupportCaseFilter();
                objFilter.APIExternalID = ToolotsSupportCase.SupportCaseID;

                objSupportCases = GetSupportCases(Service, objFilter);
                if (objSupportCases != null && objSupportCases.Count() > 0)
                {
                    if (objSupportCases.Count > 1) throw new Exception("More than one SupportCases with API External ID:" + ToolotsSupportCase.SupportCaseID + " found in Netsuite with InternalIDs " + string.Join(", ", objSupportCases.Select(m => m.NetSuiteSupportCase.internalId)));
                    objReturn = objSupportCases[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSupportCases = null;
                objFilter = null;
            }
            return objReturn;
        }

        public static List<SupportCase> GetSupportCases(SupportCaseFilter Filter)
        {
            return GetSupportCases(Service, Filter);
        }

        public static SupportCase GetSupportCase(NetSuiteService Service, SupportCaseFilter Filter)
        {
            List<SupportCase> objSupportCases = null;
            SupportCase objReturn = null;

            try
            {
                objSupportCases = GetSupportCases(Service, Filter);
                if (objSupportCases != null && objSupportCases.Count >= 1) objReturn = objSupportCases[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSupportCases = null;
            }
            return objReturn;
        }

        public static List<SupportCase> GetSupportCases(NetSuiteService Service, SupportCaseFilter Filter)
        {
            List<SupportCase> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<SupportCase>();
                objSearchResult = GetNetSuiteSupportCases(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNSSupportCase in objSearchResult.recordList)
                        {
                            if (objNSSupportCase is NetSuiteLibrary.com.netsuite.webservices.SupportCase)
                            {
                                objReturn.Add(new SupportCase((NetSuiteLibrary.com.netsuite.webservices.SupportCase)objNSSupportCase));
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

        private static SearchResult GetNetSuiteSupportCases(NetSuiteService Service, SupportCaseFilter Filter)
        {
            SearchResult objSearchResult = null;
            SupportCaseSearch objSupportCaseSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objSupportCaseSearch = new com.netsuite.webservices.SupportCaseSearch();
                objSupportCaseSearch.basic = new com.netsuite.webservices.SupportCaseSearchBasic();

                if (Filter != null)
                {
                    if (Filter.NetSuiteInternalIDs != null)
                    {
                        objSupportCaseSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.NetSuiteInternalIDs);
                    }

                    if (Filter.LastModified != null)
                    {
                        objSupportCaseSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        objSupportCaseSearch.basic.customFieldList = new SearchCustomField[1];

                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custevent_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objSupportCaseSearch.basic.customFieldList[0] = objAPIExternalID;
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objSupportCaseSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find SupportCase - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSupportCaseSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
}
