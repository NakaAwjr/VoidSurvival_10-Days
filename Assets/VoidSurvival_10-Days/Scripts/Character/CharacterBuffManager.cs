using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのバフ管理クラス
/// </summary>
public class CharacterBuffManager : MonoBehaviour
{
    public List<CharacterBuff> ActiveBuffs = new List<CharacterBuff>();
    public int GetPowerBuff(int basePower)
    {
        var powerBuffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(CharacterBuffType.Power));
        foreach (var buff in powerBuffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                basePower = (int)(basePower * buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                basePower += (int)buff.GetValue();
            }
        }
        return basePower;
    }
    public int GetDefenseBuff(int baseDefense)
    {
        var defenseBuffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(CharacterBuffType.Defense));
        foreach (var buff in defenseBuffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseDefense = (int)(baseDefense * buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseDefense += (int)buff.GetValue();
            }
        }
        return baseDefense;
    }
    public int GetMaxHitPointBuff(int baseMaxHitPoint)
    {
        var maxHitPointBuffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(CharacterBuffType.MaxHitPoint));
        foreach (var buff in maxHitPointBuffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseMaxHitPoint = (int)(baseMaxHitPoint * buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseMaxHitPoint += (int)buff.GetValue();
            }
        }
        return baseMaxHitPoint;
    }
    public int GetMaxStaminaPointBuff(int baseMaxStaminaPoint)
    {
        var maxStaminaPointBuffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(CharacterBuffType.MaxStaminaPoint));
        foreach (var buff in maxStaminaPointBuffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseMaxStaminaPoint = (int)(baseMaxStaminaPoint * buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseMaxStaminaPoint += (int)buff.GetValue();
            }
        }
        return baseMaxStaminaPoint;
    }
    public int GetStaminaHealSpeedBuff(int baseStaminaHealSpeed)
    {
        var staminaHealSpeedBuffs = ActiveBuffs.FindAll(b => b.BuffType.Equals(CharacterBuffType.StaminaHealSpeed));
        foreach (var buff in staminaHealSpeedBuffs)
        {
            // Multiply処理を先に適用し、その後Add処理を適用する
            if (buff.OperationType == BuffOperationType.Multiply)
            {
                baseStaminaHealSpeed = (int)(baseStaminaHealSpeed * buff.GetValue());
            }
            if (buff.OperationType == BuffOperationType.Add)
            {
                baseStaminaHealSpeed += (int)buff.GetValue();
            }
        }
        return baseStaminaHealSpeed;
    }
}