using UnityEngine;

public class TestInventry : MonoBehaviour
{
    [SerializeField] private ItemDatabase itemDatabase;
    // Start is called before the first frame update
    void Start()
    {
        // ItemManagerの初期化
        // ItemManager.Initialize();

        // アイテムの追加
        Item item = itemDatabase.GetValue(0); // ID 1のアイテムを取得
        ItemManager.Instance.AddItem(item, 5); // そのアイテムを5個追加

        // クイックアイテムの設定
        ItemManager.Instance.SetQuickItem(0, item); // インデックス0にアイテムを設定

        // 装備スロットにアイテムを装備
        ItemManager.Instance.SetEquipmentSlot(ItemManager.EquipmentType.Head, item);

        Debug.Log("Inventory and quick items initialized.");
        Debug.Log("Inventory: " + ItemManager.Instance.inventory[0]?.Item.name + " x" + ItemManager.Instance.inventory[0]?.Amount);
        Debug.Log("Quick Item 0: " + ItemManager.Instance.quickItems[0]?.Item.name);
        Debug.Log("Equipment Slot Head: " + ItemManager.Instance.equipmentSlot[ItemManager.EquipmentType.Head]?.name);

        ItemManager.Instance.RemoveItem(item, 2); // アイテムを2個削除
        Debug.Log("Inventory after removal: " + ItemManager.Instance.inventory[0]?.Item.name + " x" + ItemManager.Instance.inventory[0]?.Amount);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
