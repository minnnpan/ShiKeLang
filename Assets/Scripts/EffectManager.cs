using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [System.Serializable]
    public class EffectEntry
    {
        public string effectName;
        public GameObject effectPrefab;
        public int poolSize = 10;
    }

    public List<EffectEntry> effects = new List<EffectEntry>();

    private Dictionary<string, Queue<GameObject>> effectPools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (var effect in effects)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < effect.poolSize; i++)
            {
                GameObject obj = Instantiate(effect.effectPrefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            effectPools[effect.effectName] = pool;
        }
    }

    public GameObject PlayEffect(string effectName, Vector3 position, Quaternion rotation, float duration = 0f)
    {
        if (effectPools.TryGetValue(effectName, out Queue<GameObject> pool))
        {
            GameObject effectInstance;
            if (pool.Count > 0)
            {
                effectInstance = pool.Dequeue();
            }
            else
            {
                EffectEntry entry = effects.Find(e => e.effectName == effectName);
                effectInstance = Instantiate(entry.effectPrefab);
            }

            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            effectInstance.SetActive(true);

            if (duration > 0)
            {
                StartCoroutine(ReturnToPool(effectInstance, effectName, duration));
            }

            return effectInstance;
        }
        else
        {
            Debug.LogWarning($"Effect '{effectName}' not found in EffectManager.");
            return null;
        }
    }
    
    public GameObject PlayEffect(string effectName, Vector3 position, float duration = 0f)
    {
        if (effectPools.TryGetValue(effectName, out Queue<GameObject> pool))
        {
            GameObject effectInstance;
            if (pool.Count > 0)
            {
                effectInstance = pool.Dequeue();
            }
            else
            {
                EffectEntry entry = effects.Find(e => e.effectName == effectName);
                effectInstance = Instantiate(entry.effectPrefab);
            }

            effectInstance.transform.position = position;
            effectInstance.SetActive(true);

            if (duration > 0)
            {
                StartCoroutine(ReturnToPool(effectInstance, effectName, duration));
            }

            return effectInstance;
        }
        else
        {
            Debug.LogWarning($"Effect '{effectName}' not found in EffectManager.");
            return null;
        }
    }

    private System.Collections.IEnumerator ReturnToPool(GameObject obj, string effectName, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obj, effectName);
    }

    public void ReturnToPool(GameObject obj, string effectName)
    {
        obj.SetActive(false);
        effectPools[effectName].Enqueue(obj);
    }
}