using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

#if UNITY_EDITOR
    /// <summary>
    /// Inspectorで値が変更されたときに自動で呼ばれる（エディタ専用）
    /// Keyの重複、null要素をチェックしてConsoleに警告を出す
    /// </summary>
    private void OnValidate()
    {
        if (Values == null) return;

        var seen = new Dictionary<K, int>();
        for (int i = 0; i < Values.Count; i++)
        {
            var value = Values[i];
            if (value == null)
            {
                Debug.LogWarning($"[{name}] Values[{i}] is null.", this);
                continue;
            }

            var key = value.Key;
            if (seen.TryGetValue(key, out int firstIndex))
            {
                Debug.LogError(
                    $"[{name}] Duplicate key '{key}' found at index {i} " +
                    $"(first used at index {firstIndex}). Fix the ID before saving.",
                    this);
            }
            else
            {
                seen[key] = i;
            }
        }
    }

    /// <summary>
    /// 現在使われていない最小のIDを取得する（int型のKeyのみ対応）
    /// エディタ拡張から呼び出して自動採番に使う
    /// </summary>
    public int GetNextAvailableIntId()
    {
        if (Values == null || Values.Count == 0) return 0;

        var usedIds = Values
            .Where(v => v != null)
            .Select(v => System.Convert.ToInt32(v.Key))
            .ToHashSet();

        int candidate = 0;
        while (usedIds.Contains(candidate))
        {
            candidate++;
        }
        return candidate;
    }
#endif
}
