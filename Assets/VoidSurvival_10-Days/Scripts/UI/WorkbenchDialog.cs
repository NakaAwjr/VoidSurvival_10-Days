using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkbenchDialog : Dialog
{
    [Header("参照")]
    [SerializeField] private DialogID inventoryDialog;
    private InventoryDialog inventoryDialogInstance => MainUI.Instance.GetDialog(inventoryDialog) as InventoryDialog;
    [Header("スロット参照")]
    [SerializeField] private RectTransform content;
    [SerializeField] private WorkbenchRecipeButton recipeButton;
    [Header("UI参照")]
    [SerializeField] private Slider craftCountSlider;
    [SerializeField] private TMP_Text craftCountText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private ItemButton requiredItemButton;
    [SerializeField] private ItemButton resultItemButton;

    private WorkbenchRecipeListUI recipeSlots;

    private Item _selectedItem;
    private Item selectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            // レシピを取得して表示
            var recipes = WorkbenchManager.Instance.GetRecipes(_selectedItem);
            recipeSlots.Refresh(recipes);
            // 最初のレシピを選択状態にする
            selectRecipe = recipes.Count > 0 ? recipes[0] : null;
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
                craftCount = (int)craftCountSlider.value;
                craftCountText.text = craftCountSlider.value.ToString();
            }
            else
            {
                craftCountSlider.maxValue = 1;
                craftCountSlider.value = 1;
                craftCount = 1;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OnWarkbenchStateChanged();
        // スライダー初期化
        craftCountSlider.onValueChanged.AddListener(OnCraftCountSliderChanged);
        craftCountSlider.wholeNumbers = true;
        craftCountSlider.minValue = 1;
        craftCountSlider.maxValue = 1;
        craftCountSlider.value = 1;
        // 選択状態を初期化
        selectedItem = ItemManager.Instance.inventory[0]?.Item;
        selectRecipe = recipeSlots.Slots[0]?.WorkbenchRecipe;
        // イベント登録
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
        WorkbenchManager.Instance.OnChangeWorkingState.AddListener(OnWarkbenchStateChanged);
        inventoryDialogInstance.OnButtonClicked.AddListener(ItemSelectedFromInventory);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        // イベント解除
        ItemManager.Instance.OnChanged.RemoveListener(UpdateUI);
        WorkbenchManager.Instance.OnChangeWorkingState.RemoveListener(OnWarkbenchStateChanged);
        inventoryDialogInstance.OnButtonClicked.RemoveListener(ItemSelectedFromInventory);
    }
    private void Awake()
    {
        recipeSlots = new WorkbenchRecipeListUI(content, recipeButton);
    }
    public override void UpdateUI()
    {
        selectRecipe = _selectRecipe;
    }
    public void OnWarkbenchStateChanged()
    {
        requiredItemButton.SetItem(WorkbenchManager.Instance.WorkingRequiredItem);
        resultItemButton.SetItem(WorkbenchManager.Instance.WorkingResultItem);
        progressSlider.value = WorkbenchManager.Instance.WorkingProgress;
        progressSlider.maxValue = _selectRecipe != null ? _selectRecipe.time : 1;
    }

    #region Events
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
    /// <summary>
    /// クラフト停止(ReqiredItemButtonをクリックしたとき)
    /// </summary>
    public void Stop()
    {
        WorkbenchManager.Instance.StopCraftItem();
    }
    /// <summary>
    /// 完成品回収(ResultItemButtonをクリックしたとき)
    /// </summary>
    public void CollectResultItem()
    {
        WorkbenchManager.Instance.CollectResultItem();
    }
    #endregion
}
