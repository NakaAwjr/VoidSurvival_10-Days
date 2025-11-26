using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 施設管理クラス
/// </summary>
public class FacilityManager : MonoBehaviour
{
    public enum FacilityType
    {
        None,
        PowerSpupply,
        ThermalControl,
        Mission,
        Communication,
        OBC
    }
    public static FacilityManager Instance { get; private set; }

    [Header("施設一覧(OBCは別途参照用に保持)")]
    [SerializeField] private FacilityBase[] facilities;
    [SerializeField] private OBC obc;
    /// <summary>
    /// 毎分施設に与えるダメージ量
    /// 1分ごとのダメージ量：4　雷雨時：10　雨天時：8
    /// </summary>
    [SerializeField] private int damagePerMinute = 1;

    private List<BuffBase> allBuffs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        allBuffs = CollectAllBuffs();
    }

    private void Start()
    {
        // 毎分施設にダメージを与える
        TimeManager.Instance.OnMinuteChanged.AddListener(DamageFacilitys);
    }

    /// <summary>
    /// 指定した施設を取得する
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public FacilityBase GetFacility(FacilityType type)
    {
        if (type == FacilityType.OBC)
        {
            return obc;
        }
        foreach (var facility in facilities)
        {
            if (facility.FacilityType == type)
            {
                return facility;
            }
        }
        return null;
    }

    /// <summary>
    /// OBCの効果を載せる
    /// </summary>
    /// <returns></returns>
    public List<BuffBase> CollectAllBuffs()
    {
        List<BuffBase> _allBuffs = new List<BuffBase>();
        foreach (var facility in facilities)
        {
            foreach (var buff in facility.Buffs)
            {
                var modified = new DynamicBuff(() =>
                {
                    if (obc.IsBroken) return 0f;
                    if (buff.GetValue() < 0)
                    {
                        return buff.GetValue() - obc.GetOBCEffect();
                    }
                    else
                    {
                        return buff.GetValue() + obc.GetOBCEffect();
                    }
                })
                {
                    BuffType = buff.BuffType,
                    OperationType = buff.OperationType,
                    Description = buff.Description + " (Modified by OBC)"
                };
                _allBuffs.Add(modified);
            }
        }
        return _allBuffs;
    }

    /// <summary>
    /// 施設にダメージを与える
    /// </summary>
    private void DamageFacilitys()
    {
        // 衛星施設のダメージ蓄積量の増加　一段階ごとに＋25％(電源)
        var _powerSpupply = GetFacility(FacilityType.PowerSpupply) as PowerSpupply;
        var _damagePerMinute = (int)(damagePerMinute * (1 + 0.25f * _powerSpupply.Level));
        foreach (var facility in facilities)
        {
            facility.DamageFacility(_damagePerMinute);
        }
    }
    /// <summary>
    /// 全ての施設のバフをプレイヤーに付与する
    /// </summary>
    public void AllBuffAddToPlayer()
    {
        var buffManager = FindAnyObjectByType<PlayerStatus>().GetComponent<CharacterBuffManager>();
        foreach (var buff in allBuffs)
        {
            if (buff.BuffType is CharacterBuffType type)
            {
                buffManager.ActiveBuffs.Add(buff);
            }
        }
    }
}