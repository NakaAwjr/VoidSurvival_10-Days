using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private SaveDialog saveDialog;
    void Update()
    {
        if (Input.anyKeyDown)
        {
            saveDialog.OpenDialog();
        }
    }
}
