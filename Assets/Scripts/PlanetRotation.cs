using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    [SerializeField] private new bool enabled = true;
    [SerializeField] private Transform planet;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float inertia = 0.9f;
    private Quaternion lastRotation;
    void OnMouseDown()
    {
        StopAllCoroutines();
    }
    void OnMouseDrag()
    {
        if(!enabled) return;
        float rotX = Input.GetAxis("Mouse X") * speed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * speed * Mathf.Deg2Rad;

        Quaternion rot = Quaternion.Euler(rotY, 0, -rotX);
        lastRotation = rot;
        planet.rotation = rot * planet.rotation;
    }

    void OnMouseUp()
    {
        if(!enabled) return;
        StartCoroutine(Inertia());
    }


    private IEnumerator Inertia()
    {
        float currentInertia = 1;
        while (currentInertia > 0)
        {
            planet.rotation = Quaternion.Lerp(Quaternion.identity, lastRotation, currentInertia) * planet.rotation;
            currentInertia = Mathf.Lerp(currentInertia, 0, Time.deltaTime * inertia);
            yield return null;
        }
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }
}
