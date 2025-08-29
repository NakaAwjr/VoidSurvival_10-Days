using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    //スタミナ
    public int MaxStaminaPoint => maxStaminaPoint;
    public int CurrentStaminaPoint => _currentStaminaPoint;

    [SerializeField] private int maxStaminaPoint = 100;
    private int _currentStaminaPoint;

    protected override void Start()
    {
        base.Start();
        _currentStaminaPoint = MaxStaminaPoint;
    }

    protected override int getPower()
    {
        //将来的に装備品やバフなどで変動する場合を考慮してオーバーライド
        return power;
    }
    protected override int getDefense()
    {
        //将来的に装備品やバフなどで変動する場合を考慮してオーバーライド
        return defense;
    }
}
