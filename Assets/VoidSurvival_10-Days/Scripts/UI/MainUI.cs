using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }
    [SerializeField] private GameObject controllers;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject dialogs;
    [SerializeField] private Button backButton;
    private RectTransform backButtonRect => backButton.GetComponent<RectTransform>();
    [SerializeField] private List<DialogEntry> dialogEntries;

    private Dictionary<DialogID, Dialog> cache = new();
    private Stack<List<DialogID>> backStack = new();

    // Start is called before the first frame update
    void Awake()
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

        backButton.onClick.AddListener(() =>
        {
            CloseCurrentDialog();
        });

        OpenMainUI();
        SetActiveBackButton();
    }

    /// <summary>
    /// メインUIを開く
    /// コントローラーも効かせる
    /// </summary>
    public void OpenMainUI()
    {
        mainUI.SetActive(true);
        controllers.SetActive(true);
    }

    /// <summary>
    /// メインUIを閉じる
    /// コントローラーも効かなくする
    /// </summary>
    public void CloseMainUI()
    {
        mainUI.SetActive(false);
        controllers.SetActive(false);
    }

    private void SetActiveBackButton()
    {
        backButton.gameObject.SetActive(isDialogOpen);
        backButtonRect.SetAsLastSibling();
    }

    #region メニュー
    [SerializeField] private Dialog settingDialog;
    [SerializeField] private Dialog inventoryDialog;
    [SerializeField] private Dialog craftDialog;
    [SerializeField] private Dialog workbenchDialog;

    private bool isDialogOpen => backStack.Count > 0;

    /// <summary>
    /// ダイアログを開く(単体)
    /// </summary>
    /// <param name="id"></param>
    public void OpenDialog(DialogID id)
    {
        var dialog = GetOrCreate(id);
        dialog.OpenDialog();
        backStack.Push(new List<DialogID> { id });
        SetActiveBackButton();
    }
    /// <summary>
    /// ダイアログを開く(グループ)
    /// </summary>
    public void OpenGroup(params DialogID[] ids)
    {
        var group = new List<DialogID>();

        foreach (var id in ids)
        {
            var dlg = GetOrCreate(id);
            dlg.OpenDialog();
            group.Add(id);
        }

        backStack.Push(group);
        SetActiveBackButton();
    }
    /// <summary>
    /// 現在のダイアログを閉じる
    /// </summary>
    public void CloseCurrentDialog()
    {
        if (backStack.Count == 0)
            return;

        var currentGroup = backStack.Pop();
        foreach (var id in currentGroup)
        {
            if (cache.TryGetValue(id, out var dlg) && dlg != null)
            {
                dlg.CloseDialog();
            }
        }
        SetActiveBackButton();
    }
    /// <summary>
    /// ダイアログを取得する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Dialog GetDialog(DialogID id)
    {
        if (cache.TryGetValue(id, out var dlg) && dlg != null)
            return dlg;
        return null;
    }
    private Dialog GetOrCreate(DialogID id)
    {
        if (cache.TryGetValue(id, out var dlg) && dlg != null)
            return dlg;

        var entry = dialogEntries.Find(e => e.dialogID == id);
        var instance = Instantiate(entry.dialog, dialogs.transform);
        cache[id] = instance;
        return instance;
    }

    public void OpenSetting()
    {
        // 設定画面を開く
        settingDialog.OpenDialog();
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

