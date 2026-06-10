using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }
    [SerializeField] private GameObject controllers;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject dialogs;
    [SerializeField] private Button backButton;
    private RectTransform backButtonRect => backButton.GetComponent<RectTransform>();
    [SerializeField] private DialogID othersID;
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
        SetActiveBackButtonAndMainUI();
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

    /// <summary>
    /// バックボタンの表示を切り替える
    //  ダイアログが開いているときはバックボタンを表示し、メインUIを閉じる
    //  ダイアログが開いていないときはバックボタンを非表示にし、メインUIを開く
    /// </summary>
    private void SetActiveBackButtonAndMainUI()
    {
        if (isDialogOpen)
        {
            CloseMainUI();
            // 最後に開いたダイアログのグループにothersIDが含まれているかでバックボタンの表示を切り替える
            if (backStack.Peek().Contains(othersID))
            {
                backButton.gameObject.SetActive(false);
            }
            else
            {
                backButton.gameObject.SetActive(true);
            }
        }
        else
        {
            OpenMainUI();
            backButton.gameObject.SetActive(false);
        }
        // backButtonRect.SetAsLastSibling();
    }

    #region Dialog Management
    /// <summary>
    /// ダイアログが開かれているか
    /// </summary>
    public bool isDialogOpen => backStack.Count > 0;

    /// <summary>
    /// ダイアログを開く(単体)
    /// </summary>
    /// <param name="id"></param>
    public void OpenDialog(DialogID id)
    {
        var dialog = GetOrCreate(id);
        dialog.OpenDialog();
        backStack.Push(new List<DialogID> { id });
        SetActiveBackButtonAndMainUI();
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
        SetActiveBackButtonAndMainUI();
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
        SetActiveBackButtonAndMainUI();
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
    /// <summary>
    /// othersIDを現在のダイアロググループに追加する
    /// MainUIのOpenDialogを使わないダイアログ用
    /// </summary>
    public void AddBackStack()
    {
        backStack.Push(new List<DialogID> { othersID });
        SetActiveBackButtonAndMainUI();
    }
    /// <summary>
    /// othersIDを現在のダイアロググループから削除する
    /// </summary>
    public void RemoveBackStack()
    {
        if (backStack.Count > 0 && backStack.Peek().Contains(othersID))
        {
            backStack.Pop();
        }
        SetActiveBackButtonAndMainUI();
    }
    #endregion
}

