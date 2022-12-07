using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GPRandomExtensions
{
    public static T NextEnum<T>(this System.Random random)
    {
        var values = System.Enum.GetValues(typeof(T));
        return (T)values.GetValue(random.Next(values.Length));
    }
}
