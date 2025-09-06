using DG.Tweening;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public Item Item
    {
        get => _item;
        set
        {
            _item = value;
            var image = GetComponent<SpriteRenderer>();
            image.sprite = _item.ItemIcon;
        }
    }
    private Item _item;

    public void Initialize()
    {
        var colliderCache = GetComponent<Collider2D>();
        colliderCache.enabled = false;
        var transformCache = transform;
        var dropPosition = transform.localPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        transformCache.DOLocalMove(dropPosition, 0.5f);
        var defaultScale = transform.localScale;
        transformCache.localScale = Vector3.zero;
        transformCache.DOScale(defaultScale, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            colliderCache.enabled = true;
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        if (player == null) return;
        ItemManager.Instance.AddItem(_item);
        Destroy(gameObject);
    }
}
