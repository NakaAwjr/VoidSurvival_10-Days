using System.Collections.Generic;
using UnityEngine;

public class WorkbenchRecipeListUI : SlotContainer<WorkbenchRecipeButton>
{
    public WorkbenchRecipeListUI(RectTransform content, WorkbenchRecipeButton slotPrefab) : base(content, slotPrefab)
    {
    }
    public void Refresh(List<WorkbenchRecipe> recipes)
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