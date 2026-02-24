using UnityEngine;

[System.Serializable]
public abstract class Dialog : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        UpdateUI();
    }
    protected virtual void OnDisable() { }
    protected virtual void Start()
    {
        // CloseDialog();
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
        MainUI.Instance?.CloseMainUI();
        // TimeManagerのタイマーを停止
        // TimeManager.Instance.PauseTimer();
    }
    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public virtual void CloseDialog()
    {
        gameObject.SetActive(false);
        MainUI.Instance?.OpenMainUI();
        // TimeManagerのタイマーを再開
        // TimeManager.Instance.ResumeTimer();
    }
}