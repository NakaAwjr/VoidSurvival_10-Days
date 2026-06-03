using System;
using Cysharp.Threading.Tasks;
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
    private PlayerStatus _playerStatus;

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

        _playerStatus = FindAnyObjectByType<PlayerStatus>();
    }

    public async void LoadGameAsync(string key)
    {
        Debug.Log("ロード中");
        SceneLording.Instance.OpenDialog();
        // TimeManager.Instance.PauseTimer();
        SaveData _data = await _saveService.LoadAsync(key);
        var s = (_data != null) ? _data.Scene : scene;
        AsyncOperation _loadOp = SceneManager.LoadSceneAsync(s);
        _loadOp.allowSceneActivation = false;
        await UniTask.WaitUntil(() => _loadOp.progress >= 0.9f);
        if (_data != null)
        {
            ItemManager.Instance.FromSaveData(_data.ItemData);
            CraftingManager.Instance.FromSaveData(_data.RecipeData);
            TimeManager.Instance.FromSaveData(_data.ElapsedTime);
            FacilityManager.Instance.FromSaveData(_data.FacilityDatas);
            WorkbenchManager.Instance.FromSaveData(_data.WorkbenchData);
            _playerStatus.FromSaveData(_data.PlayerSaveData);
        }
        _loadOp.allowSceneActivation = true;
        await UniTask.WaitUntil(() => _loadOp.isDone);
        Debug.Log("ロード完了");
        SceneLording.Instance.CloseDialog();
        // TimeManager.Instance.ResumeTimer();
    }

    public async void SaveGameAsync(string key)
    {
        SaveData _data = new SaveData();
        _data.ItemData = ItemManager.Instance.ToSaveData();
        _data.RecipeData = CraftingManager.Instance.ToSaveData();
        _data.ElapsedTime = TimeManager.Instance.ElapsedTime;
        _data.FacilityDatas = FacilityManager.Instance.ToSaveData();
        _data.WorkbenchData = WorkbenchManager.Instance.ToSaveData();
        _data.PlayerSaveData = _playerStatus.ToSaveData();
        _data.Scene = SceneManager.GetActiveScene().name;
        saveKeys.SetValue(key, DateTime.Now.ToString());
        await _saveService.SaveAsync(key, _data);
        await _saveKeyService.SaveAsync(SaveSlotKey, saveKeys);
    }
}
