using UnityEngine;

/// <summary>
/// アイテムの基本クラス
/// このクラスはScriptableObjectを継承しており、アイテムの基本的な情報を定義します。
/// アイテムの種類ごとにこのクラスを継承して、具体的なアイテムの動作を実装します。
/// </summary>
public abstract class Item : ScriptableObject, IDatabaseEntry<int>
{
    int IDatabaseEntry<int>.Key => ItemID; // IDatabaseEntryインターフェースの実装
    public int ItemID; // アイテムのID
    public string ItemName; // アイテムの名前
    public string ItemDescription; // アイテムの説明
    public Sprite ItemIcon; // アイテムのアイコン
    /// <summary>
    /// アイテムを使用できるかどうかの判定
    /// </summary>
    /// <returns></returns>
    public abstract bool CanUseOn();
    /// <summary>
    /// アイテムを使用する処理
    /// </summary>
    public abstract void Use(GameObject player); // アイテムを使用する処理
}
