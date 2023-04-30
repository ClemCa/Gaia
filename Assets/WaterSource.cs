using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSource : MonoBehaviour
{
    [SerializeField] private float epsilon = 0.001f;
    [SerializeField] private float minScale = 0.0001f;
    [SerializeField] private float maxScale = 0.05f;
    [SerializeField] private float maxAmount = 1000f;
    [SerializeField] private float minPrecision = 0f;
    [SerializeField] private float maxPrecision = 0.5f;
    [SerializeField] private Vector3 trueSize = Vector3.one;
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
        var multiply = Multiply(position.normalized, trueSize) * 0.5f;
        transform.localPosition = (multiply - position.normalized * transform.localScale.x * 0.5f) * (1 + Mathf.Lerp(minPrecision, maxPrecision, amount / maxAmount));
    }

    private Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
