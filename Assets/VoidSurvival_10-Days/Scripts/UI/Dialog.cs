using UnityEngine;

public abstract class Dialog : MonoBehaviour
{
    protected virtual void Start()
    {
        gameObject.SetActive(false);
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
    }
    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public void ClseDialog()
    {
        gameObject.SetActive(false);
    }
}