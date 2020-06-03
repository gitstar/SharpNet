using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> enumerable, Func<TSource, TKey> keySelector, bool descending)
    {
        if (enumerable == null)
        {
            return null;
        }

        if (descending)
        {
            return enumerable.OrderByDescending(keySelector);
        }

        return enumerable.OrderBy(keySelector);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> enumerable, Func<TSource, IComparable> keySelector1, Func<TSource, IComparable> keySelector2, params Func<TSource, IComparable>[] keySelectors)
    {
        if (enumerable == null)
        {
            return null;
        }

        IEnumerable<TSource> current = enumerable;

        if (keySelectors != null)
        {
            for (int i = keySelectors.Length - 1; i >= 0; i--)
            {
                current = current.OrderBy(keySelectors[i]);
            }
        }

        current = current.OrderBy(keySelector2);

        return current.OrderBy(keySelector1);
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> enumerable, bool descending, Func<TSource, IComparable> keySelector, params Func<TSource, IComparable>[] keySelectors)
    {
        if (enumerable == null)
        {
            return null;
        }

        IEnumerable<TSource> current = enumerable;

        if (keySelectors != null)
        {
            for (int i = keySelectors.Length - 1; i >= 0; i--)
            {
                current = current.OrderBy(keySelectors[i], descending);
            }
        }

        return current.OrderBy(keySelector, descending);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> knownKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (knownKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    public static List<T> EnumerableToList<T>(this IEnumerable<T> source)
    {
        if (source == null)
            return new List<T>();

        return new List<T>(source);
    }
}
