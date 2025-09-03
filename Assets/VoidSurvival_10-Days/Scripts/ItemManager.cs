using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager
{
    public const int QUICK_ITEM_COUNT = 4;
    public static ItemManager Instance { get; private set; }
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


    private ItemManager(ItemDatabase itemDatabase, List<ItemStack> inventory = null)
    {
        _itemDatabase = itemDatabase;
        quickItems = new ItemStack[QUICK_ITEM_COUNT];
        selectedQuickItemIndex = 0;
        equipmentSlot = new Dictionary<EquipmentType, Item>()
        {
            [EquipmentType.Head] = null,
            [EquipmentType.Body] = null,
            [EquipmentType.Legs] = null
        };
        if (inventory != null)
        {
            this.inventory = new List<ItemStack>(inventory);
        }
        else
        {
            this.inventory = new List<ItemStack>();
        }
    }
    public static void Initialize(ItemDatabase itemDatabase, List<ItemStack> inventory = null)
    {
        if (Instance == null)
        {
            Instance = new ItemManager(itemDatabase, inventory);
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
        }
        else
        {
            // アイテムが存在しない場合は新たに追加
            inventory.Add(new ItemStack(item, amount));
        }
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
        quickItems[index] = GetItemStack(item);
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
    }

    public void FromSaveData(ItemSaveData saveData)
    {
        selectedQuickItemIndex = saveData.SelectedQuickItemIndex;

        // アイテムIDを使用してアイテムを取得し、インベントリに設定
        inventory = new List<ItemStack>();
        foreach (var inv in saveData.Inventory)
        {
            Item itemData = _itemDatabase.GetValue(inv.ItemID);
            if (itemData != null)
            {
                // アイテムが存在する場合は数量を設定
                inventory.Add(new ItemStack(itemData, inv.Amount));
            }
            else
            {
                Debug.LogWarning($"Item with ID {inv.ItemID} not found in database.");
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
        foreach (var slot in saveData.EquipmentSlot)
        {
            var itemData = _itemDatabase.GetValue(slot.ItemID);
            if (equipmentSlot.ContainsKey(slot.Type))
            {
                equipmentSlot[slot.Type] = itemData;
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
            Inventory = new List<ItemStackSaveData>(),
            QuickItems = new int[QUICK_ITEM_COUNT],
            SelectedQuickItemIndex = selectedQuickItemIndex,
            EquipmentSlot = new List<EquipmentSlotSaveData>()
        };

        // アイテムのIDと数量を保存
        foreach (var itemStack in inventory)
        {
            saveData.Inventory.Add(new ItemStackSaveData()
            {
                ItemID = itemStack.Item.ItemID,
                Amount = itemStack.Amount
            });
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
                saveData.EquipmentSlot.Add(new EquipmentSlotSaveData() { Type = slot.Key, ItemID = slot.Value.ItemID });
            }
            else
            {
                saveData.EquipmentSlot.Add(new EquipmentSlotSaveData() { Type = slot.Key, ItemID = -1 });
            }
        }

        return saveData;
    }
}
