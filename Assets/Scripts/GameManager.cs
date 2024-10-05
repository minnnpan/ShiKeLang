using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DungBeetle player;

    private void Start()
    {
        if (player != null)
        {
            player.OnDungPickup += HandleDungPickup;
            player.OnDungDrop += HandleDungDrop;
        }
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