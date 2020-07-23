using UnityEngine;
using System.Collections;

public class BoundedValue<T> where T : System.IComparable
{
    public BoundedValue(T value, T minValue, T maxValue)
    {
        this.value = value;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    T value;
    public T MinValue;
    public T MaxValue;

    public T Value
    {
        get => value;
        set
        {
           if (MinValue.CompareTo(value) > 0)
            {
                this.value = MinValue;
            } else if (MaxValue.CompareTo(value) < 0) {
                this.value = MaxValue;
            } else
            {
                this.value = value;
            }

        }
    }

    public static implicit operator T (BoundedValue<T> t) => t.Value;
}
