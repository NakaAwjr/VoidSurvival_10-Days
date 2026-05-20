using System.Collections.Generic;

/// <summary>
/// その他のバフを管理するクラス
/// </summary>
public static class OthersBuffManager
{
    public static List<BuffBase> AllBuffs => _allBuffs;
    private static List<BuffBase> _allBuffs = new List<BuffBase>();
    public static void AddBuff(BuffBase buff)
    {
        _allBuffs.Add(buff);
    }
    public static void RemoveBuff(BuffBase buff)
    {
        _allBuffs.Remove(buff);
    }
    public static List<BuffBase> GetBuffs(OthersBuffType buffType)
    {
        List<BuffBase> buffs = new List<BuffBase>();
        foreach (var buff in _allBuffs)
        {
            if (buff.BuffType.Equals(buffType) && buff.IsAcctive)
            {
                buffs.Add(buff);
            }
        }
        return buffs;
    }
    /// <summary>
    /// baseValueに対して適用されているバフをすべて考慮した最終的な値を返す
    /// </summary>
    /// <param name="buffType"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public static float GetBuffValue(OthersBuffType buffType, float baseValue)
    {
        var buffs = GetBuffs(buffType);
        float baseValueF = baseValue;
        // Multiply処理を先に適用し、その後Add処理を適用する
        foreach (var buff in buffs)
        {
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseValueF = baseValueF * (1 + buff.GetValue());
            }
        }
        foreach (var buff in buffs)
        {
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseValueF += buff.GetValue();
            }
        }
        return baseValueF;
    }
}