using ImageSolutions.Custom;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.ShoppingCart
{
    public class ShoppingCartLine : ISBase.BaseClass
    {
        public string ShoppingCartLineID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(ShoppingCartLineID); } }
        public string ShoppingCartID { get; set; }
        public string ItemID { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double? OnlinePrice { get { return Item.OnlinePrice; } }
        public double? TariffCharge
        {
            get
            {
                if (ShoppingCart.UserWebsite.WebSite.DisplayTariffCharge)
                {
                    if (Item.BasePrice != null && Item.OnlinePrice != null)
                        return Item.BasePrice - Item.OnlinePrice;
                    else
                        return 0;
                }
                else
                {
                    return (double?)null;
                }
            }
        }
        public string UserInfoID { get; set; }
        public string CustomListID_1 { get; set; }
        public string CustomListValueID_1 { get; set; }
        public string CustomListID_2 { get; set; }
        public string CustomListValueID_2 { get; set; }
        public string CustomDesignImagePath { get; set; }
        public string CustomDesignName { get; set; }
        public DateTime CreatedOn { get; private set; }

        public string LogosDescription
        {
            get
            {
                string _ret = string.Empty;

                if (Item.ParentItem != null && Item.ParentItem.EnableSelectableLogo && Item.ParentItem.ItemSelectableLogos.Count > 0)
                {
                    if (ShoppingCartLineSelectableLogos != null && ShoppingCartLineSelectableLogos.Count > 0)
                    {
                        foreach (ShoppingCartLineSelectableLogo _ShoppingCartLineSelectableLogo in ShoppingCartLineSelectableLogos)
                        {
                            if (string.IsNullOrEmpty(_ret))
                            {
                                _ret = string.Format(@"Logo: {0}{1}{2}"
                                    , !string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectableLogoID)
                                        ? _ShoppingCartLineSelectableLogo.SelectableLogo.Name
                                        : "No Logo"
                                    , string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectYear) ? string.Empty : string.Format(" ({0}: {1})", string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectableLogo.SelectYearsLabel) ? "Year" : _ShoppingCartLineSelectableLogo.SelectableLogo.SelectYearsLabel, _ShoppingCartLineSelectableLogo.SelectYear)
                                    , _ShoppingCartLineSelectableLogo.BasePrice != null && _ShoppingCartLineSelectableLogo.BasePrice > 0
                                        ? string.Format(" - {0:C}", Convert.ToDecimal(_ShoppingCartLineSelectableLogo.BasePrice))
                                        : string.Empty
                                    );
                            }
                            else
                            {
                                _ret = string.Format(@"{0}, {1}", _ret
                                    , !string.IsNullOrEmpty(_ShoppingCartLineSelectableLogo.SelectableLogoID)
                                        ? _ShoppingCartLineSelectableLogo.SelectableLogo.Name
                                        : "No Logo");
                            }
                        }
                    }
                    else
                    {
                        _ret = string.Format(@"Logo: {1}", _ret, "No Logo");
                    }
                }
                return _ret;
            }
        }
        public string PersonalizationDescription
        {
            get
            {
                string _ret = string.Empty;
                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0)
                {
                    foreach (Item.ItemPersonalizationValue _ItemPersonalizationValue in ItemPersonalizationValues)
                    {
                        if (string.IsNullOrEmpty(_ret))
                        {
                            _ret = string.Format(@"{0}({1})", _ItemPersonalizationValue.Value
                                , !string.IsNullOrEmpty(_ItemPersonalizationValue.TextOption)
                                    ? _ItemPersonalizationValue.TextOption
                                    : !string.IsNullOrEmpty(_ItemPersonalizationValue.ItemPersonalization.Label)
                                        ? _ItemPersonalizationValue.ItemPersonalization.Label
                                        : _ItemPersonalizationValue.ItemPersonalization.Name);
                        }
                        else
                        {
                            _ret = string.Format(@"{0}, {1}({2})", _ret, _ItemPersonalizationValue.Value
                                , !string.IsNullOrEmpty(_ItemPersonalizationValue.TextOption)
                                    ? _ItemPersonalizationValue.TextOption
                                    : !string.IsNullOrEmpty(_ItemPersonalizationValue.ItemPersonalization.Label)
                                        ? _ItemPersonalizationValue.ItemPersonalization.Label
                                        : _ItemPersonalizationValue.ItemPersonalization.Name);
                        }
                    }

                    if (PersonalizationPrice > 0)
                    {
                        _ret = String.Format("{0}-{1:C}", _ret, PersonalizationPrice);
                    }
                }
                return _ret;
            }
        }

        public bool HasCusomtization
        {
            get
            {
                bool _ret = false;
                if (ShoppingCartLineSelectableLogos != null && ShoppingCartLineSelectableLogos.Count > 0) _ret = true;
                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0) _ret = true;
                return _ret;
            }
        }

        public string EmployeeDescription
        {
            get
            {
                string _ret = string.Empty;
                if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID))
                {
                    _ret = string.Format("Employee: {0}", UserInfo.FullName);
                }
                return _ret;
            }
        }

        private ShoppingCart mShoppingCart = null;
        public ShoppingCart ShoppingCart
        {
            get
            {
                if (mShoppingCart == null && !string.IsNullOrEmpty(ShoppingCartID))
                {
                    mShoppingCart = new ShoppingCart(ShoppingCartID);
                }
                return mShoppingCart;
            }
        }

        private Item.Item mItem = null;
        public Item.Item Item
        {
            get
            {
                if (mItem == null && !string.IsNullOrEmpty(ItemID))
                {
                    mItem = new ImageSolutions.Item.Item(ItemID);
                }
                return mItem;
            }
            //set
            //{
            //    mItem = value;
            //}
        }

        private UserInfo mUserInfo = null;
        public UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }

        private Custom.CustomList mCustomList_1 = null;
        public Custom.CustomList CustomList_1
        {
            get
            {
                if (mCustomList_1 == null && !string.IsNullOrEmpty(CustomListID_1))
                {
                    mCustomList_1 = new Custom.CustomList(CustomListID_1);
                }
                return mCustomList_1;
            }
        }

        private Custom.CustomListValue mCustomListValue_1 = null;
        public Custom.CustomListValue CustomListValue_1
        {
            get
            {
                if (mCustomListValue_1 == null && !string.IsNullOrEmpty(CustomListValueID_1))
                {
                    mCustomListValue_1 = new Custom.CustomListValue(CustomListValueID_1);
                }
                return mCustomListValue_1;
            }
        }

        private Custom.CustomList mCustomList_2 = null;
        public Custom.CustomList CustomList_2
        {
            get
            {
                if (mCustomList_2 == null && !string.IsNullOrEmpty(CustomListID_2))
                {
                    mCustomList_2 = new Custom.CustomList(CustomListID_2);
                }
                return mCustomList_2;
            }
        }

        private Custom.CustomListValue mCustomListValue_2 = null;
        public Custom.CustomListValue CustomListValue_2
        {
            get
            {
                if (mCustomListValue_2 == null && !string.IsNullOrEmpty(CustomListValueID_2))
                {
                    mCustomListValue_2 = new Custom.CustomListValue(CustomListValueID_2);
                }
                return mCustomListValue_2;
            }
        }
        private List<ShoppingCartLineSelectableLogo> mShoppingCartLineSelectableLogos = null;
        public List<ShoppingCartLineSelectableLogo> ShoppingCartLineSelectableLogos
        {
            get
            {
                if (mShoppingCartLineSelectableLogos == null && !string.IsNullOrEmpty(ShoppingCartLineID))
                {
                    ShoppingCartLineSelectableLogoFilter objFilter = null;

                    try
                    {
                        objFilter = new ShoppingCartLineSelectableLogoFilter();
                        objFilter.ShoppingCartLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ShoppingCartLineID.SearchString = ShoppingCartLineID;

                        mShoppingCartLineSelectableLogos = ShoppingCartLineSelectableLogo.GetShoppingCartLineSelectableLogos(objFilter);
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
                return mShoppingCartLineSelectableLogos;
            }
        }
        private List<Item.ItemPersonalizationValue> mItemPersonalizationValues = null;
        public List<Item.ItemPersonalizationValue> ItemPersonalizationValues
        {
            get
            {
                if (mItemPersonalizationValues == null && !string.IsNullOrEmpty(ShoppingCartLineID))
                {
                    Item.ItemPersonalizationValueFilter objFilter = null;

                    try
                    {
                        objFilter = new Item.ItemPersonalizationValueFilter();
                        objFilter.ShoppingCartLineID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.ShoppingCartLineID.SearchString = ShoppingCartLineID;

                        mItemPersonalizationValues = ImageSolutions.Item.ItemPersonalizationValue.GetItemPersonalizationValues(objFilter);
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
                return mItemPersonalizationValues;
            }
        }
        public double LogoPrice
        {
            get
            {
                double _ret = 0;

                if (ShoppingCartLineSelectableLogos != null && ShoppingCartLineSelectableLogos.Count > 0)
                {
                    return Math.Round(Convert.ToDouble(ShoppingCartLineSelectableLogos.Sum(x => x.BasePrice == null ? 0 : x.BasePrice)), 2);

                    //foreach(ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo _ShoppingCartLineSelectableLogo in ShoppingCartLineSelectableLogos)
                    //{
                    //    _ret = _ret + (_ShoppingCartLineSelectableLogo.BasePrice == null ? 0 : Convert.ToDouble(_ShoppingCartLineSelectableLogo.BasePrice)) ;
                    //}
                }

                return _ret;
            }
        }

        public double PersonalizationPrice
        {
            get
            {
                double _ret = 0;

                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0)
                {
                    if (Item.ParentItem != null && Item.ParentItem.ItemSelectableLogos != null && Item.ParentItem.ItemSelectableLogos.Count > 0
                        && ItemPersonalizationValues.Exists(x => !string.IsNullOrEmpty(x.Value)))
                    {
                        List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = Item.ParentItem.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive && x.SelectableLogo.IsPersonalization);

                        if (ItemSelectableLogos != null && ItemSelectableLogos.Count > 0)
                        {
                            _ret = Math.Round(Convert.ToDouble(ItemSelectableLogos.Sum(y => y.SelectableLogo.BasePrice == null ? 0 : y.SelectableLogo.BasePrice)), 2);
                        }
                    }
                }

                return _ret;
            }
        }

        public double UnitTotal
        {
            get
            {
                if (ShoppingCart.UserWebsite.WebSite.DisplayTariffCharge)
                    return Math.Round((Item.OnlinePrice == null ? UnitPrice : Item.OnlinePrice.Value) + LogoPrice + PersonalizationPrice, 2);
                else
                    return Math.Round(UnitPrice + LogoPrice + PersonalizationPrice, 2);
            }
        }


        public double LineTotal
        {
            get
            {
                if (ShoppingCart.UserWebsite.WebSite.DisplayTariffCharge)
                    return Math.Round((UnitTotal + (TariffCharge == null ? 0 : Convert.ToDouble(TariffCharge))) * Quantity, 2);
                else
                    return Math.Round((UnitTotal) * Quantity, 2);

                //return Math.Round((UnitTotal) * Quantity, 2);
            }
        }




        public bool HasCustomizedItem { get; set; }
        public ShoppingCartLine()
        {
        }
        public ShoppingCartLine(bool hasCustomizeItem)
        {
            HasCustomizedItem = hasCustomizeItem;
        }
        public ShoppingCartLine(string ShoppingCartLineID)
        {
            this.ShoppingCartLineID = ShoppingCartLineID;
            Load();
        }

        public ShoppingCartLine(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
                            SELECT sl.* 
                            FROM ShoppingCartLine (NOLOCK) sl 
                            WHERE sl.ShoppingCartLineID = {0} "
                                                , Database.HandleQuote(ShoppingCartLineID)
                            );

                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("ShoppingCartLineID=" + ShoppingCartLineID + " is not found");
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

                if (objColumns.Contains("ShoppingCartLineID")) ShoppingCartLineID = Convert.ToString(objRow["ShoppingCartLineID"]);
                if (objColumns.Contains("ShoppingCartID")) ShoppingCartID = Convert.ToString(objRow["ShoppingCartID"]);
                if (objColumns.Contains("ItemID")) ItemID = Convert.ToString(objRow["ItemID"]);
                if (objColumns.Contains("Quantity")) Quantity = Convert.ToInt32(objRow["Quantity"]);
                if (objColumns.Contains("UnitPrice") && objRow["UnitPrice"] != DBNull.Value) UnitPrice = Convert.ToDouble(objRow["UnitPrice"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("CustomListID_1")) CustomListID_1 = Convert.ToString(objRow["CustomListID_1"]);
                if (objColumns.Contains("CustomListValueID_1")) CustomListValueID_1 = Convert.ToString(objRow["CustomListValueID_1"]);
                if (objColumns.Contains("CustomListID_2")) CustomListID_2 = Convert.ToString(objRow["CustomListID_2"]);
                if (objColumns.Contains("CustomListValueID_2")) CustomListValueID_2 = Convert.ToString(objRow["CustomListValueID_2"]);
                if (objColumns.Contains("CustomDesignImagePath")) CustomDesignImagePath = Convert.ToString(objRow["CustomDesignImagePath"]);
                if (objColumns.Contains("CustomDesignName")) CustomDesignName = Convert.ToString(objRow["CustomDesignName"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(ShoppingCartLineID)) throw new Exception("Missing ShoppingCartLineID in the datarow");
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

        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }


        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            if (!IsActive) return true;

            Hashtable dicParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(ShoppingCartID)) throw new Exception("ShoppingCartID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity < 0) throw new Exception("Quantity cannot be less than 0");
                if (!ValidateQuantityPerPerson()) throw new Exception("Exceeded the purchase limit");
                if (Item.ItemID == "2132155" && Quantity > 10) throw new Exception(String.Format("{0} - Exceeded the purchase limit of 10", Item.StoreDisplayName));
                if (!ValidateMixCart()) throw new Exception("Mix cart not allowed");
                if (!IsNew) throw new Exception("Create cannot be performed, ShoppingCartLineID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                ShoppingCartLine ShoppingCartLine = new ShoppingCartLine();
                //ShoppingCartLineFilter ShoppingCartLineFilter = new ShoppingCartLineFilter();
                //ShoppingCartLineFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                //ShoppingCartLineFilter.ShoppingCartID.SearchString = ShoppingCartID;
                //ShoppingCartLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                //ShoppingCartLineFilter.ItemID.SearchString = ItemID;
                //ShoppingCartLineFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                //ShoppingCartLineFilter.UserInfoID.SearchString = UserInfoID;
                //ShoppingCartLineFilter.CustomDesignImagePath = new Database.Filter.StringSearch.SearchFilter();
                //ShoppingCartLineFilter.CustomDesignImagePath.SearchString = CustomDesignImagePath;
                //ShoppingCartLineFilter.CustomDesignName = new Database.Filter.StringSearch.SearchFilter();
                //ShoppingCartLineFilter.CustomDesignName.SearchString = CustomDesignName;
                //ShoppingCartLine = ShoppingCartLine.GetShoppingCartLine(ShoppingCartLineFilter);

                List<ShoppingCartLine> ShoppingCartLines = new List<ShoppingCartLine>();
                ShoppingCartLineFilter ShoppingCartLineFilter = new ShoppingCartLineFilter();
                ShoppingCartLineFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.ShoppingCartID.SearchString = ShoppingCartID;
                ShoppingCartLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.ItemID.SearchString = ItemID;
                ShoppingCartLineFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.UserInfoID.SearchString = UserInfoID;
                ShoppingCartLineFilter.CustomDesignImagePath = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.CustomDesignImagePath.SearchString = CustomDesignImagePath;
                ShoppingCartLineFilter.CustomDesignName = new Database.Filter.StringSearch.SearchFilter();
                ShoppingCartLineFilter.CustomDesignName.SearchString = CustomDesignName;
                ShoppingCartLines = ShoppingCartLine.GetShoppingCartLines(ShoppingCartLineFilter);

                foreach (ShoppingCartLine _ShoppingCartLine in ShoppingCartLines)
                {
                    if (
                        (_ShoppingCartLine.ShoppingCartLineSelectableLogos == null || _ShoppingCartLine.ShoppingCartLineSelectableLogos.Count == 0)
                        &&
                        (_ShoppingCartLine.ItemPersonalizationValues == null || _ShoppingCartLine.ItemPersonalizationValues.Count == 0)
                    )
                    {
                        ShoppingCartLine = _ShoppingCartLine;
                    }
                }

                if (ShoppingCartLine != null && !string.IsNullOrEmpty(ShoppingCartLine.ShoppingCartLineID) && !base.IsCopied && !HasCustomizedItem)
                {
                    ShoppingCartLine.Quantity = ShoppingCartLine.Quantity + Quantity;
                    ShoppingCartLine.Update(objConn, objTran);
                }
                else
                {
                    dicParam["ShoppingCartID"] = ShoppingCartID;
                    dicParam["ItemID"] = ItemID;
                    dicParam["Quantity"] = Quantity;
                    dicParam["UnitPrice"] = UnitPrice;
                    dicParam["UserInfoID"] = UserInfoID;
                    dicParam["CustomListID_1"] = CustomListID_1;
                    dicParam["CustomListValueID_1"] = CustomListValueID_1;
                    dicParam["CustomListID_2"] = CustomListID_2;
                    dicParam["CustomListValueID_2"] = CustomListValueID_2;
                    dicParam["CustomDesignImagePath"] = CustomDesignImagePath;
                    dicParam["CustomDesignName"] = CustomDesignName;

                    ShoppingCartLineID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "ShoppingCartLine"), objConn, objTran).ToString();
                    Load(objConn, objTran);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            if (!IsActive) return Delete(objConn, objTran);

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (string.IsNullOrEmpty(ShoppingCartID)) throw new Exception("ShoppingCartID is required");
                if (string.IsNullOrEmpty(ItemID)) throw new Exception("ItemID is required");
                if (Quantity < 0) throw new Exception("Quantity cannot be less than 0");
                if (!ValidateQuantityPerPerson()) throw new Exception(String.Format("{0} - Exceeded the purchase limit", Item.ItemNumber));
                if (Item.ItemID == "2132155" && Quantity > 10) throw new Exception(String.Format("{0} - Exceeded the purchase limit of 10", Item.StoreDisplayName));
                if (IsNew) throw new Exception("Update cannot be performed, ShoppingCartLineID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                if (Quantity <= 0)
                {
                    this.Delete(objConn, objTran);
                }
                else
                {
                    dicParam["ShoppingCartID"] = ShoppingCartID;
                    dicParam["ItemID"] = ItemID;
                    dicParam["Quantity"] = Quantity;
                    dicParam["UnitPrice"] = UnitPrice;
                    dicParam["UserInfoID"] = UserInfoID;
                    dicParam["CustomListID_1"] = CustomListID_1;
                    dicParam["CustomListValueID_1"] = CustomListValueID_1;
                    dicParam["CustomListID_2"] = CustomListID_2;
                    dicParam["CustomListValueID_2"] = CustomListValueID_2;
                    dicParam["CustomDesignImagePath"] = CustomDesignImagePath;
                    dicParam["CustomDesignName"] = CustomDesignName;
                    dicWParam["ShoppingCartLineID"] = ShoppingCartLineID;
                    Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "ShoppingCartLine"), objConn, objTran);
                    Load(objConn, objTran);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }
            return true;
        }

        public override bool Copy()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Copy(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Copy(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Copy();

            ShoppingCartLineID = String.Empty;

            return Create(objConn, objTran);
        }

        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, ShoppingCartLinesID is missing");

                dicDParam["ShoppingCartLineID"] = ShoppingCartLineID;

                if (ShoppingCartLineSelectableLogos != null && ShoppingCartLineSelectableLogos.Count > 0)
                {
                    foreach (ShoppingCartLineSelectableLogo _ShoppingCartLineSelectableLogo in ShoppingCartLineSelectableLogos)
                    {
                        _ShoppingCartLineSelectableLogo.Delete(objConn, objTran);
                    }
                }

                if (ItemPersonalizationValues != null && ItemPersonalizationValues.Count > 0)
                {
                    foreach (Item.ItemPersonalizationValue _ItemPersonalizationValue in ItemPersonalizationValues)
                    {
                        _ItemPersonalizationValue.Delete(objConn, objTran);
                    }
                }

                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "ShoppingCartLine"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        private bool ObjectAlreadyExists()
        {
            return false;
        }

        private bool ValidateMixCart()
        {
            bool _ret = true;

            foreach (Website.WebsiteTabItem _WebsiteTabItem in Item.ParentItem.WebsiteTabItems)
            {
                if (_WebsiteTabItem.WebsiteTab.DoNotAllowMixCart)
                {
                    foreach(ShoppingCartLine _ShoppingCartLine in ShoppingCart.ShoppingCartLines)
                    {
                        _ret = _WebsiteTabItem.WebsiteTab.WebsiteTabItems.Exists(x => x.ItemID == _ShoppingCartLine.Item.ParentID);
                    }
                }
            }

            foreach (ShoppingCartLine _ShoppingCartLine in ShoppingCart.ShoppingCartLines)
            {
                if (!string.IsNullOrEmpty(_ShoppingCartLine.Item.ParentID) && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems != null && _ShoppingCartLine.Item.ParentItem.WebsiteTabItems.Exists(x => x.WebsiteTab.DoNotAllowMixCart))
                {
                    _ret = _ShoppingCartLine.Item.ParentItem.WebsiteTabItems.Exists(x => x.WebsiteTab.WebsiteTabItems.Exists(y => y.ItemID == Item.ParentID));
                }
            }

            return _ret;
        }

        private bool ValidateQuantityPerPerson()
        {
            bool _ret = true;

            if (Item.ParentItem == null)
            {
                _ret = false;
            }
            else
            if (Item.ParentItem.QuantityPerPerson != null && Item.ParentItem.QuantityPerPerson > 0)
            {

                ImageSolutions.ShoppingCart.ShoppingCart ShoppingCart = new ShoppingCart(ShoppingCartID);

                if (string.IsNullOrEmpty(ShoppingCart.SalesOrderID))
                {
                    decimal decTotal = 0;

                    SqlDataReader objRead = null;
                    string strSQL = string.Empty;

                    strSQL = String.Format(@"
SELECT ISNULL(SUM(sl.Quantity),0) Quantity
FROM SalesOrderLine (NOLOCK) sl
Inner Join SalesOrder (NOLOCK) s on s.SalesOrderID = sl.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
WHERE s.Status != 'Rejected' 
and s.UserInfoID = {0}
and i.ParentID = {1}
"
                        , Database.HandleQuote(ShoppingCart.UserWebsite.UserInfoID)
                        , Database.HandleQuote(Item.ParentID));

                    objRead = Database.GetDataReader(strSQL);
                    if (objRead.Read())
                    {
                        decTotal += Convert.ToDecimal(objRead["Quantity"]);
                    }

                    //List<ImageSolutions.SalesOrder.SalesOrderLine> SalesOrderLines = new List<SalesOrder.SalesOrderLine>();
                    //ImageSolutions.SalesOrder.SalesOrderLineFilter SalesOrderLineFilter = new SalesOrder.SalesOrderLineFilter();
                    //SalesOrderLineFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    //SalesOrderLineFilter.ItemID.SearchString = Item.ParentID;
                    //SalesOrderLineFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    //SalesOrderLineFilter.UserInfoID.SearchString = UserInfoID;
                    //SalesOrderLineFilter.SalesOrderStatus = new Database.Filter.StringSearch.SearchFilter();
                    //SalesOrderLineFilter.SalesOrderStatus.Operator = Database.Filter.StringSearch.SearchOperator.notContain;
                    //SalesOrderLineFilter.SalesOrderStatus.SearchString = "Rejected";
                    //SalesOrderLines = ImageSolutions.SalesOrder.SalesOrderLine.GetSalesOrderLines(SalesOrderLineFilter);
                    //if (SalesOrderLines != null && SalesOrderLines.Count > 0)
                    //{
                    //    decTotal += SalesOrderLines.Sum(x => x.Quantity);
                    //}

                    List<ShoppingCartLine> ShoppingCartLines = new List<ShoppingCartLine>();
                    ShoppingCartLineFilter ShoppingCartLineFilter = new ShoppingCartLineFilter();
                    ShoppingCartLineFilter.ShoppingCartID = new Database.Filter.StringSearch.SearchFilter();
                    ShoppingCartLineFilter.ShoppingCartID.SearchString = ShoppingCartID;
                    ShoppingCartLines = ShoppingCartLine.GetShoppingCartLines(ShoppingCartLineFilter);
                    if (ShoppingCartLines != null && ShoppingCartLines.Count > 0)
                    {
                        decTotal += ShoppingCartLines.FindAll(x => x.Item.ParentID == Item.ParentID && x.ShoppingCartLineID != ShoppingCartLineID).Sum(x => x.Quantity);
                    }

                    _ret = decTotal + Quantity <= Item.ParentItem.QuantityPerPerson;
                }                
            }

            return _ret;
        }

        public static ShoppingCartLine GetShoppingCartLine(ShoppingCartLineFilter Filter)
        {
            List<ShoppingCartLine> objShoppingCartLines = null;
            ShoppingCartLine objReturn = null;

            try
            {
                objShoppingCartLines = GetShoppingCartLines(Filter);
                if (objShoppingCartLines != null && objShoppingCartLines.Count >= 1) objReturn = objShoppingCartLines[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objShoppingCartLines = null;
            }
            return objReturn;

        }

        public static List<ShoppingCartLine> GetShoppingCartLines()
        {
            int intTotalCount = 0;
            return GetShoppingCartLines(null, null, null, out intTotalCount);
        }

        public static List<ShoppingCartLine> GetShoppingCartLines(ShoppingCartLineFilter Filter)
        {
            int intTotalCount = 0;
            return GetShoppingCartLines(Filter, null, null, out intTotalCount);
        }

        public static List<ShoppingCartLine> GetShoppingCartLines(ShoppingCartLineFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShoppingCartLines(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShoppingCartLine> GetShoppingCartLines(ShoppingCartLineFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShoppingCartLine> objReturn = null;
            ShoppingCartLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShoppingCartLine>();

                strSQL = string.Format(@"
                            SELECT sl.* 
                            FROM ShoppingCartLine (NOLOCK) sl 
                            WHERE 1=1 ");

                if (Filter != null)
                {
                    if (Filter.ShoppingCartLineID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartLineID, "sl.ShoppingCartLineID");
                    if (Filter.ShoppingCartID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ShoppingCartID, "sl.ShoppingCartID");
                    if (Filter.ItemID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ItemID, "sl.ItemID");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "sl.UserInfoID");
                    if (Filter.CustomListID_1 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListID_1, "sl.CustomListID_1");
                    if (Filter.CustomListValueID_1 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListValueID_1, "sl.CustomListValueID_1");
                    if (Filter.CustomListID_2 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListID_2, "sl.CustomListID_2");
                    if (Filter.CustomListValueID_2 != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomListValueID_2, "sl.CustomListValueID_2");
                    if (Filter.CustomDesignImagePath != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomDesignImagePath, "sl.CustomDesignImagePath");
                    if (Filter.CustomDesignName != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomDesignName, "sl.CustomDesignName");
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShoppingCartLineID" : Utility.CustomSorting.GetSortExpression(typeof(ShoppingCartLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShoppingCartLine(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
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
            }
            return objReturn;
        }

        //Sales Orders with line item from a Vendor
        public static List<ShoppingCartLine> GetShoppingCartLinesByVendorSalesOrder(string VendorID, string SalesOrderID)
        {
            int intTotalCount = 0;
            return GetShoppingCartLinesByVendorSalesOrder(VendorID, SalesOrderID, null, null, out intTotalCount);
        }

        public static List<ShoppingCartLine> GetShoppingCartLinesByVendorSalesOrder(string VendorID, string SalesOrderID, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetShoppingCartLinesByVendorSalesOrder(VendorID, SalesOrderID, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<ShoppingCartLine> GetShoppingCartLinesByVendorSalesOrder(string VendorID, string SalesOrderID, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<ShoppingCartLine> objReturn = null;
            ShoppingCartLine objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<ShoppingCartLine>();

                strSQL = string.Format(@"
                        SELECT sl.ShoppingCartLineID, sl.SalesOrderID, sl.ParentShoppingCartLineID, sl.Location, sl.ItemID, sl.UnitPrice
	                        , sl.Quantity	
	                        , sl.Quantity * sl.UnitPrice - isnull(sl.DiscountAmount,0) as VendorLineSubTotal
                            , sl.DiscountAmount, sl.DiscountRate
                        FROM SalesOrder (NOLOCK) s
                        Inner Join ShoppingCartLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
                        Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
                        WHERE i.VendorID = {0}
                        and sl.SalesOrderID = {1}
                        and isnull(sl.ParentShoppingCartLineID,'') = ''
                        "
                , Database.HandleQuote(VendorID)
                , Database.HandleQuote(SalesOrderID));

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "ShoppingCartLineID" : Utility.CustomSorting.GetSortExpression(typeof(ShoppingCartLine), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new ShoppingCartLine(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
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
            }
            return objReturn;
        }
    }
}
