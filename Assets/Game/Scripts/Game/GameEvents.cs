using Arixen.ScriptSmith;
using UnityEngine;

public struct GameStateChangedEvent : IGameEventData
{
    public GameState NewState;
}

public struct ScoreUpdatedEvent : IGameEventData
{
    public int NewScore;
}

public struct PlayerJumpEvent : IGameEventData
{
    public Vector3 Position;
    public float JumpDuration;
}

public struct PlayerCollectEvent : IGameEventData
{
    public Vector3 Position;
    public int CollectibleID;
}

public struct PlayerCollideEvent : IGameEventData
{
    public Vector3 Position;
}

public struct RestartGameEvent : IGameEventData { }

public struct ExitGameEvent : IGameEventData { }