using UnityEngine;

public class PlayerStatus : CharacterStatus
{
    // キャラクターのステータスがIdleまたはFatigueのときに移動可能
    public override bool IsMovable => _status == StatusEnum.Idle || _status == StatusEnum.Fatigue;
    // ダッシュ可能かどうか
    public bool IsDashing => _status == StatusEnum.Idle;
    // 入力反転(ビット反転による)
    public bool IsInversion = false;
    //スタミナ
    public int MaxStaminaPoint => getMaxStaminaPoint();
    public int CurrentStaminaPoint => _currentStaminaPoint;
    //スタミナ回復速度(時間経過で回復)
    public int StaminaHealSpeed => getStaminaHealSpeed();
    [SerializeField] private int staminaHealSpeed = 5;
    [SerializeField] private int maxStaminaPoint = 100;
    private int _currentStaminaPoint;

    private void Awake()
    {
        _currentHitPoint = maxHitPoint;
        _currentStaminaPoint = maxStaminaPoint;
    }
    protected override void Start()
    {
        _buffManager = GetComponent<CharacterBuffManager>();
        FacilityManager.Instance.AllBuffAddToPlayer();
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
        return _buffManager.GetStatusBuff(CharacterBuffType.Power, _power);
    }
    protected override int getDefense()
    {
        var _defense = defense;

        // この辺に装備品の影響を考慮するコードを追加

        return _buffManager.GetStatusBuff(CharacterBuffType.Defense, _defense);
    }
    protected int getMaxStaminaPoint()
    {
        return _buffManager.GetStatusBuff(CharacterBuffType.MaxStaminaPoint, maxStaminaPoint);
    }
    protected int getStaminaHealSpeed()
    {
        return _buffManager.GetStatusBuff(CharacterBuffType.StaminaHealSpeed, staminaHealSpeed);
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

    #region Save
    public void FromSaveData(PlayerSaveData saveData)
    {
        maxHitPoint = saveData.MaxHitPoint;
        maxStaminaPoint = saveData.MaxStaminaPoint;
        power = saveData.Power;
        defense = saveData.Defense;
        _currentHitPoint = saveData.CurrentHitPoint;
        _currentStaminaPoint = saveData.CurrentStaminaPoint;
        staminaHealSpeed = saveData.StaminaHealSpeed;
        transform.position = new Vector3(saveData.PositionX, saveData.PositionY, 0);
    }
    public PlayerSaveData ToSaveData()
    {
        PlayerSaveData saveData = new PlayerSaveData
        {
            MaxHitPoint = maxHitPoint,
            MaxStaminaPoint = maxStaminaPoint,
            Power = power,
            Defense = defense,
            CurrentHitPoint = _currentHitPoint,
            CurrentStaminaPoint = _currentStaminaPoint,
            PositionX = this.transform.position.x,
            PositionY = this.transform.position.y,
        };
        return saveData;
    }
    #endregion
}
