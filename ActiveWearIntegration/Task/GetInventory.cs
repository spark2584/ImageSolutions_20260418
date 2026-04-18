using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveWearIntegration.Task
{
    public class GetInventory
    {
        public static bool Start()
        {
            ImageSolutions.Item.Item objItem = null;
            ImageSolutions.Item.ItemFilter objFilter = null;
            ActiveWearAPI.Request request = null;

            try
            {
                //objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //objFilter.ExternalID.SearchString = "B17086097";

                objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ExternalID.Operator = Database.Filter.StringSearch.SearchOperator.notEmpty;

                objItem = ImageSolutions.Item.Item.GetItem(objFilter);

                request = new ActiveWearAPI.Request();
                request.GetProductsByExternalID(objItem);

            }
            catch (Exception ex)
            {
                objItem.ErrorMessage = ex.Message;
                objItem.Update();
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
