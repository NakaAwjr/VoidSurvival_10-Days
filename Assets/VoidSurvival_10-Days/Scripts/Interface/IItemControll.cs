interface IItemControll
{
    /// <summary>
    /// インベントリからドラッグ＆ドロップで呼ばれる
    /// クイックアイテムにセットする，装備するなど
    /// </summary>
    public void OnDrop(Item item);
}