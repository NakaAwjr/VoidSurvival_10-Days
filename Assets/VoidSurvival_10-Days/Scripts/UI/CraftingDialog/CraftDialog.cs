using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftDialog : Dialog
{
    [Header("スロット参照")]
    [SerializeField] private RectTransform recipeContent;
    [SerializeField] private RectTransform requiredItemContent;
    [SerializeField] private RecipeButton recipeButtonTemplate;
    [SerializeField] private ItemButton requiredItemButtonTemplate;
    [Header("UI参照")]
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDescription;

    [SerializeField] private Slider craftCountSlider;
    [SerializeField] private TMP_Text craftCountText;

    private RecipeListUI recipeSlots;
    private RequiredItemListUI requiredItemSlots;

    private int _craftCount;
    private int craftCount
    {
        get => _craftCount;
        set
        {
            _craftCount = value;
            craftCountText.text = _craftCount.ToString();
            // 必要アイテムリスト更新
            requiredItemSlots.Refresh(_selectRecipe.RequiredItems, _craftCount);
        }
    }
    private CraftingRecipe _selectRecipe;
    private CraftingRecipe selectRecipe
    {
        get => _selectRecipe;
        set
        {
            _selectRecipe = value;
            if (_selectRecipe != null)
            {
                // 選択されたアイテム情報を表示
                selectedIcon.sprite = _selectRecipe.ResultItem.ItemIcon;
                selectedItemName.text = _selectRecipe.ResultItem.ItemName;
                selectedItemDescription.text = _selectRecipe.ResultItem.ItemDescription;
                // 可能なクラフト数を設定
                craftCountSlider.maxValue = CraftingManager.Instance.NumberCanCrafting(_selectRecipe) > 0 ? CraftingManager.Instance.NumberCanCrafting(_selectRecipe) : 1;
                craftCountSlider.value = 1;
                craftCount = (int)craftCountSlider.value;
                craftCountText.text = craftCountSlider.value.ToString();
                // 必要なアイテムを表示
                var requiredItems = _selectRecipe.RequiredItems;
                // 必要アイテムリスト更新
                requiredItemSlots.Refresh(requiredItems, craftCount);
            }
            else
            {
                selectedIcon.sprite = null;
                selectedItemName.text = "No Selection";
                selectedItemDescription.text = string.Empty;
                craftCountSlider.maxValue = 1;
                craftCountSlider.value = 1;
                requiredItemSlots.ClearFrom(0);
            }
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        // イベント登録
        ItemManager.Instance.OnChanged.AddListener(UpdateUI);
        CraftingManager.Instance.OnChanged.AddListener(UpdateUI);
        // スライダー初期化
        craftCountSlider.onValueChanged.AddListener(OnCraftCountSliderChanged);
        craftCountSlider.wholeNumbers = true;
        craftCountSlider.minValue = 1;
        craftCountSlider.maxValue = 1;
        craftCountSlider.value = 1;
        // 選択状態を初期化
        selectRecipe = recipeSlots.Slots.Count > 0 ? recipeSlots.Slots[0].CraftingRecipe : null;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        // イベント解除
        ItemManager.Instance.OnChanged.RemoveListener(UpdateUI);
        CraftingManager.Instance.OnChanged.RemoveListener(UpdateUI);
        craftCountSlider.onValueChanged.RemoveListener(OnCraftCountSliderChanged);
    }
    private void Awake()
    {
        // スロットコンテナ初期化
        recipeSlots = new RecipeListUI(recipeContent, recipeButtonTemplate);
        requiredItemSlots = new RequiredItemListUI(requiredItemContent, requiredItemButtonTemplate);
    }
    public override void OpenDialog()
    {
        base.OpenDialog();
    }
    public override void UpdateUI()
    {
        var craftingRecipes = CraftingManager.Instance.craftingRecipes;
        selectRecipe = _selectRecipe;
        // レシピスロット更新
        recipeSlots.Refresh(craftingRecipes);
    }

    #region Button Callbacks
    /// <summary>
    /// レシピが選択されたとき(ボタン)
    /// </summary>
    /// <param name="recipe"></param>
    public void RecipeSelected(CraftingRecipe recipe)
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
            CraftingManager.Instance.CraftItem(_selectRecipe, _craftCount);
        }
    }
    #endregion
    /// <summary>
    /// クラフト数スライダー変更時に呼ばれる
    /// </summary>
    /// <param name="value"></param>
    public void OnCraftCountSliderChanged(float value)
    {
        craftCount = (int)value;
    }
}
