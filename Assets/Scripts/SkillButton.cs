using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float hoverSize = 1.2f;
    [SerializeField] private float hoverDuration = 0.1f;
    [SerializeField] private float clickedSize = 0.8f;
    [SerializeField] private float clickedDuration = 0.25f;
    [SerializeField] private CanvasGroup description;
    [SerializeField] private float descriptionFadeDelay = 0.5f;
    [SerializeField] private float descriptionFadeDuration = 0.5f;
    private bool majorCoroutineRunning = false;
    private bool isHovered = false;
    void Start()
    {
        transform.localScale = Vector3.zero;
        description.gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        // don't if there's already a coroutine running
        if(majorCoroutineRunning)
            return;
        StopAllCoroutines();
        StartCoroutine(ResizeRoutine(hoverSize, hoverDuration));
        StartCoroutine(FadeRoutine(1));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        // don't if there's already a coroutine running
        if(majorCoroutineRunning)
            return;
        StopAllCoroutines();
        StartCoroutine(ResizeRoutine(1, hoverDuration));
        StartCoroutine(FadeRoutine(0));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        StopAllCoroutines();
        majorCoroutineRunning = true;
        description.alpha = 0;
        description.gameObject.SetActive(false);
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
            StartCoroutine(FadeRoutine(1));
        }
        majorCoroutineRunning = false;
    }
    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(clickedDuration);
        transform.localScale = Vector3.one;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        if(targetAlpha == description.alpha)
        {
            yield break;
        }
        if(targetAlpha == 1)
            description.gameObject.SetActive(true);
        if(description.alpha == 0 || description.alpha == 1)
            yield return new WaitForSeconds(descriptionFadeDelay);
        float t = 0;
        float fromAlpha = description.alpha;
        while (t < 1)
        {
            t += Time.deltaTime / descriptionFadeDuration;
            description.alpha = Mathf.Lerp(fromAlpha, targetAlpha, t);
            yield return null;
        }
        description.alpha = targetAlpha;
        if(targetAlpha == 0)
            description.gameObject.SetActive(false);
    }
}
