using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPlacer : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private float radius = 1f;
    private SkillUseData? data = null;
    private static SkillPlacer instance;
    public struct SkillUseData
    {
        public System.Action<Vector3> callback;
        public Transform target;
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if(!data.HasValue)
            return;
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
        point.y = 10 - Mathf.Abs(point.x) - Mathf.Abs(point.z);
        data.Value.target.position = point.normalized * radius;
        data.Value.target.rotation = Quaternion.identity;
        if(Input.GetMouseButtonDown(0))
        {
            data.Value.callback?.Invoke(data.Value.target.localPosition);
            Destroy(data.Value.target.gameObject);
            data = null;
        }
        if(Input.GetMouseButtonDown(1))
        {
            Destroy(data.Value.target.gameObject);
            data = null;
        }
    }
    public static void PlaceSkill(System.Action<Vector3> callback, float radius = 1f)
    {
        if(instance.data.HasValue)
        {
            Destroy(instance.data.Value.target.gameObject);
        }
        instance.data = new SkillUseData()
        {
            callback = callback,
            target = Instantiate(instance.targetPrefab, instance.transform.position, Quaternion.identity, instance.transform).transform
        };
        instance.data.Value.target.localScale = new Vector3(radius, 0.02f, radius);
    }
}
