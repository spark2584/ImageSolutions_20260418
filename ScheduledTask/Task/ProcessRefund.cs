using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task
{
    public class ProcessRefund
    {
        public bool Execute()
        {
            RefundPending();
            return true;
        }

        public bool RefundPending()
        {
            try
            {
                List<ImageSolutions.Refund.Refund> Refunds = new List<ImageSolutions.Refund.Refund>();
                ImageSolutions.Refund.RefundFilter RefundFilter = new ImageSolutions.Refund.RefundFilter();
                RefundFilter.Status = new Database.Filter.StringSearch.SearchFilter();
                RefundFilter.Status.SearchString = "Pending";
                Refunds = ImageSolutions.Refund.Refund.GetRefunds(RefundFilter);

                foreach (ImageSolutions.Refund.Refund _Refund in Refunds)
                {
                    ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                    StripeAPI.Refund(_Refund);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return true;
        }
    }
}
