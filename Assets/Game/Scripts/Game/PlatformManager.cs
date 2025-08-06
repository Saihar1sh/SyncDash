using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")] public GameObject platformPrefab;
    public int numberOfPlatforms = 5;
    public float platformLength = 50f;
    public Transform platformsParent;
    public Transform playerTransform;

    public bool isGhostWorld = false;

    [Header("Object Pooling")] public GameObject obstaclePrefab;
    public GameObject collectiblePrefab;
    public int obstaclePoolSize = 15;
    public int collectiblePoolSize = 30;

    private List<GameObject> platformPool = new List<GameObject>();
    private List<GameObject> activePlatforms = new List<GameObject>();
    private Queue<GameObject> obstaclePool = new Queue<GameObject>();
    private Queue<GameObject> collectiblePool = new Queue<GameObject>();

    private int obstacleLayer;
    private int collectibleLayer;
    private int networkedLayer;

    private static int randomSeed;

    private int nextCollectibleID = 0;

    private List<Collectible> activeCollectibles = new List<Collectible>();

    private const string NetworkedLayerName = "Networked";

    public void InitSeed(int seed)
    {
        randomSeed = seed;
    }

    void Start()
    {
        Random.InitState(randomSeed);

        if (isGhostWorld)
        {
            networkedLayer = LayerMask.NameToLayer(NetworkedLayerName);
        }
        else
        {
            obstacleLayer = LayerMask.NameToLayer("Obstacle");
            collectibleLayer = LayerMask.NameToLayer("Collectible");
        }

        if (obstacleLayer == -1 || collectibleLayer == -1 || networkedLayer == -1)
        {
            Debug.LogError($"Please define layers correctly");
            return;
        }

        InitializePools();

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            GameObject platform = Instantiate(platformPrefab, platformsParent);
            platform.SetActive(false);
            platformPool.Add(platform);
            if (isGhostWorld)
            {
                platform.layer = networkedLayer;
            }
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
            PopulatePlatform(platform);
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
            obj.layer = obstacleLayer;
            obj.SetActive(false);
            obstaclePool.Enqueue(obj);
        }

        for (int i = 0; i < collectiblePoolSize; i++)
        {
            GameObject obj = Instantiate(collectiblePrefab, platformsParent);
            obj.layer = collectibleLayer;
            obj.SetActive(false);
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
        int obstacleCount = Random.Range(1, 4);
        for (int i = 0; i < obstacleCount; i++)
        {
            if (obstaclePool.Count > 0)
            {
                GameObject obstacle = obstaclePool.Dequeue();
                obstacle.transform.SetParent(platform.transform);
                obstacle.transform.localPosition = GetRandomPositionOnPlatform();
                obstacle.SetActive(true);
            }
        }

        int collectibleCount = Random.Range(1, 3);
        for (int i = 0; i < collectibleCount; i++)
        {
            if (collectiblePool.Count > 0)
            {
                GameObject collectible = collectiblePool.Dequeue();
                collectible.transform.SetParent(platform.transform);
                if (isGhostWorld)
                {
                    collectible.layer = networkedLayer;
                }

                Collectible collectibleScript = collectible.GetComponent<Collectible>();
                if (collectibleScript != null)
                {
                    collectibleScript.collectibleID = nextCollectibleID++;
                    activeCollectibles.Add(collectibleScript);
                }

                collectible.transform.localPosition = GetRandomPositionOnPlatform();
                collectible.SetActive(true);
            }
        }
    }

    void ReturnItemsToPool(GameObject platform)
    {
        foreach (Transform child in platform.transform)
        {
            if (child.gameObject.activeSelf)
            {
                if (child.gameObject.layer == obstacleLayer)
                {
                    child.gameObject.SetActive(false);
                    obstaclePool.Enqueue(child.gameObject);
                    child.SetParent(platformsParent);
                }
                else if (child.gameObject.layer == collectibleLayer)
                {
                    child.gameObject.SetActive(false);
                    collectiblePool.Enqueue(child.gameObject);
                    child.SetParent(platformsParent);
                    Collectible collectibleScript = child.GetComponent<Collectible>();
                    if (collectibleScript != null)
                    {
                        activeCollectibles.Remove(collectibleScript);
                    }
                }
            }
        }
    }

    Vector3 GetRandomPositionOnPlatform()
    {
        return new Vector3(0, 1f, Random.Range(-platformLength / 2, platformLength / 2));
    }

    public Collectible GetCollectibleByID(int id)
    {
        return activeCollectibles.Find(c => c.collectibleID == id);
    }
}
