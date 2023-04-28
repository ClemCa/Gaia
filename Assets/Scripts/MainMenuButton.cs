using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float hoverSize = 1.2f;
    [SerializeField] private float hoverDuration = 0.1f;
    [SerializeField] private float clickedSize = 0.8f;
    [SerializeField] private float clickedDuration = 0.25f;
    private bool majorCoroutineRunning = false;
    private bool isHovered = false;public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        // don't if there's already a coroutine running
        if(majorCoroutineRunning)
            return;
        StartCoroutine(ResizeRoutine(hoverSize, hoverDuration));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        // don't if there's already a coroutine running
        if(majorCoroutineRunning)
            return;
        StartCoroutine(ResizeRoutine(1, hoverDuration));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        majorCoroutineRunning = true;
        StartCoroutine(ResizeRoutine(clickedSize, clickedDuration));
        StartCoroutine(DelayedReset());
    }
    public void Shrink()
    {
        StopAllCoroutines();
        majorCoroutineRunning = true;
        StartCoroutine(ResizeRoutine(0));
    }
    public void Grow()
    {
        StopAllCoroutines();
        majorCoroutineRunning = true;
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
        if(majorCoroutineRunning && isHovered)
        {
            StartCoroutine(ResizeRoutine(hoverSize, hoverDuration));
        }
        majorCoroutineRunning = false;
    }
    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(clickedDuration);
        transform.localScale = Vector3.one;
    }
}
