using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// シリアライズ化される簡易辞書
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[System.Serializable]
public class SerializableDictionary<TKey, TValue>
{
    public List<KeyValuePair<TKey, TValue>> keyValuePairs = new List<KeyValuePair<TKey, TValue>>();
    public void Add(TKey key, TValue value)
    {
        keyValuePairs.Add(new KeyValuePair<TKey, TValue>(key, value));
    }
    public TValue GetValue(TKey key)
    {
        return keyValuePairs.FirstOrDefault(pair => pair.Key.Equals(key)).Value;
    }
    public bool ContainsKey(TKey key)
    {
        foreach (KeyValuePair<TKey, TValue> pair in keyValuePairs)
        {
            if (Equals(pair.Key, key))
            {
                return true;
            }
        }
        return false;
    }
    public void SetValue(TKey key, TValue value)
    {
        keyValuePairs.FirstOrDefault(pair => pair.Key.Equals(key))?.SetValue(value);
    }
}

[System.Serializable]
public class KeyValuePair<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
    public KeyValuePair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    public void SetValue(TValue value)
    {
        Value = value;
    }
}