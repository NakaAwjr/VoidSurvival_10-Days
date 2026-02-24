using System.Collections.Generic;
using UnityEngine;

public class RecipeListUI : SlotContainer<RecipeButton>
{
    public RecipeListUI(RectTransform content, RecipeButton slotPrefab) : base(content, slotPrefab)
    {
    }
    public void Refresh(List<CraftingRecipe> recipes)
    {
        EnsureSlotCount(recipes.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < recipes.Count)
                slots[i].SetRecipe(recipes[i]);
            else
                slots[i].Clear();
        }
    }
}

public class RequiredItemListUI : SlotContainer<ItemButton>
{
    public RequiredItemListUI(RectTransform content, ItemButton slotPrefab) : base(content, slotPrefab)
    {
    }
    public void Refresh(List<ItemStack> requiredItems, int craftCount)
    {
        EnsureSlotCount(requiredItems.Count);

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < requiredItems.Count)
            {
                var required = requiredItems[i];
                slots[i].SetItem(new ItemStack(required.Item, required.Amount * craftCount));
            }
            else
            {
                slots[i].Clear();
            }
        }
    }
}