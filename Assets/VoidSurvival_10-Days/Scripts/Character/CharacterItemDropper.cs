using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus))]
public class CharacterItemDropper : MonoBehaviour
{
    [Header("ドロップアイテム(デフォルト)")]
    [SerializeField] private List<DropItemInfo> dropItems;
    [Header("ドロップアイテム(天気ごと)")]
    [SerializeField] private List<WeatherDropItem> weatherDropItems;
    [SerializeField] private FieldItem fieldItemPrefab;
    private CharacterStatus _status;
    private bool _isDropInvoked;
    private List<DropItemInfo> _items;
    private void Start()
    {
        _status = GetComponent<CharacterStatus>();
        _status.onDie.AddListener(DropIfNeeded);
        _items = dropItems;
        foreach (var value in weatherDropItems)
        {
            if (EqualityComparer<Weather>.Default.Equals(WeatherTester.Instance.CurrentRuleBasedWeather, value.weather))
            {
                _items = value.dropItems;
            }
        }
    }

    private void DropIfNeeded()
    {
        if (_isDropInvoked) return;
        _isDropInvoked = true;
        foreach (var drop in _items)
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
[System.Serializable]
class WeatherDropItem
{
    public Weather weather;
    public List<DropItemInfo> dropItems;
}