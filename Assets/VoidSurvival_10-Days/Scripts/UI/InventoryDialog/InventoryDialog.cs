using UnityEngine;
using UnityEngine.Events;

public class InventoryDialog : Dialog
{
    [Header("スロット参照")]
    [SerializeField] private RectTransform itemsContent;
    [SerializeField] private RectTransform quickItemsContent;
    [SerializeField] private InventoryItemButton itemButtonTemplate;
    [SerializeField] private QuickItemButton quickItemButtonTemplate;
    /// <summary>
    /// アイテムボタンがクリックされたときに呼ばれるイベント
    /// </summary>
    public UnityEvent<ItemStack> OnButtonClicked;

    private SlotContainer<InventoryItemButton> inventorySlots;
    private SlotContainer<QuickItemButton> quickItemSlots;

    protected override void OnEnable()
    {
        base.OnEnable();
        ItemManager.Instance.OnItemAdded.AddListener(OnItemAdded);
        ItemManager.Instance.OnSlotCleared.AddListener(OnItemRemoved);
        ItemManager.Instance.OnItemUpdated.AddListener(OnItemAmountChanged);
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
        RefreshInventoryAll();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        ItemManager.Instance.OnItemAdded.RemoveListener(OnItemAdded);
        ItemManager.Instance.OnSlotCleared.RemoveListener(OnItemRemoved);
        ItemManager.Instance.OnItemUpdated.RemoveListener(OnItemAmountChanged);
        ItemManager.Instance.OnChanged.RemoveListener(UpdateUI);
    }
    private void Awake()
    {
        inventorySlots = new SlotContainer<InventoryItemButton>(itemsContent, itemButtonTemplate);
        quickItemSlots = new SlotContainer<QuickItemButton>(quickItemsContent, quickItemButtonTemplate);
    }
    private void RefreshInventoryAll()
    {
        var inventory = ItemManager.Instance.inventory;
        inventorySlots.EnsureSlotCount(inventory.Count);
        for (int i = 0; i < inventory.Count; i++)
        {
            inventorySlots.Slots[i].SetItem(inventory[i]);
        }
        inventorySlots.ClearFrom(inventory.Count);
    }
    // クイックアイテムスロットの更新だけ
    public override void UpdateUI()
    {
        var quickItems = ItemManager.Instance.quickItems;
        quickItemSlots.EnsureSlotCount(quickItems.Length);
        for (int i = 0; i < quickItems.Length; i++)
        {
            quickItemSlots.Slots[i].SetItem(quickItems[i]);
        }
        quickItemSlots.ClearFrom(quickItems.Length);
    }

    #region Item Events
    /// <summary>
    /// アイテムが追加されたときに呼ばれる
    /// </summary>
    /// <param name="index"></param>
    private void OnItemAdded(int index)
    {
        inventorySlots.EnsureSlotCount(index + 1);
        inventorySlots.Slots[index].SetItem(ItemManager.Instance.inventory[index]);
    }
    /// <summary>
    /// アイテムが削除されたときに呼ばれる
    /// </summary>
    /// <param name="index"></param>
    private void OnItemRemoved(int index)
    {
        inventorySlots.ClearFrom(index);
    }
    /// <summary>
    /// アイテムの数量が変更されたときに呼ばれる
    /// </summary>
    /// <param name="index"></param>
    private void OnItemAmountChanged(int index, ItemStack itemStack)
    {
        inventorySlots.Slots[index].SetItem(itemStack);
    }
    #endregion

    #region Button Callbacks
    /// <summary>
    /// アイテムボタンがクリックされたときに呼ばれる
    /// </summary>
    /// <param name="itemStack"></param>
    public void OnItemButtonClicked(ItemStack itemStack)
    {
        OnButtonClicked?.Invoke(itemStack);
    }
    #endregion
}