using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの初期化を行うコンポーネント
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private ISaveService _saveService = new LocalSaveService();

    [SerializeField] private string scene;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private CraftingRecipeDatabase recipeDatabase;
    /// <summary>
    /// 初期インベントリを設定
    /// </summary>
    [SerializeField] private List<ItemStack> initialInventory = new List<ItemStack>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        ItemManager.Initialize(itemDatabase, initialInventory);
        CraftingManager.Initialize(recipeDatabase);
    }

    public async void LoadGameAsync(string key)
    {
        Debug.Log("ロード中");
        AsyncOperation _loadOp = SceneManager.LoadSceneAsync(scene);
        _loadOp.allowSceneActivation = false;
        SaveData _data = await _saveService.LoadAsync(key);
        if (_data != null)
        {
            ItemManager.Instance.FromSaveData(_data.ItemData);
            CraftingManager.Instance.FromSaveData(_data.RecipeData);
        }
        _loadOp.allowSceneActivation = true;
        Debug.Log("ロード完了");
    }

    public async void SaveGameAsync(string key)
    {
        SaveData _data = new SaveData();
        _data.ItemData = ItemManager.Instance.ToSaveData();
        _data.RecipeData = CraftingManager.Instance.ToSaveData();
        await _saveService.SaveAsync(key, _data);
    }
}
