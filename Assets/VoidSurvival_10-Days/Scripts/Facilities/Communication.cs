using System;

/// <summary>
/// 通信施設
/// 通信で得られる情報の増加（初期段階で10個）　一段階ごとに＋20個
/// 3段階目までアップグレードしたときに得られる最後の情報は、隠しエンディングのイベントにする
/// 地上の基地のアンテナが一段階ごとに大きくなる
/// </summary>
class Communication : FacilityBase
{
    public override FacilityManager.FacilityType FacilityType => FacilityManager.FacilityType.Communication;
    public override String FacilityName { get; protected set; } = "通信施設";
    public override int MaxHealthPoint { get; protected set; } = 250;
}