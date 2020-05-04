using System;
using System.Collections.Generic;
using System.Linq;

// from https://stackoverflow.com/questions/972307/can-you-loop-through-all-enum-values
public static class EnumUtil
{
    public static IEnumerable<T> values<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static int length<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }

    public static T[] toArray<T>() {

        return Enum.GetValues(typeof(T)) as T[];   
    }
}