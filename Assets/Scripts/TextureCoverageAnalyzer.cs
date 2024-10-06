using UnityEngine;
using PaintCore;

public class TextureCoverageAnalyzer : MonoBehaviour
{
    public CwChannelCounter channelCounter;

    void Update()
    {
        if (channelCounter != null && channelCounter.HasRead)
        {
            int coveredPixelsR = channelCounter.CountR;
            int coveredPixelsG = channelCounter.CountG;
            int coveredPixelsB = channelCounter.CountB;
            int totalPixels = channelCounter.Total;

            // 计算RGB通道的平均覆盖像素数
            float averageCoveredPixels = (coveredPixelsR + coveredPixelsG + coveredPixelsB) / 3f;

            float coverageRatio = averageCoveredPixels / totalPixels;
            Debug.Log($"Texture coverage: {coverageRatio * 100:F2}%");
        }
    }
}