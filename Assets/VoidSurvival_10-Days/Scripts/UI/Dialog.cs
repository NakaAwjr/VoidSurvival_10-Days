using UnityEngine;

public abstract class Dialog : MonoBehaviour
{
    protected virtual void Start()
    {
        CloseDialog();
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
    public virtual void OpenDialog()
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