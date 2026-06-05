using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager
{
    public const int QUICK_ITEM_COUNT = 4;
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ItemManager();
            return _instance;
        }
    }
    private ItemDatabase _itemDatabase;
    /// <summary>
    /// ItemStack(Itemと数量)の集合
    /// </summary>
    public List<ItemStack> inventory { get; private set; }
    /// <summary>
    /// inventoryにあるItemStackを選ぶ
    /// この中で選択されているアイテムが使用可能
    /// </summary>
    public ItemStack[] quickItems { get; private set; }
    /// <summary>
    /// quickItemsのどれを選択するかを決めるインデックス
    /// </summary>
    public int selectedQuickItemIndex { get; private set; }
    /// <summary>
    /// 装備スロット
    /// 装備するとアイテムがインベントリから1つ減る
    /// </summary>
    public Dictionary<EquipmentType, Item> equipmentSlot { get; private set; }

    /// <summary>
    /// 装備スロットの列挙型
    /// </summary>
    public enum EquipmentType
    {
        Head,
        Body,
        Legs,
    }

    #region Events
    /// <summary>
    /// 何かしら変更されたときに発火するイベント
    /// </summary>
    public UnityEvent OnChanged = new UnityEvent();
    /// <summary>
    /// アイテムスロットの中身が変更されたときに発火するイベント
    /// </summary>
    public UnityEvent<int, ItemStack> OnItemUpdated = new UnityEvent<int, ItemStack>();
    /// <summary>
    /// アイテムが追加されたときに発火するイベント
    /// </summary>
    public UnityEvent<int> OnItemAdded = new UnityEvent<int>();
    /// <summary>
    /// アイテムが削除されたときに発火するイベント
    /// </summary>
    public UnityEvent<int> OnSlotCleared = new UnityEvent<int>();
    /// <summary>
    /// アイテムを取得したときに発火するイベント
    /// </summary>
    public UnityEvent<Item> OnGetItem = new UnityEvent<Item>();
    #endregion

    #region Init
    private ItemManager()
    {
        _itemDatabase = Resources.Load<ItemDatabase>("ItemDatabase");
        List<ItemStack> _initialInventory = Resources.Load<InitData>("InitData")?.initialInventory;
        quickItems = new ItemStack[QUICK_ITEM_COUNT];
        selectedQuickItemIndex = 0;
        equipmentSlot = new Dictionary<EquipmentType, Item>()
        {
            [EquipmentType.Head] = null,
            [EquipmentType.Body] = null,
            [EquipmentType.Legs] = null
        };
        if (_initialInventory != null)
        {
            this.inventory = new List<ItemStack>(_initialInventory);
        }
        else
        {
            this.inventory = new List<ItemStack>();
        }
    }
    // public static void Initialize()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = new ItemManager();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("ItemManager instance already exists. Reinitializing.");
    //     }
    // }
    #endregion

    #region Methods
    /// <summary>
    /// アイテムスタックを取得する処理
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ItemStack GetItemStack(Item item)
    {
        return inventory.FirstOrDefault(stack => stack.Item == item);
    }

    /// <summary>
    /// アイテムを追加する処理
    /// アイテムが既に存在する場合は数量を増やし、存在しない場合は新たに追加
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void AddItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogError("Attempted to add a null item.");
            return;
        }
        if (amount <= 0)
        {
            Debug.LogError("Attempted to add an item with non-positive amount: " + amount);
            return;
        }
        var existingStack = GetItemStack(item);
        if (existingStack != null)
        {
            // アイテムが既に存在する場合は数量を増やす
            existingStack.Add(amount);
            OnItemUpdated?.Invoke(inventory.IndexOf(existingStack), existingStack);
        }
        else
        {
            // アイテムが存在しない場合は新たに追加
            inventory.Add(new ItemStack(item, amount));
            OnItemAdded?.Invoke(inventory.Count - 1);
        }
        OnChanged?.Invoke();
        OnGetItem?.Invoke(item);
    }
    /// <summary>
    /// アイテムを削除する処理
    /// アイテムの数量を減らし、数量が0以下になった場合はアイテムをリストから削除
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    public void RemoveItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogError("Attempted to remove a null item.");
            return;
        }
        if (amount <= 0)
        {
            Debug.LogError("Attempted to remove an item with non-positive amount: " + amount);
            return;
        }
        var existingStack = GetItemStack(item);
        if (existingStack.Item != null)
        {
            existingStack.Remove(amount);
            OnItemUpdated?.Invoke(inventory.IndexOf(existingStack), existingStack);
            if (existingStack.Amount <= 0)
            {
                // 数量が0以下になった場合はアイテムを削除
                var index = inventory.IndexOf(existingStack);
                inventory.Remove(existingStack);
                for (int i = index; i < inventory.Count; i++)
                {
                    OnItemUpdated?.Invoke(i, inventory[i]);
                }
                OnSlotCleared?.Invoke(inventory.Count);
                if (quickItems.Contains(existingStack))
                {
                    // クイックアイテムからも削除
                    for (int i = 0; i < QUICK_ITEM_COUNT; i++)
                    {
                        if (quickItems[i] == existingStack)
                        {
                            quickItems[i] = null;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Item {item.name} not found in inventory.");
        }
        OnChanged?.Invoke();
    }
    /// <summary>
    /// クイックアイテムを設定
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void SetQuickItem(int index, Item item)
    {
        if (index < 0 || index >= QUICK_ITEM_COUNT)
        {
            Debug.LogError("Invalid quick item index: " + index);
            return;
        }
        for (int i = 0; i < QUICK_ITEM_COUNT; i++)
        {
            if (quickItems[i] != null && quickItems[i].Item == item)
            {
                // 既にクイックアイテムに設定されている場合は解除
                quickItems[i] = null;
            }
        }
        quickItems[index] = GetItemStack(item);
        OnChanged?.Invoke();
    }
    /// <summary>
    /// クイックアイテムのインデックスを選択
    /// </summary>
    /// <param name="index"></param>
    public void SelectQuickItem(int index)
    {
        if (index < 0 || index >= QUICK_ITEM_COUNT)
        {
            Debug.LogError("Invalid quick item index: " + index);
            return;
        }
        selectedQuickItemIndex = index;
        OnChanged?.Invoke();
    }
    /// <summary>
    /// 装備スロットにアイテムを設定する処理
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    public void SetEquipmentSlot(EquipmentType type, Item item)
    {
        if (equipmentSlot.ContainsKey(type))
        {
            if (equipmentSlot[type] != null) AddItem(equipmentSlot[type]);
            equipmentSlot[type] = item;
            if (item != null) RemoveItem(item);
        }
        else
        {
            Debug.LogError($"スロット{type}は存在しません");
            return;
        }
        OnChanged?.Invoke();
    }
    #endregion

    #region Save
    public void FromSaveData(ItemSaveData saveData)
    {
        selectedQuickItemIndex = saveData.SelectedQuickItemIndex;

        // アイテムIDを使用してアイテムを取得し、インベントリに設定
        inventory = new List<ItemStack>();
        foreach (var inv in saveData.Inventory.keyValuePairs)
        {
            Item itemData = _itemDatabase.GetValue(inv.Key);
            if (itemData != null)
            {
                // アイテムが存在する場合は数量を設定
                inventory.Add(new ItemStack(itemData, inv.Value));
            }
            else
            {
                Debug.LogWarning($"Item with ID {inv.Key} not found in database.");
            }
        }

        quickItems = new ItemStack[QUICK_ITEM_COUNT];
        // クイックアイテムの配列を設定
        for (int i = 0; i < QUICK_ITEM_COUNT; i++)
        {
            if (saveData.QuickItems[i] != -1) // -1は未設定を示す
            {
                Item itemData = _itemDatabase.GetValue(saveData.QuickItems[i]);
                if (itemData != null)
                {
                    quickItems[i] = GetItemStack(itemData);
                }
            }
            else
            {
                quickItems[i] = null; // 未設定の場合はnull
            }
        }

        // 装備スロットのアイテムも同様に設定
        foreach (var slot in saveData.EquipmentSlot.keyValuePairs)
        {
            var itemData = _itemDatabase.GetValue(slot.Value);
            if (equipmentSlot.ContainsKey(slot.Key))
            {
                equipmentSlot[slot.Key] = itemData;
            }
            else
            {
                Debug.LogWarning($"スロットが存在しません。コンストラクタを見直してください。");
            }
        }
    }
    public ItemSaveData ToSaveData()
    {
        ItemSaveData saveData = new ItemSaveData
        {
            Inventory = new SerializableDictionary<int, int>(),
            QuickItems = new int[QUICK_ITEM_COUNT],
            SelectedQuickItemIndex = selectedQuickItemIndex,
            EquipmentSlot = new SerializableDictionary<EquipmentType, int>()
        };

        // アイテムのIDと数量を保存
        foreach (var itemStack in inventory)
        {
            saveData.Inventory.Add(itemStack.Item.ItemID, itemStack.Amount);
        }

        // クイックアイテムのIDを保存
        for (int i = 0; i < QUICK_ITEM_COUNT; i++)
        {
            if (quickItems[i] != null)
            {
                saveData.QuickItems[i] = quickItems[i].Item.ItemID;
            }
            else
            {
                saveData.QuickItems[i] = -1; // クイックアイテムが設定されていない場合は-1
            }
        }

        // 装備スロットのアイテムIDを保存
        foreach (var slot in equipmentSlot)
        {
            if (slot.Value != null)
            {
                saveData.EquipmentSlot.Add(slot.Key, slot.Value.ItemID);
            }
            else
            {
                saveData.EquipmentSlot.Add(slot.Key, -1);
            }
        }

        return saveData;
    }
    #endregion
}
