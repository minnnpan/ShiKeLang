using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class HintOnUI : MonoBehaviour
{
    public GameObject player;
    public GameObject itemParent;
    public RectTransform indicatorUI_outside;
    public RectTransform indicatorUI;
    //public CinemachineVirtualCamera virtualCamera;

    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
    }

    
    void Update()
    {
        Transform ts = findnearbyShit();
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(ts.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height;

        if (onScreen)
        {
            indicatorUI.gameObject.SetActive(false);
        }
        else
        {
            indicatorUI.gameObject.SetActive(true);
            Vector3 direction = (ts.position - player.transform.position).normalized;
            Vector3 forward = player.transform.right;
            float angle = Mathf.Atan2(direction.z, direction.x) - Mathf.Atan2(forward.z, forward.x);

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            float x = screenCenter.x + Mathf.Cos(angle) * (Screen.width / 2 - 50);
            float y = screenCenter.y + Mathf.Sin(angle) * (Screen.height / 2 - 50);

            x = Mathf.Clamp(x, 50, Screen.width - 50);
            y = Mathf.Clamp(y, 50, Screen.height - 50);

            indicatorUI.position = new Vector3(x, y, 0);
            indicatorUI_outside.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        }
    }
    //找到最近的shit
    Transform findnearbyShit()
    {
        Transform nearestItem = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform child in itemParent.transform)
        {
            float distance = Vector3.Distance(player.transform.position, child.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestItem = child;
            }
        }
        return nearestItem;
    }
}
