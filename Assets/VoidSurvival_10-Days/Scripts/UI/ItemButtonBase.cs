using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// 共通の長押し・ドラッグ＆ドロップ機能を持つアイテムボタン基底クラス
/// </summary>
public abstract class ItemButtonBase : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI参照")]
    [SerializeField] protected Image icon;
    [SerializeField] protected TMP_Text amountText;
    [SerializeField] protected float longPushTime = 0.5f;
    [Header("設定")]
    /// <summary>
    /// ItemStackがnullのときに非表示にするかどうか
    /// </summary>
    [SerializeField] protected bool isHide = true;
    /// <summary>
    /// ドラッグ＆ドロップを有効にするかどうか
    /// </summary>
    [SerializeField] protected bool isControllable = true;

    protected RectTransform rectTransform;
    protected RectTransform iconRectTransform;
    protected Vector2 prevPos;
    protected int siblingIndex;

    protected bool isLongPushing = false;
    protected ItemStack itemStack;
    protected IEnumerator longPushCoroutine;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        iconRectTransform = icon.GetComponent<RectTransform>();
        prevPos = iconRectTransform.anchoredPosition;
        longPushCoroutine = WaitLongPush();
    }

    public virtual ItemStack ItemStack
    {
        get => itemStack;
        set
        {
            itemStack = value;
            if (itemStack != null)
            {
                icon.sprite = itemStack.Item.ItemIcon;
                amountText.text = itemStack.Amount > 1 ? itemStack.Amount.ToString() : string.Empty;
                icon.enabled = true;
                gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = null;
                amountText.text = string.Empty;
                icon.enabled = false;
                if (isHide) gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 長押しを検出するコルーチン
    /// </summary>
    /// <returns></returns>
    protected IEnumerator WaitLongPush()
    {
        yield return new WaitForSeconds(longPushTime);
        isLongPushing = true;
    }

    // イベント共通部分
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isLongPushing = false;
        if (!isControllable) return;
        StartCoroutine(longPushCoroutine);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isControllable) return;
        StopCoroutine(longPushCoroutine);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine(longPushCoroutine);
        if (!isControllable || !isLongPushing || itemStack == null) return;

        // 一番前に表示
        siblingIndex = iconRectTransform.GetSiblingIndex();
        iconRectTransform.SetAsLastSibling();
        // マスクを無効にする
        icon.maskable = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isControllable || !isLongPushing || itemStack == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        iconRectTransform.anchoredPosition = localPoint;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isControllable || !isLongPushing || itemStack == null) return;

        HandleDrop(eventData);

        iconRectTransform.anchoredPosition = prevPos;
        // 元の順序に戻す
        iconRectTransform.SetSiblingIndex(siblingIndex);
        // マスクを有効にする
        icon.maskable = true;
        isLongPushing = false;
    }

    /// <summary>
    /// 派生クラスで実装：ドロップ時の挙動を定義
    /// </summary>
    protected abstract void HandleDrop(PointerEventData eventData);
}
