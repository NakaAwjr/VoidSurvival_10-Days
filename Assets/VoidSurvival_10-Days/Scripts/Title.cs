using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private CraftingRecipeDatabase recipeDatabase;
    [SerializeField] private WorkbenchRecipeDataBase workbenchRecipeDatabase;
    [SerializeField] private SaveDialog saveDialog;
    [SerializeField] private GameObject player;

    /// <summary>
    /// 初期インベントリを設定
    /// </summary>
    [SerializeField] private List<ItemStack> initialInventory = new List<ItemStack>();
    /// <summary>
    /// 初期レシピを設定
    /// </summary>
    [SerializeField] private List<CraftingRecipe> initialRecipes = new List<CraftingRecipe>();

    private void Awake()
    {
        // フレームレート設定
        Application.targetFrameRate = 61;
        // Initialize
        ItemManager.Initialize(itemDatabase, initialInventory);
        CraftingManager.Initialize(recipeDatabase, initialRecipes);
        WorkbenchManager.Initialize(itemDatabase, workbenchRecipeDatabase);
    }
    private void Start()
    {
        player.SetActive(false);
        MainUI.Instance.CloseMainUI();
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            saveDialog.OpenDialog();
        }
    }

    /// <summary>
    /// タイトル経由のデータロード
    /// </summary>
    public void FirstLoadGame()
    {
        saveDialog.Load();
        Debug.Log("Load Game");
        player.SetActive(true);
        MainUI.Instance.OpenMainUI();
    }
}
