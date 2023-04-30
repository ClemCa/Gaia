using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private float myRadius = 0.5f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float pathfindingRefreshRate = 0.1f;
    [SerializeField] private float pathfindingPrecision = 0.1f;
    [SerializeField] private float pathfindingGranularity = 0.05f;
    [SerializeField] private float pathfindingMaxIterations = 100;
    [SerializeField] private Quaternion defaultOffset = Quaternion.identity;
    private Vector3 lastDirection;
    void Start()
    {
        lastDirection = Vector3.Cross(transform.localPosition, Vector3.up).normalized;
    }
    public void SetSpeed(float speed)
    {
        movementSpeed = speed;
    }
    public void Place(Vector3 position)
    {
        Vector3 targetPosition = position.normalized * radius / transform.parent.localScale.x;
        (int count, List<Obstacle.ObstacleQuery> queries) = Obstacle.QueryAll(targetPosition, myRadius);
        if(count == 0)
        {
            Vector3 direction = (targetPosition - transform.localPosition).normalized;
            if(direction != Vector3.zero)
                lastDirection = direction;
            transform.localPosition = targetPosition;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(lastDirection, transform.localPosition.normalized) * defaultOffset, Time.deltaTime * rotationSpeed);
            return;
        }
        Vector3 normal = Vector3.zero;
        foreach (Obstacle.ObstacleQuery query in queries)
        {
            Vector3 obstacleDirection = (query.position - transform.localPosition).normalized;
            float distance = (query.position - transform.localPosition).magnitude;
            float overlap = myRadius + query.radius - distance;
            normal -= obstacleDirection * overlap;
        }
        normal = normal.normalized;
        Vector3 velocity = position - transform.localPosition;
        if(Vector3.Dot(normal, velocity) > 0)
        {
            if(velocity != Vector3.zero)
            {
                Vector3 newPos = (transform.localPosition + velocity).normalized * radius / transform.parent.localScale.x;
                lastDirection = (newPos - transform.localPosition).normalized;
            }
            transform.localPosition = (transform.localPosition + velocity).normalized * radius / transform.parent.localScale.x;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(lastDirection, transform.localPosition.normalized) * defaultOffset, Time.deltaTime * rotationSpeed);
            return;
        }
        Vector3 newDirection = Vector3.ProjectOnPlane(velocity, normal);
        if(newDirection != Vector3.zero)
        {
            Vector3 newPos = (transform.localPosition + newDirection).normalized * radius / transform.parent.localScale.x;
            lastDirection = (newPos - transform.localPosition).normalized;
        }
        transform.localPosition = (transform.localPosition + newDirection).normalized * radius / transform.parent.localScale.x;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(lastDirection, transform.localPosition.normalized) * defaultOffset, Time.deltaTime * rotationSpeed);
    }
    public void Move(Vector2 direction)
    {
        Vector3 up = transform.localPosition.normalized;
        Vector3 right = Vector3.Cross(up, Vector3.back).normalized;
        Vector3 forward = Vector3.Cross(-right, up).normalized;
        if(right == Vector3.zero)
            forward = Vector3.Cross(up, Vector3.right).normalized;
        Place(transform.localPosition + (right * direction.x + forward * direction.y).normalized * movementSpeed * Time.deltaTime);
    }
    public void Move(Vector2 direction, float distance)
    {
        Move(direction, distance, null);
    }
    public void Move(Vector2 direction, float distance, Action callback)
    {
        Vector3 up = transform.localPosition.normalized;
        Vector3 right = Vector3.Cross(up, Vector3.back).normalized;
        Vector3 forward = Vector3.Cross(-right, up).normalized;
        if(right == Vector3.zero)
            forward = Vector3.Cross(up, Vector3.right).normalized;
        MoveTo(transform.localPosition + (right * direction.x + forward * direction.y).normalized * distance, callback);
    }
    public void MoveTo(Vector3 position)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(position));
    }
    public void MoveTo(Vector3 position, Action callback)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(position, callback));
    }
    public void MoveTo(Transform target, Action callback)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(target, callback));
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
    public void Stop()
    {
        StopAllCoroutines();
    }
    private IEnumerator MoveCoroutine(Transform target, Action callback = null)
    {
        float refresh = -UnityEngine.Random.Range(0, pathfindingRefreshRate);
        List<Vector3> path = PlanPath(transform.localPosition, target.localPosition).ToList();
        Vector3 nextPoint = path[0];
        while(true)
        {
            if(refresh > pathfindingRefreshRate)
            {
                refresh = 0;
                path = PlanPath(transform.localPosition, target.localPosition).ToList();
                if(path.Count == 0)
                {
                    callback?.Invoke();
                    Debug.Log("Arrived at target");
                    yield break;
                }
                nextPoint = path[0];
            }
            refresh += Time.deltaTime;
            if((transform.localPosition - nextPoint).sqrMagnitude < pathfindingPrecision * pathfindingGranularity)
            {
                path.RemoveAt(0);
                if(path.Count == 0)
                {
                    Debug.Log("Arrived at target");
                    callback?.Invoke();
                    yield break;
                }
                nextPoint = path[0];
            }
            MoveTowards(nextPoint);
            yield return null;
        }
    }
    private IEnumerator MoveCoroutine(Vector3 position, Action callback = null)
    {
        float refresh = -UnityEngine.Random.Range(0, pathfindingRefreshRate);
        List<Vector3> path = PlanPath(transform.localPosition, position).ToList();
        Vector3 nextPoint = path[0];
        while(true)
        {
            if(refresh > pathfindingRefreshRate)
            {
                refresh = 0;
                path = PlanPath(transform.localPosition, position).ToList();
                if(path.Count == 0)
                {
                    callback?.Invoke();
                    Debug.Log("Arrived at destination");
                    yield break;
                }
                nextPoint = path[0];
            }
            refresh += Time.deltaTime;
            if((transform.localPosition - nextPoint).sqrMagnitude < pathfindingPrecision * pathfindingGranularity)
            {
                path.RemoveAt(0);
                if(path.Count == 0)
                {
                    Debug.Log("Arrived at destination");
                    callback?.Invoke();
                    yield break;
                }
                nextPoint = path[0];
            }
            MoveTowards(nextPoint);
            yield return null;
        }
    }

    private Vector3[] PlanPath(Vector3 from, Vector3 to)
    {
        float distance = (to - from).magnitude;
        int steps = Mathf.CeilToInt(distance / pathfindingPrecision);
        Vector3 previousStep = from;
        Vector3[] path = new Vector3[steps];
        float scaleFactor = radius / transform.parent.localScale.x;
        int pathIndex = 0;
        for(int i = 1; i < steps; i++)
        {
            Vector3 position = Vector3.Lerp(from, to, (float)i / steps);
            position = position.normalized * scaleFactor;
            (int count, List<Obstacle.ObstacleQuery> queries) = Obstacle.QueryAll(position, myRadius);
            if(count == 0)
            {
                path[pathIndex++] = position;
                previousStep = position;
                continue;
            }
            Vector3 normal = Vector3.zero;
            foreach (Obstacle.ObstacleQuery query in queries)
            {
                Vector3 obstacleDirection = (query.position - position).normalized;
                float obstacleDistance = (query.position - position).magnitude;
                float overlap = myRadius + query.radius - obstacleDistance;
                normal -= obstacleDirection * overlap;
            }
            normal = normal.normalized;
            Vector3 velocity = position - previousStep;
            if(Vector3.Dot(normal, velocity) > 0)
            {
                path[pathIndex++] = position;
                previousStep = position;
                break;
            }
            Vector3 newDirection = Vector3.ProjectOnPlane(velocity, normal);
            Vector3 right = Vector3.Cross(position, normal);
            float rightDot = Vector3.Dot(newDirection, right);
            if(rightDot < 0)
                rightDot = -pathfindingGranularity;
            else
                rightDot = pathfindingGranularity;
            newDirection = velocity + right * rightDot;
            for(count = 0; count < pathfindingMaxIterations; count++)
            {
                previousStep += newDirection;
                position = previousStep.normalized * scaleFactor;
                if(!Obstacle.QueryAllNoReturn(position, myRadius))
                {
                    break;
                }
            }
            from = (to + (position - to).normalized * distance).normalized * scaleFactor;
            path[pathIndex++] = position;
            previousStep = position;
        }
        if(pathIndex == 0)
        {
            path[pathIndex++] = to;
        }
        #if(UNITY_EDITOR)
        Debug.DrawLine(transform.parent.TransformPoint(from), transform.parent.TransformPoint(path[0]), Color.red, pathfindingRefreshRate);
        for(int i = 0; i < pathIndex - 1; i++)
        {
            Debug.DrawLine(transform.parent.TransformPoint(path[i]), transform.parent.TransformPoint(path[i + 1]), Color.red, pathfindingRefreshRate);
        }
        #endif
        Array.Resize(ref path, pathIndex);
        return path;
    }
}
