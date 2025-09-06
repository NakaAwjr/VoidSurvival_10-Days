using TMPro;
using UnityEngine;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] private SaveDialog _dialog;
    [SerializeField] private TMP_Text text;
    public KeyValuePair<string, string> Pair
    {
        get => _pair;
        set
        {
            _pair = value;
            if (_pair.Value == "")
            {
                text.text = $"{_pair.Key}: No Data";
            }
            else
            {
                text.text = $"{_pair.Key}: {_pair.Value}";
            }
        }
    }
    private KeyValuePair<string, string> _pair;

    public void OnClick()
    {
        _dialog.ButtonSerected(_pair.Key);
    }
}