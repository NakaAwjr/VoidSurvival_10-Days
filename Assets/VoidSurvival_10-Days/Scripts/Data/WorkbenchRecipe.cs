using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkbenchRecipe", menuName = "Items/WorkbenchRecipe")]
public class WorkbenchRecipe : ScriptableObject, IDatabaseEntry<int>
{
    public int Key => RecipeID; // IDatabaseEntryインターフェースの実装
    public int RecipeID;
    public string RecipeName; // レシピの名前
    public string RecipeDescription; // レシピの説明
    public Item ResultItem; // このレシピで作成されるアイテム
    public int ResultAmount; // 作成されるアイテムの数量
    public ItemStack RequiredItem; // このレシピで必要なアイテム
    public float time; // かかる時間
}
