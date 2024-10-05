using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungCoverageCalculator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coverageText;
    [SerializeField] private int raycastResolution = 100;
    [SerializeField] private float raycastHeight = 10f;

    private GameObject ground;

    private void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground == null)
        {
            Debug.LogError("No object with 'Ground' tag found!");
        }
    }

    private void Update()
    {
        float coverage = CalculateCoverage();
        UpdateCoverageDisplay(coverage);
    }

    private float CalculateCoverage()
    {
        if (ground == null) return 0f;

        Renderer groundRenderer = ground.GetComponent<Renderer>();
        if (groundRenderer == null) return 0f;

        Bounds bounds = groundRenderer.bounds;
        int totalHits = 0;
        int dungHits = 0;

        for (int x = 0; x < raycastResolution; x++)
        {
            for (int z = 0; z < raycastResolution; z++)
            {
                Vector3 rayStart = new Vector3(
                    Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / raycastResolution),
                    bounds.max.y + raycastHeight,
                    Mathf.Lerp(bounds.min.z, bounds.max.z, (float)z / raycastResolution)
                );

                RaycastHit hit;
                if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastHeight * 2))
                {
                    totalHits++;
                    if (hit.collider.CompareTag("DungTrail"))
                    {
                        dungHits++;
                    }
                }
            }
        }

        return (float)dungHits / totalHits;
    }

    private void UpdateCoverageDisplay(float coverage)
    {
        if (coverageText != null)
        {
            coverageText.text = $"Dung Coverage: {coverage:P2}";
        }
    }

    public float GetCurrentCoverage()
    {
        return CalculateCoverage();
    }
}