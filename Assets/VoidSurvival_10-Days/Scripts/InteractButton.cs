using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;

    // このフラグはドラッグ中にクリックイベントを無効にするために使用されます
    private bool _isDragging = false;

    private void Start()
    {
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
        UpdateUI();
    }
    private void UpdateUI()
    {
        var itemStack = ItemManager.Instance.quickItems[ItemManager.Instance.selectedQuickItemIndex];
        if (itemStack != null && itemStack.Amount > 0)
        {
            icon.sprite = itemStack.Item.ItemIcon;
            text.text = itemStack.Amount.ToString();
        }
        else
        {
            icon.sprite = null;
            text.text = string.Empty;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    /// ボタンがクリックされたときの処理
    public void OnPointerUp(PointerEventData eventData)
    {
        // ここにボタンがクリックされたときの処理を記述
        if (!_isDragging)
        {
            playerController.ActInteract();
        }
    }

    /// ボタンがドラッグされ始めたら、クリック機能を停止させる
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    /// ドラッグが終了したときに、クリック機能を再度有効にする
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }
}
