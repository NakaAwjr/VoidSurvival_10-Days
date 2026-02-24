using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class RecipeButton : RecipeSlotUI
{
    [SerializeField] private CraftDialog dialog;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    public CraftingRecipe CraftingRecipe
    {
        get => _craftingRecipe;
        private set
        {
            _craftingRecipe = value;
            if (_craftingRecipe != null)
            {
                icon.sprite = _craftingRecipe.ResultItem.ItemIcon;
                text.text = _craftingRecipe.ResultItem.ItemName;
                gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = null;
                text.text = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }
    private CraftingRecipe _craftingRecipe;

    public override void SetRecipe(CraftingRecipe recipe)
    {
        CraftingRecipe = recipe;
    }

    public override void Clear()
    {
        CraftingRecipe = null;
    }

    public void OnClick()
    {
        dialog.RecipeSelected(CraftingRecipe);
    }
}
