using System;
using System.Collections.Generic;

/// <summary>
/// OBC施設
/// 以上述べてきた「％」と書かれた効果の増加（メリット、デメリット含め、その値の絶対値を増やす）　一段階ごとに＋10％
/// 故障：
/// 電源、ミッション、通信から得られる効果がなくなる（天候操作、通信も無くなる）
/// </summary>
class OBC : FacilityBase
{
    public override FacilityManager.FacilityType FacilityType => FacilityManager.FacilityType.OBC;
    public override String FacilityName { get; protected set; } = "OBC施設";
    public override int MaxHealthPoint { get; protected set; } = 150;
    public float GetOBCEffect()
    {
        if (IsBroken) return 0f;
        return 0.1f * Level;
    }
}