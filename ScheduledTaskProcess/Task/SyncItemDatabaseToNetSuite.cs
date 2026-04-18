using NetSuiteLibrary;
using System;
using System.Collections.Generic;

namespace ScheduledTaskProcess.Task
{
    public class SyncItemDatabaseToNetSuite : NetSuiteBase
    {
        public void SyncChildItems()
        {
            List<ImageSolutions.Item.Item> objItems = null;
            ImageSolutions.Item.ItemFilter objItemFilter = null;
            NetSuiteLibrary.Item.Item NetSuiteItem = null;

            try
            {
                objItemFilter = new ImageSolutions.Item.ItemFilter();
                objItemFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                objItemFilter.InternalID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                objItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                objItemFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;


                //objItemFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                //objItemFilter.InternalID.SearchString = "467724";

                objItems = ImageSolutions.Item.Item.GetItems(objItemFilter);

                foreach(ImageSolutions.Item.Item _item in objItems)
                {
                    NetSuiteItem = new NetSuiteLibrary.Item.Item(_item);
                    NetSuiteItem.Create();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objItems = null;
                objItemFilter = null;
                NetSuiteItem = null;
            }
        }
    }
}
