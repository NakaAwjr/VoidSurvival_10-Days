using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealItem", menuName = "Items/HealItem")]
public class HealItem : Item
{
    public enum HealType
    {
        Health,
        Stamina
    }
    public HealType TypeOfHeal = HealType.Health;
    public int HealAmount; // 回復量
    public override bool CanUseOn()
    {
        return true;
    }

    public override void Use(GameObject player)
    {
        var status = player.GetComponent<PlayerStatus>();
        if (status != null)
        {
            switch (TypeOfHeal)
            {
                case HealType.Health:
                    status.Heal(HealAmount);
                    break;
                case HealType.Stamina:
                    status.RecoverStamina(HealAmount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ItemManager.Instance.RemoveItem(this, 1);
        }
    }
}
