using Arixen.ScriptSmith;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject gameUIScreen;
    [SerializeField] private GameObject gameOverScreen;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button mainmenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    void Awake()
    {
        startButton.onClick.AddListener(() => EventBusService.InvokeEvent(new RestartGameEvent()));
        restartButton.onClick.AddListener(() => EventBusService.InvokeEvent(new RestartGameEvent()));
        mainmenuButton.onClick.AddListener(ShowMainMenu);
        exitButton.onClick.AddListener(ExitGame);
    }

    void OnEnable()
    {
        EventBusService.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBusService.Subscribe<ScoreUpdatedEvent>(OnScoreUpdated);
    }

    void Start()
    {
        ShowMainMenu();
    }

    private void OnDisable()
    {
        EventBusService.UnSubscribe<GameStateChangedEvent>(OnGameStateChanged);
        EventBusService.UnSubscribe<ScoreUpdatedEvent>(OnScoreUpdated);
    }

    private void OnGameStateChanged(GameStateChangedEvent e)
    {
        switch (e.NewState)
        {
            case GameState.MainMenu:
                ShowMainMenu();
                break;
            case GameState.Playing:
                ShowGameUI();
                break;
            case GameState.GameOver:
                ShowGameOverScreen(GameManager.Instance.score + (int)GameManager.Instance.DistanceTraveled);
                break;
        }
    }

    private void OnScoreUpdated(ScoreUpdatedEvent e)
    {
        UpdateScore(e.NewScore);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    private void ShowGameOverScreen(int finalScore)
    {
        gameOverScoreText.text = "GameOver\nScore: " + finalScore;
        mainMenuScreen.SetActive(false);
        gameUIScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    private void ShowGameUI()
    {
        mainMenuScreen.SetActive(false);
        gameUIScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    private void ShowMainMenu()
    {
        mainMenuScreen.SetActive(true);
        gameUIScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }
    private void ExitGame()
    {
        Application.Quit();
    }

}
