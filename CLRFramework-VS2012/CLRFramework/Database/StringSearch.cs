using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class Database
{
    public partial class Filter
    {
        public class StringSearch
        {
            public class SearchFilter
            {
                public string SearchString { get; set; }
                public SearchOperator? Operator { get; set; }
            }

            public enum SearchOperator
            {
                notEmpty,
                empty,
                notContain
            }

            public static SearchFilter GetSearchFilter(string StringToSet)
            {
                SearchFilter objReturn = new SearchFilter();

                try
                {
                    objReturn.SearchString = StringToSet;
                }
                catch
                {
                    objReturn = null;
                }
                return objReturn;
            }

            public static string GetSQLQuery(SearchFilter SearchFilter, string SQLFieldNme)
            {
                string strReturn = string.Empty;

                if (SearchFilter != null)
                {
                    if (SearchFilter.Operator != null)
                    {
                        switch (SearchFilter.Operator.Value)
                        {
                            case SearchOperator.empty:
                                strReturn = "AND " + SQLFieldNme + " IS NULL ";
                                break;
                            case SearchOperator.notEmpty:
                                strReturn = "AND " + SQLFieldNme + " IS NOT NULL ";
                                break;
                            case SearchOperator.notContain:
                                strReturn = "AND " + SQLFieldNme + " NOT LIKE " + Database.HandleQuote(SearchFilter.SearchString.Replace("*", "%"));
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(SearchFilter.SearchString))
                        {
                            if (SearchFilter.SearchString.Contains("*"))
                            {
                                strReturn += "AND " + SQLFieldNme + " LIKE " + Database.HandleQuote(SearchFilter.SearchString.Replace("*", "%"));
                            }
                            else
                            {
                                strReturn += "AND " + SQLFieldNme + " = " + Database.HandleQuote(SearchFilter.SearchString);
                            }
                        }
                    }
                }
                return strReturn;
            }
        }
    }
}