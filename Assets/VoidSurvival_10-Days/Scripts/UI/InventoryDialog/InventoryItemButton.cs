using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryItemButton : ItemButtonBase
{
    [SerializeField] private InventoryDialog inventoryDialog;
    [SerializeField] private ScrollRect scrollRect;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (!isControllable || !isLongPushing)
        {
            scrollRect.OnBeginDrag(eventData);
            return;
        }
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!isControllable || !isLongPushing)
        {
            scrollRect.OnDrag(eventData);
            return;
        }
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!isControllable || !isLongPushing)
        {
            scrollRect.OnEndDrag(eventData);
            return;
        }
        base.OnEndDrag(eventData);
    }

    protected override void HandleDrop(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var itemControll = result.gameObject.GetComponent<IItemControl>();
            if (itemControll != null)
            {
                itemControll.OnDrop(itemStack.Item);
            }
        }
    }

    public void OnClick()
    {
        if (isControllable && !isLongPushing)
        {
            inventoryDialog.OnItemButtonClicked(itemStack);
        }
    }
}
