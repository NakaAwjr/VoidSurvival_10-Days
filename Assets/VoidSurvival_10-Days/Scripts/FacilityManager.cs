using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private float damagePerMinute = 1;

    /// <summary>
    /// 全ての施設のバフ一覧
    /// </summary>
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
        AllBuffAddToOthers();
    }

    private void Start()
    {
        // 毎分施設にダメージを与える
        TimeManager.Instance.OnMinuteChanged.AddListener(DamageFacilities);
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
                    if (buff.GetValue() == 0) return 0f;
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
    private void DamageFacilities()
    {
        var _damagePerMinute = OthersBuffManager.GetBuffValue(OthersBuffType.FacilityDamageAccumulation, damagePerMinute);
        foreach (var facility in facilities)
        {
            facility.DamageFacility(_damagePerMinute);
        }
        obc.DamageFacility(_damagePerMinute);
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
                buffManager.AddBuff(buff);
            }
        }
    }
    /// <summary>
    /// 全ての施設のバフをその他バフマネージャーに付与する
    /// </summary>
    public void AllBuffAddToOthers()
    {
        foreach (var buff in allBuffs)
        {
            if (buff.BuffType is OthersBuffType type)
            {
                OthersBuffManager.AddBuff(buff);
            }
        }
    }

    #region SaveData
    /// <summary>
    /// セーブデータから施設情報を復元する
    /// </summary>
    /// <param name="facilityDatas"></param>
    public void FromSaveData(FacilitySaveData[] facilityDatas)
    {
        if (facilityDatas == null) return;
        foreach (var data in facilityDatas)
        {
            var facility = GetFacility(data.Type);
            if (facility != null)
            {
                facility.initialize(data.Level, data.CurrentHealthPoint);
            }
        }
    }
    /// <summary>
    /// 施設情報をセーブデータに変換する
    /// </summary>
    /// <returns></returns>
    public FacilitySaveData[] ToSaveData()
    {
        List<FacilitySaveData> facilityDatas = new List<FacilitySaveData>();
        foreach (var facility in facilities)
        {
            FacilitySaveData data = new FacilitySaveData
            {
                Type = facility.FacilityType,
                Level = facility.Level,
                CurrentHealthPoint = facility.CurrentHealthPoint
            };
            facilityDatas.Add(data);
        }
        // OBCのデータも追加
        FacilitySaveData obcData = new FacilitySaveData
        {
            Type = FacilityType.OBC,
            Level = obc.Level,
            CurrentHealthPoint = obc.CurrentHealthPoint
        };
        facilityDatas.Add(obcData);

        return facilityDatas.ToArray();
    }
    #endregion
}