using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    // キャラクターのステータスがIdleまたはFatigueのときに移動可能
    public override bool IsMoovable => _status == StatusEnum.Idle || _status == StatusEnum.Fatigue;
    //スタミナ
    public int MaxStaminaPoint => getMaxStaminaPoint();
    public int CurrentStaminaPoint => _currentStaminaPoint;
    //スタミナ回復速度(時間経過で回復)
    public int StaminaHealSpeed => getStaminaHealSpeed();
    [SerializeField] private int staminaHealSpeed = 5;
    [SerializeField] private int maxStaminaPoint = 100;
    private int _currentStaminaPoint;

    protected override void Start()
    {
        base.Start();
        FacilityManager.Instance.AllBuffAddToPlayer();
        _currentStaminaPoint = MaxStaminaPoint;
        //時間経過でスタミナ回復、ゲーム内時間か現実時間かどうしようか？
        TimeManager.Instance.OnMinuteChanged.AddListener(() =>
        {
            RecoverStamina(StaminaHealSpeed);
        });
    }

    #region Getters
    protected override int getPower()
    {
        var _power = power;
        // 装備品の影響を考慮
        var weapon = ItemManager.Instance.quickItems[ItemManager.Instance.selectedQuickItemIndex]?.Item as WeaponItem;
        if (weapon != null)
        {
            _power += weapon.Power;
        }
        return _buffManager.GetPowerBuff(_power);
    }
    protected override int getDefense()
    {
        var _defense = defense;

        // この辺に装備品の影響を考慮するコードを追加

        return _buffManager.GetDefenseBuff(_defense);
    }
    protected int getMaxStaminaPoint()
    {
        return _buffManager.GetMaxStaminaPointBuff(maxStaminaPoint);
    }
    protected int getStaminaHealSpeed()
    {
        return _buffManager.GetStaminaHealSpeedBuff(staminaHealSpeed);
    }
    #endregion

    #region Methods
    /// <summary>
    /// スタミナを消費する処理
    /// </summary>
    /// <param name="amount"></param>
    public void ConsumeStamina(int amount)
    {
        _currentStaminaPoint = Mathf.Max(_currentStaminaPoint - amount, 0);
        if (_currentStaminaPoint == 0)
        {
            GoToFatigueStateIfPossible();
        }
    }
    /// <summary>
    /// スタミナを回復する処理
    /// </summary>
    /// <param name="amount"></param>
    public void RecoverStamina(int amount)
    {
        _currentStaminaPoint = Mathf.Min(_currentStaminaPoint + amount, MaxStaminaPoint);
        if (_currentStaminaPoint > 0 && _status == StatusEnum.Fatigue)
        {
            GoToIdleStateIfPossible();
        }
    }
    public override void GoToIdleStateIfPossible()
    {
        if (_status == StatusEnum.Dead || (_status == StatusEnum.Fatigue && CurrentStaminaPoint == 0)) return;
        _status = StatusEnum.Idle;
    }
    /// <summary>
    /// スタミナ切れ状態に移行する処理
    /// </summary>
    public void GoToFatigueStateIfPossible()
    {
        if (_status == StatusEnum.Dead) return;
        _status = StatusEnum.Fatigue;
    }
    #endregion
}
