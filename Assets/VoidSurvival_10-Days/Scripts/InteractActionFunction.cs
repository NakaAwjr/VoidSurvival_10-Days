using UnityEngine.Events;

public class InteractActionFunction : InteractAction
{
    public UnityEvent onAction;
    public override void Action()
    {
        onAction?.Invoke();
    }
}
