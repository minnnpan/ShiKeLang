using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject startWindow;
    public GameObject pausedWindow;
    public GameObject endWindow;
    public TextMeshProUGUI gameResultText;
    public TextMeshProUGUI coverageText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void ShowStartWindow()
    {
        startWindow.SetActive(true);
        pausedWindow.SetActive(false);
        endWindow.SetActive(false);
        coverageText.gameObject.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        startWindow.SetActive(false);
        coverageText.gameObject.SetActive(true);
        gameManager.StartGame();
    }

    public void TogglePauseMenu(bool isPaused)
    {
        pausedWindow.SetActive(isPaused);
    }

    public void OnResumeButtonClicked()
    {
        gameManager.TogglePause();
    }

    public void OnRestartButtonClicked()
    {
        gameManager.RestartGame();
        coverageText.gameObject.SetActive(false);
        endWindow.SetActive(false);
        pausedWindow.SetActive(false);
        gameResultText.gameObject.SetActive(false);
        UpdateCoverageText(0f);
    }

    public void ShowGameResult(bool isWin)
    {
        endWindow.SetActive(true);
        gameResultText.gameObject.SetActive(true);
        gameResultText.text = isWin ? "You Win！" : "Game Over！";
        gameResultText.color = isWin ? Color.green : Color.red;
    }

    public void UpdateCoverageText(float coverage)
    {
        coverageText.text = $"Coverage: {coverage:P2}";
    }
}