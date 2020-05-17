using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public class CollectionUtils
{

    public static string Print<E>(IEnumerable<E> enumerable)
    {
       
        string str = "[";
        foreach (E e in enumerable)
        {
            if (e == null)
            {
                str += "null";
            }
            else
            {
                str += e.ToString();
            }
            str += ", ";
        }

        if (str.Length == 1)
        {
            return "[]";
        }
        else
        {
            str = str.Substring(0, str.Length - 1) + "]";
        }
        return str;
    }

    public static ICollection<D> CastCopy<S, D>(ICollection<S> source) where D : S
    {
        List<D> copied = new List<D>();
        foreach (S item in source)
        {
            if (item is D)
            {
                copied.Add((D)item);
            }
        }
        return copied;
    }

    public static T first<T>(IEnumerable<T> collection)
    {
        IEnumerator<T> enumerator = collection.GetEnumerator();
        T t = default(T);
        if (enumerator.MoveNext())
        {
            t = enumerator.Current;
        }
        enumerator.Dispose();
        enumerator = null;
        return t;
    }

    public static int Count(IEnumerable enumerable)
    {
        int count = 0;
        foreach (var item in enumerable)
        {
            count++;
        }
        return count;
    }
}