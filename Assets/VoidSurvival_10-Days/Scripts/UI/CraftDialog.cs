using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftDialog : Dialog
{
    [SerializeField] private GameObject vewportContent;
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDescription;
    [SerializeField] private GameObject requiredItemPanel;
    [SerializeField] private Slider craftCountSlider;
    [SerializeField] private TMP_Text craftCountText;

    [SerializeField] private RecipeButton recipeButton;
    [SerializeField] private ItemButton requiredItemButton;
    private RecipeButton[] _recipeButtons;
    private ItemButton[] _requireItemButtons;

    private int _craftCount;
    private int craftCount
    {
        get => _craftCount;
        set
        {
            _craftCount = value;
            craftCountText.text = _craftCount.ToString();
            for (int i = 0; i < _requireItemButtons.Length; i++)
            {
                if (_selectRecipe != null && i < _selectRecipe.RequiredItems.Count)
                {
                    var required = _selectRecipe.RequiredItems[i];
                    _requireItemButtons[i].ItemStack = new ItemStack(required.Item, required.Amount * _craftCount);
                }
                else
                {
                    _requireItemButtons[i].ItemStack = null;
                }
            }
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
                craftCountSlider.maxValue = CraftingManager.Instance.NumberCanCrafting(_selectRecipe);
                craftCountSlider.value = craftCountSlider.maxValue > 0 ? 1 : 0;
                craftCountText.text = craftCountSlider.value.ToString();
                // 必要なアイテムを表示
                var requiredItems = _selectRecipe.RequiredItems;
                // アイテムボタンの数が必要なアイテム数より少ない場合、追加する
                if (_requireItemButtons.Length < requiredItems.Count)
                {
                    for (int i = _requireItemButtons.Length; i < requiredItems.Count; i++)
                    {
                        Instantiate(requiredItemButton, requiredItemPanel.transform);
                    }
                    _requireItemButtons = requiredItemPanel.GetComponentsInChildren<ItemButton>();
                }
                // 必要なアイテムを割り当てる
                for (int i = 0; i < _requireItemButtons.Length; i++)
                {
                    if (i < requiredItems.Count)
                    {
                        _requireItemButtons[i].ItemStack = new ItemStack(requiredItems[i].Item, requiredItems[i].Amount);
                    }
                    else
                    {
                        _requireItemButtons[i].ItemStack = null;
                    }
                }
            }
            else
            {
                selectedIcon.sprite = null;
                selectedItemName.text = "No Selection";
                selectedItemDescription.text = string.Empty;
                craftCountSlider.maxValue = 0;
                craftCountSlider.value = 0;

                foreach (var button in _requireItemButtons)
                {
                    button.ItemStack = null;
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        _recipeButtons = vewportContent.GetComponentsInChildren<RecipeButton>();
        _requireItemButtons = requiredItemPanel.GetComponentsInChildren<ItemButton>();
        // スライダー初期化
        craftCountSlider.onValueChanged.AddListener(OnCraftCountSliderChanged);
        craftCountSlider.wholeNumbers = true;
        craftCountSlider.minValue = 0;
        craftCountSlider.maxValue = 0;
        craftCountSlider.value = 0;
        // 選択状態を初期化
        selectRecipe = _recipeButtons[0]?.CraftingRecipe;
    }
    public override void OpenDialog()
    {
        base.OpenDialog();
        // 選択状態を初期化
        selectRecipe = _recipeButtons[0]?.CraftingRecipe;
    }
    public override void UpdateUI()
    {
        var craftingRecipes = CraftingManager.Instance.craftingRecipes;
        selectRecipe = _selectRecipe;
        // レシピボタンの数がレシピ数より少ない場合、追加する
        if (_recipeButtons.Length < craftingRecipes.Count)
        {
            for (int i = _recipeButtons.Length; i < craftingRecipes.Count; i++)
            {
                Instantiate(recipeButton, vewportContent.transform);
            }
            _recipeButtons = vewportContent.GetComponentsInChildren<RecipeButton>();
        }
        // レシピボタンにレシピを割り当てる
        for (int i = 0; i < _recipeButtons.Length; i++)
        {
            if (i < craftingRecipes.Count)
            {
                _recipeButtons[i].CraftingRecipe = craftingRecipes[i];
            }
            else
            {
                _recipeButtons[i].CraftingRecipe = null;
            }
        }
    }

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
            UpdateUI();
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
}
