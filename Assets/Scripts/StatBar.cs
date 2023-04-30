using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBar : MonoBehaviour
{
    [SerializeField] private Nature.EntityType entityType;
    [SerializeField] private float highestValue = 100f;
    private float value;
    private RectTransform child;
    void Start()
    {
        child = transform.GetChild(0).GetComponent<RectTransform>();
    }
    void Update()
    {
        value = entityType switch
        {
            Nature.EntityType.Herbivore => Nature.Instance.MeasureEntities(Nature.EntityType.Herbivore),
            Nature.EntityType.Carnivorous => Nature.Instance.MeasureEntities(Nature.EntityType.Carnivorous),
            Nature.EntityType.Plant => Nature.Instance.MeasureEntities(Nature.EntityType.Plant),
            Nature.EntityType.Water => Nature.Instance.MeasureEntities(Nature.EntityType.Water),
            _ => 0f,
        };
        child.anchorMax = new Vector2(value / highestValue, 1f);
    }
}
