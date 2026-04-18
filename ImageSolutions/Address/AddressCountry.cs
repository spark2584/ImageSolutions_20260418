using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Address
{
    public class AddressCountry : ISBase.BaseClass
    {
        public string CountryID { get; private set; }
        public string CountryName { get; private set; }
        public string USPSCountryName { get; private set; }
        private List<AddressState> mStates = null;
        public List<AddressState> States
        {
            get
            {
                if (mStates == null && !string.IsNullOrEmpty(CountryID))
                {
                    mStates = AddressState.GetStates(CountryID);
                }
                return mStates;
            }
        }

        public AddressCountry()
        {
            IsActive = true;
        }

        public AddressCountry(string CountryID)
        {
            this.CountryID = CountryID;
            Load();
        }

        public AddressCountry(DataRow objRow)
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
                         "FROM AddressCountry (NOLOCK) " +
                         "WHERE CountryID=" + Database.HandleQuote(CountryID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("CountryID=" + CountryID + " is not found");
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

                if (objColumns.Contains("CountryID")) CountryID = Convert.ToString(objRow["CountryID"]);
                if (objColumns.Contains("CountryName")) CountryName = Convert.ToString(objRow["CountryName"]);
                if (objColumns.Contains("USPSCountryName")) USPSCountryName = Convert.ToString(objRow["USPSCountryName"]);

                if (string.IsNullOrEmpty(CountryID)) throw new Exception("Missing CountryID in the datarow");
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

        public static List<AddressCountry> GetCountries()
        {
            List<AddressCountry> objReturn = null;
            AddressCountry objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                objReturn = new List<AddressCountry>();

                strSQL = "SELECT * " +
                         "FROM AddressCountry(NOLOCK) " +
                         "ORDER BY CountryName";

                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new AddressCountry(objData.Tables[0].Rows[i]);
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
    }
}
