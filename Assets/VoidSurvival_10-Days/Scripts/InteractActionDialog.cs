using System.Collections.Generic;

public class InteractActionDialog : InteractAction
{
    public List<DialogID> dialogIDs;
    public override void Action()
    {
        if (dialogIDs.Count > 1)
        {
            MainUI.Instance.OpenGroup(dialogIDs.ToArray());
        }
        else if (dialogIDs.Count == 1)
        {
            MainUI.Instance.OpenDialog(dialogIDs[0]);
        }
    }
}
