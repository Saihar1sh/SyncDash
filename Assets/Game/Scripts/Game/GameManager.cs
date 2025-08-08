using Arixen.ScriptSmith;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState { get; private set; }
    public float DistanceTraveled { get; private set; } = 0f;

    public GameState InitialState = GameState.MainMenu;
    public float initialPlatformSpeed = 5f;
    public float maxPlatformSpeed = 20f;
    public float speedIncreaseRate = 0.1f;
    public float speedMultiplier = 1f;
    public float currentPlatformSpeed { get; private set; }

    public int score = 0;
    
    public PlatformManager platformManager;
    public PlatformManager networkedPlatformManager;


    void OnEnable()
    {
        EventBusService.Subscribe<RestartGameEvent>(e => RestartGame());
        EventBusService.Subscribe<ExitGameEvent>(e => ExitGame());
        EventBusService.Subscribe<PlayerCollideEvent>(e => ChangeState(GameState.GameOver));
        EventBusService.Subscribe<PlayerCollectEvent>(OnPlayerCollect);
    }

    void OnDisable()
    {
        EventBusService.UnSubscribe<RestartGameEvent>(e => RestartGame());
        EventBusService.UnSubscribe<ExitGameEvent>(e => ExitGame());
        EventBusService.UnSubscribe<PlayerCollideEvent>(e => ChangeState(GameState.GameOver));
        EventBusService.UnSubscribe<PlayerCollectEvent>(OnPlayerCollect);
    }

    private void OnPlayerCollect(PlayerCollectEvent e)
    {
        AddScore(10);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DOTween.Init();
        ChangeState(InitialState);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1;
                currentPlatformSpeed = 0;
                break;
            case GameState.Playing:
                Time.timeScale = 1;
                score = 0;
                DistanceTraveled = 0f;
                speedMultiplier = 1f;
                currentPlatformSpeed = initialPlatformSpeed;
                int seed = System.Guid.NewGuid().GetHashCode();
                platformManager.InitSeed(seed);
                networkedPlatformManager.InitSeed(seed);
                break;
            case GameState.GameOver:
                Time.timeScale = 0.5f; // Slow motion effect
                break;
        }
        EventBusService.InvokeEvent(new GameStateChangedEvent { NewState = currentState });
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            if (currentPlatformSpeed < maxPlatformSpeed)
            {
                currentPlatformSpeed += speedIncreaseRate * Time.deltaTime * speedMultiplier;
            }

            DistanceTraveled += currentPlatformSpeed * Time.deltaTime;
            EventBusService.InvokeEvent(new ScoreUpdatedEvent(){ NewScore = score + (int)DistanceTraveled});
        }
    }

    public void AddScore(int points)
    {
        if (currentState != GameState.Playing) return;
        score += points;
    }

    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1f;
    }

    public void RestartGame()
    {
        ChangeState(GameState.Playing);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

public enum GameState { MainMenu, Playing, GameOver }
