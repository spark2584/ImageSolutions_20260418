using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace NetSuiteLibrary.File
{
    public class File : NetSuiteBase
    {

        //private NetSuiteLibrary.com.netsuite.webservices.File mNetSuiteFile = null;
        //public NetSuiteLibrary.com.netsuite.webservices.File NetSuiteFile
        //{
        //    get
        //    {
        //        if (mNetSuiteFile == null)
        //        {
        //            mNetSuiteFile = LoadNetSuiteFile("");
        //        }
        //        return mNetSuiteFile;
        //    }
        //    private set
        //    {
        //        mNetSuiteFile = value;
        //    }
        //}

        private string InternalID = string.Empty;

        private static NetSuiteLibrary.com.netsuite.webservices.File LoadNetSuiteFile(NetSuiteService service, string netsuiteinternalid)
        {
            RecordRef objFileRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.File objReturn = null;

            try
            {
                objFileRef = new RecordRef();
                objFileRef.type = RecordType.file;
                objFileRef.typeSpecified = true;
                objFileRef.internalId = netsuiteinternalid;

                service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                objReadResult = service.get(objFileRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.File))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.File)objReadResult.record;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFileRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.File mNetsuiteFile = null;
        public NetSuiteLibrary.com.netsuite.webservices.File NetsuiteFile
        {
            get
            {
                if (mNetsuiteFile == null && string.IsNullOrEmpty(InternalID))
                {
                    mNetsuiteFile = LoadNetSuiteFile(Service, InternalID);
                }
                return mNetsuiteFile;
            }
            set
            {
                mNetsuiteFile = value;
            }
        }
        public File()
        {

        }
        public File(string internalid)
        {
            InternalID = internalid;
        }
        public File(NetSuiteLibrary.com.netsuite.webservices.File netsuitefile)
        {
            InternalID = netsuitefile.internalId;
            mNetsuiteFile = netsuitefile;
        }

        public static List<File> GetFileSearch(NetSuiteService service, FileFilter filter)
        {
            List<File> Files = null;
            List<string> FileInternalIds = null;

            try
            {
                Files = new List<File>();

                FileInternalIds = NetSuiteLibrary.File.File.GetNetsuiteInternalIDs(service, filter);
                foreach (string _fileInternalID in FileInternalIds)
                {
                    Console.WriteLine(@"{0}", _fileInternalID);

                    Files.Add(new File(LoadNetSuiteFile(service, _fileInternalID)));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Files;
        }
        public static SearchResult GetNetSuiteFiles(NetSuiteService service, FileFilter filter)
        {
            SearchResult objSearchResult = null;
            FileSearch objFileSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objFileSearch = new FileSearch();
                objFileSearch.basic = new FileSearchBasic();

                if (filter != null)
                {
                    if (filter.CreatedAfter != null)
                    {
                        objFileSearch.basic.created = new SearchDateField();
                        objFileSearch.basic.created.searchValue = filter.CreatedAfter;
                        objFileSearch.basic.created.searchValueSpecified = true;
                        objFileSearch.basic.created.@operator = SearchDateFieldOperator.after;
                        objFileSearch.basic.created.operatorSpecified = true;
                    }

                    if (!string.IsNullOrEmpty(filter.Folder))
                    {
                        objFileSearch.basic.folder = new SearchMultiSelectField();
                        objFileSearch.basic.folder.@operator = SearchMultiSelectFieldOperator.anyOf;
                        objFileSearch.basic.folder.operatorSpecified = true;
                        objFileSearch.basic.folder.searchValue = new RecordRef[1];
                        objFileSearch.basic.folder.searchValue[0] = NetSuiteHelper.GetRecordRef(filter.Folder, RecordType.folder);
                    }
                }

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 250;
                objSearchPreferences.pageSizeSpecified = true;

                service.searchPreferences = objSearchPreferences;
                objSearchResult = service.search(objFileSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find File - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFileSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }

        //public static List<string> GetNetsuiteFileURLs(NetSuiteService service, FileFilter filefilter)
        //{
        //    List<string> objReturn = null;
        //    SearchResult objSearchResult = null;

        //    NetSuiteLibrary.File.FileFilter NetSuiteFileFilter = new NetSuiteLibrary.File.FileFilter();

        //    try
        //    {
        //        objReturn = new List<string>();
        //        objSearchResult = GetNetSuiteFiles(service, filefilter);

        //        if (objSearchResult != null && objSearchResult.totalRecords > 0)
        //        {
        //            do
        //            {
        //                foreach (NetSuiteLibrary.com.netsuite.webservices.File _Record in objSearchResult.recordList)
        //                {
        //                    //Console.WriteLine(_Record.content);
        //                    //Console.WriteLine(_Record.url);
        //                    objReturn.Add(_Record.url);
        //                }

        //                service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
        //                objSearchResult = service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
        //            }
        //            while (objSearchResult.pageSizeSpecified == true && objSearchResult.totalPages >= objSearchResult.pageIndex);
        //        }            
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {

        //    }
        //    return objReturn;
        //}
        
        public static List<string> GetNetsuiteInternalIDs(NetSuiteService service, FileFilter filefilter)
        {
            List<File> Files = null;
            List<string> objReturn = null;
            SearchResult objSearchResult = null;

            NetSuiteLibrary.File.FileFilter NetSuiteFileFilter = new NetSuiteLibrary.File.FileFilter();

            try
            {
                Files = new List<File>();
                objReturn = new List<string>();
                objSearchResult = GetNetSuiteFiles(service, filefilter);

                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (NetSuiteLibrary.com.netsuite.webservices.File _Record in objSearchResult.recordList)
                        {
                            Console.WriteLine(_Record.internalId);
                            objReturn.Add(_Record.internalId);
                        }

                        service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
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

            }
            return objReturn;
        }
    }
}
