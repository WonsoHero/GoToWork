using UnityEngine;

/// <summary>
///  Action에 parameter 하나만 전달되길래 두개 보내려고 대충 struct로 만듦
/// </summary>
public struct PlayerStateChangedParam
{
    public PlayerStateChangedParam(PlayerState oldState, PlayerState newState)
    {
        OldState = oldState;
        NewState = newState;
    }

    /// <summary>
    ///  변경 이전 상태
    /// </summary>
    public PlayerState OldState { get; }

    /// <summary>
    ///  변경 이후 상태
    /// </summary>
    public PlayerState NewState { get; }
}
