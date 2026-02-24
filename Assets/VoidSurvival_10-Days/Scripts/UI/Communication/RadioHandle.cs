using UnityEngine;
using UnityEngine.EventSystems;

public class RadioHandle : MonoBehaviour, IDragHandler
{
    /// <summary>
    /// 現在の値
    /// </summary>
    public float currentValue { get; private set; }
    /// <summary>
    /// ハンドルがドラッグ可能かどうか
    /// </summary>
    public bool isDragable = true;
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragable) return;
        var angle = Mathf.Atan2(eventData.position.y - initialPosition.y,
                                eventData.position.x - initialPosition.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        currentValue = (angle + 180f) / 360f * 100f;
    }
}
