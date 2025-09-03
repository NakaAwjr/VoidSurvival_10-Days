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
    public EquipmentSlotSaveData EquipmentSlot = new EquipmentSlotSaveData()
    {
        Head = -1,
        Body = -1,
        Legs = -1
    };
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
    public int Head;
    public int Body;
    public int Legs;
}