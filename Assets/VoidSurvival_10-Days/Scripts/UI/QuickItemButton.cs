using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class QuickItemButton : ItemButtonBase, IItemControll
{
    [SerializeField] private int index;

    public void OnDrop(Item item)
    {
        ItemManager.Instance.SetQuickItem(index, item);
    }

    protected override void HandleDrop(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        bool foundTarget = false;

        foreach (var result in results)
        {
            var target = result.gameObject.GetComponent<QuickItemButton>();
            if (target == null) continue;

            foundTarget = true;
            if (target.ItemStack != null)
            {
                // スワップ
                var temp = target.ItemStack;
                target.OnDrop(itemStack.Item);
                OnDrop(temp.Item);
            }
            else
            {
                // 移動
                target.OnDrop(itemStack.Item);
                OnDrop(null);
            }
        }

        if (!foundTarget)
        {
            // クイック枠外にドロップ → 削除
            OnDrop(null);
        }
    }
}