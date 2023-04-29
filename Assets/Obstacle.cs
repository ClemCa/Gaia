using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    private static List<Obstacle> obstacles = new List<Obstacle>();
    void Awake()
    {
        obstacles.Add(this);
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
        float distance = (transform.localPosition - from).sqrMagnitude * transform.parent.localScale.x / 2f;
        if (distance <= Mathf.Pow((radius + originRadius), 2) / transform.parent.localScale.x)
        {
            return (true, new ObstacleQuery(transform.localPosition, radius / transform.parent.localScale.x));
        }
        return (false, new ObstacleQuery());
    }

    public static (int, ObstacleQuery[]) QueryAll(Vector3 from, float originRadius)
    {
        List<ObstacleQuery> queries = new List<ObstacleQuery>();
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
        return (count, queries.ToArray());
    }

    public static (bool, ObstacleQuery[]) QueryAny(Vector3 from, float originRadius)
    {
        foreach (Obstacle obstacle in obstacles)
        {
            (bool hit, ObstacleQuery query) = obstacle.Query(from, originRadius);
            if (hit)
            {
                return (true, new ObstacleQuery[] { query });
            }
        }
        return (false, new ObstacleQuery[0]);
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
