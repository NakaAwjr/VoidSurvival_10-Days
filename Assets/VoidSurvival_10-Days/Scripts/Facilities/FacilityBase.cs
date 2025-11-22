using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 施設の基底クラス
/// </summary>
public abstract class FacilityBase : MonoBehaviour
{
    /// <summary>
    /// 施設が破壊されているかどうか
    /// </summary>
    public bool IsBroken => CurrentHealthPoint <= 0;

    #region Virtual Properties
    // 継承したらオーバーライドするプロパティ
    public abstract int MaxHealthPoint { get; protected set; }
    public abstract String FacilityName { get; protected set; }
    public abstract FacilityManager.FacilityType FacilityType { get; }
    /// <summary>
    /// 通常効果
    /// </summary>
    public virtual List<BuffBase> NormalBuffs => new List<BuffBase>();
    /// <summary>
    /// 施設が破壊されたときの効果
    /// </summary>
    public virtual List<BuffBase> BreakBuffs => new List<BuffBase>();
    #endregion

    protected const int MaxLevel = 4;
    public int Level { get; protected set; } = 1;
    public int CurrentHealthPoint => _currentHealthPoint;
    protected int _currentHealthPoint;
    /// <summary>
    /// 施設の効果
    /// HPが0より大きい場合は通常効果、0以下の場合は破壊効果を返す
    /// </summary>
    /// <returns></returns>
    public List<BuffBase> Buffs => CurrentHealthPoint > 0 ? NormalBuffs : BreakBuffs;

    #region Inspector Fields
    // インスペクターで設定する項目
    [SerializeField]
    protected List<ValueList<ItemStack>> requiredItemsForEachLevelUp;
    /// <summary>
    /// 施設のアップグレードに必要なアイテムリスト
    /// </summary>
    public List<List<ItemStack>> RequiredItemsForEachLevelUp
    {
        get
        {
            List<List<ItemStack>> result = new List<List<ItemStack>>();
            foreach (var valueList in requiredItemsForEachLevelUp)
            {
                result.Add(valueList.Values);
            }
            return result;
        }
    }
    [SerializeField]
    protected List<ItemStack> requiredItemsForRepair;
    /// <summary>
    /// 施設の修理に必要なアイテムリスト
    /// </summary>
    public List<ItemStack> RequiredItemsForRepair => requiredItemsForRepair;
    #endregion

    #region Methods
    void Start()
    {
        _currentHealthPoint = MaxHealthPoint;
    }
    /// <summary>
    /// 施設の初期化
    /// </summary>
    /// <param name="level"></param>
    public void initialize(int level, int currentHealthPoint)
    {
        Level = level;
        _currentHealthPoint = currentHealthPoint;
    }
    /// <summary>
    /// 施設がアップグレード可能かどうかをチェックする
    /// </summary>
    public bool CanUpgrade()
    {
        if (Level >= MaxLevel)
        {
            return false;
        }
        for (int i = 0; i < RequiredItemsForEachLevelUp[Level - 1].Count; i++)
        {
            var itemStack = RequiredItemsForEachLevelUp[Level - 1][i];
            var playerItemStack = ItemManager.Instance.GetItemStack(itemStack.Item);
            if (playerItemStack == null || playerItemStack.Amount < itemStack.Amount)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 施設をアップグレードする
    /// </summary>
    public virtual void UpgradeFacility()
    {
        if (!CanUpgrade())
        {
            Debug.LogWarning("Not enough resources to upgrade the facility or already at max level.");
            return;
        }
        foreach (var itemStack in RequiredItemsForEachLevelUp[Level - 1])
        {
            ItemManager.Instance.RemoveItem(itemStack.Item, itemStack.Amount);
        }
        Level++;
    }
    /// <summary>
    /// 施設を修理可能かどうかをチェックする
    /// </summary>
    public bool CanRepair()
    {
        for (int i = 0; i < RequiredItemsForRepair.Count; i++)
        {
            var itemStack = RequiredItemsForRepair[i];
            var playerItemStack = ItemManager.Instance.GetItemStack(itemStack.Item);
            if (playerItemStack == null || playerItemStack.Amount < itemStack.Amount)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 施設を修理する
    /// </summary>
    /// <param name="repairAmount"></param>
    public virtual void RepairFacility(int repairAmount)
    {
        if (!CanRepair())
        {
            Debug.LogWarning("Not enough resources to repair the facility.");
            return;
        }
        foreach (var itemStack in RequiredItemsForRepair)
        {
            ItemManager.Instance.RemoveItem(itemStack.Item, itemStack.Amount);
        }
        _currentHealthPoint += repairAmount;
    }
    /// <summary>
    /// 施設にダメージを与える
    /// </summary>
    /// <param name="damageAmount"></param>
    public virtual void DamageFacility(int damageAmount)
    {
        _currentHealthPoint -= damageAmount;
        if (_currentHealthPoint < 0)
        {
            _currentHealthPoint = 0;
        }
    }
    #endregion
}

/// <summary>
/// 汎用の多重リストクラス(インスペクター表示用)
/// </summary>
/// <typeparam name="T"></typeparam>
[SerializableAttribute]
public class ValueList<T>
{
    public List<T> Values = new List<T>();
    public ValueList(List<T> values)
    {
        Values = values;
    }
}