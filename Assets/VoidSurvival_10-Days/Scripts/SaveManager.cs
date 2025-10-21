using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームデータのセーブ・ロードを扱う
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public const int SaveSlotNumber = 10;
    public const string SaveSlotKey = "SaveSlot";

    private ISaveService<SaveData> _saveService = new LocalSaveService<SaveData>();
    private ISaveService<SerializableDictionary<string, string>> _saveKeyService = new LocalSaveService<SerializableDictionary<string, string>>();

    /// <summary>
    /// 各セーブスロットの名前とセーブした時間を記録する
    /// </summary>
    public SerializableDictionary<string, string> saveKeys { get; private set; }

    [SerializeField] private string scene;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private CraftingRecipeDatabase recipeDatabase;
    /// <summary>
    /// 初期インベントリを設定
    /// </summary>
    [SerializeField] private List<ItemStack> initialInventory = new List<ItemStack>();
    /// <summary>
    /// 初期レシピを設定
    /// </summary>
    [SerializeField] private List<CraftingRecipe> initialRecipes = new List<CraftingRecipe>();

    private async void Awake()
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
        CraftingManager.Initialize(recipeDatabase, initialRecipes);

        saveKeys = await _saveKeyService.LoadAsync(SaveSlotKey);
        if (saveKeys == null)
        {
            saveKeys = new SerializableDictionary<string, string>();
            //スロット10個作成
            for (int i = 0; i < SaveSlotNumber; i++)
            {
                saveKeys.Add($"SaveData{i}", DateTime.MinValue.ToString());
            }
        }
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
            TimeManager.Instance.FromSaveData(_data.ElapsedTime);
        }
        _loadOp.allowSceneActivation = true;
        Debug.Log("ロード完了");
        TimeManager.Instance.ResumeTimer();
    }

    public async void SaveGameAsync(string key)
    {
        SaveData _data = new SaveData();
        _data.ItemData = ItemManager.Instance.ToSaveData();
        _data.RecipeData = CraftingManager.Instance.ToSaveData();
        _data.ElapsedTime = TimeManager.Instance.ElapsedTime;
        saveKeys.SetValue(key, DateTime.Now.ToString());
        await _saveService.SaveAsync(key, _data);
        await _saveKeyService.SaveAsync(SaveSlotKey, saveKeys);
    }
}
