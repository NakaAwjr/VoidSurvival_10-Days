using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Items/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject, IDatabaseEntry<int>
{
    int IDatabaseEntry<int>.Key => RecipeID; // IDatabaseEntryインターフェースの実装
    public int RecipeID; // レシピのID
    public string RecipeName; // レシピの名前
    public string RecipeDescription; // レシピの説明
    public Item ResultItem; // このレシピで作成されるアイテム
    public int ResultAmount; // 作成されるアイテムの数量
    public List<ItemStack> RequiredItems; // このレシピで必要なアイテムのリスト
}