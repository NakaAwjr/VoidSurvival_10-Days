using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Keyがintのデータベース（ItemDatabase, CraftingRecipeDatabaseなど）向けの共通エディタ拡張。
/// 「IDが0のまま/未設定の要素」に、使われていない最小のIDを自動で割り当てるボタンを追加する。
///
/// 使い方：
/// ItemDatabase.cs や CraftingRecipeDatabase.cs と同じように、
/// [CustomEditor(typeof(ItemDatabase))] を付けた小さいクラスをEditorフォルダに作り、
/// このクラスを継承するだけでよい。
///
/// 例:
/// [CustomEditor(typeof(ItemDatabase))]
/// public class ItemDatabaseEditor : IntKeyDatabaseEditor { }
///
/// [CustomEditor(typeof(CraftingRecipeDatabase))]
/// public class CraftingRecipeDatabaseEditor : IntKeyDatabaseEditor { }
/// </summary>
public abstract class IntKeyDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (GUILayout.Button("IDが未設定(0)の要素に自動でIDを割り当てる"))
        {
            AssignMissingIds();
        }
    }

    private void AssignMissingIds()
    {
        var valuesField = target.GetType().BaseType?.GetField("Values", BindingFlags.Public | BindingFlags.Instance);
        if (valuesField == null)
        {
            Debug.LogError("Values フィールドが見つかりませんでした。");
            return;
        }

        var values = valuesField.GetValue(target) as System.Collections.IList;
        if (values == null || values.Count == 0)
        {
            Debug.LogWarning("データベースが空です。");
            return;
        }

        // 各要素からIDを格納しているintフィールドを探す（例: ItemID, RecipeID）
        // 先頭要素の型を基準に、名前が "ID" で終わるpublic intフィールドを探す
        var elementType = values[0]?.GetType();
        if (elementType == null)
        {
            Debug.LogWarning("要素の型を特定できませんでした。");
            return;
        }

        var idField = elementType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(int) && f.Name.EndsWith("ID"));

        if (idField == null)
        {
            Debug.LogError($"{elementType.Name} に 'XxxID' という名前のintフィールドが見つかりませんでした。");
            return;
        }

        // 使用済みIDを収集
        var usedIds = new System.Collections.Generic.HashSet<int>();
        foreach (var v in values)
        {
            if (v == null) continue;
            usedIds.Add((int)idField.GetValue(v));
        }

        int nextId = 0;
        int assignedCount = 0;

        foreach (var v in values)
        {
            if (v == null) continue;
            int currentId = (int)idField.GetValue(v);
            if (currentId != 0) continue; // 0以外(=設定済み)はスキップ

            while (usedIds.Contains(nextId))
            {
                nextId++;
            }
            idField.SetValue(v, nextId);
            usedIds.Add(nextId);
            assignedCount++;
            nextId++;
        }

        if (assignedCount > 0)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            Debug.Log($"{assignedCount} 件の要素にIDを割り当てました。");
        }
        else
        {
            Debug.Log("IDが未設定(0)の要素はありませんでした。");
        }
    }
}