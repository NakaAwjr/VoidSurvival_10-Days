using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitData", menuName = "InitData")]
public class InitData : ScriptableObject
{
    [Header("初期インベントリ")]
    public List<ItemStack> initialInventory = new List<ItemStack>();
    [Header("初期レシピ")]
    public List<CraftingRecipe> initialRecipes = new List<CraftingRecipe>();
}