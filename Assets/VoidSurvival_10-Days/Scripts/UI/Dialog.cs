using UnityEngine;

public abstract class Dialog : MonoBehaviour
{
    protected virtual void Start()
    {
        gameObject.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// UIの更新
    /// </summary>
    public virtual void UpdateUI()
    {

    }
    /// <summary>
    /// ダイアログを開く
    /// </summary>
    public void OpenDialog()
    {
        gameObject.SetActive(true);
        UpdateUI();
        MainUI.Instance?.CloseMainUI();
    }
    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public void CloseDialog()
    {
        gameObject.SetActive(false);
        MainUI.Instance?.OpenMainUI();
    }
}