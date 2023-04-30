using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUser : MonoBehaviour
{
    [SerializeField] private Button[] buttons = new Button[6];
    [SerializeField] private float[] radiuses = new float[6];
    [SerializeField] private float[] cooldowns = new float[6];
    private float[] cooldownTimers = new float[6];
    void Start()
    {
        cooldownTimers = new float[cooldowns.Length];
    }
    void Update()
    {
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            cooldownTimers[i] -= Time.deltaTime;
            buttons[i].interactable = cooldownTimers[i] <= 0f;
        }
    }
    public void UseSkill(int id)
    {
        if (cooldownTimers[id] > 0f)
            return;
        StartCoroutine(WaitThenPlaceSkill(id));
    }
    private IEnumerator WaitThenPlaceSkill(int id)
    {
        yield return null; // wait for a frame to not be on the frame the button is pressed
        SkillPlacer.PlaceSkill((position) =>
        {
            UseSkill(position, id);
        }, radiuses[id]);
    }
    public void UseSkill(Vector3 position, int id)
    {
        cooldownTimers[id] = cooldowns[id];
        var radius = radiuses[id];
        switch (id)
        {
            case 0:
                {
                    var targets = Nature.Instance.GetEntities((entity) =>
                    {
                        if (entity is not Nature.IAnimal animal)
                            return false;
                        return Vector3.Distance(position, animal.Transform.localPosition) <= radius;
                    });
                    foreach (var target in targets)
                    {
                        var entity = Nature.Instance.Entities[target] as Nature.IAnimal;
                        entity.Hunger = 0;
                        entity.Sleepiness = 0;
                        entity.Horniness *= 2;
                        Nature.Instance.Entities[target] = entity;
                    }
                    break;
                }
            case 1:
                {
                    var targets = Nature.Instance.GetEntities((entity) =>
                    {
                        if (entity is not Nature.IAnimal animal)
                            return false;
                        return Vector3.Distance(position, animal.Transform.localPosition) <= radius;
                    });
                    foreach (var target in targets)
                    {
                        // 50% chance to kill
                        if (Random.value < 0.5f)
                        {
                            var entity = Nature.Instance.Entities[target] as Nature.IAnimal;
                            entity.Health = 0;
                            entity.Animation.SetAnimationState(NPCAnimation.AnimationState.Dead);
                            if (entity.Type is Nature.EntityType.Herbivore)
                            {
                                if (entity.Age < entity.AdultAge)
                                {
                                    entity.Sounds.PlaySound(NPCSounds.Sounds.GoatBabyDead);
                                }
                                else
                                {
                                    entity.Sounds.PlaySound(NPCSounds.Sounds.GoatDead);
                                }
                            }
                            else
                            {
                                entity.Sounds.PlaySound(NPCSounds.Sounds.BearDead);
                            }
                            Nature.Instance.Entities[target] = entity;
                        }
                    }
                }
                break;
            case 2:
                {
                    var targets = Nature.Instance.GetEntities((entity) =>
                    {
                        if (entity is not Nature.IWater water)
                            return false;
                        return Vector3.Distance(position, water.Transform.localPosition) <= radius;
                    });
                    foreach (var target in targets)
                    {
                        // 50% chance to kill
                        if (Random.value < 0.5f)
                        {
                            var entity = Nature.Instance.Entities[target] as Nature.IWater;
                            entity.Substance /= 2;
                            Nature.Instance.Entities[target] = entity;
                        }
                    }
                }
                break;
            case 3:
                {
                    var targets = Nature.Instance.GetEntities((entity) =>
                    {
                        if (entity is not Nature.IPlant plant)
                            return false;
                        return Vector3.Distance(position, plant.Transform.localPosition) <= radius;
                    });
                    foreach (var target in targets)
                    {
                        // 50% chance to kill
                        if (Random.value < 0.5f)
                        {
                            var entity = Nature.Instance.Entities[target] as Nature.IPlant;
                            entity.ReproductionRate *= 2;
                            Nature.Instance.Entities[target] = entity;
                        }
                    }
                }
                break;
            case 4:
                {
                    var targets = Nature.Instance.GetEntities((entity) =>
                    {
                        if (entity is not Nature.IWater water)
                            return false;
                        return Vector3.Distance(position, water.Transform.localPosition) <= radius;
                    });
                    foreach (var target in targets)
                    {
                        // 50% chance to kill
                        if (Random.value < 0.5f)
                        {
                            var entity = Nature.Instance.Entities[target] as Nature.IWater;
                            entity.Substance *= 2;
                            Nature.Instance.Entities[target] = entity;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}
