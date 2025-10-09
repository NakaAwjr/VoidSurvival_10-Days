using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

class QuickItemButton : MonoBehaviour, IItemControll, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private int index;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    private Vector2 _prevPos;
    private RectTransform _iconRectTransform;
    private RectTransform _rectTransform;

    public ItemStack ItemStack
    {
        get => _itemStack;
        set
        {
            _itemStack = value;
            if (_itemStack != null)
            {
                icon.sprite = _itemStack.Item.ItemIcon;
                amountText.text = _itemStack.Amount > 1 ? _itemStack.Amount.ToString() : string.Empty;
            }
            else
            {
                icon.sprite = null;
                amountText.text = string.Empty;
            }
        }
    }
    private ItemStack _itemStack;

    public void OnDrop(Item item)
    {
        ItemManager.Instance.SetQuickItem(index, item);
    }

    #region ドラッグ＆ドロップ
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _iconRectTransform = icon.GetComponent<RectTransform>();
        _prevPos = _iconRectTransform.anchoredPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ItemStack != null)
        {
            _iconRectTransform.SetAsLastSibling();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (ItemStack != null)
        {
            _iconRectTransform.anchoredPosition += eventData.delta;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemStack != null)
        {
            // ドロップ先はQuickItemButtonのみ
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                var itemControll = result.gameObject.GetComponent<QuickItemButton>();
                if (itemControll != null)
                {
                    if (itemControll.ItemStack != null)
                    {
                        // スワップ
                        var temp = itemControll.ItemStack;
                        itemControll.OnDrop(this.ItemStack.Item);
                        OnDrop(temp.Item);
                    }
                    else
                    {
                        // 移動
                        itemControll.OnDrop(this.ItemStack.Item);
                        OnDrop(null);
                    }
                }
                else
                {
                    // クイックアイテム以外にドロップした場合は解除
                    OnDrop(null);
                }
            }
            _iconRectTransform.anchoredPosition = _prevPos;
        }
    }
    #endregion
}