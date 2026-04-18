using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace NetSuiteLibrary.Employee
{
    public class Employee
    {
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

        private Toolots.Employee.Employee mToolotsEmployee = null;
        public Toolots.Employee.Employee ToolotsEmployee
        {
            get
            {
                if (mToolotsEmployee == null && mNetSuiteEmployee != null && !string.IsNullOrEmpty(mNetSuiteEmployee.internalId))
                {
                    Toolots.Employee.EmployeeFilter objFilter = null;

                    try
                    {
                        objFilter = new Toolots.Employee.EmployeeFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteEmployee.internalId;
                        mToolotsEmployee = Toolots.Employee.Employee.GetEmployee(objFilter);
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
                return mToolotsEmployee;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.Employee mNetSuiteEmployee = null;
        public NetSuiteLibrary.com.netsuite.webservices.Employee NetSuiteEmployee
        {
            get
            {
                if (mNetSuiteEmployee == null && mToolotsEmployee != null && !string.IsNullOrEmpty(mToolotsEmployee.NetSuiteInternalID))
                {
                    mNetSuiteEmployee = LoadNetSuiteEmployee(mToolotsEmployee.NetSuiteInternalID);
                }
                return mNetSuiteEmployee;
            }
            set
            {
                mNetSuiteEmployee = value;
            }
        }

        public Employee(Toolots.Employee.Employee ToolotsEmployee)
        {
            mToolotsEmployee = ToolotsEmployee;
        }

        public Employee(NetSuiteLibrary.com.netsuite.webservices.Employee NetSuiteEmployee)
        {
            mNetSuiteEmployee = NetSuiteEmployee;
        }

        private NetSuiteLibrary.com.netsuite.webservices.Employee LoadNetSuiteEmployee(string NetSuiteInternalID)
        {
            RecordRef objEmployeeRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Employee objReturn = null;
            try
            {
                objEmployeeRef = new RecordRef();
                objEmployeeRef.internalId = NetSuiteInternalID;
                objEmployeeRef.type = RecordType.employee;
                objEmployeeRef.typeSpecified = true;

                objReadResult = Service.get(objEmployeeRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is com.netsuite.webservices.Employee))
                {
                    objReturn = (com.netsuite.webservices.Employee)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Employee with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmployeeRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public static List<Employee> GetEmployees(EmployeeFilter Filter)
        {
            return GetEmployees(Service, Filter);
        }

        public static Employee GetEmployee(NetSuiteService Service, EmployeeFilter Filter)
        {
            List<Employee> objEmployees = null;
            Employee objReturn = null;

            try
            {
                objEmployees = GetEmployees(Service, Filter);
                if (objEmployees != null && objEmployees.Count >= 1) objReturn = objEmployees[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmployees = null;
            }
            return objReturn;
        }

        public static List<Employee> GetEmployees(NetSuiteService Service, EmployeeFilter Filter)
        {
            List<Employee> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Employee>();
                objSearchResult = GetNetSuiteEmployees(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNSEmployee in objSearchResult.recordList)
                        {
                            if (objNSEmployee is NetSuiteLibrary.com.netsuite.webservices.Employee)
                            {
                                objReturn.Add(new Employee((NetSuiteLibrary.com.netsuite.webservices.Employee)objNSEmployee));
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

        private static SearchResult GetNetSuiteEmployees(NetSuiteService Service, EmployeeFilter Filter)
        {
            SearchResult objSearchResult = null;
            EmployeeSearch objEmployeeSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objEmployeeSearch = new com.netsuite.webservices.EmployeeSearch();
                objEmployeeSearch.basic = new com.netsuite.webservices.EmployeeSearchBasic();

                if (Filter != null)
                {
                    if (Filter.EmployeeInternalIDs != null)
                    {
                        objEmployeeSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.EmployeeInternalIDs);
                    }

                    if (Filter.LastModified != null)
                    {
                        objEmployeeSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    objEmployeeSearch.basic.customFieldList = new SearchCustomField[99];

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custentity_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objEmployeeSearch.basic.customFieldList[0] = objAPIExternalID;
                    }

                    if (Filter.IsPM != null)
                    {
                        SearchBooleanCustomField objIsPM = new SearchBooleanCustomField();
                        objIsPM.scriptId = "custentity_is_product_manager";
                        objIsPM.searchValue = Filter.IsPM.Value;
                        objIsPM.searchValueSpecified = true;
                        objEmployeeSearch.basic.customFieldList[1] = objIsPM;
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objEmployeeSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Employee - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmployeeSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }
    }
}
