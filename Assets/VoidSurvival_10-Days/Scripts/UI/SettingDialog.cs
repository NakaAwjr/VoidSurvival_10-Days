using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingDialog : Dialog
{
    [SerializeField] private Dialog saveDialog;

    public void OpenSaveDialog()
    {
        saveDialog.OpenDialog();
    }
}
