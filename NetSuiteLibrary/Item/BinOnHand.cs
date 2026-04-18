using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;

namespace NetSuiteLibrary.Item
{
    public class BinOnHand : NetSuiteBase
    {
        public string BinInternalID { get; set; }
        public string LocationInternalID { get; set; }
        public int QtyOnHand { get; set; }
        public int QtyAvailable { get; set; }

        public BinOnHand()
        {

        }

        private BinOnHand(ItemSearchRow SearchRow)
        {
            if (SearchRow != null && SearchRow.binOnHandJoin != null)
            {
                BinInternalID = SearchRow.binOnHandJoin.binNumber[0].searchValue.internalId;
                LocationInternalID = SearchRow.binOnHandJoin.location[0].searchValue.internalId;
                QtyOnHand = Convert.ToInt32(SearchRow.binOnHandJoin.quantityOnHand[0].searchValue);
                QtyAvailable = Convert.ToInt32(SearchRow.binOnHandJoin.quantityAvailable[0].searchValue);
            }
        }

        //public static List<BinOnHand> GetInventoryDetails(BinOnHandFilter Filter)
        //{
        //    List<BinOnHand> objReturn = null;
        //    SearchResult objSearchResult = null;

        //    try
        //    {
        //        objReturn = new List<BinOnHand>();
        //        objSearchResult = GetNetSuiteInventoryDetails(Service, Filter);
        //        if (objSearchResult != null && objSearchResult.totalRecords > 0)
        //        {
        //            do
        //            {
        //                foreach (ItemSearchRow objSearchRow in objSearchResult.searchRowList)
        //                {
        //                    objReturn.Add(new BinOnHand(objSearchRow));
        //                }
        //                Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
        //                objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);

        //            }
        //            while (objSearchResult.pageSizeSpecified = true && objSearchResult.totalPages >= objSearchResult.pageIndex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objSearchResult = null;
        //    }
        //    return objReturn;
        //}

        private static SearchResult GetNetSuiteInventoryDetails(NetSuiteService Service, BinOnHandFilter Filter)
        {

            SearchResult objSearchResult = null;
            ItemSearchAdvanced objItemSearchAdvanced = null;
            SearchPreferences objSearchPreferences = null;
            ItemSearchRow objRows = null;

            try
            {
                objItemSearchAdvanced = new ItemSearchAdvanced();
                objItemSearchAdvanced.criteria = new ItemSearch();
                objItemSearchAdvanced.criteria.basic = new ItemSearchBasic();

                if (Filter != null)
                {
                    if (Filter.ItemInternalIDs != null && Filter.ItemInternalIDs.Count > 0)
                    {
                        objItemSearchAdvanced.criteria.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.ItemInternalIDs);
                    }
                    if (Filter.LocationIDs != null && Filter.LocationIDs.Count > 0)
                    {
                        if (objItemSearchAdvanced.criteria.binOnHandJoin == null) objItemSearchAdvanced.criteria.binOnHandJoin = new ItemBinNumberSearchBasic();
                        objItemSearchAdvanced.criteria.binOnHandJoin.location = NetSuiteHelper.LoadMultiSearchField(Filter.LocationIDs);
                    }
                }

                objRows = new ItemSearchRow();
                objRows.binOnHandJoin = new ItemBinNumberSearchRowBasic();

                objRows.binOnHandJoin.binNumber = new []{ new SearchColumnSelectField() };
                objRows.binOnHandJoin.location = new[] { new SearchColumnSelectField() };
                objRows.binOnHandJoin.quantityAvailable = new[] { new SearchColumnDoubleField() };
                objRows.binOnHandJoin.quantityOnHand = new[] { new SearchColumnDoubleField() };

                objRows.inventoryDetailJoin = new InventoryDetailSearchRowBasic();
                objRows.inventoryDetailJoin.binNumber = new[] { new SearchColumnSelectField() };

                objItemSearchAdvanced.columns = objRows;

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objItemSearchAdvanced);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Bin On Hand - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItemSearchAdvanced = null;
                objSearchPreferences = null;
                objRows = null;
            }
            return objSearchResult;
        }
    }
}
