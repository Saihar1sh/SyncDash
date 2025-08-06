using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState InitialState = GameState.MainMenu;
    public GameState currentState { get; private set; }

    public float initialPlatformSpeed = 5f;
    public float maxPlatformSpeed = 20f;
    public float speedIncreaseRate = 0.1f;
    public float speedMultiplier = 1f;
    public float currentPlatformSpeed { get; private set; }

    public int score = 0;
    private float distanceTraveled = 0f;

    // --- References ---
    public UIManager uiManager;
    public PlatformManager platformManager;
    public PlatformManager networkedPlatformManager;
    public PlayerController playerController;
    public ScreenEffectsController screenEffects;

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
        int seed = System.Guid.NewGuid().GetHashCode();
        platformManager.InitSeed(seed);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1;
                currentPlatformSpeed = 0;
                uiManager.ShowMainMenu();
                platformManager.StopPlatforms();
                networkedPlatformManager.StopPlatforms();
                break;
            case GameState.Playing:
                Time.timeScale = 1;
                score = 0;
                distanceTraveled = 0f;
                speedMultiplier = 1f;
                currentPlatformSpeed = initialPlatformSpeed;
                uiManager.ShowGameUI();
                uiManager.UpdateScore(score);
                platformManager.StartPlatforms();
                networkedPlatformManager.StartPlatforms();
                playerController.ResetPlayer();
                break;
            case GameState.GameOver:
                Time.timeScale = 0.5f; // Slow motion effect
                screenEffects.TriggerCrashEffects();
                uiManager.ShowGameOverScreen(score + (int)distanceTraveled);
                platformManager.StopPlatforms();
                networkedPlatformManager.StopPlatforms();
                break;
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            if (currentPlatformSpeed < maxPlatformSpeed)
            {
                currentPlatformSpeed += speedIncreaseRate * Time.deltaTime * speedMultiplier;
            }

            distanceTraveled += currentPlatformSpeed * Time.deltaTime;
            uiManager.UpdateScore(score + (int)distanceTraveled);
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
