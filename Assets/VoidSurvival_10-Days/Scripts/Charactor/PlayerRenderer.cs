public class PlayerRenderer : CharacterRenderer
{
    //プレイヤー特有の動き
    public enum ActionType
    {
        Till
    }
    private static readonly string[] tillDirections = { "Till N", "Till S", "Till W" };

    /// <summary>
    /// アクションアニメーションを設定する
    /// アクションタイプに応じて異なるアニメーションを再生
    /// </summary>
    public void SetActionAnimation(ActionType actionType)
    {
        if (_characterStatus.IsActive)
        {
            switch (actionType)
            {
                case ActionType.Till:
                    _animator.Play(tillDirections[lastDirection]);
                    break;
            }
        }
    }
}
