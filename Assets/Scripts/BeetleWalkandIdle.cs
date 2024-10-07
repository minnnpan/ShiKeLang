using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeetleWalkandIdle : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator Anim;
    public float IdleSpeed;
    private Vector3 lastPosition; // Store the object's previous position
    private float speed;          // Store the calculated speed
    public void SetBeetleAnimToWalk(bool YesOrNo)
    {
        Anim.SetBool("IsWalking", YesOrNo);
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        // Calculate the speed (distance per frame divided by time between frames)
        speed = distance / Time.deltaTime;
        //Debug.Log(speed);

        // Update the last position for the next frame
        lastPosition = transform.position;

        if (speed >= IdleSpeed && !Anim.GetBool("IsWalking"))
        {
            //SetBeetleAnimToWalk(true);
            Anim.SetBool("IsWalking", true);
        }
        if (speed < IdleSpeed && Anim.GetBool("IsWalking"))
        {
            Anim.SetBool("IsWalking", false);
        }

    }
}
