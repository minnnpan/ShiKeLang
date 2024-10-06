using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public interface StampedeState
{
    void OnEnable();
    void OnDisable();
}

public class Stampede : MonoBehaviour, StampedeState
{
    //踩踏开启状态
    private bool StampedeState = false;
    public void OnEnable()
    {
        if (!StampedeState)
        {
            StampedeState = true;
            StartCoroutine(SpawnStampede());
            Debug.Log("开启踩踏");
        }
    }
    public void OnDisable()
    {
        StampedeState = false;
        StopCoroutine(SpawnStampede());
        Debug.Log("关闭踩踏");
    }
    //踩踏频率
    public float stampedeInterval = 3;
    //生成的数量上限
    public int stampedeMaxFoots = 3;
    //踩踏范围,长方形
    public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    //踩踏示意
    public GameObject stampedeCircle;
    //踩踏示意~落下时间
    public float stampedeTime = 1;
    //踩踏实体
    public GameObject stampedeFoot;
    //踩踏实体生成时高度
    public float stampedeHeight = 10;
    
        private float elapsedTime = 0f;
    private float difficultyIncreaseInterval = 30f;
    //每隔一段时间随机踩踏
    System.Collections.IEnumerator SpawnStampede()
    {
        while (StampedeState)
        {
            //生成随机数量
            int objectCount = Random.Range(1, stampedeMaxFoots + 1);

            for (int i = 0; i < objectCount; i++)
            {
                SpawnFoot();
            }

            // 等待指定的时间间隔
            yield return new WaitForSeconds(stampedeInterval);

            elapsedTime += stampedeInterval;
            if (elapsedTime >= difficultyIncreaseInterval)
            {
                IncreaseDifficulty();
                elapsedTime = 0f;
            }
        }
    }

    private void SpawnFoot()
    {
        float random_x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float random_z = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        
        Vector3 spawnPosition = new Vector3(random_x, 0, random_z);

        //实例化 示意物体
        GameObject go = Instantiate(stampedeCircle, spawnPosition, Quaternion.identity);
        Destroy(go, stampedeTime);

        //实例化 下落物体
        Vector3 spawnPosition2 = new Vector3(random_x, stampedeHeight, random_z);
        GameObject go2 = Instantiate(stampedeFoot, spawnPosition2, Quaternion.identity);
        go2.transform.DOMoveY(0, stampedeTime).SetEase(Ease.Linear)
            .OnComplete(() => Destroy(go2));
    }

    private void IncreaseDifficulty()
    {
        stampedeInterval = Mathf.Max(stampedeInterval * 0.9f, 0.5f);
        stampedeMaxFoots = Mathf.Min(stampedeMaxFoots + 1, 10);
        spawnAreaSize *= 1.1f;
    }
}
