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

    public static List<T> ShuffleList<T>(this List<T> list)
    {
        var random = new System.Random();
        var newShuffledList = new List<T>();
        var listcCount = list.Count;
        for (int i = 0; i < listcCount; i++)
        {
            var randomElementInList = random.Next(0, list.Count);
            newShuffledList.Add(list[randomElementInList]);
            list.Remove(list[randomElementInList]);
        }
        return newShuffledList;
    }
}
