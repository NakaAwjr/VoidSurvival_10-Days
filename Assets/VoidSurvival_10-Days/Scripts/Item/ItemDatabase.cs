using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> Items; // アイテムのリスト

    /// <summary>
    /// アイテムIDからアイテムを取得する
    /// </summary>
    /// <param name="itemId">アイテムのID</param>
    /// <returns>対応するアイテム</returns>
    public Item GetItem(int itemId)
    {
        return Items.Find(item => item.ItemID == itemId);
    }
}
