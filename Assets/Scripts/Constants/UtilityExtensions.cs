using System;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityExtensions
{
    // for shuffling lists
    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    /*
    // json helpers
    // for getting a top-level json array
    public static T[] FromJson<T>(string jsonArray)
    {
        jsonArray = WrapArray(jsonArray);
        return FromJsonWrapped<T>(jsonArray);
    }

    public static T[] FromJsonWrapped<T>(string jsonObject)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonObject);
        return wrapper.items;
    }

    private static string WrapArray(string jsonArray)
    {
        return "{ \"items\": " + jsonArray + "}";
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }*/


}