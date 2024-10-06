using System;
using PaintCore;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager uiManager;
    // public DungBallController dungBallPrefab;
    public Transform spawnPoint;
    public DungBallController player;
    public TextureCoverageAnalyzer coverageCalculator;
    
    [SerializeField] private float winPercentage = 0.8f;
    [SerializeField] private float initialDungSize = 1f;
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
        uiManager.ShowStartWindow();
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
    }

    private void InitializeGame()
    {
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.position = new Vector3(0, 3, 0);
        }
        //
        // if (dungBallPrefab != null)
        // {
        //     player = Instantiate(dungBallPrefab, spawnPoint.position, Quaternion.identity);
        //     DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
        //     if (movementController != null)
        //     {
        //         movementController.enabled = true;
        //         movementController.InitializeSize(initialDungSize);
        //         movementController.OnDungPickup += HandleDungPickup;
        //         movementController.OnDungDrop += HandleDungDrop;
        //     }
        // }
        // else
        // {
        //     Debug.LogError("DungBall预制体未设置！");
        // }

        // if (topDownCamera != null && player != null)
        // {
        //     topDownCamera.SetTarget(player.transform);
        // }
        // else
        // {
        //     Debug.LogWarning("未找到 TopDownCamera 或 player 为空");
        // }

        if (stampede != null)
        {
            stampede.OnEnable();
        }

        gameEnded = false;
        hasDroppedDung = false;
    }

    private void Update()
    {
        if (!gameEnded && !isPaused && coverageCalculator != null && gameStarted == true)
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
            Destroy(player.gameObject);
            player = null;
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
    }
}