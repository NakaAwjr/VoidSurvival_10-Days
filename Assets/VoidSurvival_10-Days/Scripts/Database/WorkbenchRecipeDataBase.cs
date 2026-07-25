using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkbenchRecipeDataBase", menuName = "Database/WorkbenchRecipeDataBase")]
public class WorkbenchRecipeDataBase : BaseDatabase<WorkbenchRecipe, int>
{
    /// <summary>
    /// 入力した要求アイテムから作られるレシピのすべてを取得
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<WorkbenchRecipe> GetRecipes(Item item)
    {
        List<WorkbenchRecipe> recipes = new List<WorkbenchRecipe>();
        foreach (var value in Values)
        {
            if (EqualityComparer<Item>.Default.Equals(value.RequiredItem.Item, item))
            {
                recipes.Add(value);
            }
        }
        return recipes;
    }
}
