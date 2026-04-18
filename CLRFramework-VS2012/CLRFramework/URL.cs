using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

public partial class WebUtility : Utility
{
    public class URL
    {
        public static string RemovePaging(string URL)
        {
            Regex objReg = null;
            NameValueCollection objQuery = null;
            bool blnMatch = false;
            string strUrlPath = string.Empty;
            string strUrlQuery = string.Empty;
            string strReturn = string.Empty;

            strUrlPath = URL;
            foreach (char chr in new char[] { '?', '#' })
            {
                int intCharIndex = URL.IndexOf(chr);
                if (intCharIndex >= 0) strUrlPath = strUrlPath.Substring(0, intCharIndex);
            }
            strUrlQuery = URL.Replace(strUrlPath, "");

            objReg = new Regex(@"(.*)/page/(\d+)/(.+)");
            blnMatch = objReg.IsMatch(strUrlPath);
            if (blnMatch)
            {
                strUrlPath = objReg.Replace(strUrlPath, "$1/$3");
            }

            if (!string.IsNullOrEmpty(strUrlQuery))
            {
                objQuery = System.Web.HttpUtility.ParseQueryString(strUrlQuery);
                if (objQuery != null)
                {
                    foreach (string strKey in objQuery.AllKeys)
                    {
                        if (strKey.ToLower() == "page") objQuery.Remove(strKey);
                    }
                }
            }

            if (objQuery != null && objQuery.Count > 0)
                strReturn = strUrlPath + "?" + objQuery.ToString();
            else
                strReturn = strUrlPath;

            return strReturn;
        }

        public static NameValueCollection QueryStringRaw
        {
            get
            {
                NameValueCollection objReturn = new NameValueCollection();
                if (RawUrlNoQuery.ToLower() != PhysicalUrlNoQuery.ToLower()) objReturn = System.Web.HttpUtility.ParseQueryString(new Uri(AbsoluteRawUrlAndQuery).Query);
                return objReturn;
            }
        }

        public static NameValueCollection QueryStringPhysical
        {
            get
            {
                return HttpContext.Current.Request.QueryString;
            }
        }

        public static NameValueCollection QueryStringRawAndPhysical
        {
            get
            {
                NameValueCollection objReturn = System.Web.HttpUtility.ParseQueryString(QueryStringPhysical.ToString());
                for (int i = 0; i < QueryStringRaw.Keys.Count; i++)
                {
                    objReturn[QueryStringRaw.Keys[i]] = QueryStringRaw[QueryStringRaw.Keys[i]];
                }
                return objReturn;
            }
        }

        public static string RawUrlNoQuery
        {
            get
            {
                string strRawUrl = RawUrlAndQuery;
                foreach (char chr in new char[] { '?', '#' })
                {
                    int intCharIndex = strRawUrl.IndexOf(chr);
                    if (intCharIndex >= 0) strRawUrl = strRawUrl.Substring(0, intCharIndex);
                }
                return strRawUrl;
            }
        }

        public static string RawUrlAndQuery
        {
            get
            {
                return HttpContext.Current.Request.RawUrl.ToString();
            }
        }

        public static string PhysicalUrlNoQuery
        {
            get
            {
                return HttpContext.Current.Request.Url.AbsolutePath;
            }
        }

        public static string PhysicalUrlAndQuery
        {
            get
            {
                return HttpContext.Current.Request.Url.PathAndQuery;
            }
        }

        public static string AbsoluteRawUrlNoQuery
        {
            get
            {
                return AbsoluteDomain + RawUrlNoQuery;
            }
        }

        public static string AbsoluteRawUrlAndQuery
        {
            get
            {
                return AbsoluteDomain + HttpContext.Current.Request.RawUrl.ToString();
            }
        }

        public static string AbsolutePhysicalUrlNoQuery
        {
            get
            {
                return AbsoluteDomain + PhysicalUrlNoQuery;
            }
        }

        public static string AbsolutePhysicalUrlAndQuery
        {
            get
            {
                return AbsoluteDomain + PhysicalUrlAndQuery;
            }
        }

        public static string AbsoluteDomain
        {
            get
            {
                return HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpUtility.UrlDecode(HttpContext.Current.Request.Url.PathAndQuery), "");
            }
        }
    }
}