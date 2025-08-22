using System.Collections.Generic;

public class ItemSaveData
{
    public Dictionary<int, int> Inventory;
    public int[] QuickItems = new int[ItemManager.QUICK_ITEM_COUNT] { -1, -1, -1, -1 };
    public int SelectedQuickItemIndex;
    public Dictionary<ItemManager.EquipmentSlot, int> EquipmentSlots;
}
