using UnityEngine;

[System.Serializable]
public class PlayerAction
{
    public ActionType actionType;
    public Vector3 position;
    public float timestamp;
    public float delay;
    public float jumpDuration; // Added for jump synchronization
    public int collectibleID; // Unique ID for collected item
    
    public PlayerAction(ActionType type, float time, Vector3 pos = default, float duration = 0f, int collectibleId = 0)
    {
        actionType = type;
        position = pos;
        timestamp = time;
        delay = 0f;
        jumpDuration = duration;
        collectibleID = collectibleId;
    }
}

public enum ActionType
{
    Jump,
    Collect,
    Collide,
    Move
}