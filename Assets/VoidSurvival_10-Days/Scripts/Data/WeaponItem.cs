using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item
{
    public int Power; // 武器の攻撃力
    public override bool CanUseOn()
    {
        // 武器は常に振るえる
        return true;
    }

    public override void Use(GameObject player)
    {
        var attack = player.GetComponent<CharactorAttack>();
        attack?.AttackIfPossible();
    }
}
