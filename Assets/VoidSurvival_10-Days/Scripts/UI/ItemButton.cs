using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    public ItemStack ItemStack
    {
        get => _itemStack;
        set
        {
            _itemStack = value;
            if (_itemStack != null)
            {
                icon.sprite = _itemStack.Item.ItemIcon;
                amountText.text = _itemStack.Amount > 1 ? _itemStack.Amount.ToString() : string.Empty;
                gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = null;
                amountText.text = string.Empty;
                gameObject.SetActive(false);
            }
        }
    }
    private ItemStack _itemStack;
}
