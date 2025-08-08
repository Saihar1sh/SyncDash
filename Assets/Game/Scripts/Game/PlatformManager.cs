using System;
using System.Collections.Generic;
using Arixen.ScriptSmith;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")] public GameObject platformPrefab;
    public int numberOfPlatforms = 5;
    public float platformLength = 50f;
    public Transform platformsParent;
    public Transform playerTransform;
    public int safePlatformsCount = 2; // Number of initial platforms without obstacles/collectibles
    public bool isGhostWorld = false;

    [Header("Object Pooling")] public GameObject obstaclePrefab;
    public Collectible collectiblePrefab;
    public int obstaclePoolSize = 15;
    public int collectiblePoolSize = 30;

    private List<GameObject> platformPool = new List<GameObject>();
    private List<GameObject> activePlatforms = new List<GameObject>();
    private Queue<GameObject> obstaclePool = new Queue<GameObject>();
    private Queue<Collectible> collectiblePool = new Queue<Collectible>();
    
    private int localLayer;
    private int networkedLayer;

    private System.Random random;   //For player and ghost to have same random values

    private int nextCollectibleID = 0;

    private List<Collectible> activeCollectibles = new List<Collectible>();

    private const string NetworkedLayerName = "Networked";
    private const string LocalLayerName = "Local";

    private void Awake()
    {
        localLayer = LayerMask.NameToLayer(LocalLayerName);
        networkedLayer = LayerMask.NameToLayer(NetworkedLayerName);
    }

    void OnEnable()
    {
        EventBusService.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    void OnDisable()
    {
        EventBusService.UnSubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    void Start()
    {
        InitializePools();

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            GameObject platform = Instantiate(platformPrefab, platformsParent);
            platform.SetActive(false);
            platformPool.Add(platform);
        }
    }
    public void InitSeed(int seed)
    {
        random = new System.Random(seed);
    }
    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.NewState)
        {
            case GameState.MainMenu:
                StopPlatforms();
                break;
            case GameState.Playing:
                StartPlatforms();
                break;
            case GameState.GameOver:
                StopPlatforms();
                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameState.Playing) return;

        float speed = GameManager.Instance.currentPlatformSpeed;
        foreach (GameObject platform in activePlatforms)
        {
            platform.transform.localPosition -= new Vector3(0, 0, speed * Time.deltaTime);
        }

        if (activePlatforms.Count > 0)
        {
            GameObject firstPlatform = activePlatforms[0];
            if (firstPlatform.transform.position.z < playerTransform.position.z - (platformLength * 1.5f))
            {
                RecyclePlatform(firstPlatform);
            }
        }
    }

    public void StartPlatforms()
    {
        activePlatforms.Clear();

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            GameObject platform = platformPool[i];
            platform.transform.localPosition = new Vector3(0, 0, i * platformLength);
            platform.SetActive(true);
            platform.layer = isGhostWorld? networkedLayer : localLayer;

            if (i >= safePlatformsCount)
            {
                PopulatePlatform(platform);
            }
            activePlatforms.Add(platform);
        }
    }

    public void StopPlatforms()
    {
        foreach (GameObject platform in activePlatforms)
        {
            ReturnItemsToPool(platform);
            platform.SetActive(false);
        }

        activePlatforms.Clear();
    }

    void InitializePools()
    {
        for (int i = 0; i < obstaclePoolSize; i++)
        {
            GameObject obj = Instantiate(obstaclePrefab, platformsParent);
            obj.transform.localRotation = Quaternion.Euler(0,0,90);
            obj.SetActive(false);
            obstaclePool.Enqueue(obj);
        }

        for (int i = 0; i < collectiblePoolSize; i++)
        {
            Collectible obj = Instantiate(collectiblePrefab, platformsParent);
            obj.gameObject.SetActive(false);
            collectiblePool.Enqueue(obj);
        }
    }

    void RecyclePlatform(GameObject platform)
    {
        ReturnItemsToPool(platform);

        GameObject lastPlatform = activePlatforms[activePlatforms.Count - 1];
        platform.transform.localPosition = lastPlatform.transform.localPosition + new Vector3(0, 0, platformLength);

        PopulatePlatform(platform);

        activePlatforms.RemoveAt(0);
        activePlatforms.Add(platform);
    }

    void PopulatePlatform(GameObject platform)
    {
        // Decide if an obstacle should be spawned on this platform
        if (random.NextDouble() < 0.45) // Roughly 45% chance for an obstacle
        {
            if (obstaclePool.Count > 0)
            {
                GameObject obstacle = obstaclePool.Dequeue();
                obstacle.transform.SetParent(platform.transform);
                obstacle.transform.localPosition = GetRandomPositionOnPlatform();
                obstacle.SetActive(true);
                obstacle.layer = isGhostWorld ? networkedLayer : localLayer;
            }
        }

        int collectibleCount = random.Next(0, 2);
        for (int i = 0; i < collectibleCount; i++)
        {
            if (collectiblePool.Count > 0)
            {
                Collectible collectible = collectiblePool.Dequeue();
                collectible.transform.SetParent(platform.transform);

                collectible.collectibleID = nextCollectibleID++;
                activeCollectibles.Add(collectible);
                
                collectible.transform.localPosition = GetRandomPositionOnPlatform();
                collectible.gameObject.SetActive(true);
                collectible.gameObject.layer = isGhostWorld ? networkedLayer : localLayer;

            }
        }
    }

    void ReturnItemsToPool(GameObject platform)
    {
        foreach (Transform child in platform.transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (child.CompareTag("Obstacle"))
                {
                    child.gameObject.SetActive(false);
                    obstaclePool.Enqueue(child.gameObject);
                    child.SetParent(platformsParent);
                }
                else if (child.CompareTag("Collectible"))
                {
                    if (child.TryGetComponent<Collectible>(out Collectible collectible))
                    {
                        collectible.gameObject.SetActive(false);
                        collectiblePool.Enqueue(collectible);
                        child.SetParent(platformsParent);
                        activeCollectibles.Remove(collectible);
                    }
                }
            }
        }
    }

    Vector3 GetRandomPositionOnPlatform()
    {
        float GetRandomFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        return new Vector3(0, 1f, GetRandomFloat(-platformLength / 2, platformLength / 2));
    }

    public Collectible GetCollectibleByID(int id)
    {
        return activeCollectibles.Find(c => c.collectibleID == id);
    }
}
