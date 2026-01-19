using System;
using System.Collections.Generic;

/// <summary>
/// ミッション管理クラス
/// ダメージの蓄積度合いの低下　一段階ごとに-5%
/// 放射線、磁場、温度によって変化する天気における、特定のモブ、素材、植物の出現率の増加　一段階ごとに＋25％
/// 放射線、磁場、温度によるダメージの蓄積度合いを減らす　一段階ごとに-5%
/// ジャイロ、電圧、電流の値による施設の恩恵の増加（メリットのある効果ならその値の絶対値を増やす、デメリットのある効果ならその値の絶対値を減らす）
/// 一段階ごとに±5％
/// 3段階目までアップグレードした場合、一日一回、その日が終わるまで天候をプレイヤーが指定したものに変える。
/// </summary>
class Mission : FacilityBase
{
    public override FacilityManager.FacilityType FacilityType => FacilityManager.FacilityType.Mission;
    public override String FacilityName { get; protected set; } = "ミッション施設";
    public override int MaxHealthPoint { get; protected set; } = 250;
    public override List<BuffBase> Buffs => new List<BuffBase>()
    {
        new DynamicBuff(() =>
        {
            if (IsBroken) return 0f;
            return -0.05f * Level;
        })
        {
            BuffType = BuffType.FacilityDamageAccumulation,
            OperationType = BuffOperationType.Multiply,
            Description = "FacilityDamageAccumulation reduced by " + (5 * Level) + "%"
        },
    };
}