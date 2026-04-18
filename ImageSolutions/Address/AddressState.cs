using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Address
{
    public class AddressState : ISBase.BaseClass
    {
        public string CountryID { get; private set; }
        public string StateID { get; private set; }
        public string StateName { get; private set; }

        public AddressState()
        {
            IsActive = true;
        }

        public AddressState(string CountryID, string StateID)
        {
            this.CountryID = CountryID;
            this.StateID = StateID;
            Load();
        }

        public AddressState(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                base.Load();

                strSQL = "SELECT * " +
                         "FROM AddressState (NOLOCK) " +
                         "WHERE CountryID=" + Database.HandleQuote(CountryID) +
                         "AND StateID=" + Database.HandleQuote(StateID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("StateID=" + StateID + " and CountryID=" + CountryID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }

        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("StateID")) StateID = Convert.ToString(objRow["StateID"]);
                if (objColumns.Contains("StateName")) StateName = Convert.ToString(objRow["StateName"]);
                if (objColumns.Contains("CountryID")) CountryID = Convert.ToString(objRow["CountryID"]);

                if (string.IsNullOrEmpty(StateID)) throw new Exception("Missing StateID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
        }

        public static List<AddressState> GetStates(string CountryID)
        {
            List<AddressState> objReturn = null;
            AddressState objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                objReturn = new List<AddressState>();

                strSQL = "SELECT * " +
                         "FROM AddressState (NOLOCK) " +
                         "WHERE CountryID=" + Database.HandleQuote(CountryID) +
                         "ORDER BY StateName";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AddressState(objData.Tables[0].Rows[i]);
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
                objNew = null;
            }
            return objReturn;
        }

        public static AddressState GetState(string countryid, string state)
        {
            AddressState objReturn = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM AddressState (NOLOCK) " +
                         "WHERE CountryID=" + Database.HandleQuote(countryid) +
                         "AND StateName=" + Database.HandleQuote(state);

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    objReturn = new AddressState(objData.Tables[0].Rows[0]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }

    }
}
