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
    public TextMeshProUGUI countdownText;
    private GameManager gameManager;
    public GameObject VictoryImg;
    public GameObject LoseRunOutPoop;
    public GameObject LoseGotStamp;

    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.countdownTimer.onCountdownStart.AddListener(() => countdownText.gameObject.SetActive(true));
        gameManager.countdownTimer.onCountdownEnd.AddListener(() => countdownText.gameObject.SetActive(false));
        gameManager.countdownTimer.OnCountdownTick += UpdateCountdownText;
        gameManager.countdownTimer.OnCountdownComplete += ShowGoText;
    }

    private void OnDestroy()
    {
        if (gameManager != null && gameManager.countdownTimer != null)
        {
            gameManager.countdownTimer.onCountdownStart.RemoveListener(() => countdownText.gameObject.SetActive(true));
            gameManager.countdownTimer.onCountdownEnd.RemoveListener(() => countdownText.gameObject.SetActive(false));
            gameManager.countdownTimer.OnCountdownTick -= UpdateCountdownText;
            gameManager.countdownTimer.OnCountdownComplete -= ShowGoText;
        }
    }

    private void UpdateCountdownText(int number)
    {
        countdownText.text = number.ToString();
    }

    private void ShowGoText()
    {
        countdownText.text = "GO!";
    }

    public void ShowStartWindow()
    {
        startWindow.SetActive(true);
        pausedWindow.SetActive(false);
        endWindow.SetActive(false);
        gameResultText.gameObject.SetActive(false);
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
        ShowStartWindow();
        UpdateCoverageText(0f);
        gameManager.ResetGame();
    }

    public void ShowGameResult(EndGameCondition gameCondition)
    {
        switch (gameCondition)
        {
            case EndGameCondition.win:
                OpenVictory();
                break;
            case EndGameCondition.gotStamp:
                OpenLoseGotStamp();
                break;
            case EndGameCondition.outOfPoop:
                OpenLoseOutOfPoop();
                break;
        }
        
    }

    public void UpdateCoverageText(float coverage)
    {
        coverageText.text = $"Coverage: {coverage:P2}";
    }

    public void OpenVictory()
    {
        VictoryImg.SetActive(true);
    }

    public void OpenLoseOutOfPoop()
    {
        LoseRunOutPoop.SetActive(true);
    }

    public void OpenLoseGotStamp()
    {
        LoseGotStamp.SetActive(true);
    }
}