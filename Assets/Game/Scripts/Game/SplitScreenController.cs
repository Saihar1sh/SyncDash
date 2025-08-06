using UnityEngine;

public class SplitScreenController : MonoBehaviour
{
    public Camera playerCamera;
    public Camera ghostCamera;

    private const string PlayerLayerName = "Player";
    private const string GroundLayerName = "Ground";
    private const string GhostLayerName = "GhostPlayer";
    private const string ObstacleLayerName = "Obstacle";
    private const string CollectibleLayerName = "Collectible";
    private const string NetworkedLayerName = "Networked";

    void Start()
    {
        playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
        ghostCamera.rect = new Rect(0, 0, 0.5f, 1);

        int playerLayer = LayerMask.NameToLayer(PlayerLayerName);
        int ghostLayer = LayerMask.NameToLayer(GhostLayerName);
        int obstacleLayer = LayerMask.NameToLayer(ObstacleLayerName);
        int collectibleLayer = LayerMask.NameToLayer(CollectibleLayerName);
        int networkedLayer = LayerMask.NameToLayer(NetworkedLayerName);
        int groundLayer = LayerMask.NameToLayer(GroundLayerName);

        if (playerLayer == -1 || ghostLayer == -1 || obstacleLayer == -1 || collectibleLayer == -1 || networkedLayer == -1)
        {
            Debug.LogError("Please define all required layers");
            return;
        }

        playerCamera.cullingMask = (1 << playerLayer) | (1 << obstacleLayer) | (1 << collectibleLayer) | (1 << groundLayer);

        ghostCamera.cullingMask = (1 << ghostLayer) | (1 << networkedLayer);
    }
}