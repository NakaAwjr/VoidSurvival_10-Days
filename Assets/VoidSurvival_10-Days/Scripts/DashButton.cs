using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DashButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;
    private Image image;

    /// <summary>
    /// ダッシュ中かどうか
    /// </summary>
    public static bool IsDashing { get; private set; } = false;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsDashing = !IsDashing;
        if (IsDashing)
        {
            image.sprite = pressedSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
    }
}
