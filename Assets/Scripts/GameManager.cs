using System;
using PaintCore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum EndGameCondition
{
    win,
    gotStamp,
    outOfPoop
}

public enum GameState
{
    NotStarted,
    Playing,
    Ended,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager uiManager;
    public GameObject player;
    public TextureCoverageAnalyzer coverageCalculator;
    public Vector3 SpawnPosition;
    public CountdownTimer countdownTimer;
    public CwPaintableTexture PaintableTexture;
    public Texture paintTexture;
    public BarControl barControl;
    
    [SerializeField] private float winPercentage = 0.8f;
    private bool gameEnded = false;
    private bool hasDroppedDung = false;
    private bool isPaused = false;
    private bool gameStarted = false;

    // public TopDownCamera topDownCamera;
    public Stampede stampede;
    
    public GameState currentState = GameState.NotStarted;
    public string[] gameBGMs = new string[3]; // 对应三个阶段的BGM名称
    public string victorySound;
    public string defeatSound;
    public string victoryBGM;
    public string defeatBGM;
    private float gameTimer = 0f;
    private int currentBGMIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        countdownTimer.onCountdownStart.AddListener(DisablePlayerMovement);
        countdownTimer.onCountdownEnd.AddListener(EnablePlayerMovement);
        PaintableTexture.Texture = paintTexture;
        SpawnPosition = player.transform.position;
        InitializeGame();
        DisablePlayerMovement();
        StartGame();
    }

    public void StartGame()
    {
        if (currentState != GameState.NotStarted)
        {
            Debug.LogWarning("Game is not in NotStarted state. Ignoring StartGame call.");
            return;
        }
        
        currentState = GameState.Playing;
        gameStarted = true;
        currentBGMIndex = 0;
        SoundManager.Instance.Play(gameBGMs[currentBGMIndex]);
        Debug.Log($"Started playing BGM: {gameBGMs[currentBGMIndex]}");
        countdownTimer.StartCountdown();
    }

    private void InitializeGame()
    {
        if (player == null)
        {
            Debug.LogError("Player is not set in the scene!");
            return;
        }

        DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
        if (movementController != null)
        {
            movementController.InitializeSize();
            movementController.OnDungPickup += HandleDungPickup;
            movementController.OnDungDrop += HandleDungDrop;
        }
        else
        {
            Debug.LogError("DungBallMovementController not found on player!");
        }

        if (stampede != null)
        {
            stampede.OnDifficultyIncreased += HandleDifficultyIncrease;
            stampede.OnEnable();
        }

        gameEnded = false;
        hasDroppedDung = false;

        if (coverageCalculator != null)
        {
            coverageCalculator.ResetCoverage();
        }

        ResetGroundTexture();
    }

    private void EnablePlayerMovement()
    {
        if (player != null)
        {
            BeetleController beetleController = player.GetComponent<BeetleController>();
            if (beetleController != null)
            {
                beetleController.enabled = true;
            }
        }
    }

    private void DisablePlayerMovement()
    {
        if (player != null)
        {
            BeetleController beetleController = player.GetComponent<BeetleController>();
            if (beetleController != null)
            {
                beetleController.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (!gameStarted)
        {
            Debug.Log("game not started, gamemanager update");
            return;  // 如果游戏还没开始，直接返回
        }

        if (!gameEnded && !isPaused && coverageCalculator != null)
        {

            float coverage = coverageCalculator.GetCurrentCoverage();
            uiManager.UpdateCoverageText(coverage);
            
            // Update BarControl
            if (barControl != null && player != null)
            {
                DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
                if (movementController != null)
                {
                    barControl.UpdateBars(movementController.CurrentSize, coverage);
                }
            }
            else
            {
                Debug.Log($"{barControl != null} {player != null}");
            }
            
            CheckWinCondition(coverage);
            CheckLoseCondition(coverage);
            
        }else
        {
            Debug.Log($"{!gameEnded} {!isPaused} {coverageCalculator != null}");
        }

        // 添加暂停输入检测
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void CheckWinCondition(float coverage)
    {
        if (coverage >= winPercentage)
        {
            EndGame(EndGameCondition.win);
        }
    }

    private void CheckLoseCondition(float coverage)
    {
        DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
        if (hasDroppedDung && coverage < winPercentage && !movementController.HasDung)
        {
            EndGame(EndGameCondition.outOfPoop);
        }
    }

    public void OnPlayerDroppedDung()
    {
        hasDroppedDung = true;
    }

    public void EndGame(EndGameCondition gameCondition)
    {
        if (currentState != GameState.Playing) return;
        StopAllCoroutines();
        currentState = GameState.Ended;
        gameEnded = true;
        isPaused = false;
        Time.timeScale = 1;

        // Play victory or defeat sound
        if (gameCondition == EndGameCondition.win)
        {
            //SoundManager.Instance.Play(victorySound);
        }
        else
        {
            //SoundManager.Instance.Play(defeatSound);
        }

        // Stop game BGM
        SoundManager.Instance.Stop(gameBGMs[currentBGMIndex]);

        // Disable player movement
        DisablePlayerMovement();

        if (stampede != null)
        {
            stampede.OnDisable();
        }

        // Show game result after a short delay
        StartCoroutine(ShowGameResultAfterDelay(gameCondition));
    }

    private System.Collections.IEnumerator ShowGameResultAfterDelay(EndGameCondition gameCondition)
    {
        yield return new WaitForSeconds(1f); // Wait for 2 seconds

        currentState = GameState.Result;
        uiManager.ShowGameResult(gameCondition);

        // Play victory or defeat BGM
        if (gameCondition == EndGameCondition.win)
        {
            // SoundManager.Instance.Play(victoryBGM);
        }
        else
        {
            // SoundManager.Instance.Play(defeatBGM);
        }
    }

    private void DestroyAllDungPiles()
    {
        DungPile[] dungPiles = FindObjectsOfType<DungPile>();
        foreach (DungPile dungPile in dungPiles)
        {
            Destroy(dungPile.gameObject);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        uiManager.TogglePauseMenu(isPaused);
    }

    public void ResetGame()
    {
        if (currentState == GameState.NotStarted)
        {
            Debug.LogWarning("Cannot reset game before it has started. Ignoring ResetGame call.");
            return;
        }

        SoundManager.Instance.Stop(gameBGMs[currentBGMIndex]);
        SoundManager.Instance.Stop(victoryBGM);
        SoundManager.Instance.Stop(defeatBGM);

        currentState = GameState.NotStarted;
        gameEnded = false;
        hasDroppedDung = false;
        isPaused = false;
        gameStarted = false;
        gameTimer = 0f;
        currentBGMIndex = 0;

        if (stampede != null)
        {
            stampede.Reset();
        }

        if (barControl != null)
        {
            barControl.UpdateBars(player.GetComponent<DungBallMovementController>().CurrentSize, 0f);
        }

        ResetGroundTexture();
        ReloadScene();
    }
    
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void HandleDungPickup(float size)
    {
        Debug.Log($"玩家拾取了大小为 {size} 的大便！");
    }

    private void HandleDungDrop(float size)
    {
        Debug.Log($"玩家丢弃了大小为 {size} 的大便！");
        OnPlayerDroppedDung();
    }

    public void ResetGroundTexture()
    {
        if (PaintableTexture != null)
        {
            PaintableTexture.Clear(paintTexture, Color.white);
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
            if (movementController != null)
            {
                movementController.OnDungPickup -= HandleDungPickup;
                movementController.OnDungDrop -= HandleDungDrop;
            }
        }
        countdownTimer.onCountdownStart.RemoveListener(DisablePlayerMovement);
        countdownTimer.onCountdownEnd.RemoveListener(EnablePlayerMovement);
        if (stampede != null)
        {
            stampede.OnDifficultyIncreased -= HandleDifficultyIncrease;
        }
    }

    private void HandleDifficultyIncrease(int newDifficultyLevel)
    {
        if (currentBGMIndex < gameBGMs.Length - 1)
        {
            currentBGMIndex++;
            SoundManager.Instance.Stop(gameBGMs[currentBGMIndex - 1]);
            SoundManager.Instance.Play(gameBGMs[currentBGMIndex]);
            Debug.Log($"Switched to BGM: {gameBGMs[currentBGMIndex]}");
        }
    }
}
