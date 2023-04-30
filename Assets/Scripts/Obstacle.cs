using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    private Vector3 position;
    private float parentScale;
    private static List<Obstacle> obstacles = new List<Obstacle>();
    void Awake()
    {
        obstacles.Add(this);
        parentScale = transform.parent.localScale.x;
        position = transform.localPosition;
    }
    void OnDestroy()
    {
        obstacles.Remove(this);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius/2f);
    }
    public struct ObstacleQuery
    {
        public Vector3 position;
        public float radius;
        public ObstacleQuery(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
    public (bool, ObstacleQuery) Query(Vector3 from, float originRadius)
    {
        float radiusSquared = Mathf.Pow(radius + originRadius, 2) / parentScale;
        float distance = (position - from).sqrMagnitude * parentScale / 2f;
        if (distance <= radiusSquared)
        {
            return (true, new ObstacleQuery(position, radius / parentScale));
        }
        return (false, new ObstacleQuery());
    }
    public bool QueryNoReturn(Vector3 from, float originRadius)
    {
        float distance = (position - from).sqrMagnitude * parentScale / 2f;
        if (distance <= Mathf.Pow(radius + originRadius, 2) / parentScale)
        {
            return true;
        }
        return false;
    }
    public static bool QueryAllNoReturn(Vector3 from, float originRadius)
    {
        int count = 0;
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (obstacles[i].QueryNoReturn(from, originRadius))
            {
                count++;
            }
        }
        return false;
    }
    public static (int, List<ObstacleQuery>) QueryAll(Vector3 from, float originRadius)
    {
        List<ObstacleQuery> queries = new();
        int count = 0;
        foreach (Obstacle obstacle in obstacles)
        {
            (bool hit, ObstacleQuery query) = obstacle.Query(from, originRadius);
            if (hit)
            {
                count++;
                queries.Add(query);
            }
        }
        return (count, queries);
    }

    public static (bool, ObstacleQuery) QueryAny(Vector3 from, float originRadius)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            (bool hit, ObstacleQuery query) = obstacle.Query(from, originRadius);
            if (hit)
            {
                return (true, query);
            }
        }
        return (false, default);
    }

    public static bool Check(Vector3 from, float originRadius)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            (bool hit, ObstacleQuery _) = obstacle.Query(from, originRadius);
            if (hit)
            {
                return true;
            }
        }
        return false;
    }
}
