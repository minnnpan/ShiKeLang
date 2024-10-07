using UnityEngine;
using UnityEngine.UI;

public class BarControl : MonoBehaviour
{
    public Image ProgressFill, PoopFill;
    public Animator ProgressBarAnim;
    private int ProgressLv;
    
    private float maxDungSize = 3f; // the max dung size is 3
    private float minDungSize = 0.5f; // the max dung size is 3
    private float winPercentage = 0.8f;

    private void Start()
    {
        ProgressLv = 0;
    }

    public void UpdateBars(float currentDungSize, float currentCoverage)
    {
        // Update PoopFill (dung size)
        PoopFill.fillAmount = (currentDungSize - minDungSize) / (maxDungSize - minDungSize);

        // Update ProgressFill (coverage)
        float progressValue = currentCoverage / winPercentage;
        ProgressFill.fillAmount = Mathf.Clamp01(progressValue);

        // Update ProgressBar animation
        if (progressValue >= 0.3f && ProgressLv == 0)
        {
            ProgressLv = 1;
            ProgressBarAnim.SetTrigger("Next");
        }
        if (progressValue >= 0.6f && ProgressLv == 1)
        {
            ProgressLv = 2;
            ProgressBarAnim.SetTrigger("Next");
        }
    }
}
