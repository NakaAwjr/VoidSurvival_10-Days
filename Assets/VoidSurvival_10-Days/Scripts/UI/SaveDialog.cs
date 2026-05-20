using UnityEngine;

public class SaveDialog : Dialog
{
    [SerializeField] private GameObject viewPortContent;
    private SaveSlotButton[] _saveSlotButtons;
    private string _selected;

    private void Awake()
    {
        _saveSlotButtons = viewPortContent.GetComponentsInChildren<SaveSlotButton>();
    }

    /// <summary>
    /// 押されたボタンのkeyを取得
    /// </summary>
    /// <param name="str"></param>
    public void ButtonSelected(string str)
    {
        _selected = str;
    }

    public override void UpdateUI()
    {
        for (int i = 0; i < SaveManager.SaveSlotNumber; i++)
        {
            _saveSlotButtons[i].Pair = SaveManager.Instance.saveKeys.keyValuePairs[i];
        }
    }

    public void Save()
    {
        SaveManager.Instance.SaveGameAsync(_selected);
        UpdateUI();
    }
    public void Load()
    {
        SaveManager.Instance.LoadGameAsync(_selected);
    }
}