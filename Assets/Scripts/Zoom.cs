using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField] private float defaultDistance = 10f;
    [SerializeField] private float furthest = 10f;
    [SerializeField] private float closest = 1f;
    [SerializeField] private int steps = 10;
    [SerializeField] private float inertia = 0.9f;
    private int currentStep = 0;
    void Start()
    {
        currentStep = Mathf.RoundToInt(Mathf.InverseLerp(furthest, closest, defaultDistance) * steps);
        transform.localPosition = transform.localPosition.normalized * Mathf.Lerp(furthest, closest, (float)currentStep / steps);
    }
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentStep = Mathf.Clamp(currentStep + 1, 0, steps);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentStep = Mathf.Clamp(currentStep - 1, 0, steps);
        }
        float currentDistance = Mathf.Lerp(furthest, closest, (float)currentStep / steps);
        transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition.normalized * currentDistance, Time.deltaTime * inertia);
    }
}
