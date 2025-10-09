using UnityEngine;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }
    [SerializeField] private GameObject controllers;
    [SerializeField] private GameObject dialogs;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(controllers);
        DontDestroyOnLoad(dialogs);
        OpenMainUI();
    }

    /// <summary>
    /// メインUIを開く
    /// コントローラーも効かせる
    /// </summary>
    public void OpenMainUI()
    {
        gameObject.SetActive(true);
        controllers.SetActive(true);
    }

    /// <summary>
    /// メインUIを閉じる
    /// コントローラーも効かなくする
    /// </summary>
    public void CloseMainUI()
    {
        gameObject.SetActive(false);
        controllers.SetActive(false);
    }

    #region メニュー
    [SerializeField] private Dialog inventoryDialog;
    [SerializeField] private Dialog craftDialog;

    public void OpenSetting()
    {
        // 設定画面を開く
    }
    public void OpenPlayerInfo()
    {
        // プレイヤー情報画面を開く
        inventoryDialog.OpenDialog();
    }
    public void OpenCraft()
    {
        // クラフト画面を開く
        craftDialog.OpenDialog();
    }
    public void OpenArchive()
    {
        // アーカイブ画面を開く
    }
    #endregion
}
