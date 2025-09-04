using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDatabase<T, K> : ScriptableObject where T : class, IDatabaseEntry<K>
{
    [SerializeReference] public List<T> Values;

    /// <summary>
    /// データベースから値を取得する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T GetValue(K id)
    {
        foreach (var value in Values)
        {
            if (EqualityComparer<K>.Default.Equals(value.Key, id))
            {
                return value;
            }
        }
        Debug.LogWarning($"Value with key {id} not found in database.");
        return null;
    }
}
