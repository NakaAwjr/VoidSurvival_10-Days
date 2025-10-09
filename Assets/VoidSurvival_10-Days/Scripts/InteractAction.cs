using UnityEngine;

public abstract class InteractAction : MonoBehaviour
{
    /// <summary>
    /// プレイヤーがインタラクトボタンをクリックしたときの処理
    /// </summary>
    public virtual void Action()
    {
    }
    public void ShowInteractTarget()
    {
        InteractTarget.Instance?.ShowTarget(transform);
    }
    public void HideInteractTarget()
    {
        InteractTarget.Instance?.HideTarget();
    }
}