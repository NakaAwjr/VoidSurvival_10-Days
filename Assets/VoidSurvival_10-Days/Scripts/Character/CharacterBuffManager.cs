using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのバフ管理クラス
/// </summary>
public class CharacterBuffManager : MonoBehaviour
{
    public List<BuffBase> ActiveBuffs = new List<BuffBase>();
    public int GetStatusBuff(CharacterBuffType buffType, int baseValue)
    {
        var buffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(buffType));
        float baseValueF = baseValue;
        foreach (var buff in buffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseValueF = baseValueF * (1 + buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseValueF += buff.GetValue();
            }
        }
        return Mathf.RoundToInt(baseValueF);
    }
}