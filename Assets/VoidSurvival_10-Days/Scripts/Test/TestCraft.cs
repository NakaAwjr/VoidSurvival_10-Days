using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCraft : MonoBehaviour
{
    [SerializeField] private CraftingRecipeDatabase craftingRecipeDatabase;
    [SerializeField] private ItemDatabase itemDatabase;
    // Start is called before the first frame update
    void Start()
    {
        CraftingManager.Initialize(craftingRecipeDatabase);
        CraftingManager.Instance.AddRecipe(craftingRecipeDatabase.GetValue(0)); // 例としてID 0のレシピを追加
        ItemManager.Initialize(itemDatabase);
        Item item = itemDatabase.GetValue(0); // ID 0のアイテムを取得
        ItemManager.Instance.AddItem(item, 10); // そのアイテムを10個追加
        Debug.Log("Inventory before crafting: " + ItemManager.Instance.GetItemStack(item)?.Amount);
        int canCraftCount = CraftingManager.Instance.NumberCanCrafting(craftingRecipeDatabase.GetValue(0));
        Debug.Log($"Can craft {canCraftCount} items from recipe 0.");
        CraftingManager.Instance.CraftItem(craftingRecipeDatabase.GetValue(0), canCraftCount);
        Debug.Log($"Crafted {canCraftCount} items from recipe 0.");
        Debug.Log("Inventory after crafting: " + ItemManager.Instance.GetItemStack(item)?.Amount);
    }
}
