using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    // 攻撃力、防御力は後で書き換える、これは装備に依存するようにする

    public int MaxStaminaPoint => maxStaminaPoint;
    public int CurrentStaminaPoint => _currentStaminaPoint;

    [SerializeField] private int maxStaminaPoint = 100;
    private int _currentStaminaPoint;

    protected override void Start()
    {
        base.Start();
        _currentStaminaPoint = MaxStaminaPoint;
    }
}
