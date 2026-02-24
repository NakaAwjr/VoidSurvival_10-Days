using UnityEngine;

public class SettingDialog : Dialog
{
    public void OpenDialogID(DialogID id)
    {
        MainUI.Instance.OpenDialog(id);
    }
}
