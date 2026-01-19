using System.Collections.Generic;
using UnityEngine;

public class InventoryListUI : SlotContainer<InventoryItemButton>
{
    public InventoryListUI(RectTransform content, InventoryItemButton slotPrefab) : base(content, slotPrefab)
    {
    }
    public void Refresh(List<ItemStack> inventory)
    {
        EnsureSlotCount(inventory.Count);
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.Count) slots[i].SetItem(inventory[i]);
            else slots[i].Clear();
        }
    }
}