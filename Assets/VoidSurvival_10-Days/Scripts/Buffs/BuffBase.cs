using System;

/// <summary>
/// バフの適用方法
/// add: 加算
/// multiply: 乗算
/// </summary>
public enum BuffOperationType
{
    Add,
    Multiply,
}

/// <summary>
/// キャラクターに適用されるバフの種類
/// </summary>
public enum CharacterBuffType
{
    MaxHitPoint,
    Power,
    Defense,
    MaxStaminaPoint,
    StaminaHealSpeed,
}

/// <summary>
/// その他のバフの種類
/// </summary>
public enum OthersBuffType
{
    WorkbenchEfficiency,
    FacilityDamageAccumulation,
}

[Serializable]
public abstract class BuffBase
{
    public Enum BuffType { get; set; }
    public virtual bool IsActive { get; set; } = true;
    public abstract float GetValue();
    public BuffOperationType OperationType { get; set; }
    public String Description { get; set; }
}