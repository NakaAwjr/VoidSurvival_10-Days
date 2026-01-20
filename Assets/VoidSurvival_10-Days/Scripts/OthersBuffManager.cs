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
}