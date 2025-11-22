using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 作業台の作業時間の短縮　一段階ごとに-15％
/// 主人公のスタミナの最大値の上昇　一段階ごとに＋33％
/// 主人公のスタミナの回復速度の上昇　一段階ごとに+33%
/// 衛星施設のダメージ蓄積量の増加　一段階ごとに＋25％
/// 見た目の変更、地上の基地上部にある太陽光パネルの面積が一段階ごとに広くなる
/// </summary>
class PowerSpupply : FacilityBase
{
    public override FacilityManager.FacilityType FacilityType => FacilityManager.FacilityType.PowerSpupply;
    public override String FacilityName { get; protected set; } = "電源";
    public override int MaxHealthPoint { get; protected set; } = 200;
    public override List<BuffBase> NormalBuffs => new List<BuffBase>()
    {
        new StaminaBuff(this)
        {
            BuffType = CharacterBuffType.MaxStaminaPoint,
            OperationType = BuffOperationType.Multiply,
            Description = "MaxStaminaPoint increased by " + (33 * Level) + "%"
        },
        new StaminaBuff(this)
        {
            BuffType = CharacterBuffType.StaminaHealSpeed,
            OperationType = BuffOperationType.Multiply,
            Description = "StaminaHealSpeed increased by " + (33 * Level) + "%"
        }
    };

    // バフ
    public class StaminaBuff : CharacterBuff
    {
        private PowerSpupply _powerSpupply;
        public StaminaBuff(PowerSpupply powerSpupply)
        {
            _powerSpupply = powerSpupply;
        }
        public override float GetValue()
        {
            return 1 + (0.33f * _powerSpupply.Level);
        }
    }
}