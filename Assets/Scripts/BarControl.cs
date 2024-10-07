using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarControl : MonoBehaviour
{
    public float ProgressValue, PoopValue;
    public Image ProgressFill, PoopFill;
    public Animator ProgressBarAnim;
    private int ProgressLv;
    // Start is called before the first frame update
    void Start()
    {
        ProgressLv = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float ProgressValueReal = ProgressValue * .8f;
        ProgressFill.fillAmount = ProgressValueReal;
        PoopFill.fillAmount = PoopValue;

        if (ProgressValue >= .3f && ProgressLv == 0)
        {
            ProgressLv = 1;
            ProgressBarAnim.SetTrigger("Next");
        }
        if (ProgressValue >= .6f && ProgressLv == 1)
        {
            ProgressLv = 2;
            ProgressBarAnim.SetTrigger("Next");
        }

    }
}
