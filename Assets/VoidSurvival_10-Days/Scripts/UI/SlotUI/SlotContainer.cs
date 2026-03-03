using System.Collections.Generic;
using UnityEngine;

public class SlotContainer<Tslot> where Tslot : SlotUIBase
{
    protected readonly RectTransform content;
    protected readonly Tslot template;
    protected readonly List<Tslot> slots = new List<Tslot>();

    /// <summary>
    /// スロット一覧
    /// </summary>
    public IReadOnlyList<Tslot> Slots => slots;

    public SlotContainer(RectTransform content, Tslot template)
    {
        this.content = content;
        this.template = template;
        template.gameObject.SetActive(false);
    }
    /// <summary>
    /// スロットの数を確保する
    /// </summary>
    /// <param name="count"></param>
    public void EnsureSlotCount(int count)
    {
        while (slots.Count < count)
        {
            var slot = Object.Instantiate(template, content);
            slot.gameObject.SetActive(true);
            slot.SetIndex(slots.Count);
            slots.Add(slot);
        }
    }
    /// <summary>
    /// 指定したインデックス以降のスロットをクリアする
    /// </summary>
    /// <param name="index"></param>
    public void ClearFrom(int index)
    {
        for (int i = index; i < slots.Count; i++)
        {
            slots[i].Clear();
        }
    }
}