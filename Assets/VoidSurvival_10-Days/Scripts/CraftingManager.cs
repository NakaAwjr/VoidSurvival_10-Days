using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }
    private CraftingRecipeDatabase _craftingRecipeDatabase;
    public List<CraftingRecipe> craftingRecipes { get; private set; } // クラフト可能なレシピのリスト

    private CraftingManager(CraftingRecipeDatabase craftingRecipeDatabase)
    {
        _craftingRecipeDatabase = craftingRecipeDatabase;
        craftingRecipes = new List<CraftingRecipe>();
    }
    public static void Initialize(CraftingRecipeDatabase craftingRecipeDatabase)
    {
        if (Instance == null)
        {
            Instance = new CraftingManager(craftingRecipeDatabase);
        }
        else
        {
            Debug.LogWarning("CraftingManager instance already exists. Reinitializing.");
        }
    }

    /// <summary>
    /// 指定されたレシピに基づいてクラフト可能なアイテムの数を計算する
    /// </summary>
    /// <param name="recipe"></param>
    /// <returns></returns>
    public int NumberCanCrafting(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("Crafting recipe is null.");
            return 0;
        }

        int minCount = int.MaxValue;

        foreach (var requirement in recipe.RequiredItems)
        {
            var itemStack = ItemManager.Instance.GetItemStack(requirement.Item);
            if (itemStack == null || itemStack.Amount < requirement.Amount)
            {
                return 0;
            }
            minCount = Mathf.Min(minCount, itemStack.Amount / requirement.Amount);
        }

        return minCount;
    }
    /// <summary>
    /// 指定されたレシピに基づいてアイテムをクラフトする
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="count"></param>
    public void CraftItem(CraftingRecipe recipe, int count)
    {
        if (recipe == null || count <= 0)
        {
            Debug.LogError("Invalid crafting recipe or count.");
            return;
        }

        if (NumberCanCrafting(recipe) < count)
        {
            Debug.LogWarning("Not enough resources to craft the item.");
            return;
        }

        // アイテムを消費
        foreach (var requirement in recipe.RequiredItems)
        {
            ItemManager.Instance.RemoveItem(requirement.Item, requirement.Amount * count);
        }

        // 結果のアイテムを追加
        ItemManager.Instance.AddItem(recipe.ResultItem, recipe.ResultAmount * count);
    }
    /// <summary>
    /// クラフト可能なレシピを追加する
    /// </summary>
    /// <param name="recipe"></param>
    public void AddRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("Cannot add a null recipe.");
            return;
        }

        if (craftingRecipes == null)
        {
            craftingRecipes = new List<CraftingRecipe>();
        }

        if (!craftingRecipes.Contains(recipe))
        {
            craftingRecipes.Add(recipe);
        }
        else
        {
            Debug.LogWarning("Recipe already exists in the crafting manager.");
        }
    }

    public void FromSaveData(RecipeSaveData saveData)
    {
        if (saveData == null || saveData.Recipes == null)
        {
            Debug.LogError("Invalid crafting save data.");
            return;
        }

        craftingRecipes = new List<CraftingRecipe>();
        foreach (var recipeId in saveData.Recipes)
        {
            var recipe = _craftingRecipeDatabase.GetValue(recipeId);
            if (recipe != null)
            {
                craftingRecipes.Add(recipe);
            }
            else
            {
                Debug.LogWarning($"Crafting recipe with ID {recipeId} not found in database.");
            }
        }
    }
    public RecipeSaveData ToSaveData()
    {
        var saveData = new RecipeSaveData();
        saveData.Recipes = new List<int>();

        foreach (var recipe in craftingRecipes)
        {
            saveData.Recipes.Add(recipe.RecipeID);
        }

        return saveData;
    }
}
