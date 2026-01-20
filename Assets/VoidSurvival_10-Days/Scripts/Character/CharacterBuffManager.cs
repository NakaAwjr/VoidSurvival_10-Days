using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのバフ管理クラス
/// </summary>
public class CharacterBuffManager : MonoBehaviour
{
    public List<BuffBase> ActiveBuffs => _activeBuffs;
    private List<BuffBase> _activeBuffs = new List<BuffBase>();
    /// <summary>
    /// バフを追加する
    /// </summary>
    /// <param name="buff"></param>
    public void AddBuff(BuffBase buff)
    {
        _activeBuffs.Add(buff);
    }
    /// <summary>
    /// バフを削除する
    /// </summary>
    public void RemoveBuff(BuffBase buff)
    {
        _activeBuffs.Remove(buff);
    }
    /// <summary>
    /// baseValueに対して適用されているバフをすべて考慮した最終的な値を返す
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
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