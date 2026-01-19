using UnityEngine;

[CreateAssetMenu(fileName = "DialogID", menuName = "UI/DialogID", order = 1)]
public class DialogID : ScriptableObject { }

[System.Serializable]
public class DialogEntry
{
    public DialogID dialogID;
    public Dialog dialog;
}