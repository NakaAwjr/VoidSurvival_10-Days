using UnityEngine;

public class SaveDialog : Dialog
{
    [SerializeField] private GameObject viewPortContent;
    private SaveSlotButton[] _saveSlotButtons;
    private string _selected;

    protected override void Start()
    {
        base.Start();
        _saveSlotButtons = viewPortContent.GetComponentsInChildren<SaveSlotButton>();
        OpenDialog();
    }
    /// <summary>
    /// 押されたボタンのkeyを取得
    /// </summary>
    /// <param name="str"></param>
    public void ButtonSerected(string str)
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