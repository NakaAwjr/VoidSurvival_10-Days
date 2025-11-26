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
    // public override List<BuffBase> NormalBuffs => new List<BuffBase>()
    // {
    //     new WorkbenchBuff(this)
    //     {
    //         BuffType = CharacterBuffType.WorkbenchWorkTime,
    //         OperationType = BuffOperationType.Multiply,
    //         Description = "WorkbenchWorkTime decreased by " + (15 * Level) + "%"
    //     }
    // };

    // バフ
    // public class WorkbenchBuff : CharacterBuff
    // {
    //     private ThermalControl _thermalControl;
    //     public WorkbenchBuff(ThermalControl thermalControl)
    //     {
    //         _thermalControl = thermalControl;
    //     }
    //     public override float GetValue()
    //     {
    //         return 1 - (0.15f * _thermalControl.Level);
    //     }
    // }
}