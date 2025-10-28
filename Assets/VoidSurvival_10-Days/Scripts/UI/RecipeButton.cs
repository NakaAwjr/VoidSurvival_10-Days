using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class RecipeButton : MonoBehaviour
{
    [SerializeField] private CraftDialog dialog;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    public CraftingRecipe CraftingRecipe
    {
        get => _craftingRecipe;
        set
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

    public void OnClick()
    {
        dialog.RecipeSelected(CraftingRecipe);
    }
}
