using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampedeOpen : MonoBehaviour
{
    private StampedeState stampedeState;
    void Start()
    {
        stampedeState = GetComponent<StampedeState>();
    }

    
    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            stampedeState.OnEnable();
        }
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(1))
        {
            stampedeState.OnDisable();
        }
        
    }
}
