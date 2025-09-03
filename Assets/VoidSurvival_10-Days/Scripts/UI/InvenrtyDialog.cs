using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenrtyDialog : MonoBehaviour
{
    [SerializeField] private GameObject vewportContent;
    [SerializeField] private GameObject itemButtonPrefab;
    private ItemButton[] _itemButtons;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        _itemButtons = vewportContent.GetComponentsInChildren<ItemButton>();
        OpenInventory();
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        UpdateInventoryUI();

    }
    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInventoryUI()
    {
        var inventory = ItemManager.Instance.inventory;

        // アイテムボタンの数がインベントリのアイテム数より少ない場合、追加する
        if (_itemButtons.Length < inventory.Count)
        {
            for (int i = _itemButtons.Length; i < inventory.Count; i++)
            {
                Instantiate(itemButtonPrefab, vewportContent.transform).GetComponent<ItemButton>();
                _itemButtons = vewportContent.GetComponentsInChildren<ItemButton>();
            }
        }
        for (int i = 0; i < _itemButtons.Length; i++)
        {
            if (i < inventory.Count)
            {
                _itemButtons[i].ItemStack = inventory[i];
            }
            else
            {
                _itemButtons[i].ItemStack = null;
            }
        }
    }
}
