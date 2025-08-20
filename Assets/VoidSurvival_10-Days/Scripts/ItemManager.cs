using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager
{
    public const int QUICK_ITEM_COUNT = 4;
    public static ItemManager Instance { get; private set; }
    private ItemDatabase _itemDatabase;
    public List<ItemStack> inventory { get; private set; } // アイテムIDと数量の辞書
    public Item[] quickItems { get; private set; } // クイックアイテムの配列
    public int selectedQuickItemIndex { get; private set; } // 選択中のクイックアイテムのインデックス
    public Dictionary<EquipmentSlot, Item> equipmentSlots { get; private set; } // 装備スロットの辞書

    /// <summary>
    /// 装備スロットの列挙型
    /// </summary>
    public enum EquipmentSlot
    {
        Head,
        Body,
        Legs,
        Feet,
        Hands,
        Accessory
    }


    private ItemManager(ItemDatabase itemDatabase)
    {
        _itemDatabase = itemDatabase;
        inventory = new List<ItemStack>();
        quickItems = new Item[QUICK_ITEM_COUNT];
        selectedQuickItemIndex = 0;
        equipmentSlots = new Dictionary<EquipmentSlot, Item>();
    }
    public static void Initialize(ItemDatabase itemDatabase)
    {
        if (Instance == null)
        {
            Instance = new ItemManager(itemDatabase);
        }
        else
        {
            Debug.LogWarning("ItemManager instance already exists. Reinitializing.");
        }
    }

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
    /// アイテムが既に存在する場合は数量を増やし、存在しない場合は新たに追加します。
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
        }
        else
        {
            // アイテムが存在しない場合は新たに追加
            inventory.Add(new ItemStack(item, amount));
        }
    }
    /// <summary>
    /// アイテムを削除する処理
    /// アイテムの数量を減らし、数量が0以下になった場合はアイテムを辞書から削除します。
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
            if (existingStack.Amount <= 0)
            {
                // 数量が0以下になった場合はアイテムを削除
                inventory.Remove(existingStack);
            }
        }
        else
        {
            Debug.LogWarning($"Item {item.name} not found in inventory.");
        }
    }
    /// <summary>
    /// クイックアイテムのインデックスを設定する処理
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
        quickItems[index] = item;
    }
    /// <summary>
    /// クイックアイテムを選択する処理
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
    }
    /// <summary>
    /// 装備スロットにアイテムを設定する処理
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="item"></param>
    public void SetEquipmentSlot(EquipmentSlot slot, Item item)
    {
        if (equipmentSlots.ContainsKey(slot))
        {
            equipmentSlots[slot] = item;
        }
        else
        {
            equipmentSlots.Add(slot, item);
        }
    }

    public void FromSaveData(ItemSaveData saveData)
    {
        selectedQuickItemIndex = saveData.SelectedQuickItemIndex;

        // アイテムIDを使用してアイテムを取得し、辞書に設定
        inventory = new List<ItemStack>();
        foreach (var inv in saveData.Inventory)
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

        quickItems = new Item[QUICK_ITEM_COUNT];
        // クイックアイテムの配列を設定
        for (int i = 0; i < QUICK_ITEM_COUNT; i++)
        {
            if (saveData.QuickItems[i] != -1) // -1は未設定を示す
            {
                Item itemData = _itemDatabase.GetValue(saveData.QuickItems[i]);
                if (itemData != null)
                {
                    quickItems[i] = itemData;
                }
            }
            else
            {
                quickItems[i] = null; // 未設定の場合はnull
            }
        }

        // 装備スロットのアイテムも同様に設定
        foreach (var slot in equipmentSlots.Keys)
        {
            Item itemData = _itemDatabase.GetValue(equipmentSlots[slot].ItemID);
            if (itemData != null)
            {
                equipmentSlots[slot] = itemData;
            }
        }
    }
    public ItemSaveData ToSaveData()
    {
        ItemSaveData saveData = new ItemSaveData
        {
            Inventory = new Dictionary<int, int>(),
            QuickItems = new int[QUICK_ITEM_COUNT],
            SelectedQuickItemIndex = selectedQuickItemIndex,
            EquipmentSlots = new Dictionary<EquipmentSlot, int>()
        };

        // アイテムのIDと数量を保存
        foreach (var itemStack in inventory)
        {
            saveData.Inventory[itemStack.Item.ItemID] = itemStack.Amount;
        }

        // クイックアイテムのIDを保存
        for (int i = 0; i < QUICK_ITEM_COUNT; i++)
        {
            if (quickItems[i] != null)
            {
                saveData.QuickItems[i] = quickItems[i].ItemID;
            }
            else
            {
                saveData.QuickItems[i] = -1; // クイックアイテムが設定されていない場合は-1
            }
        }

        // 装備スロットのアイテムIDを保存
        foreach (var slot in equipmentSlots)
        {
            saveData.EquipmentSlots[slot.Key] = slot.Value.ItemID;
        }

        return saveData;
    }
}
