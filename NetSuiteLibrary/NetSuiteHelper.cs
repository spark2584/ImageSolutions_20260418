using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.Reflection.Emit;
using System.Reflection;

namespace NetSuiteLibrary
{
    public class NetSuiteHelper
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

        public static CustomList GetCustomList(string InternalID)
        {
            CustomList objReturn = null;
            RecordRef objCustomListRef = null;
            ReadResponse objGetResult = null;

            try
            {
                objCustomListRef = new RecordRef();
                objCustomListRef.type = RecordType.customList;
                objCustomListRef.typeSpecified = true;
                objCustomListRef.internalId = InternalID;

                objGetResult = Service.get(objCustomListRef);

                if (objGetResult.status.isSuccess != true)
                {
                    throw new Exception("Can not find customlist: " + objGetResult.status.statusDetail[0].message);
                }
                else
                {
                    if (objGetResult.record is CustomList) objReturn = (CustomList)objGetResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomListRef = null;
                objGetResult = null;
            }
            return objReturn;
        }
        //only return the list internalid but not the list of values 
        public static string GetCustomListID(string ListName)
        {
            CustomList objCustomList = null;
            string strReturn = string.Empty;
            SearchPreferences objSearchPref = null;
            SearchResult objSearchResult = null;
            CustomListSearch objSearch = null;

            try
            {
                objSearch = new CustomListSearch();
                objSearch.basic = new CustomListSearchBasic();

                if (ListName == null) throw new Exception("List Name must be provided");
                SearchStringField objStringField = new SearchStringField();
                objStringField.searchValue = ListName;
                objStringField.@operator = SearchStringFieldOperator.@is;
                objStringField.operatorSpecified = true;
                objSearch.basic.name = objStringField;

                objSearchPref = new SearchPreferences();
                objSearchPref.bodyFieldsOnly = false;

                Service.searchPreferences = objSearchPref;

                objSearchResult = Service.search(objSearch);
                if (objSearchResult.status.isSuccess != true)
                {
                    throw new Exception("Can not find custom list- " + objSearchResult.status.statusDetail[0].message);
                }
                else
                {
                    foreach (object objResult in objSearchResult.recordList)
                        if (objResult is CustomList)
                        {
                            objCustomList = (CustomList)objResult;
                            strReturn = objCustomList.internalId;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSearch = null;
                objSearchPref = null;
                objSearchResult = null;
            }
            return strReturn;
        }
        //currently only return the active options
        public static Dictionary<string, ListOrRecordRef> MakeSelectValueRefs(CustomList CustomList)
        {
            Dictionary<string, ListOrRecordRef> objReturn = null;
            try
            {
                if (CustomList != null & CustomList.customValueList != null && CustomList.customValueList.customValue != null && CustomList.customValueList.customValue.Count() > 0)
                {
                    objReturn = new Dictionary<string, ListOrRecordRef>();
                    foreach (CustomListCustomValue objCustomValue in CustomList.customValueList.customValue)
                    {
                        if (!objCustomValue.isInactive)
                        {
                            ListOrRecordRef objCustomValueRef = new ListOrRecordRef();
                            objCustomValueRef.internalId = Convert.ToString(objCustomValue.valueId);
                            objCustomValueRef.typeId = CustomList.internalId;
                            objReturn[objCustomValue.value] = objCustomValueRef;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return objReturn;
        }

        public static Dictionary<string, ListOrRecordRef> MakeSelectValueRefs(String CustomListID)
        {
            CustomList objCustomList = GetCustomList(CustomListID);
            return NetSuiteHelper.MakeSelectValueRefs(objCustomList);
        }
        //Add value to customlist when not exist
        public static string AddValueToCustomList(string InternalID, string Value, string Abbreviation)
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomList objCustomList = null;
            NetSuiteLibrary.com.netsuite.webservices.WriteResponse objResponse = null;
            string strReturn = string.Empty;
            try
            {
                objCustomList = new CustomList();
                objCustomList.internalId = InternalID;
                objCustomList.customValueList = new CustomListCustomValueList();
                objCustomList.customValueList.replaceAll = false;
                objCustomList.customValueList.customValue = new CustomListCustomValue[1];
                objCustomList.customValueList.customValue[0] = new CustomListCustomValue();
                objCustomList.customValueList.customValue[0].value = Value;
                objCustomList.customValueList.customValue[0].abbreviation = Abbreviation;
                objResponse = Service.update(objCustomList);
                if (!objResponse.status.isSuccess) throw new Exception("New Value can not be added - " + objResponse.status.statusDetail[0].message);
                strReturn = ((RecordRef)objResponse.baseRef).internalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomList = null;
            }
            return strReturn;
        }

        public static string AddValueToCustomList(string InternalID, string Value)
        {
            return AddValueToCustomList(InternalID, Value, null);
        }
            
        public static void Reset(object Instance)
        {
            if (Instance != null && Instance.GetType() != null)
            {
                PropertyInfo[] Properties = Instance.GetType().GetProperties();
                try
                {
                    if (Properties != null && Properties.Count() > 0)
                    {
                        foreach (PropertyInfo Property in Properties)
                        {
                            if (Property.SetMethod != null) Property.SetValue(Instance, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                }
            }
        }

        //handle netsuite customfield value 
        public static SelectCustomFieldRef CreateSelectCustomField(string ValueInternalID, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Script ID must be specified");
            SelectCustomFieldRef objReturn = null;
            objReturn = new SelectCustomFieldRef();
            objReturn.scriptId = ScriptID;
            objReturn.value = new ListOrRecordRef();
            objReturn.value.internalId = ValueInternalID;
            return objReturn;
        }

        public static DoubleCustomFieldRef CreateDoubleCustomField(double Value, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Custom Field ID must be specified");
            DoubleCustomFieldRef objReturn = null;
            objReturn = new DoubleCustomFieldRef();
            objReturn.scriptId = ScriptID;
            objReturn.value = Value;
            return objReturn;
        }

        public static BooleanCustomFieldRef CreateBooleanCustomField(bool Value, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Custom Field ID must be specified");
            BooleanCustomFieldRef objReturn = null;
            objReturn = new BooleanCustomFieldRef();
            objReturn.value = Value;
            objReturn.scriptId = ScriptID;
            return objReturn;
        }

        public static StringCustomFieldRef CreateStringCustomField(string StringValue, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Custom Field ID must be specified");
            StringCustomFieldRef objReturn = null;
            //if (!string.IsNullOrEmpty(StringValue))
            //{
            //    objReturn = new StringCustomFieldRef();
            //    objReturn.scriptId = ScriptID;
            //    objReturn.value = StringValue;
            //}
            objReturn = new StringCustomFieldRef();
            objReturn.scriptId = ScriptID;
            objReturn.value = StringValue;
            return objReturn;
        }

        public static LongCustomFieldRef CreateLongCustomField(long Value, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Custom Field ID must be specified");
            LongCustomFieldRef objReturn = new LongCustomFieldRef();
            objReturn.value = Value;
            objReturn.scriptId = ScriptID;
            return objReturn;
        }

        public static DateCustomFieldRef CreateDateCustomField(DateTime Value, string ScriptID)
        {
            if (string.IsNullOrEmpty(ScriptID)) throw new Exception("Custom Field ID must be specified");
            DateCustomFieldRef objReturn = new DateCustomFieldRef();
            objReturn.scriptId = ScriptID;
            objReturn.value = Value;
            return objReturn;
        }

        public static CustomFieldRef GetCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            CustomFieldRef objReturn = null;
            CustomFieldRef[] objCustomFields = null;

            try
            {
                if (NetSuiteObject.GetType().GetProperty("customFieldList") != null)
                {
                    objCustomFields = NetSuiteObject.customFieldList;
                    if (objCustomFields != null)
                    {
                        for (int i = 0; i < objCustomFields.Length; i++)
                        {
                            if (objCustomFields[i].scriptId == ScriptID)
                            {
                                objReturn = (CustomFieldRef)objCustomFields[i];
                                break;
                                //LongCustomFieldRef lCFR = (LongCustomFieldRef)customFieldsReturned[j];
                                //quantity = lCFR.value;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("NetSuite object does not contain customfieldlist as a property.");
                }
            }
            catch (Exception ex)    
            {
                throw ex;
            }
            finally
            {
                objCustomFields = null;
            }
            return objReturn;
        }

        public static SearchColumnCustomField GetSearchColumnCustomField(dynamic NetSuiteObject, string ScriptID)
        {
            SearchColumnCustomField objReturn = null;
            SearchColumnCustomField[] objCustomFields = null;

            try
            {
                if (NetSuiteObject.GetType().GetProperty("customFieldList") != null)
                {
                    objCustomFields = NetSuiteObject.customFieldList;
                    if (objCustomFields != null)
                    {
                        for (int i = 0; i < objCustomFields.Length; i++)
                        {
                            if (objCustomFields[i].scriptId == ScriptID)
                            {
                                objReturn = (SearchColumnCustomField)objCustomFields[i];
                                break;
                                //LongCustomFieldRef lCFR = (LongCustomFieldRef)customFieldsReturned[j];
                                //quantity = lCFR.value;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("NetSuite object does not contain customfieldlist as a property.");
                }
            }
            catch
            {
            }
            finally
            {
                objCustomFields = null;
            }
            return objReturn;
        }

        public static ListOrRecordRef[] GetMultiSelectCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            MultiSelectCustomFieldRef objMultiSelect = null;
            ListOrRecordRef[] objReturn = null;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is MultiSelectCustomFieldRef)
                    {
                        objMultiSelect = (MultiSelectCustomFieldRef)NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                        if (objMultiSelect.value != null && objMultiSelect.value.Count() > 0) objReturn = objMultiSelect.value;                            
                    }
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a multiselect custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objMultiSelect = null;
                objCustomFiledValue = null;
            }
            return objReturn;
        }

        public static double? GetDoubleCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            double? dbReturn = null;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is DoubleCustomFieldRef)
                        dbReturn = ((DoubleCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a Double custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return dbReturn;
        }

        public static string GetStringCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            string strReturn = null;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is StringCustomFieldRef)
                        strReturn = ((StringCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a String custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return strReturn;
        }

        public static DateTime? GetDateCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            DateTime? dtReturn = null;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is DateCustomFieldRef)
                        dtReturn = ((DateCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a DateTime custom field.", ScriptID));
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return dtReturn;
        }

        public static DateTime? GetDateSearchColumnFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            DateTime? dtReturn = null;
            SearchColumnCustomField objCustomFiledValue = null;

            try
            {
                objCustomFiledValue = NetSuiteHelper.GetSearchColumnCustomField(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SearchColumnDateCustomField)
                        dtReturn = ((SearchColumnDateCustomField)objCustomFiledValue).searchValue;
                    else
                        throw new InvalidOperationException(string.Format("The search column custom field {0} is not a DateTime search column custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return dtReturn;
        }

        public static bool GetBoolCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            bool blReturn = false;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is BooleanCustomFieldRef)
                        blReturn = ((BooleanCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a Boolean custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return blReturn;
        }

        public static long? GetLongCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            CustomFieldRef objCustomFiledValue = null; ;
            long? lgReturn = null;

            try
            {
                objCustomFiledValue = GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is LongCustomFieldRef)
                        lgReturn = ((LongCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a long number custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return lgReturn;
        }

        public static long? GetLongSearchColumnFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            SearchColumnCustomField objCustomFiledValue = null; ;
            long? lgReturn = null;

            try
            {
                objCustomFiledValue = GetSearchColumnCustomField(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SearchColumnLongCustomField)
                        lgReturn = ((SearchColumnLongCustomField)objCustomFiledValue).searchValue;
                    else
                        throw new InvalidOperationException(string.Format("The search field custom field {0} is not a long number search field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return lgReturn;
        }

        public static ListOrRecordRef GetSelectCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            ListOrRecordRef objReturn = null;
            CustomFieldRef objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SelectCustomFieldRef)
                        objReturn = ((SelectCustomFieldRef)objCustomFiledValue).value;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a Select custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return objReturn;
        }

        public static string GetSelectCustomFieldValueID(dynamic NetSuiteObject, string ScriptID)
        {
            ListOrRecordRef objSelectFieldValue = null;
            string strReturn = null;

            try
            {
                objSelectFieldValue = GetSelectCustomFieldValue(NetSuiteObject, ScriptID);
                if (objSelectFieldValue != null && !string.IsNullOrEmpty(objSelectFieldValue.internalId))
                {
                    strReturn = objSelectFieldValue.internalId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSelectFieldValue = null;
            }
            return strReturn;
        }

        public static double? GetSearchColumnDoubleCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            double? dbReturn = null;
            SearchColumnDoubleCustomField objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetSearchColumnCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SearchColumnDoubleCustomField)
                        dbReturn = ((SearchColumnDoubleCustomField)objCustomFiledValue).searchValue;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a Double custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return dbReturn;
        }
        public static string GetSearchColumnSelectCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            string strReturn = null;
            SearchColumnCustomField objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetSearchColumnCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SearchColumnCustomField)
                        strReturn = ((SearchColumnSelectCustomField)objCustomFiledValue).searchValue.internalId;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a String custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return strReturn;
        }

        public static string GetSearchColumnStringCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            string strReturn = null;
            SearchColumnCustomField objCustomFiledValue = null;
            try
            {
                objCustomFiledValue = NetSuiteHelper.GetSearchColumnCustomFieldValue(NetSuiteObject, ScriptID);
                if (objCustomFiledValue != null)
                {
                    if (objCustomFiledValue is SearchColumnCustomField)
                        strReturn = ((SearchColumnStringCustomField)objCustomFiledValue).searchValue;
                    else
                        throw new InvalidOperationException(string.Format("The custom field {0} is not a String custom field.", ScriptID));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomFiledValue = null;
            }
            return strReturn;
        }
        public static SearchColumnCustomField GetSearchColumnCustomFieldValue(dynamic NetSuiteObject, string ScriptID)
        {
            SearchColumnCustomField objReturn = null;

            SearchColumnCustomField[] searchCustomFields = null;
            try
            {
                if (NetSuiteObject.GetType().GetProperty("customFieldList") != null)
                {
                    searchCustomFields = NetSuiteObject.customFieldList;

                    if (searchCustomFields != null)
                    {
                        for (int i = 0; i < searchCustomFields.Length; i++)
                        {
                            if (searchCustomFields[i].scriptId == ScriptID)
                            {
                                objReturn = (SearchColumnCustomField)searchCustomFields[i];
                                break;
                                //LongCustomFieldRef lCFR = (LongCustomFieldRef)customFieldsReturned[j];
                                //quantity = lCFR.value;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("NetSuite object does not contain customfieldlist as a property.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                searchCustomFields = null;
            }
            return objReturn;
        }

        public static T GetSelectCustomFieldEnum<T>(dynamic NetSuiteObject, string ScriptID)
        {
            ListOrRecordRef objSelectValue = NetSuiteHelper.GetSelectCustomFieldValue(NetSuiteObject, ScriptID);
            dynamic objParsedValue = default(T);
            if (objSelectValue != null && !string.IsNullOrEmpty(objSelectValue.internalId))
            {
                if (Enum.IsDefined(typeof(T), Convert.ToInt32(objSelectValue.internalId)))
                {
                    objParsedValue = Enum.Parse(typeof(T), objSelectValue.internalId);
                }
                else
                {
                    throw new Exception(string.Format("Enum {0} does not have the value \"{1}\" in Catworld API", typeof(T).FullName, objSelectValue.name));
                }
            }
            return (T)objParsedValue;
        }

        public static RecordRef GetRecordRef(string InternalID, RecordType RecordType)
        {
            RecordRef objReturn = new RecordRef();
            objReturn.internalId = InternalID;
            objReturn.type = RecordType;
            objReturn.typeSpecified = true;
            return objReturn;
        }

        public static ReadResponse GetRecord(string internalID, RecordType recordType, NetSuiteService connection)
        {
            RecordRef objRecordRef = null;
            ReadResponse objReturn = null;

            try
            {
                objRecordRef = GetRecordRef(internalID, recordType);
                objReturn = connection.get(objRecordRef);
                if (!objReturn.status.isSuccess) throw new Exception("Can not find Record " + recordType.ToString() + " with InternalID - " + recordType + " : " + objReturn.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRecordRef = null;
            }
            return objReturn;
        }
        
        public static SearchMultiSelectField LoadMultiSearchField(List<string> IDNumbers)
        {
            return LoadMultiSearchField(IDNumbers, null);
        }

        public static SearchMultiSelectField LoadMultiSearchField(List<string> IDNumbers, RecordType? RecordType)
        {
            SearchMultiSelectField objReturn = null;
            RecordRef[] objRecordRefs = null;
            try
            {
                if (IDNumbers != null && IDNumbers.Count() > 0)
                {
                    objReturn = new SearchMultiSelectField();
                    objReturn.@operator = SearchMultiSelectFieldOperator.anyOf;
                    objReturn.operatorSpecified = true;

                    objRecordRefs = new RecordRef[IDNumbers.Count()];
                    for (int i = 0; i < IDNumbers.Count(); i++)
                    {
                        if (IDNumbers[i] != null)
                        {
                            RecordRef objRecordRef = new RecordRef();
                            if (RecordType != null)
                            {
                                objRecordRef.type = RecordType.Value;
                                objRecordRef.typeSpecified = true;
                            }
                            objRecordRef.internalId = IDNumbers[i];
                            objRecordRefs[i] = objRecordRef;
                        }
                    }
                    objReturn.searchValue = objRecordRefs;
                }
                else
                {
                    throw new Exception("Input numbers can not be a null or empty list");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRecordRefs = null;
            }
            return objReturn;
        }

        public static SearchBooleanField LoadBooleanSearchField(bool SearchValue)
        {
            SearchBooleanField objReturn = null;
            try
            {
                objReturn = new SearchBooleanField();
                objReturn.searchValue = SearchValue;
                objReturn.searchValueSpecified = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public static GetDeletedResult GetDeletedRecordAfter(DeletedRecordType RecordType, DateTime Date)
        {
            GetDeletedResult objReturn = null;
            GetDeletedFilter objFilter = null;
            try
            {
                objFilter = new GetDeletedFilter();
                objFilter.deletedDate = new SearchDateField();
                objFilter.deletedDate.@operator = SearchDateFieldOperator.onOrAfter;
                objFilter.deletedDate.operatorSpecified = true;
                objFilter.deletedDate.searchValue = Date;
                objFilter.deletedDate.searchValueSpecified = true;

                objFilter.type = new SearchEnumMultiSelectField();
                objFilter.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objFilter.type.operatorSpecified = true;
                objFilter.type.searchValue = new string[] { RecordType.ToString() };
                objReturn = Service.getDeleted(objFilter, 1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReturn;
        }

        public enum DeletedRecordType
        {
            advInterCompanyJournalEntry,
            assemblyBuild,
            assemblyItem,
            assemblyUnbuild,
            billingSchedule,
            bin,
            binTransfer,
            binWorksheet,
            calendarEvent,
            campaign,
            cashRefund,
            cashSale,
            charge,
            check,
            contact,
            contactCategory,
            costCategory,
            couponCode,
            creditMemo,
            currencyRate,
            customRecord,
            customTransaction,
            customer,
            customerCategory,
            customerMessage,
            customerDeposit,
            customerPayment,
            customerRefund,
            customerStatus,
            deposit,
            depositApplication,
            descriptionItem,
            discountItem,
            downloadItem,
            employee,
            estimate,
            expenseReport,
            file,
            folder,
            giftCertificateItem,
            globalAccountMapping,
            interCompanyJournalEntry,
            interCompanyTransferOrder,
            inventoryAdjustment,
            inventoryCostRevaluation,
            inventoryItem,
            inventoryNumber,
            inventoryTransfer,
            invoice,
            issue,
            itemAccountMapping,
            itemDemandPlan,
            itemFulfillment,
            itemSupplyPlan,
            itemGroup,
            itemReceipt,
            itemRevision,
            job,
            jobStatus,
            journalEntry,
            kitItem,
            lotNumberedAssemblyItem,
            lotNumberedInventoryItem,
            markupItem,
            message,
            manufacturingCostTemplate,
            manufacturingOperationTask,
            manufacturingRouting,
            nexus,
            nonInventoryPurchaseItem,
            nonInventoryResaleItem,
            nonInventorySaleItem,
            note,
            noteType,
            opportunity,
            otherChargePurchaseItem,
            otherChargeResaleItem,
            otherChargeSaleItem,
            otherNameCategory,
            partner,
            paycheck,
            paymentItem,
            paymentMethod,
            payrollItem,
            phoneCall,
            priceLevel,
            pricingGroup,
            projectTask,
            promotionCode,
            purchaseOrder,
            purchaseRequisition,
            resourceAllocation,
            returnAuthorization,
            salesOrder,
            salesTaxItem,
            serializedAssemblyItem,
            serializedInventoryItem,
            servicePurchaseItem,
            serviceResaleItem,
            serviceSaleItem,
            statisticalJournalEntry,
            subtotalItem,
            supportCase,
            supportCaseIssue,
            supportCaseOrigin,
            supportCasePriority,
            supportCaseStatus,
            supportCaseType,
            task,
            term,
            timeSheet,
            transferOrder,
            usage,
            vendor,
            vendorBill,
            vendorCredit,
            vendorPayment,
            vendorReturnAuthorization,
            winLossReason,
            workOrder,
            workOrderIssue,
            workOrderCompletion,
            workOrderClose
        }

    } 
}
