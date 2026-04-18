using ImageSolutions.Website;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace ImageSolutions.Budget
{
    public class MyBudgetAssignment
    {
        public string UserInfoID { get; set; }
        public string BudgetAssignmentID { get; set; }
        public string Description
        {
            get
            {
                return BudgetAssignment == null ? string.Empty : BudgetAssignment.Budget.BudgetName + " (Balance: " + string.Format("{0:c}", Balance) + ")";
            }
        }
        //private List<SalesOrder.SalesOrder> mSalesOrders = null;
        //public List<SalesOrder.SalesOrder> SalesOrders
        //{
        //    get
        //    {
        //        if (mSalesOrders == null && !string.IsNullOrEmpty(UserInfoID) && !string.IsNullOrEmpty(BudgetAssignmentID))
        //        {

        //        }
        //        return mSalesOrders;
        //    }
        //}
        private List<Payment.Payment> mPayments = null;
        public List<Payment.Payment> Payments
        {
            get
            {
                if (mPayments == null && !string.IsNullOrEmpty(UserInfoID) && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    ImageSolutions.Payment.PaymentFilter objFilter = null;
                    try
                    {
                        List<Payment.Payment> Payments = new List<Payment.Payment>();
                        objFilter = new Payment.PaymentFilter();
                        objFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.UserInfoID.SearchString = UserInfoID;
                        objFilter.BudgetAssignmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetAssignmentID.SearchString = BudgetAssignmentID;
                        mPayments = Payment.Payment.GetPayments(objFilter);
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
                return mPayments;
            }
        }

        private BudgetAssignment mBudgetAssignment = null;
        public BudgetAssignment BudgetAssignment
        {
            get
            {
                if (mBudgetAssignment == null && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    mBudgetAssignment = new BudgetAssignment(BudgetAssignmentID);
                }
                return mBudgetAssignment;
            }
        }

        private List<BudgetAssignmentAdjustment> mBudgetAssignmentAdjustments = null;
        public List<BudgetAssignmentAdjustment> BudgetAssignmentAdjustments
        {
            get
            {
                if (mBudgetAssignmentAdjustments == null && !string.IsNullOrEmpty(UserInfoID) && !string.IsNullOrEmpty(BudgetAssignmentID))
                {
                    BudgetAssignmentAdjustmentFilter objFilter = null;
                    try
                    {
                        objFilter = new BudgetAssignmentAdjustmentFilter();
                        objFilter.BudgetAssignmentID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.BudgetAssignmentID.SearchString = BudgetAssignmentID;
                        mBudgetAssignmentAdjustments = BudgetAssignmentAdjustment.GetBudgetAssignmentAdjustments(objFilter);
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
                return mBudgetAssignmentAdjustments;
            }
        }

        private User.UserInfo mUserInfo = null;
        public User.UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new User.UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }
        public double Balance
        {
            get
            {
                double dbReturn = 0;

                if (BudgetAssignment != null)
                {
                    dbReturn = BudgetAssignment.Budget.BudgetAmount;
                    //if (SalesOrders != null)
                    //{
                    //    dbReturn -= SalesOrders.Sum(m => m.Total);
                    //}
                    if (Payments != null)
                    {
                        dbReturn -= Payments.FindAll(x => x.SalesOrder.Status != "Rejected").Sum(m => m.AmountPaid);
                    }

                    if (BudgetAssignmentAdjustments != null)
                    {
                        dbReturn += Convert.ToDouble(BudgetAssignmentAdjustments.Sum(x => x.Amount));
                    }
                }
                return dbReturn < 0 ? 0 : Math.Round(dbReturn, 2);
            }
        }

        public double PendingAmount
        {
            get
            {
                double dbReturn = 0;

                if (BudgetAssignment != null)
                {
                    if (Payments != null)
                    {
                        dbReturn = Payments.FindAll(x => x.SalesOrder.Status != "Rejected" && (x.SalesOrder.IsPendingApproval || x.SalesOrder.IsPendingItemPersonalizationApproval)).Sum(m => m.AmountPaid);
                    }
                }

                return Math.Round(dbReturn, 2);
            }
        }

        public MyBudgetAssignment(string UserInfoID, string BudgetAssignmentID)
        {
            this.UserInfoID = UserInfoID;
            this.BudgetAssignmentID = BudgetAssignmentID;
        }
    }
}
