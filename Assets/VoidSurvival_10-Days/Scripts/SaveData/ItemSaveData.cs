using System.Collections.Generic;

/// <summary>
/// アイテム関連のセーブデータ、アイテムをIDにして管理
/// IDが-1はnullを表すので、アイテムIDに-1を設定しないように
/// </summary>
[System.Serializable]
public struct ItemSaveData
{
    public SerializableDictionary<int, int> Inventory;
    public int[] QuickItems;
    public int SelectedQuickItemIndex;
    public SerializableDictionary<ItemManager.EquipmentType, int> EquipmentSlot;
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