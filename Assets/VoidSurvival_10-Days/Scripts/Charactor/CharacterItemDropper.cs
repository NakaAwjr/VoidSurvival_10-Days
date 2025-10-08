using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus))]
public class CharacterItemDropper : MonoBehaviour
{
    [SerializeField] private List<DropItemInfo> dropItems;
    [SerializeField] private FieldItem fieldItemPrefab;
    private CharacterStatus _status;
    private bool _isDropInvoked;
    private void Start()
    {
        _status = GetComponent<CharacterStatus>();
        _status.onDie.AddListener(DropIfNeeded);
    }

    private void DropIfNeeded()
    {
        if (_isDropInvoked) return;
        _isDropInvoked = true;
        foreach (var drop in dropItems)
        {
            if (Random.Range(0, 1f) >= drop.dropRate) break;
            for (int i = 0; i < drop.number; i++)
            {
                var item = Instantiate(fieldItemPrefab, transform.position, Quaternion.identity);
                item.Item = drop.item;
                item.Initialize();
            }
        }
    }
}

[System.Serializable]
class DropItemInfo
{
    [Range(0, 1)] public float dropRate;
    public Item item;
    public int number = 1;
}
