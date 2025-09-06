using UnityEngine;
using UnityEngine.Events;

public abstract class CharacterStatus : MonoBehaviour
{
    protected enum StatusEnum
    {
        Idle,
        Active,
        Dead
    }


    // キャラクターのステータスがIdleのときに移動、行動可能
    public bool IsMoovable => _status == StatusEnum.Idle;
    public bool IsActive => _status == StatusEnum.Idle;

    public int MaxHitPoint => maxHitPoint;
    public int CurrentHealth => _currentHitPoint;
    public int Power => getPower();
    public int Defense => getDefense();

    [SerializeField] protected int maxHitPoint = 100;
    [SerializeField] protected int power = 10;
    [SerializeField] protected int defense = 5;

    protected int _currentHitPoint;
    protected StatusEnum _status = StatusEnum.Idle;

    public UnityEvent onDie;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHitPoint = MaxHitPoint;
    }

    /// <summary>
    /// キャラクターがダメージを受けたときの処理
    /// ダメージは防御力を考慮して計算される
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        if (_status == StatusEnum.Dead) return;

        int actualDamage = Mathf.Max(damage - Defense, 0);
        _currentHitPoint -= actualDamage;

        if (_currentHitPoint <= 0)
        {
            _currentHitPoint = 0;
            _status = StatusEnum.Dead;
            OnDie();
        }
    }
    /// <summary>
    /// キャラクターが回復したときの処理
    /// 回復量は最大HPを超えないように制限される
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int amount)
    {
        if (_status == StatusEnum.Dead) return;

        _currentHitPoint = Mathf.Min(_currentHitPoint + amount, MaxHitPoint);
    }
    /// <summary>
    /// 死んだときの処理
    /// </summary>
    public virtual void OnDie()
    {
        onDie?.Invoke();
    }

    /// <summary>
    /// アクティブ状態に移行する処理
    /// アクティブ状態では他の行動はできない
    /// </summary>
    public void GoToActiveStatefPossible()
    {
        if (!IsActive) return;

        _status = StatusEnum.Active;
    }
    /// <summary>
    /// 待機状態に戻る処理
    /// </summary>
    public void GoToIdleStateIfPossible()
    {
        if (_status == StatusEnum.Dead) return;

        _status = StatusEnum.Idle;
    }

    protected virtual int getPower()
    {
        return power;
    }
    protected virtual int getDefense()
    {
        return defense;
    }
}
