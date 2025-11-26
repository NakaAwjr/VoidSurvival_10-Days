using System.Collections.Generic;

/// <summary>
/// 施設のバフ修正インターフェース
/// </summary>
interface IFacilityBuffModifier
{
    void ModifyBuffs(List<BuffBase> allBuffs);
}