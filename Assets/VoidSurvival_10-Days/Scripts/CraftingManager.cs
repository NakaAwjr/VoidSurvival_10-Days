using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CraftingManager
{
    private static CraftingManager _instance;
    public static CraftingManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new CraftingManager();
            return _instance;
        }
    }
    private CraftingRecipeDatabase _craftingRecipeDatabase;
    public List<CraftingRecipe> craftingRecipes { get; private set; } // クラフト可能なレシピのリスト

    /// <summary>
    /// Craftingrecipesが変更されたときに発火するイベント
    /// </summary>
    public UnityEvent OnChanged = new UnityEvent();

    private CraftingManager()
    {
        _craftingRecipeDatabase = Resources.Load<CraftingRecipeDatabase>("CraftingRecipeDatabase");
        List<CraftingRecipe> _initialRecipes = Resources.Load<InitData>("InitData")?.initialRecipes;
        if (_initialRecipes != null)
        {
            craftingRecipes = new List<CraftingRecipe>(_initialRecipes);
        }
        else
        {
            craftingRecipes = new List<CraftingRecipe>();
        }
    }
    // public static void Initialize()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = new CraftingManager();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("CraftingManager instance already exists. Reinitializing.");
    //     }
    // }

    #region Methods
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
        OnChanged?.Invoke();
    }
    #endregion

    #region Save
    public void FromSaveData(RecipeSaveData saveData)
    {
        if (saveData.Recipes == null)
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
    #endregion
}
