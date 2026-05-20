using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponItem", menuName = "Items/WeaponItem")]
public class WeaponItem : Item
{
    public int Power; // 武器の攻撃力
    public int StaminaCost; // 武器のスタミナ消費量
    public override bool CanUseOn()
    {
        // 武器は常に振るえる
        return true;
    }

    public override void Use(GameObject player)
    {
        var renderer = player.GetComponent<PlayerRenderer>();
        var playerStatus = player.GetComponent<PlayerStatus>();
        if (!playerStatus.IsActive || playerStatus.CurrentStaminaPoint < StaminaCost) return;
        renderer.SetAttackAnimation();
        playerStatus.GoToActiveStateIfPossible();
        playerStatus.ConsumeStamina(StaminaCost);
    }
}
