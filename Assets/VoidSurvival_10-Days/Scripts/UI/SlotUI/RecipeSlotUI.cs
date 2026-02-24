public abstract class RecipeSlotUI : SlotUIBase
{
    public abstract void SetRecipe(CraftingRecipe recipe);
}

public abstract class WorkbenchRecipeSlotUI : SlotUIBase
{
    public abstract void SetRecipe(WorkbenchRecipe recipe);
}