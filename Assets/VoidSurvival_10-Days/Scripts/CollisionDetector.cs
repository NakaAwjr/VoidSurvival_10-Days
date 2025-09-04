using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Collider2DのOnTriggerStayイベントを検知するコンポーネント
/// レイヤーの衝突関係に注意
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent onTriggerStay = new TriggerEvent();
    [SerializeField] private TriggerEvent onTriggerEnter = new TriggerEvent();
    private void OnTriggerStay2D(Collider2D other)
    {
        onTriggerStay.Invoke(other);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        onTriggerEnter.Invoke(other);
    }

    [Serializable]
    public class TriggerEvent : UnityEvent<Collider2D> { }
}
