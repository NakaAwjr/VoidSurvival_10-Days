using System;
using System.Collections.Generic;


/// <summary>
/// 熱制御施設のクラス
/// ダメージの蓄積量の低下　一段階ごとに-15%
/// 作業台の作業時間の短縮　一段階ごとに-10%
/// 故障：
/// 蓄積ダメージ量が＋100％
/// </summary>
class ThermalControl : FacilityBase
{
    public override FacilityManager.FacilityType FacilityType => FacilityManager.FacilityType.ThermalControl;
    public override String FacilityName { get; protected set; } = "熱制御";
    public override int MaxHealthPoint { get; protected set; } = 300;
    public override List<BuffBase> Buffs => new List<BuffBase>()
    {
        new DynamicBuff(() =>
        {
            if (IsBroken) return 0f;
            return -0.15f * Level;
        })
        {
            BuffType = OthersBuffType.FacilityDamageAccumulation,
            OperationType = BuffOperationType.Multiply,
            Description = "FacilityDamageAccumulation reduced by " + (15 * Level) + "%"
        },
        new DynamicBuff(() =>
        {
            if (IsBroken) return 0f;
            return -0.10f * Level;
        })
        {
            BuffType = OthersBuffType.WorkbenchEfficiency,
            OperationType = BuffOperationType.Multiply,
            Description = "WorkbenchEfficiency increased by " + (10 * Level) + "%"
        },
        // デバフ
        new DynamicBuff(() =>
        {
            if (!IsBroken) return 0f;
            return 1.0f; // +100%
        })
        {
            BuffType = OthersBuffType.FacilityDamageAccumulation,
            OperationType = BuffOperationType.Multiply,
            Description = "FacilityDamageAccumulation increased by 100% when broken"
        }
    };
}