using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float radius = 10f;
    void Update()
    {
        // debug code
        if(target != null)
            MoveTowards(target.localPosition);
        else
            Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }
    public void Place(Vector3 position)
    {
        transform.localPosition = position.normalized * radius / transform.parent.localScale.x;
    }
    public void Move(Vector2 direction)
    {
        Vector3 up = transform.localPosition.normalized;
        Vector3 right = Vector3.Cross(up, Vector3.back).normalized;
        Vector3 forward = Vector3.Cross(-right, up).normalized;
        if(right == Vector3.zero)
            forward = Vector3.Cross(up, Vector3.right).normalized;
        Place(transform.localPosition + (right * direction.x + forward * direction.y) * Time.deltaTime);
    }
    public void MoveTo(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(position));
    }
    public void MoveTowards(Vector3 position)
    {
        Vector3 up = transform.localPosition.normalized;
        Vector3 right = Vector3.Cross(up, Vector3.back);
        Vector3 forward = Vector3.Cross(-right, up);
        float forwardDot = Vector3.Dot(position - transform.localPosition, forward);
        float rightDot = Vector3.Dot(position - transform.localPosition, right);
        Move(new Vector2(rightDot, forwardDot).normalized);
    }
    private IEnumerator MoveCoroutine(Vector3 position)
    {
        float t = 0;
        Vector3 start = transform.localPosition;
        while (t < 1)
        {
            t += Time.deltaTime;
            Place(Vector3.Lerp(start, position, t));
            yield return null;
        }
    }
}
