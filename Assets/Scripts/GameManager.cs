using System;
using PaintCore;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EndGameCondition
{
    win,
    gotStamp,
    outOfPoop
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
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
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
        if (gameStarted)
        {
            Debug.LogWarning("Game is already started. Ignoring StartGame call.");
            return;
        }
        
        gameStarted = true;
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
        if (gameEnded) return;

        gameEnded = true;
        isPaused = false;
        Time.timeScale = 1;
        uiManager.ShowGameResult(gameCondition);

        // 停止玩家移动
        DisablePlayerMovement();

        if (stampede != null)
        {
            stampede.OnDisable();
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
        // if (!gameStarted)
        // {
        //     Debug.LogWarning("Cannot reset game before it has started. Ignoring ResetGame call.");
        //     return;
        // }
        //
        // Debug.Log("Reset Game called. Stack trace: " + Environment.StackTrace);
        //
        // isPaused = false;
        // gameEnded = false;
        // hasDroppedDung = false;
        // gameStarted = false;
        // Time.timeScale = 1;
        //
        // if (player != null)
        // {
        //     player.transform.position = SpawnPosition;
        //     DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
        //     if (movementController != null)
        //     {
        //         movementController.InitializeSize();
        //     }
        // }
        //
        // if (stampede != null)
        // {
        //     stampede.Reset();
        // }
        //
        // if (coverageCalculator != null)
        // {
        //     coverageCalculator.ResetCoverage();
        // }
        //
        // countdownTimer.ResetTimer();
        //
        // DestroyAllDungPiles();
        //
        // ResetGroundTexture();

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
    }
}