using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NeutralShrinkable : MonoBehaviour
{
    [SerializeField] private float animationDuration = 2f;
    void Start()
    {
        transform.localScale = Vector3.zero;
    }
    public void Shrink()
    {
        StopAllCoroutines();
        StartCoroutine(ResizeRoutine(0));
    }
    public void Grow()
    {
        StopAllCoroutines();
        StartCoroutine(ResizeRoutine(1));
    }
    private IEnumerator ResizeRoutine(float targetScale, float duration = -1)
    {
        if(duration < 0)
            duration = animationDuration;
        float Sigmoid(float v, float m)
        {
            float k = Mathf.Exp(-(v*2-1) * m);
            return 1 / (1f + k);
        }
        float t = 0;
        float fromScale = transform.localScale.x;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            float s = Sigmoid(t, 10);
            transform.localScale = Vector3.one * Mathf.Lerp(fromScale, targetScale, s);
            yield return null;
        }
        transform.localScale = Vector3.one * targetScale;
    }
}
