using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkbenchDialog : Dialog
{
    [SerializeField] private InventoryDialog inventoryDialog;
    [SerializeField] private GameObject vewportContent;
    [SerializeField] private Slider craftCountSlider;
    [SerializeField] private TMP_Text craftCountText;
    [SerializeField] private Slider progressSlider;

    [SerializeField] private WorkbenchRecipeButton recipeButton;
    [SerializeField] private ItemButton requiredItemButton;
    [SerializeField] private ItemButton resultItemButton;
    private WorkbenchRecipeButton[] _recipeButtons;

    private Item _selectedItem;
    private Item selectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            // レシピを取得して表示
            var recipes = WorkbenchManager.Instance.GetRecipes(_selectedItem);
            // レシピボタンの数がレシピ数より少ない場合、追加する
            if (_recipeButtons.Length < recipes.Count)
            {
                for (int i = _recipeButtons.Length; i < recipes.Count; i++)
                {
                    Instantiate(recipeButton, vewportContent.transform);
                }
                _recipeButtons = vewportContent.GetComponentsInChildren<WorkbenchRecipeButton>();
            }
            // レシピボタンにレシピを割り当てる
            for (int i = 0; i < _recipeButtons.Length; i++)
            {
                if (i < recipes.Count)
                {
                    _recipeButtons[i].WorkbenchRecipe = recipes[i];
                }
                else
                {
                    _recipeButtons[i].WorkbenchRecipe = null;
                }
            }
        }
    }
    private int _craftCount;
    private int craftCount
    {
        get => _craftCount;
        set
        {
            _craftCount = value;
            craftCountText.text = _craftCount.ToString();
        }
    }
    private WorkbenchRecipe _selectRecipe;
    private WorkbenchRecipe selectRecipe
    {
        get => _selectRecipe;
        set
        {
            _selectRecipe = value;
            if (_selectRecipe != null)
            {
                // 可能なクラフト数を設定
                craftCountSlider.maxValue = WorkbenchManager.Instance.NumberCanCrafting(_selectRecipe) > 0 ? WorkbenchManager.Instance.NumberCanCrafting(_selectRecipe) : 1;
                craftCountSlider.value = 1;
                craftCountText.text = craftCountSlider.value.ToString();
            }
            else
            {
                craftCountSlider.maxValue = 1;
                craftCountSlider.value = 1;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
        _recipeButtons = vewportContent.GetComponentsInChildren<WorkbenchRecipeButton>();
        // スライダー初期化
        craftCountSlider.onValueChanged.AddListener(OnCraftCountSliderChanged);
        craftCountSlider.wholeNumbers = true;
        craftCountSlider.minValue = 1;
        craftCountSlider.maxValue = 1;
        craftCountSlider.value = 1;
        // 選択状態を初期化
        selectedItem = ItemManager.Instance.inventory[0]?.Item;
        selectRecipe = _recipeButtons[0]?.WorkbenchRecipe;
        // イベント登録
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
    }
    private void Update()
    {
        progressSlider.value = WorkbenchManager.Instance.workingProgress;
        progressSlider.maxValue = _selectRecipe != null ? _selectRecipe.time : 1;
    }
    public override void OpenDialog()
    {
        base.OpenDialog();
        inventoryDialog.OpenDialog();
        inventoryDialog.OnButtonClicked.AddListener(ItemSelectedFromInventory);
        // 選択状態を初期化
        selectedItem = ItemManager.Instance.inventory[0]?.Item;
        selectRecipe = _recipeButtons[0]?.WorkbenchRecipe;
    }
    public override void CloseDialog()
    {
        base.CloseDialog();
        inventoryDialog.CloseDialog();
        inventoryDialog.OnButtonClicked.RemoveListener(ItemSelectedFromInventory);
    }
    public override void UpdateUI()
    {
        requiredItemButton.ItemStack = WorkbenchManager.Instance.WorkingRequiredItem;
        resultItemButton.ItemStack = WorkbenchManager.Instance.WorkingResultItem;
        if (_selectRecipe != null)
        {
            // 可能なクラフト数を設定
            craftCountSlider.maxValue = WorkbenchManager.Instance.NumberCanCrafting(_selectRecipe) > 0 ? WorkbenchManager.Instance.NumberCanCrafting(_selectRecipe) : 1;
            craftCountSlider.value = 1;
            craftCountText.text = craftCountSlider.value.ToString();
        }
        else
        {
            craftCountSlider.maxValue = 1;
            craftCountSlider.value = 1;
        }
    }

    /// <summary>
    /// インベントリからアイテムが選択されたとき
    /// </summary>
    /// <param name="itemStack"></param>
    public void ItemSelectedFromInventory(ItemStack itemStack)
    {
        if (itemStack != null)
        {
            selectedItem = itemStack.Item;
        }
    }
    /// <summary>
    /// レシピが選択されたとき(ボタン)
    /// </summary>
    /// <param name="recipe"></param>
    public void RecipeSelected(WorkbenchRecipe recipe)
    {
        selectRecipe = recipe;
    }
    /// <summary>
    /// クラフト実行(ボタン)
    /// </summary> <param name="count"></param>
    public void Craft()
    {
        if (_selectRecipe != null && _craftCount > 0)
        {
            WorkbenchManager.Instance.StartCraftItem(_selectRecipe, _craftCount);
        }
    }
    /// <summary>
    /// クラフト数スライダー変更時に呼ばれる
    /// </summary>
    /// <param name="value"></param>
    public void OnCraftCountSliderChanged(float value)
    {
        craftCount = (int)value;
    }
    public void Stop()
    {
        WorkbenchManager.Instance.StopCraftItem();
    }
    public void CollectResultItem()
    {
        WorkbenchManager.Instance.CollectResultItem();
    }
}
