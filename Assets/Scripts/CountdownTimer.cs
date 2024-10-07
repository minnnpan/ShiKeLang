using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    public float countdownDuration = 3f;
    public UnityEvent onCountdownStart;
    public UnityEvent onCountdownEnd;

    private bool isCountingDown = false;
    private Coroutine countdownCoroutine;

    public void StartCountdown()
    {
        if (!isCountingDown)
        {
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }
    }

    public void StopCountdown()
    {
        if (isCountingDown)
        {
            StopCoroutine(countdownCoroutine);
            isCountingDown = false;
        }
    }

    public event Action<int> OnCountdownTick;
    public event Action OnCountdownComplete;

    private IEnumerator CountdownCoroutine()
    {
        isCountingDown = true;
        onCountdownStart.Invoke();

        float remainingTime = countdownDuration;

        while (remainingTime > 0)
        {
            int number = Mathf.CeilToInt(remainingTime);
            OnCountdownTick?.Invoke(number);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        OnCountdownComplete?.Invoke();
        yield return new WaitForSeconds(0.5f);
        isCountingDown = false;
        onCountdownEnd.Invoke();
    }

    public void ResetTimer()
    {
        StopAllCoroutines();
    }
}