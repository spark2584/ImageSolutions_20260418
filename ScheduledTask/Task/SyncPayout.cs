using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task
{
    public class SyncPayout
    {
        public bool Execute()
        {
            GetPayout();
            return true;
        }

        public bool UpdateExistingPayoutLine()
        {
            try
            {
                List<ImageSolutions.Payout.PayoutLine> PayoutLines = new List<ImageSolutions.Payout.PayoutLine>();
                PayoutLines = ImageSolutions.Payout.PayoutLine.GetPayoutLines();
                int counter = 0;
                foreach (ImageSolutions.Payout.PayoutLine _PayoutLine in PayoutLines)
                {
                    counter++;
                    Console.WriteLine(counter);
                    ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                    StripeAPI.UpdatePayoutLinePaymentIntent(_PayoutLine);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return true;
        }

        public bool GetPayout()
        {
            try
            {
                DateTime? LastArrivalDate = GetLatestArrivalDate();

                ImageSolutions.StripeAPI.StripeAPI StripeAPI = new ImageSolutions.StripeAPI.StripeAPI();
                StripeAPI.GetPayout(LastArrivalDate);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return true;
        }

        public DateTime? GetLatestArrivalDate()
        {
            DateTime? dtReturn = null;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"SELECT TOP 1 ArrivalDate FROM Payout ORDER BY ArrivalDate DESC");

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                if (objRead.Read())
                {
                    dtReturn =  Convert.ToDateTime(objRead["ArrivalDate"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return dtReturn;
        }
    }
}
