using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private PlayerController playerController;

    // このフラグはドラッグ中にクリックイベントを無効にするために使用されます
    private bool _isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    /// ボタンがクリックされたときの処理
    public void OnPointerUp(PointerEventData eventData)
    {
        // ここにボタンがクリックされたときの処理を記述
        if (!_isDragging)
        {
            playerController.ActInteract();
        }
    }

    /// ボタンがドラッグされ始めたら、クリック機能を停止させる
    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    /// ドラッグが終了したときに、クリック機能を再度有効にする
    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }
}
