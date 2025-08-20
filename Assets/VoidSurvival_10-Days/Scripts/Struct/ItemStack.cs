using System;
using UnityEngine;

[Serializable]
public class ItemStack
{
    public Item Item => item;
    public int Amount => amount;
    [SerializeField] private Item item;
    [SerializeField] private int amount;

    public ItemStack(Item item, int amount)
    {
        this.item = item ?? throw new ArgumentNullException(nameof(item), "Item cannot be null");
        this.amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero");
    }
    public void Add(int amount)
    {
        this.amount += amount;
    }
    public void Remove(int amount)
    {
        this.amount -= amount;
    }
}