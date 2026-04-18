using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for GenericComparer
/// </summary>
public class GenericComparer<T> : IComparer<T>
{
    private SortDirection _sortDirection;
    private string _sortExpression;

    public SortDirection SortDirection
    {
        get { return _sortDirection; }
        set { _sortDirection = value; }
    }

    public GenericComparer(string sortExpression, string SortDirectionID)
    {
        _sortExpression = sortExpression;
        _sortDirection = GetSortDirection(SortDirectionID);
    }

    public GenericComparer(string sortExpression, SortDirection sortDirection)
    {
        _sortExpression = sortExpression;
        _sortDirection = sortDirection;
    }

    private SortDirection GetSortDirection(string SortDirectionID)
    {
        if (HttpContext.Current.Session[SortDirectionID] == null || (SortDirection)HttpContext.Current.Session[SortDirectionID] == SortDirection.Ascending)
        {
            HttpContext.Current.Session[SortDirectionID] = SortDirection.Descending;
            return SortDirection.Ascending;
        }
        else
        {
            HttpContext.Current.Session[SortDirectionID] = SortDirection.Ascending;
            return SortDirection.Descending;
        }
    }

    public int Compare(T x, T y)
    {
        PropertyInfo propertyInfo = typeof(T).GetProperty(_sortExpression);
        IComparable obj1 = (IComparable)propertyInfo.GetValue(x, null);
        IComparable obj2 = (IComparable)propertyInfo.GetValue(y, null);

        if (_sortDirection == SortDirection.Ascending)
        {
            if (obj1 == null && obj2 == null)
                return 0;
            else if (obj1 == null)
                return -1;
            if (obj2 == null)
                return 1;
            else
                return obj1.CompareTo(obj2);
        }
        else
        {
            if (obj1 == null && obj2 == null)
                return 0;
            else if (obj2 == null)
                return -1;
            else if (obj1 == null)
                return 1;
            else
                return obj2.CompareTo(obj1);
        }
    }

    public static List<T> SortCollection(List<T> collection, string sortExpression)
    {
        string[] exp = sortExpression.Split(" ".ToCharArray());

        string prop = exp[0];
        string dir;

        GenericComparer<T> comp;

        if (exp.Length == 2)
        {
            dir = exp[1];
        }
        else
        {
            dir = "ASC";
        }

        if (dir == "ASC")
        {
            comp = new GenericComparer<T>(exp[0], SortDirection.Ascending);
        }
        else
        {
            comp = new GenericComparer<T>(exp[0], SortDirection.Descending);
        }

        collection.Sort(comp);

        return collection;
    }
}
