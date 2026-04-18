using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;

namespace ScheduledTask.Task
{
    public class Task : NetSuiteBase
    {
        public bool SyncItem()
        {
            NetSuiteLibrary.Item.ItemFilter NetSuiteItemFilter = new NetSuiteLibrary.Item.ItemFilter();
            List<NetSuiteLibrary.Item.Item> NetSuiteItems = new List<NetSuiteLibrary.Item.Item>();
            NetSuiteLibrary.Item.Item Item = null;

            try
            {
                NetSuiteItemFilter.PreferredVendor = "1020227";

                //NetSuiteItemFilter.ItemInternalIDs = new List<string>();
                //NetSuiteItemFilter.ItemInternalIDs.Add("595317");

                //NetSuiteItemFilter.PreferredVendor = "40";

                //NetSuiteItemFilter.LastModified = new SearchDateField();
                //NetSuiteItemFilter.LastModified.@operator = SearchDateFieldOperator.after; //use on or after will search date only
                //NetSuiteItemFilter.LastModified.operatorSpecified = true;

                //TimeZoneInfo tzFrom = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                //TimeZoneInfo tzTo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                //NetSuiteItemFilter.LastModified.searchValue = new DateTime(2022,22,6); //TimeZoneInfo.ConvertTime(Convert.ToDateTime(serviceaccount.SyncSOUpdatedAfter), tzFrom, tzTo).AddMinutes(-1);
                //NetSuiteItemFilter.LastModified.searchValueSpecified = true;
                ////NetSuiteItemFilter.LastModified.searchValue2 = dateto; //TimeZoneInfo.ConvertTime(objScheduler.SyncTill, tzFrom, tzTo);
                ////NetSuiteItemFilter.LastModified.searchValue2Specified = true;

                NetSuiteItemFilter.NetSuiteItemTypes = new List<NetSuiteLibrary.Item.NetSuiteItemType>();

                //NetSuiteItemFilter.NetSuiteLibraryItemTypes.Clear();
                NetSuiteItemFilter.NetSuiteItemTypes.Add(NetSuiteLibrary.Item.NetSuiteItemType._nonInventoryItem);
                //Item = new NetSuiteLibrary.Item(serviceaccount);
                //NetSuiteItems = Item.GetItems(NetSuiteItemFilter);
                //ImportNSItems(serviceaccount, NetSuiteItems);

                //NetSuiteItemFilter.NetSuiteLibraryItemTypes.Clear();
                //NetSuiteItemFilter.NetSuiteLibraryItemTypes.Add(NetSuiteLibrary.NetSuiteLibraryItemType._kit);
                //Item = new NetSuiteLibrary.Item(serviceaccount);
                NetSuiteItems = NetSuiteLibrary.Item.Item.GetItems(Service, NetSuiteItemFilter);
                ImportNSItems(NetSuiteItems);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return true;
        }

     
        public bool ImportNSItems(List<NetSuiteLibrary.Item.Item> items)
        {
            if (items != null)
            {
                if (items != null && items.Count > 0)
                {
                    Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = 42 },
                    item =>
                    {
                        SyncItem syncItem = new SyncItem();

                        if (item.NetSuiteInventoryItem != null)
                        {
                            Console.WriteLine("Processing NetSuite Item to ImageSolutions: " + item.NetSuiteInventoryItem.displayName);
                            syncItem.ImportNSItem(item);
                            syncItem.ImportStoreCategory(item);
                        }
                        else if (item.NetSuiteNonInventoryResaleItem != null)
                        {
                            Console.WriteLine("Processing NetSuite NonInventoryResale Item to ImageSolutions: " + item.NetSuiteNonInventoryResaleItem.displayName);
                            syncItem.ImportNSNonInventoryResaleItem(item);
                            syncItem.ImportStoreCategory(item);
                        }
                        else if (item.NetSuiteNonInventoryItem != null)
                        {
                            Console.WriteLine("Processing NetSuite NonInventory Item to ImageSolutions: " + item.NetSuiteNonInventoryItem.displayName);
                            syncItem.ImportNSNonInventoryItem(item);
                            syncItem.ImportStoreCategory(item);
                        }
                        else if (item.NetSuiteKitItem != null)
                        {
                            Console.WriteLine("Processing NetSuite Item to ImageSolutions: " + item.NetSuiteKitItem.displayName);
                            //syncItem.ImportNSKitItem(item);
                        }
                    });
                }
            }
            return true;
        }

        public bool UpdateInventory()
        {
            List<ImageSolutions.Item.Item> objItems = new List<ImageSolutions.Item.Item>();
            ImageSolutions.Item.ItemFilter objFilter = new ImageSolutions.Item.ItemFilter();

            NetSuiteLibrary.Item.ItemFilter NetSuiteItemFilter = new NetSuiteLibrary.Item.ItemFilter();
            List<NetSuiteLibrary.Item.Item> NetSuiteItems = new List<NetSuiteLibrary.Item.Item>();
            NetSuiteLibrary.Item.Item Item = null;

            try
            {
                objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.InternalID.SearchString = "262952";
                objFilter.VendorName = new Database.Filter.StringSearch.SearchFilter();
                objFilter.VendorName.SearchString = "San Mar";

                objItems = ImageSolutions.Item.Item.GetItems(objFilter);

                UpdateVendorInventory(objItems);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }

            return true;
        }

        public bool UpdateVendorInventory(List<ImageSolutions.Item.Item> items)
        {
            if (items != null)
            {
                if (items != null && items.Count > 0)
                {
                    Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = 42 },
                    item =>
                    {
                        UpdateInventory updateInventory = new UpdateInventory();

                        Console.WriteLine("Updating Vendor Inventory InternalID: " + item.InternalID);
                        updateInventory.UpdateVendorInventory(item);
                    });
                }
            }
            return true;
        }
    }
}
