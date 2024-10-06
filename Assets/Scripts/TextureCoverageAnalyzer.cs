using UnityEngine;
using PaintCore;
using TMPro;

public class TextureCoverageAnalyzer : MonoBehaviour
{
    public CwChannelCounter channelCounter;

    void Update()
    {
        // CalculateCoverage();
    }

    private float CalculateCoverage()
    {
        if (channelCounter != null && channelCounter.HasRead)
        {
            int coveredPixelsR = channelCounter.CountR;
            int coveredPixelsG = channelCounter.CountG;
            int coveredPixelsB = channelCounter.CountB;
            int totalPixels = channelCounter.Total;

            float averageCoveredPixels = (coveredPixelsR + coveredPixelsG + coveredPixelsB) / 3f;
            float coverageRatio = averageCoveredPixels / totalPixels;
            
            float displayValue = (1 - coverageRatio) * 2;
            
            return displayValue;
        }

        return -1f;
    }
    
    public float GetCurrentCoverage()
    {
        return CalculateCoverage();
    }

    public void ResetCoverage()
    {
        GameManager.Instance.ResetGroundTexture();
        CalculateCoverage();
    }
}