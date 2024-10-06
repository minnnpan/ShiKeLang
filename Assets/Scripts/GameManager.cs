using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DungBallController dungBallPrefab;
    public Transform spawnPoint;
    public DungBallController player;
    public DungCoverageCalculator coverageCalculator;
    public TextMeshProUGUI gameResultText;
    
    [SerializeField] private float winPercentage = 0.8f;
    [SerializeField] private float initialDungSize = 1f;
    private bool gameEnded = false;
    private bool hasDroppedDung = false;

    public TopDownCamera topDownCamera;
    
    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.position = new Vector3(0, 3, 0);
        }

        if (dungBallPrefab != null)
        {
            player = Instantiate(dungBallPrefab, spawnPoint.position, Quaternion.identity);
            DungBallMovementController movementController = player.GetComponent<DungBallMovementController>();
            if (movementController != null)
            {
                movementController.InitializeSize(initialDungSize);
                movementController.OnDungPickup += HandleDungPickup;
                movementController.OnDungDrop += HandleDungDrop;
            }
        }
        else
        {
            Debug.LogError("DungBall预制体未设置！");
        }

        if (gameResultText != null)
        {
            gameResultText.gameObject.SetActive(false);
        }

        if (topDownCamera != null && player != null)
        {
            topDownCamera.SetTarget(player.transform);
        }
        else
        {
            Debug.LogWarning("未找到 TopDownCamera 或 player 为空");
        }   

        gameEnded = false;
        hasDroppedDung = false;
    }

    private void Update()
    {
        if (!gameEnded && coverageCalculator != null)
        {
            float coverage = coverageCalculator.GetCurrentCoverage();
            CheckWinCondition(coverage);
            CheckLoseCondition(coverage);
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

    private void EndGame(bool isWin)
    {
        gameEnded = true;
        if (gameResultText != null)
        {
            gameResultText.gameObject.SetActive(true);
            gameResultText.text = isWin ? "You Win！" : "Game Over！";
            gameResultText.color = isWin ? Color.green : Color.red;
        }
    }

    public void RestartGame()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        InitializeGame();
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