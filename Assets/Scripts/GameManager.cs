using System;
using PaintCore;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager uiManager;
    public GameObject player;
    public TextureCoverageAnalyzer coverageCalculator;
    public Vector3 SpawnPosition;
    public CountdownTimer countdownTimer;
    
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
        uiManager.ShowStartWindow();
        SpawnPosition = player.transform.position;
    }

    public void StartGame()
    {
        if (gameStarted)
        {
            Debug.LogWarning("Game is already started. Ignoring StartGame call.");
            return;
        }
        
        gameStarted = true;
        InitializeGame();
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
            DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
            if (movementController != null)
            {
                movementController.enabled = true;
            }
        }
    }

    private void DisablePlayerMovement()
    {
        if (player != null)
        {
            DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
            if (movementController != null)
            {
                movementController.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (!gameStarted)
        {
            return;  // 如果游戏还没开始，直接返回
        }

        if (!gameEnded && !isPaused && coverageCalculator != null)
        {
            float coverage = coverageCalculator.GetCurrentCoverage();
            uiManager.UpdateCoverageText(coverage);
            CheckWinCondition(coverage);
            CheckLoseCondition(coverage);
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
            EndGame(true);
        }
    }

    private void CheckLoseCondition(float coverage)
    {
        DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
        if (hasDroppedDung && coverage < winPercentage && !movementController.HasDung)
        {
            EndGame(false);
        }
    }

    public void OnPlayerDroppedDung()
    {
        hasDroppedDung = true;
    }

    public void EndGame(bool isWin)
    {
        if (gameEnded) return;

        gameEnded = true;
        isPaused = false;
        Time.timeScale = 1;
        uiManager.ShowGameResult(isWin);

        // 停止玩家移动
        if (player != null)
        {
            DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
            if (movementController != null)
            {
                movementController.enabled = false;
            }
        }

        // 销毁所有 DungPile 对象
        DestroyAllDungPiles();

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

    public void RestartGame()
    {
        if (!gameStarted)
        {
            Debug.LogWarning("Cannot restart game before it has started. Ignoring RestartGame call.");
            return;
        }
        
        Debug.Log("RestartGame called. Stack trace: " + Environment.StackTrace);
        
        isPaused = false;
        gameEnded = false;
        hasDroppedDung = false;
        gameStarted = false;
        Time.timeScale = 1;

        if (player != null)
        {
            player.transform.position = SpawnPosition;
        }

        if (stampede != null)
        {
            stampede.Reset();
        }

        if (coverageCalculator != null)
        {
            coverageCalculator.ResetCoverage();
        }
        

        uiManager.ShowStartWindow();
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
        // 假设地面使用了 PaintIn3D 的 P3dPaintableTexture 组件
        CwPaintableTexture paintableTexture = FindObjectOfType<CwPaintableTexture>();
        if (paintableTexture != null)
        {
            paintableTexture.Clear();
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