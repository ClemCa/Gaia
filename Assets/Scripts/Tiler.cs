using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiler : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private int divisions = 10;
    [SerializeField] private GameObject debugPrefab;
    [SerializeField] private GameObject debugPrefab2;

    private static Tiler instance;
    public static Tiler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Tiler>();
            }
            return instance;
        }
    }
    public struct Tile
    {
        public Vector2 coordinates;
        public Vector3 center;
        public Dictionary<string, object> data;
        public object this[string key]
        {
            get
            {
                return data[key];
            }
            set
            {
                data[key] = value;
            }
        }
        // default constructor
        public Tile(bool empty = true)
        {
            coordinates = Vector2.zero;
            center = Vector3.zero;
            data = new Dictionary<string, object>();
        }
    }

    private List<Tile> tiles = new List<Tile>();

    public Tile this[int index]
    {
        get
        {
            return tiles[index];
        }
        set
        {
            tiles[index] = value;
        }
    }

    public Tile this[Vector2 coordinates]
    {
        get
        {
            return tiles.Find(t => t.coordinates == coordinates);
        }
        set
        {
            tiles[tiles.FindIndex(t => t.coordinates == coordinates)] = value;
        }
    }

    void Awake()
    {
        instance = this;
        GenerateTiles();
    }
    public int GetFreeTile(Vector2 coordinates, string key, float radius)
    {
        List<int> freeTiles = new List<int>();
        for (int i = 0; i < tiles.Count; i++)
        {
            // tiles that are touching the coordinates do not use the radius
            Vector2 diff = tiles[i].coordinates - coordinates;
            if(diff.x <= 1 && diff.y <= 1 && !tiles[i].data.ContainsKey(key))
            {
                freeTiles.Add(i);
                continue;
            }
            else if (Vector3.Distance(tiles[i].center, tiles.Find(t => t.coordinates == coordinates).center) <= radius && !tiles[i].data.ContainsKey(key))
            {
                freeTiles.Add(i);
            }
        }
        if (freeTiles.Count == 0)
        {
            return -1;
        }
        return freeTiles[Random.Range(0, freeTiles.Count)];
    }
    public Transform SpawnTile(int tile, GameObject prefab)
    {
        Tile t = tiles[tile];
        // for half, use the other prefab
        GameObject go = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        // rotate to look out from center
        go.transform.localPosition = t.center / transform.localScale.x;
        go.transform.localRotation = Quaternion.LookRotation(t.center) * Quaternion.Euler(90, 0, 0);
        return go.transform;
    }
    public void SpawnTile(Vector2 coordinates, GameObject prefab)
    {
        SpawnTile(tiles.FindIndex(t => t.coordinates == coordinates), prefab);
    }

    private void GenerateTiles()
    {
        for(int x = 0; x < divisions; x++)
        {
            for(int y = 0; y < divisions; y++)
            {
                Vector3 center = GetSpherePoint(new Vector2(x, y)) * radius;
                tiles.Add(new Tile
                {
                    data = new Dictionary<string, object>(),
                    coordinates = new Vector2(x, y),
                    center = center,
                });
            }
        }
    }

    private Vector3 GetSpherePoint(Vector2 point)
    {
        Vector3[] points = PointsOnSphere(divisions * divisions);
        if (point.x % 1 == 0 && point.y % 1 == 0)
        {
            int index = (int)(point.x * divisions + point.y);
            return points[index];
        }
        else
        {
            // we need to get the 4 points around this point
            Vector2Int bottomLeft = new Vector2Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y));
            Vector2Int bottomRight = new Vector2Int(Mathf.CeilToInt(point.x), Mathf.FloorToInt(point.y));
            Vector2Int topLeft = new Vector2Int(Mathf.FloorToInt(point.x), Mathf.CeilToInt(point.y));
            Vector2Int topRight = new Vector2Int(Mathf.CeilToInt(point.x), Mathf.CeilToInt(point.y));
            // then proportions
            float xProportion = point.x - bottomLeft.x;
            float yProportion = point.y - bottomLeft.y;
            // then get the points
            Vector3 bottomLeftPoint = points[Wrap(bottomLeft.x * divisions + bottomLeft.y, divisions * divisions)];
            Vector3 bottomRightPoint = points[Wrap(bottomRight.x * divisions + bottomRight.y, divisions * divisions)];
            Vector3 topLeftPoint = points[Wrap(topLeft.x * divisions + topLeft.y, divisions * divisions)];
            Vector3 topRightPoint = points[Wrap(topRight.x * divisions + topRight.y, divisions * divisions)];
            // then interpolate
            Vector3 bottomPoint = Vector3.Lerp(bottomLeftPoint, bottomRightPoint, xProportion);
            Vector3 topPoint = Vector3.Lerp(topLeftPoint, topRightPoint, xProportion);
            Vector3 pointOnSphere = Vector3.Lerp(bottomPoint, topPoint, yProportion);
            return pointOnSphere.normalized;
        }
    }
    private int Wrap(int value, int max)
    {
        int wrapped = value % max;
        if (wrapped < 0)
        {
            wrapped += max;
        }
        return wrapped;
    }
    Vector3[] PointsOnSphere(int n)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x;
        float y;
        float z;
        float r;
        float phi;
        for (var k = 0; k < n; k++){
            y = k * off - 1 + (off /2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;
            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }
}
