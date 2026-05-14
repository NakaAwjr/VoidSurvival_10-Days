using UnityEngine;

[System.Serializable]
public abstract class Dialog : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        UpdateUI();
    }
    protected virtual void OnDisable() { }

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
    }
    /// <summary>
    /// ダイアログを閉じる
    /// </summary>
    public virtual void CloseDialog()
    {
        gameObject.SetActive(false);
    }
}