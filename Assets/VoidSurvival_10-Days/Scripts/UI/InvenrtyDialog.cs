using UnityEngine;

public class InvenrtyDialog : Dialog
{
    [SerializeField] private GameObject vewportContent;
    [SerializeField] private GameObject quickItemsContent;
    [SerializeField] private GameObject itemButtonPrefab;
    private ItemButton[] _itemButtons;
    private QuickItemButton[] _quickItemButtons;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
        _itemButtons = vewportContent.GetComponentsInChildren<ItemButton>();
        _quickItemButtons = quickItemsContent.GetComponentsInChildren<QuickItemButton>();
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
    }

    public override void UpdateUI()
    {
        var inventory = ItemManager.Instance.inventory;
        var quickItems = ItemManager.Instance.quickItems;

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
        // クイックアイテムの更新
        for (int i = 0; i < _quickItemButtons.Length; i++)
        {
            if (i < quickItems.Length)
            {
                _quickItemButtons[i].ItemStack = quickItems[i];
            }
            else
            {
                _quickItemButtons[i].ItemStack = null;
            }
        }
    }
}
