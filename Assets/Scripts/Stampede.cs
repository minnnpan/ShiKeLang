using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

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
            stampedeLocs = ReadChildTransforms(stampedeLoc);
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
    //玩家
    public GameObject player;
    //距离玩家的Foot范围
    public float playerFootMaxDistance;
    //很多Foot
    public GameObject[] stampedeFoots;
    //很多Foot的影子
    public GameObject[] stampedeShadow;
    //很多生成点
    public Transform stampedeLoc;
    //踩踏频率
    public float stampedeInterval = 3;
    //生成的数量上限
    public int stampedeMaxFoots = 3;
    //踩踏范围,长方形
    //public Vector2 spawnAreaSize = new Vector2(10f, 10f);
    //踩踏示意
    //public GameObject stampedeCircle;
    //踩踏示意~落下时间
    public float stampedeTime = 1;
    //踩踏实体
    //public GameObject stampedeFoot;
    //踩踏实体生成时高度
    public float stampedeHeight = 10;
    
    private float elapsedTime = 0f;
    private float difficultyIncreaseInterval = 30f;

    private Transform[] stampedeLocs;

    //每隔一段时间随机踩踏
    System.Collections.IEnumerator SpawnStampede()
    {
        while (StampedeState)
        {
            //生成随机数量
            int objectCount = Random.Range(1, stampedeMaxFoots + 1);
            //找到玩家周边范围内的多个位置
            List<Vector3> selectedPos = GetRandomNearbyPositions(objectCount);
            //生成Foot
            for (int i = 0; i < selectedPos.Count; i++)
            {
                SpawnFoot(selectedPos[i]);
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

    private void SpawnFoot(Vector3 spawnPosition)
    {
        //随机一只脚的影子 并实例化
        int randomIndex_foot = Random.Range(0, stampedeShadow.Length);
        spawnPosition.y += 0.2f;
        GameObject go = Instantiate(stampedeShadow[randomIndex_foot], spawnPosition, Quaternion.Euler(-90, 0, 0));
        go.transform.localScale = new Vector3(10f, 10f, 50f);
        // 使用 DOTween 放大
        go.transform.DOScale(new Vector3(90f, 90f, 50f), stampedeTime-0.06f);
        Destroy(go, stampedeTime-0.05f);

        //实例化 下落物体
        Vector3 spawnPosition2 = new Vector3(spawnPosition.x, stampedeHeight, spawnPosition.z);
        GameObject go2 = Instantiate(stampedeFoots[randomIndex_foot], spawnPosition2, Quaternion.identity);
        AnimateFoot(go2);
    }

    private void IncreaseDifficulty()
    {
        stampedeInterval = Mathf.Max(stampedeInterval * 0.9f, 0.5f);
        stampedeMaxFoots = Mathf.Min(stampedeMaxFoots + 1, 10);
        //spawnAreaSize *= 1.1f;
    }
    
    //读取子物体的transform
    Transform[] ReadChildTransforms(Transform ts)
    {
        int childCount = ts.childCount;
        Transform[]childTransforms  = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            childTransforms[i] = ts.GetChild(i);
        }

        return childTransforms;
    }
    
    //找玩家距离内的位置
    List<Vector3> GetRandomNearbyPositions(int numberOfTransforms)
    {
        List<Vector3> nearbyPositions = new List<Vector3>();

        foreach (var transform in stampedeLocs)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= playerFootMaxDistance)
            {
                nearbyPositions.Add(transform.position);
            }
        }

        List<Vector3> selectedPositions = new List<Vector3>();
        int count = Mathf.Min(numberOfTransforms, nearbyPositions.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, nearbyPositions.Count);
            selectedPositions.Add(nearbyPositions[randomIndex]);
            nearbyPositions.RemoveAt(randomIndex);
        }

        return selectedPositions;
    }
    
    //Foot动画
    void AnimateFoot(GameObject foot)
    {
        // 下落到指定位置
        foot.transform.DOMoveY(0.8f, stampedeTime-0.12f).SetEase(Ease.InSine).OnComplete(() =>
        {
            // 左右摇晃
            foot.transform.DOShakePosition(1f, new Vector3(0.1f, 0, 0.1f), 10, 90, false, true).OnComplete(() =>
            {
                // 抬起来
                foot.transform.DOMoveY(stampedeHeight, stampedeTime).SetEase(Ease.OutFlash).OnComplete(() =>
                {
                    Destroy(foot);
                });
            });
        });
    }
    public void Reset()
    {
        OnDisable();
        stampedeInterval = 3;
        stampedeMaxFoots = 3;
        //spawnAreaSize = new Vector2(10f, 10f);
        elapsedTime = 0f;
    }
}
