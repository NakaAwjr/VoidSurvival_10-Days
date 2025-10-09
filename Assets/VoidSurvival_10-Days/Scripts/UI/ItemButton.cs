using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private bool isControllable = true;

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
                gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = null;
                amountText.text = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }
    private ItemStack _itemStack;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _iconRectTransform = icon.GetComponent<RectTransform>();
        _prevPos = _iconRectTransform.anchoredPosition;
    }

    #region ドラッグ＆ドロップ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isControllable) return;
        if (ItemStack != null)
        {
            _iconRectTransform.SetAsLastSibling();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isControllable) return;
        if (ItemStack != null)
        {
            _iconRectTransform.anchoredPosition += eventData.delta;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isControllable) return;
        if (ItemStack != null)
        {
            // ドロップ先にIItemControllを持つオブジェクトがあるか確認
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            foreach (var result in results)
            {
                var itemControll = result.gameObject.GetComponent<IItemControll>();
                if (itemControll != null)
                {
                    itemControll.OnDrop(ItemStack.Item);
                }
            }
            _iconRectTransform.anchoredPosition = _prevPos;
        }
    }
    #endregion
}
