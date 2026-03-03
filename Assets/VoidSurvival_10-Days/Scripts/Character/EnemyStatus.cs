using UnityEngine;

public class EnemyStatus : CharacterStatus
{
    [SerializeField] private float dieTime = 1.0f;
    public override void OnDie()
    {
        base.OnDie();
        // 一定時間後にオブジェクトを破棄
        Destroy(gameObject, dieTime);
    }
}
