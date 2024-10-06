using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeetleController : MonoBehaviour
{
    public float maxMoveSpeed = 5f; // 最大移动速度
    public float acceleration = 2f; // 加速度
    public float turnSpeed = 100f; // 转身速度
    public GameObject beetle;
    public GameObject shitball;

    private float currentSpeed = 0f; // 当前速度
    private Animator animator;
    private float maxBallRotationSpeed = 360f;
    private void Start()
    {
        animator = beetle.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
        Turn();
    }

    void Move()
    {
        // 获取垂直输入（前进和后退）
        float moveDirection = Input.GetAxis("Vertical");

        if (moveDirection > 0)
        {
            // 匀加速
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxMoveSpeed);
        }
        else
        {
            // 停止时减速
            currentSpeed = 0;
        }

        Vector3 movement = transform.forward * moveDirection * currentSpeed * Time.deltaTime;
        transform.position += movement;
        // 设置动画参数
        animator.SetFloat("Speed", Mathf.Abs(moveDirection));
        // 如果速度大于0，旋转球
        if (Mathf.Abs(moveDirection) > 0)
        {
            RotateBall(moveDirection, shitball);
        }
    }

    void Turn()
    {
        // 获取水平输入（左转和右转）
        float turnDirection = Input.GetAxis("Horizontal");
        float turn = turnDirection * turnSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);
    }
    
    void RotateBall(float moveDirection, GameObject ball)
    {
        // 根据移动速度调整球的旋转速度
        float rotationSpeed = maxBallRotationSpeed * moveDirection;

        // 使用DoTween旋转球
        ball.transform.DORotate(new Vector3(0, 0, rotationSpeed), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }
}
