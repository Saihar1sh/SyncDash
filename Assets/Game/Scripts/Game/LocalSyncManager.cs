using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arixen.ScriptSmith;

public class LocalSyncManager : MonoBehaviour
{
    [Header("Sync Settings")]
    [Range(0.05f, 0.5f)]
    public float networkDelay = 0.1f; // 100ms simulated lag

    public GhostPlayer ghostPlayer;
    private Queue<PlayerAction> actionQueue = new Queue<PlayerAction>();

    private void OnEnable()
    {
        EventBusService.Subscribe<PlayerJumpEvent>(OnPlayerJump);
        EventBusService.Subscribe<PlayerCollectEvent>(OnPlayerCollect);
        EventBusService.Subscribe<PlayerCollideEvent>(OnPlayerCollide);
        EventBusService.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBusService.UnSubscribe<PlayerJumpEvent>(OnPlayerJump);
        EventBusService.UnSubscribe<PlayerCollectEvent>(OnPlayerCollect);
        EventBusService.UnSubscribe<PlayerCollideEvent>(OnPlayerCollide);
        EventBusService.UnSubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        if (e.NewState == GameState.Playing)
        {
            ghostPlayer.Reset();
        }
    }

    private void OnPlayerJump(PlayerJumpEvent e)
    {
        var action = new PlayerAction(ActionType.Jump, Time.time, e.Position, e.JumpDuration);
        QueueAction(action);
    }

    private void OnPlayerCollect(PlayerCollectEvent e)
    {
        var action = new PlayerAction(ActionType.Collect, Time.time, e.Position, collectibleId: e.CollectibleID);
        QueueAction(action);
    }

    private void OnPlayerCollide(PlayerCollideEvent e)
    {
        var action = new PlayerAction(ActionType.Collide, Time.time, e.Position);
        QueueAction(action);
    }

    public void QueueAction(PlayerAction action)
    {
        actionQueue.Enqueue(action);
        StartCoroutine(ProcessActionWithDelay(action));
    }

    IEnumerator ProcessActionWithDelay(PlayerAction action)
    {
        yield return new WaitForSeconds(networkDelay);
        if (ghostPlayer != null)
        {
            ghostPlayer.ExecuteAction(action);
        }
    }
}