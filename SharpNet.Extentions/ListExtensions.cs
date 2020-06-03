using System;
using System.Collections.Generic;
using System.Text;

public static class ListExtensions
{
    public static string Join<T>(this IEnumerable<T> list, string separator)
    {
        StringBuilder sb = new StringBuilder();
        foreach (object x in list)
        {
            sb.Append(x.ToString() + separator);
        }
     
        return (sb.Length > separator.Length) ?
            sb.Remove(sb.Length - separator.Length, separator.Length).ToString() : String.Empty;
    }

    public static T MaxObject<T, U>(this List<T> source, Func<T, U> selector) where U : IComparable<U>
    {
        if (source == null)
            throw new ArgumentNullException("source");

        bool first = true;
        T maxObj = default(T);
        U maxKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                maxObj = item;
                maxKey = selector(maxObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxObj = item;
                }
            }
        }
        if (first)
            throw new InvalidOperationException("Sequence is empty.");

        return maxObj;
    }

    public static T MinObject<T, U>(this List<T> source, Func<T, U> selector) where U : IComparable<U>
    {
        if (source == null)
            throw new ArgumentNullException("source");

        bool first = true;
        T minObj = default(T);
        U minKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                minObj = item;
                minKey = selector(minObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(minKey) < 0)
                {
                    minKey = currentKey;
                    minObj = item;
                }
            }
        }

        if (first)
            throw new InvalidOperationException("Sequence is empty.");

        return minObj;
    }    
}
