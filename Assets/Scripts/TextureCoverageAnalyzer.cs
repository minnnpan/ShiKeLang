using UnityEngine;
using PaintCore;
using TMPro;

public class TextureCoverageAnalyzer : MonoBehaviour
{
    public CwChannelCounter channelCounter;
    public TextMeshProUGUI displayText;

    void Update()
    {
        if (channelCounter != null && channelCounter.HasRead)
        {
            int coveredPixelsR = channelCounter.CountR;
            int coveredPixelsG = channelCounter.CountG;
            int coveredPixelsB = channelCounter.CountB;
            int totalPixels = channelCounter.Total;

            float averageCoveredPixels = (coveredPixelsR + coveredPixelsG + coveredPixelsB) / 3f;
            float coverageRatio = averageCoveredPixels / totalPixels;
            
            float displayValue = (1 - coverageRatio) * 2 * 100;
            
            if (displayText != null)
            {
                displayText.text = $"Coverage: {displayValue:F1}%";
            }
        }
    }
}