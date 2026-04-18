using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;

namespace NetSuiteLibrary
{
    public class NetSuiteBase
    {
        public ImageSolutions.Employee.Employee APIEmployee { get; set; }

        private NetSuiteService mService = null;

        public NetSuiteService Service
        {
            get
            {
                NetSuiteLibrary.User objUser = null;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                NetSuiteService mNetSuiteService = new NetSuiteService();

                if (APIEmployee == null)
                    objUser = new NetSuiteLibrary.User();
                else
                    objUser = new NetSuiteLibrary.User(APIEmployee);

                mNetSuiteService.tokenPassport = objUser.TokenPassport();
                mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.TokenPassport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

                return mNetSuiteService;
            }
        }

        //public static NetSuiteService Service
        //{
        //    get
        //    {
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //        NetSuiteService mNetSuiteService = new NetSuiteService();
        //        NetSuiteLibrary.User objUser = new NetSuiteLibrary.User();
        //        mNetSuiteService.tokenPassport = objUser.TokenPassport();
        //        mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.TokenPassport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

        //        return mNetSuiteService;
        //    }
        //}

        //public static string BusinessID
        //{
        //    get
        //    {
        //        if (ConfigurationManager.AppSettings["BusinessID"] != null)
        //            return ConfigurationManager.AppSettings["BusinessID"].ToString();
        //        else
        //            return string.Empty;
        //    }
        //}

