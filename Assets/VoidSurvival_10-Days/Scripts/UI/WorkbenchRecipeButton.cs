using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class WorkbenchRecipeButton : SlotUIBase
{
    [SerializeField] private WorkbenchDialog dialog;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    public WorkbenchRecipe WorkbenchRecipe
    {
        get => _workbenchRecipe;
        private set
        {
            _workbenchRecipe = value;
            if (_workbenchRecipe != null)
            {
                icon.sprite = _workbenchRecipe.ResultItem.ItemIcon;
                text.text = _workbenchRecipe.ResultItem.ItemName;
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
    private WorkbenchRecipe _workbenchRecipe;

    public void SetRecipe(WorkbenchRecipe recipe)
    {
        WorkbenchRecipe = recipe;
    }
    public override void Clear()
    {
        WorkbenchRecipe = null;
    }
    public void OnClick()
    {
        dialog.RecipeSelected(WorkbenchRecipe);
    }
}
