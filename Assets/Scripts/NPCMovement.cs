using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float myRadius = 0.5f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float pathfindingRefreshRate = 0.1f;
    [SerializeField] private float pathfindingPrecision = 0.1f;
    [SerializeField] private float pathfindingGranularity = 0.05f;
    [SerializeField] private Quaternion defaultOffset = Quaternion.identity;
    private Vector3 lastDirection;
    void Start()
    {
        lastDirection = Vector3.Cross(transform.localPosition, Vector3.up).normalized;
        if(target != null)
        MoveTo(target.localPosition);
    }
    void Update()
    {
        // debug code
        if(target != null)
        {
//            PlanPath(transform.localPosition, target.localPosition);
        }
        else
            Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
    }
    public void Place(Vector3 position)
    {
        Vector3 targetPosition = position.normalized * radius / transform.parent.localScale.x;
        (int count, Obstacle.ObstacleQuery[] queries) = Obstacle.QueryAll(targetPosition, myRadius);
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
        float refresh = 0;
        List<Vector3> path = PlanPath(transform.localPosition, position);
        path.RemoveAt(0); // remove current position
        Vector3 nextPoint = path[0];
        while(true)
        {
            if(refresh > pathfindingRefreshRate)
            {
                refresh = 0;
                path = PlanPath(transform.localPosition, position);
                path.RemoveAt(0); // remove current position
                if(path.Count == 0)
                {
                    yield break;
                }
                nextPoint = path[0];
            }
            refresh += Time.deltaTime;
            if((transform.localPosition - nextPoint).sqrMagnitude < pathfindingGranularity * pathfindingGranularity)
            {
                path.RemoveAt(0);
                if(path.Count == 0)
                {
                    yield break;
                }
                nextPoint = path[0];
            }
            MoveTowards(nextPoint);
            yield return null;
        }
    }

    private List<Vector3> PlanPath(Vector3 from, Vector3 to)
    {
        List<Vector3> path = new List<Vector3>();
        float distance = (to - from).magnitude;
        int steps = Mathf.CeilToInt(distance / pathfindingPrecision);
        Vector3 previousStep = from;
        for(int i = 1; i < steps; i++)
        {
            Vector3 position = Vector3.Lerp(from, to, (float)i / steps);
            position = position.normalized * radius / transform.parent.localScale.x;
            (int count, Obstacle.ObstacleQuery[] queries) = Obstacle.QueryAll(position, myRadius);
            if(count == 0)
            {
                path.Add(position);
                previousStep = position;
                continue;
            }
            Vector3 normal = Vector3.zero;
            while(count != 0)
            {
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
                    path.Add(position);
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
                position = previousStep + newDirection;
                position = position.normalized * radius / transform.parent.localScale.x;
                (count, queries) = Obstacle.QueryAll(position, myRadius);
                from = (to + (position - to).normalized * distance).normalized * radius / transform.parent.localScale.x;
            }
            path.Add(position);
            previousStep = position;
        }
        #if(UNITY_EDITOR)
        for(int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(transform.parent.TransformPoint(path[i]), transform.parent.TransformPoint(path[i + 1]), Color.red, pathfindingRefreshRate);
        }
        #endif
        return path;
    }
}