        public static string GoDirectSubsidiaryID
        {
            get
            {
                if (ConfigurationManager.AppSettings["GoDirectSubsidiaryID"] != null)
                    return ConfigurationManager.AppSettings["GoDirectSubsidiaryID"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string GiftWrapInternalID
        {
            get
            {
                if (ConfigurationManager.AppSettings["GiftWrapInternalID"] != null)
                    return ConfigurationManager.AppSettings["GiftWrapInternalID"];
                else
                    throw new Exception("Missing GiftWrapInternalID");
            }
        }
        protected static string GeneralDiscountInternalID
        {
            get
            {
                if (ConfigurationManager.AppSettings["GeneralDiscountInternalID"] != null)
                    return ConfigurationManager.AppSettings["GeneralDiscountInternalID"];
                else
                    throw new Exception("Missing GeneralDiscountInternalID");
            }
        }
        protected static string TaxItemInternalID
        {
            get
            {
                if (ConfigurationManager.AppSettings["TaxItemInternalID"] != null)
                    return ConfigurationManager.AppSettings["TaxItemInternalID"];
                else
                    throw new Exception("Missing TaxItemInternalID");
            }
        }

        public static string NetSuiteCustomerFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteCustomerFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteCustomerFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string NetSuiteAccountNumber
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteAccountNumber"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteAccountNumber"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string NetSuiteAccountEmail
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteAccountEmail"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteAccountEmail"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string NetSuiteAccountPassword
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteAccountPassword"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteAccountPassword"].ToString();
                else
                    return string.Empty;
            }
        }
        protected static string NetSuiteAccountRoleID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteAccountRoleID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteAccountRoleID"].ToString();
                else
                    return string.Empty;
            }
        }

        protected static string AppConsumerKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppConsumerKey"] != null)
                    return ConfigurationManager.AppSettings["AppConsumerKey"].ToString();
                else
                    throw new Exception("Missing AppConsumerKey");
            }
        }

        protected static string AppConsumerSecret
        {
            get
            {
                if (ConfigurationManager.AppSettings["AppConsumerSecret"] != null)
                    return ConfigurationManager.AppSettings["AppConsumerSecret"].ToString();
                else
                    throw new Exception("Missing AppConsumerSecret");
            }
        }

        protected static string TokenID
        {
            get
            {
                if (ConfigurationManager.AppSettings["TokenID"] != null)
                    return ConfigurationManager.AppSettings["TokenID"].ToString();
                else
                    throw new Exception("Missing TokenID");
            }
        }

        protected static string TokenSecret
        {
            get
            {
                if (ConfigurationManager.AppSettings["TokenSecret"] != null)
                    return ConfigurationManager.AppSettings["TokenSecret"].ToString();
                else
                    throw new Exception("Missing TokenSecret");
            }
        }

        protected static string RestletAuthorization
        {
            get
            {
                return string.Format("NLAuth nlauth_account={0}, nlauth_email={1}, nlauth_signature={2}, nlauth_role={3}", NetSuiteAccountNumber, NetSuiteAccountEmail, NetSuiteAccountPassword, NetSuiteAccountRoleID);
            }
        }

        protected string GetCurrencyInternalID(string Currency)
        {
            string strReturn = string.Empty;

            switch (Currency)
            {
                case "USD":
                    strReturn = "1";
                    break;
                case "CAD":
                    strReturn = "3";
                    break;
                case "EUR":
                    strReturn = "4";
                    break;
                case "CNY":
                    strReturn = "5";
                    break;
                case "HKD":
                    strReturn = "6";
                    break;
                case "JPY":
                    strReturn = "7";
                    break;
                case "GBP":
                    strReturn = "8";
                    break;
                case "MXN":
                    strReturn = "9";
                    break;
                default:
                    throw new Exception("Currency is not setup");
            }
            return strReturn;
        }

        protected NetSuiteLibrary.com.netsuite.webservices.Country GetCountry(string CountryCode)
        {
            switch (CountryCode)
            {
                case "AF":
                    return Country._afghanistan;
                case "AX":
                    return Country._alandIslands;
                case "AL":
                    return Country._albania;
                case "DZ":
                    return Country._algeria;
                case "AS":
                    return Country._americanSamoa;
                case "AD":
                    return Country._andorra;
                case "AO":
                    return Country._angola;
                case "AI":
                    return Country._anguilla;
                case "AQ":
                    return Country._antarctica;
                case "AG":
                    return Country._antiguaAndBarbuda;
                case "AR":
                    return Country._argentina;
                case "AM":
                    return Country._armenia;
                case "AW":
                    return Country._aruba;
                case "AU":
                    return Country._australia;
                case "AT":
                    return Country._austria;
                case "AZ":
                    return Country._azerbaijan;
                case "BS":
                    return Country._bahamas;
                case "BH":
                    return Country._bahrain;
                case "BD":
                    return Country._bangladesh;
                case "BB":
                    return Country._barbados;
                case "BY":
                    return Country._belarus;
                case "BE":
                    return Country._belgium;
                case "BZ":
                    return Country._belize;
                case "BJ":
                    return Country._benin;
                case "BM":
                    return Country._bermuda;
                case "BT":
                    return Country._bhutan;
                case "BO":
                    return Country._bolivia;
                case "BA":
                    return Country._bonaireSaintEustatiusAndSaba;
                case "BW":
                    return Country._bosniaAndHerzegovina;
                case "BV":
                    return Country._botswana;
                case "BR":
                    return Country._bouvetIsland;
                case "VG":
                    return Country._brazil;
                case "IO":
                    return Country._britishIndianOceanTerritory;
                case "BN":
                    return Country._bruneiDarussalam;
                case "BG":
                    return Country._bulgaria;
                case "BF":
                    return Country._burkinaFaso;
                case "BI":
                    return Country._burundi;
                case "KH":
                    return Country._cambodia;
                case "CM":
                    return Country._cameroon;
                case "CA":
                    return Country._canada;
                case "CV":
                    return Country._capeVerde;
                case "KY":
                    return Country._caymanIslands;
                case "CF":
                    return Country._centralAfricanRepublic;
                case "TD":
                    return Country._ceutaAndMelilla;
                case "CL":
                    return Country._chad;
                case "CN":
                    return Country._chile;
                case "HK":
                    return Country._china;
                case "CX":
                    return Country._christmasIsland;
                case "CC":
                    return Country._cocosKeelingIslands;
                case "CO":
                    return Country._colombia;
                case "KM":
                    return Country._comoros;
                case "CG":
                    return Country._congoDemocraticPeoplesRepublic;
                case "CD":
                    return Country._congoRepublicOf;
                case "CK":
                    return Country._cookIslands;
                case "CR":
                    return Country._costaRica;
                case "CI":
                    return Country._coteDIvoire;
                case "HR":
                    return Country._croatiaHrvatska;
                case "CU":
                    return Country._cuba;
                case "CY":
                    return Country._cyprus;
                case "CZ":
                    return Country._czechRepublic;
                case "DK":
                    return Country._denmark;
                case "DJ":
                    return Country._djibouti;
                case "DM":
                    return Country._dominica;
                case "DO":
                    return Country._dominicanRepublic;
                case "EC":
                    return Country._ecuador;
                case "EG":
                    return Country._egypt;
                case "SV":
                    return Country._elSalvador;
                case "GQ":
                    return Country._equatorialGuinea;
                case "ER":
                    return Country._eritrea;
                case "EE":
                    return Country._estonia;
                case "ET":
                    return Country._ethiopia;
                case "FK":
                    return Country._falklandIslands;
                case "FO":
                    return Country._faroeIslands;
                case "FJ":
                    return Country._fiji;
                case "FI":
                    return Country._finland;
                case "FR":
                    return Country._france;
                case "GF":
                    return Country._frenchGuiana;
                case "PF":
                    return Country._frenchPolynesia;
                case "TF":
                    return Country._frenchSouthernTerritories;
                case "GA":
                    return Country._gabon;
                case "GM":
                    return Country._gambia;
                case "GE":
                    return Country._georgia;
                case "DE":
                    return Country._germany;
                case "GH":
                    return Country._ghana;
                case "GI":
                    return Country._gibraltar;
                case "GR":
                    return Country._greece;
                case "GL":
                    return Country._greenland;
                case "GD":
                    return Country._grenada;
                case "GP":
                    return Country._guadeloupe;
                case "GU":
                    return Country._guam;
                case "GT":
                    return Country._guatemala;
                case "GG":
                    return Country._guernsey;
                case "GN":
                    return Country._guinea;
                case "GW":
                    return Country._guineaBissau;
                case "GY":
                    return Country._guyana;
                case "HT":
                    return Country._haiti;
                case "HM":
                    return Country._heardAndMcDonaldIslands;
                case "VA":
                    return Country._holySeeCityVaticanState;
                case "HN":
                    return Country._honduras;
                case "HU":
                    return Country._hungary;
                case "IS":
                    return Country._iceland;
                case "IN":
                    return Country._india;
                case "ID":
                    return Country._indonesia;
                case "IR":
                    return Country._iranIslamicRepublicOf;
                case "IQ":
                    return Country._iraq;
                case "IE":
                    return Country._ireland;
                case "IM":
                    return Country._isleOfMan;
                case "IL":
                    return Country._israel;
                case "IT":
                    return Country._italy;
                case "JM":
                    return Country._jamaica;
                case "JP":
                    return Country._japan;
                case "JE":
                    return Country._jersey;
                case "JO":
                    return Country._jordan;
                case "KZ":
                    return Country._kazakhstan;
                case "KE":
                    return Country._kenya;
                case "KI":
                    return Country._kiribati;
                case "KP":
                    return Country._koreaDemocraticPeoplesRepublic;
                case "KR":
                    return Country._koreaRepublicOf;
                case "KW":
                    return Country._kuwait;
                case "KG":
                    return Country._kyrgyzstan;
                case "LA":
                    return Country._laoPeoplesDemocraticRepublic;
                case "LV":
                    return Country._latvia;
                case "LB":
                    return Country._lebanon;
                case "LS":
                    return Country._lesotho;
                case "LR":
                    return Country._liberia;
                case "LY":
                    return Country._libya;
                case "LI":
                    return Country._liechtenstein;
                case "LT":
                    return Country._lithuania;
                case "LU":
                    return Country._luxembourg;
                case "MK":
                    return Country._macedonia;
                case "MG":
                    return Country._madagascar;
                case "MW":
                    return Country._malawi;
                case "MY":
                    return Country._malaysia;
                case "MV":
                    return Country._maldives;
                case "ML":
                    return Country._mali;
                case "MT":
                    return Country._malta;
                case "MH":
                    return Country._marshallIslands;
                case "MQ":
                    return Country._martinique;
                case "MR":
                    return Country._mauritania;
                case "MU":
                    return Country._mauritius;
                case "YT":
                    return Country._mayotte;
                case "MX":
                    return Country._mexico;
                case "FM":
                    return Country._micronesiaFederalStateOf;
                case "MD":
                    return Country._moldovaRepublicOf;
                case "MC":
                    return Country._monaco;
                case "MN":
                    return Country._mongolia;
                case "ME":
                    return Country._montenegro;
                case "MS":
                    return Country._montserrat;
                case "MA":
                    return Country._morocco;
                case "MZ":
                    return Country._mozambique;
                case "MM":
                    return Country._myanmar;
                case "NA":
                    return Country._namibia;
                case "NR":
                    return Country._nauru;
                case "NP":
                    return Country._nepal;
                case "NL":
                    return Country._netherlands;
                case "NC":
                    return Country._newCaledonia;
                case "NZ":
                    return Country._newZealand;
                case "NI":
                    return Country._nicaragua;
                case "NE":
                    return Country._niger;
                case "NG":
                    return Country._nigeria;
                case "NU":
                    return Country._niue;
                case "NF":
                    return Country._norfolkIsland;
                case "MP":
                    return Country._northernMarianaIslands;
                case "NO":
                    return Country._norway;
                case "OM":
                    return Country._oman;
                case "PK":
                    return Country._pakistan;
                case "PW":
                    return Country._palau;
                case "PS":
                    return Country._stateOfPalestine;
                case "PA":
                    return Country._panama;
                case "PG":
                    return Country._papuaNewGuinea;
                case "PY":
                    return Country._paraguay;
                case "PE":
                    return Country._peru;
                case "PH":
                    return Country._philippines;
                case "PN":
                    return Country._pitcairnIsland;
                case "PL":
                    return Country._poland;
                case "PT":
                    return Country._portugal;
                case "PR":
                    return Country._puertoRico;
                case "QA":
                    return Country._qatar;
                case "RE":
                    return Country._reunionIsland;
                case "RO":
                    return Country._romania;
                case "RU":
                    return Country._russianFederation;
                case "RW":
                    return Country._rwanda;
                case "BL":
                    return Country._saintBarthelemy;
                case "SH":
                    return Country._saintHelena;
                case "KN":
                    return Country._saintKittsAndNevis;
                case "LC":
                    return Country._saintLucia;
                case "MF":
                    return Country._saintMartin;
                case "VC":
                    return Country._saintVincentAndTheGrenadines;
                case "WS":
                    return Country._samoa;
                case "SM":
                    return Country._sanMarino;
                case "ST":
                    return Country._saoTomeAndPrincipe;
                case "SA":
                    return Country._saudiArabia;
                case "SN":
                    return Country._senegal;
                case "RS":
                    return Country._serbia;
                case "SC":
                    return Country._seychelles;
                case "SL":
                    return Country._sierraLeone;
                case "SG":
                    return Country._singapore;
                case "SK":
                    return Country._slovakRepublic;
                case "SI":
                    return Country._slovenia;
                case "SB":
                    return Country._solomonIslands;
                case "SO":
                    return Country._somalia;
                case "ZA":
                    return Country._southAfrica;
                case "GS":
                    return Country._southGeorgia;
                case "SS":
                    return Country._southSudan;
                case "ES":
                    return Country._spain;
                case "LK":
                    return Country._sriLanka;
                case "SD":
                    return Country._sudan;
                case "SR":
                    return Country._suriname;
                case "SJ":
                    return Country._svalbardAndJanMayenIslands;
                case "SZ":
                    return Country._swaziland;
                case "SE":
                    return Country._sweden;
                case "CH":
                    return Country._switzerland;
                case "SY":
                    return Country._syrianArabRepublic;
                case "TW":
                    return Country._taiwan;
                case "TJ":
                    return Country._tajikistan;
                case "TZ":
                    return Country._tanzania;
                case "TH":
                    return Country._thailand;
                case "TG":
                    return Country._togo;
                case "TK":
                    return Country._tokelau;
                case "TO":
                    return Country._tonga;
                case "TT":
                    return Country._trinidadAndTobago;
                case "TN":
                    return Country._tunisia;
                case "TR":
                    return Country._turkey;
                case "TM":
                    return Country._turkmenistan;
                case "TC":
                    return Country._turksAndCaicosIslands;
                case "TV":
                    return Country._tuvalu;
                case "UG":
                    return Country._uganda;
                case "UA":
                    return Country._ukraine;
                case "AE":
                    return Country._unitedArabEmirates;
                case "GB":
                    return Country._unitedKingdom;
                case "US":
                    return Country._unitedStates;
                case "UY":
                    return Country._uruguay;
                case "UZ":
                    return Country._uzbekistan;
                case "VU":
                    return Country._vanuatu;
                case "VE":
                    return Country._venezuela;
                case "VN":
                    return Country._vietnam;
                case "VI":
                    return Country._virginIslandsBritish;
                case "WF":
                    return Country._virginIslandsUSA;
                case "EH":
                    return Country._westernSahara;
                case "YE":
                    return Country._yemen;
                case "ZM":
                    return Country._zambia;
                case "ZW":
                    return Country._zimbabwe;
                default:
                    throw new Exception("CountryCode not handeled");

            }
        }
    }
}
