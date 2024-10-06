using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class RandomSetAnimBool : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator Anim;
    public String BoolName;
    public float MaxTime, MinTime, NextChangeTime;
    public bool b;


    void Start()
    {

        b = Anim.GetComponent<Animator>().GetBool(BoolName);
    }


    private void setTimer()
    {
        float randomTime = UnityEngine.Random.Range(MinTime, MaxTime);
        NextChangeTime = Time.time + randomTime;
    }

    private void revertBool()
    {
        b = !b;
        Anim.SetBool(BoolName, b);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > NextChangeTime)
        {
            setTimer();
            revertBool();
        }

    }
}
