using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ActiveWearIntegration.Task
{
    public class SyncItems
    {
        public static bool Start()
        {
            ActiveWearAPI.Request request = new ActiveWearAPI.Request();
            request.GET_Styles();


            return true;
        }

        public static bool SyncByExternalID()
        {
            List<ImageSolutions.Item.Item> objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            ActiveWearAPI.Request request = null;

            try
            {
                objFilter = new ImageSolutions.Item.ItemFilter();

                //objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //objFilter.ExternalID.SearchString = "B54738507";

                objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ExternalID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;

                objFilter.StyleID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.StyleID.Operator = Database.Filter.StringSearch.SearchOperator.empty;


                objItem = ImageSolutions.Item.Item.GetItems(objFilter);

                int count = 0;
                int totalCount = 0;
                foreach (ImageSolutions.Item.Item _item in objItem)
                {
                    Console.WriteLine(string.Format("{0}/{1}", totalCount, objItem.Count));

                    if(count == 60)
                    {
                        Thread.Sleep(60000);
                        count = 0;
                    }

                    request = new ActiveWearAPI.Request();
                    request.GetProductsByExternalID(_item);
                    count++;
                    totalCount++;
                }

            }
            catch (Exception ex)
            {
             
            }
            finally
            {
                request = null;
                objItem = null;
                objFilter = null;
            }



            return true;
        }
    }
}
