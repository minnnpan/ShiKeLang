using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DungBeetle player;
    public DungCoverageCalculator coverageCalculator;
    public TextMeshProUGUI gameResultText;
    
    [SerializeField] private float winPercentage = 0.8f;
    private bool gameEnded = false;
    private bool hasDroppedDung = false;

    private void Start()
    {
        if (player != null)
        {
            player.OnDungPickup += HandleDungPickup;
            player.OnDungDrop += HandleDungDrop;
        }
        if (gameResultText != null)
        {
            gameResultText.gameObject.SetActive(false);
        }
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
        if (hasDroppedDung && coverage < winPercentage && !player.HasDung)
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
        gameEnded = false;
        if (gameResultText != null)
        {
            gameResultText.gameObject.SetActive(false);
        }
        // 在这里添加重置游戏状态的其他逻辑
    }

    private void HandleDungPickup(float size)
    {
        Debug.Log($"玩家拾取了大小为 {size} 的大便！");
        // 这里可以添加更多逻辑，比如更新UI、计分等
    }

    private void HandleDungDrop(float size)
    {
        Debug.Log($"玩家丢弃了大小为 {size} 的大便！");
        // 这里可以添加更多逻辑
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnDungPickup -= HandleDungPickup;
            player.OnDungDrop -= HandleDungDrop;
        }
    }
}