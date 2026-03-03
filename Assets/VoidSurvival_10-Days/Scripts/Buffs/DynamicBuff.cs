using System;

/// <summary>
/// 動的に値が変化するバフ
/// </summary>
public class DynamicBuff : BuffBase
{
    private Func<float> evaluate;
    public DynamicBuff(Func<float> evaluate)
    {
        this.evaluate = evaluate;
    }
    public override float GetValue()
    {
        return evaluate();
    }
}