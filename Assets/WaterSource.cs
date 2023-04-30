using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [SerializeField] private float epsilon = 0.001f;
    [SerializeField] private float minScale = 0.0001f;
    [SerializeField] private float maxScale = 0.05f;
    [SerializeField] private float maxAmount = 1000f;
    Vector3 position;
    public string GUID;
    void Start()
    {
        position = transform.localPosition;
    }
    void Update()
    {
        float amount = Nature.Instance.GetSubstance(GUID);
        transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, amount / maxAmount);
        // 0.5 - scale / 2
        transform.localPosition = position.normalized * (0.5f - transform.localScale.x / 2f * (1 + epsilon));
    }
}
