using UnityEngine;

/// <summary>
/// アイテムボタンやレシピボタンなど、アイテムを表示するスロットの基底クラス
/// </summary>
public abstract class SlotUIBase : MonoBehaviour
{
    public int Index { get; private set; }
    public void SetIndex(int index)
    {
        Index = index;
    }
    public abstract void Clear();
}