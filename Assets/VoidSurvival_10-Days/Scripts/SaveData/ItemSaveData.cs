using System.Collections.Generic;

/// <summary>
/// アイテム関連のセーブデータ、アイテムをIDにして管理
/// IDが-1はnullを表すので、アイテムIDに-1を設定しないように
/// </summary>
[System.Serializable]
public class ItemSaveData
{
    public List<ItemStackSaveData> Inventory;
    public int[] QuickItems = new int[ItemManager.QUICK_ITEM_COUNT] { -1, -1, -1, -1 };
    public int SelectedQuickItemIndex;
    public List<EquipmentSlotSaveData> EquipmentSlot;
}

[System.Serializable]
public struct ItemStackSaveData
{
    public int ItemID;
    public int Amount;
}

[System.Serializable]
public struct EquipmentSlotSaveData
{
    public ItemManager.EquipmentType Type;
    public int ItemID;
}