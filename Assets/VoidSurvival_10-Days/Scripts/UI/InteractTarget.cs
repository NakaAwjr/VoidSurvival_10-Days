using UnityEngine;

/// <summary>
/// インタラクト可能なオブジェクトのターゲットを表示するUI
/// </summary>
public class InteractTarget : MonoBehaviour
{
    public static InteractTarget Instance { get; private set; }
    private Camera _mainCamera;
    private RectTransform _parentRectTransform;

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
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// IInteractActionを持つオブジェクトの範囲に入った時に呼び出してターゲットを表示する
    /// </summary>
    /// <param name="target"></param>
    public void ShowTarget(Transform target)
    {
        gameObject.SetActive(true);
        var screenPos = _mainCamera.WorldToScreenPoint(target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, screenPos, null, out Vector2 localPoint);
        transform.localPosition = localPoint + new Vector2(0, 50);
    }
    /// <summary>
    /// ターゲットを非表示にする
    /// 範囲外に出たら呼び出す
    /// </summary>
    public void HideTarget()
    {
        gameObject.SetActive(false);
    }
}
