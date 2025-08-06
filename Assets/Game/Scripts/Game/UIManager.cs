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
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        // Add button listeners
        startButton.onClick.AddListener(GameManager.Instance.RestartGame);
        restartButton.onClick.AddListener(GameManager.Instance.RestartGame);
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);

        ShowMainMenu();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void ShowGameOverScreen(int finalScore)
    {
        gameOverScoreText.text = "Score: " + finalScore;
        mainMenuScreen.SetActive(false);
        gameUIScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void ShowGameUI()
    {
        mainMenuScreen.SetActive(false);
        gameUIScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    public void ShowMainMenu()
    {
        mainMenuScreen.SetActive(true);
        gameUIScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }
}
