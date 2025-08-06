// LocalSyncManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalSyncManager : MonoBehaviour
{
    [Header("Sync Settings")]
    [Range(0.05f, 0.5f)]
    public float networkDelay = 0.1f; // 100ms simulated lag

    public GhostPlayer ghostPlayer;
    private Queue<PlayerAction> actionQueue = new Queue<PlayerAction>();

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