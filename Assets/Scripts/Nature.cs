using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Nature : MonoBehaviour
{
    [SerializeField] private float globalInteractionRange = 0.1f;
    [SerializeField] private float needThreshold = 0.1f;
    [SerializeField] private float roamingDistance = 0.3f;
    [SerializeField] private float eatingSpeed = 0.1f;
    [SerializeField] private float drinkingSpeed = 0.1f;
    [SerializeField] private float sleepingSpeed = 0.1f;
    [SerializeField] private float matingSpeed = 0.1f;
    [SerializeField] private float growthSpeed = 0.1f;
    [SerializeField] private float attackDelay = 1;
    [SerializeField] private float hungerDeathThreshold = 120f;
    [SerializeField] private float thirstDeathThreshold = 120f;
    [SerializeField] private float sleepDeathThreshold = 120f;
    [SerializeField] private float ageDeathThreshold = 1200f;
    [SerializeField] private float minimumIdleTime = 1;
    [SerializeField] private float maximumIdleTime = 5;
    private List<IEntity> Entities = new List<IEntity>();
    private EntityActions Actions = new()
    {
        Actions = new List<EntityAction>()
    };
    private static Nature instance;
    public static Nature Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Nature>();
            }
            return instance;
        }
    }
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        UpdateEntities();
    }
    #region Declarations
    public struct EntityAction
    {
        public string GUID;
        public long LastAction;
        public long CancelID;
        public void Cancel()
        {
            CancelID = LastAction;
        }
        public void RegisterAction()
        {
            LastAction++;
        }
        public bool IsCancelled(long id)
        {
            return id <= CancelID;
        }
    }
    public struct ActionCancellation
    {
        public string GUID;
        public long Action;
        public bool IsCancelled(EntityActions actions)
        {
            return actions.IsCancelled(GUID, Action);
        }
    }
    public struct EntityActions
    {
        public List<EntityAction> Actions;
        public EntityAction this[string guid]
        {
            get
            {
                return Actions.Find(a => a.GUID == guid);
            }
            set
            {
                Actions[Actions.FindIndex(a => a.GUID == guid)] = value;
            }
        }
        public void Cancel(string guid)
        {
            int index = Actions.FindIndex(a => a.GUID == guid);
            if (index != -1)
            {
                EntityAction action = Actions[index];
                action.Cancel();
                Actions[index] = action;
            }
        }
        public ActionCancellation RegisterAction(string guid)
        {
            int index = Actions.FindIndex(a => a.GUID == guid);
            if(index == -1)
            {
                Actions.Add(new EntityAction() { GUID = guid });
                index = Actions.Count - 1;
            }
            EntityAction action = Actions[index];
            action.RegisterAction();
            Actions[index] = action;
            return new ActionCancellation() { GUID = guid, Action = action.LastAction };
        }
        public bool IsCancelled(string guid, long id)
        {
            return Actions[Actions.FindIndex(a => a.GUID == guid)].IsCancelled(id);
        }
    }
    [Serializable]
    public struct Water : IWater
    {
        [SerializeField] private EntityType type;
        [SerializeField] private float amount;
        [SerializeField] private float substance;
        [SerializeField] private float size;
        [SerializeField] private float interactionRange;
        public string GUID { get; set; }
        public Transform Transform { get; set; }
        public EntityType Type { get => type; set => type = value; }
        public float Amount { get => amount; set => amount = value; }
        public float Substance { get => substance; set => substance = value; }
        public float Size { get => size; set => size = value; }
        public float InteractionRange { get => interactionRange; set => interactionRange = value; }
        public static implicit operator string(Water water) => water.GUID;
    }
    [Serializable]
    public struct Animal : IAnimal
    {
        [SerializeField] private EntityType type;
        [SerializeField] private float speed;
        [SerializeField] private float strength;
        [SerializeField] private float age;
        [SerializeField] private float adultAge;
        [SerializeField] private float growthRate;
        [SerializeField] private float health;
        [SerializeField] private float adultHealth;
        [SerializeField] private float substance;
        [SerializeField] private float size;
        [SerializeField] private float childSize;
        [SerializeField] private float adultSize;
        [SerializeField] private float interactionRange;
        public string GUID { get; set; }
        public Transform Transform { get; set; }
        public NPCMovement Movement { get; set; }
        public NPCAnimation Animation { get; set; }
        public EntityType Type { get => type; set => type = value; }
        public float Speed { get => speed; set => speed = value; }
        public float Strength { get => strength; set => strength = value; }
        public float Hunger { get; set; }
        public float Sleepiness { get; set; }
        public float Horniness { get; set; }
        public float Age { get => age; set => age = value; }
        public float GrowthRate { get => growthRate; set => growthRate = value; }
        public float AdultAge { get => adultAge; set => adultAge = value; }
        public float Health { get => health; set => health = value; }
        public float Thirst { get; set; }
        public float Substance { get => substance; set => substance = value; }
        public float Size { get => size; set => size = value; }
        public float ChildSize { get => childSize; set => childSize = value; }
        public float AdultSize { get => adultSize; set => adultSize = value; }
        public float InteractionRange { get => interactionRange; set => interactionRange = value; }
        public float AdultHealth { get => adultHealth; set => adultHealth = value; }
        public static implicit operator string(Animal animal) => animal.GUID;
    }
    [Serializable]
    public struct Plant : IPlant
    {
        [SerializeField] private EntityType type;
        [SerializeField] private float drinkRate;
        [SerializeField] private float reproductionRate;
        [SerializeField] private float age;
        [SerializeField] private float growthRate;
        [SerializeField] private float adultAge;
        [SerializeField] private float health;
        [SerializeField] private float substance;
        [SerializeField] private float size;
        [SerializeField] private float interactionRange;
        [SerializeField] private float childSize;
        [SerializeField] private float adultSize;
        public string GUID { get; set; }
        public Transform Transform { get; set; }
        public EntityType Type { get => type; set => type = value; }
        public float DrinkRate { get => drinkRate; set => drinkRate = value; }
        public float ReproductionRate { get => reproductionRate; set => reproductionRate = value; }
        public float Age { get => age; set => age = value; }
        public float GrowthRate { get => growthRate; set => growthRate = value; }
        public float AdultAge { get => adultAge; set => adultAge = value; }
        public float Health { get => health; set => health = value; }
        public float Thirst { get; set; }
        public float Substance { get => substance; set => substance = value; }
        public float Size { get => size; set => size = value; }
        public float InteractionRange { get => interactionRange; set => interactionRange = value; }
        public float ChildSize { get => childSize; set => childSize = value; }
        public float AdultSize { get => adultSize; set => adultSize = value; }
        public float ReproductionProgress { get; set; }
        public Vector2 Coordinates { get; set; }
        public static implicit operator string(Plant plant) => plant.GUID;
    }
    public interface IAnimal : ICreature
    {
        NPCMovement Movement { get; set; }
        NPCAnimation Animation { get; set; }
        float Speed { get; set; }
        float Strength { get; set; }
        float Hunger { get; set; }
        float Sleepiness { get; set; }
        float Horniness { get; set; }
        float AdultHealth { get; set; }
    }
    public interface IPlant : ICreature
    {
        float DrinkRate { get; set; }
        float ReproductionRate { get; set; }
        float ReproductionProgress { get; set; }
        Vector2 Coordinates { get; set; }
    }
    public interface ICreature : IEntity
    {
        // Health is Substance
        float Age { get; set; }
        float GrowthRate { get; set; }
        float AdultAge { get; set; }
        float Health { get; set; }
        float Thirst { get; set; }
        float ChildSize { get; set; }
        float AdultSize { get; set; }
    }
    public interface IWater : IEntity
    {
        // hide substance, to show Amount instead
        float Amount { get; set; }
        new float Substance { get => Amount; set => Amount = value; }
    }
    public interface IEntity // can be water, a plant, a creature
    {
        EntityType Type { get; set; }
        Transform Transform { get; set; }
        float Substance { get; set; } // amount of water, health for others
        float Size { get; set; }
        float InteractionRange { get; set; } // percentage of global interaction range
        string GUID { get; set; }
    }
    public enum EntityType
    {
        None,
        Herbivore,
        Carnivorous,
        Plant,
        Water
    }
    public enum Needs
    {
        None,
        Hunger,
        Sleepiness,
        Horniness,
        Thirst
    }
    #endregion Declarations
    #region Entity Management
    public void AddEntity(IEntity entity)
    {
        switch(entity.Type)
        {
            case EntityType.Herbivore:
            case EntityType.Carnivorous:
                ((IAnimal)entity).Movement.SetSpeed(((IAnimal)entity).Speed);
                break;
            default:
                break;
        }
        entity.GUID = Guid.NewGuid().ToString();
        Entities.Add(entity);
        if(entity is IAnimal animal)
            FindNextAction(animal);
        if(entity is IPlant plant)
            Tiler.Instance[plant.Coordinates].data["entity"] = plant.GUID;
    }
    public int CountEntities(EntityType type)
    {
        int count = 0;
        foreach(IEntity entity in Entities)
        {
            if (entity.Type == type)
                count++;
        }
        return count;
    }
    public float MeasureEntities(EntityType type)
    {
        float substance = 0;
        foreach (IEntity entity in Entities)
        {
            if (entity is ICreature creature)
                substance += creature.Health;
            else
                substance += entity.Substance;
        }
        return substance;
    }
    #endregion Entity Management
    #region Logic
    public void FindNextAction(IAnimal animal)
    {
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Idle);
        StartCoroutine(WaitThen(() => {
            // get the highest need
            (Needs[] needs, float highestNeed) = GetHighestNeed(animal, 0.1f);
            // if the highest need is below the threshold, roam
            if (highestNeed < needThreshold)
            {
                Roam(animal);
                return;
            }
            (IEntity closestNeed, Needs need) = GetClosestNeed(animal, needs);
            SatisfyNeed(animal, closestNeed, need);
        }, UnityEngine.Random.Range(minimumIdleTime, maximumIdleTime)));
    }
    private IEnumerator WaitThen(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
    public void SatisfyNeed(IAnimal animal, IEntity target, Needs need)
    {
        // if none, roam
        if (need == Needs.None)
        {
            Roam(animal);
            return;
        }
        // if null, start immediately
        if(target == null)
        {
            StartAction(animal, target, need);
            return;
        }
        // if not null, check if in range
        if (InRange(animal, target))
        {
            // if in range, start action
            StartAction(animal, target, need);
        }
        else
        {
            // if not in range, move towards target
            animal.Movement.MoveTo(target.Transform, () => StartAction(animal, target, need));
        }
    }
    public bool InRange(ICreature creature, IEntity target)
    {
        return Vector3.Distance(creature.Transform.localPosition, target.Transform.localPosition) <= creature.InteractionRange * globalInteractionRange;
    }
    public void Roam(IAnimal animal)
    {
        // get a random direction
        Vector2 direction = UnityEngine.Random.insideUnitCircle;
        // move in that direction
        animal.Movement.Move(direction, roamingDistance, () => FindNextAction(animal));
    }
    public (IEntity target, Needs needs) GetClosestNeed(IAnimal animal, Needs[] needs)
    {
        List<(IEntity target, float distance)> targets = new List<(IEntity target, float distance)>();
        for(int i = 0; i < needs.Length; i++)
        {
            (IEntity target, float distance) = GetClosestTarget(animal, needs[i]);
            if(!float.IsPositiveInfinity(distance))
            {
                targets.Add((target, distance));
            }
        }
        if(targets.Count == 0)
        {
            return (null, Needs.None);
        }
        (IEntity closestTarget, float closestDistance) = targets[0];
        Needs closestNeed = needs[0];
        for(int i = 1; i < targets.Count; i++)
        {
            if(targets[i].distance < closestDistance)
            {
                closestTarget = targets[i].target;
                closestDistance = targets[i].distance;
                closestNeed = needs[i];
            }
        }
        return (closestTarget, closestNeed);
    }
    public IEntity[] GetWaterSources(IEntity from)
    {
        List<IEntity> targets = new List<IEntity>();
        for(int i = 0; i < Entities.Count; i++)
        {
            if(Entities[i].Type == EntityType.Water && Vector3.Distance(Entities[i].Transform.position, from.Transform.position) <= Entities[i].InteractionRange * globalInteractionRange)
            {
                targets.Add(Entities[i]);
            }
        }
        return targets.ToArray();
    }
    public (IEntity target, float distance) GetClosestTarget(IAnimal animal, Needs need)
    {
        EntityType targetType = need switch
        {
            Needs.Hunger => animal.Type switch
            {
                EntityType.Herbivore => EntityType.Plant,
                EntityType.Carnivorous => EntityType.Herbivore,
                _ => throw new System.NotImplementedException()
            },
            Needs.Sleepiness => EntityType.None,
            Needs.Horniness => animal.Type,
            Needs.Thirst => EntityType.Water,
            _ => throw new System.NotImplementedException()
        };
        if(targetType == EntityType.None)
        {
            return (null, 0);
        }
        IEntity closestTarget = null;
        float closestDistance = float.PositiveInfinity;
        for(int i = 0; i < Entities.Count; i++)
        {
            if(Entities[i].Type != targetType || Entities[i] == animal)
                continue;
            // for horniness, mate needs to have a needs above the threshold
            if(need == Needs.Horniness && GetNeed((IAnimal)Entities[i], Needs.Horniness) > needThreshold)
                continue;
            float distance = Vector2.Distance(animal.Transform.localPosition, Entities[i].Transform.localPosition);
            if(distance < closestDistance)
            {
                closestTarget = Entities[i];
                closestDistance = distance;
            }
        }
        return (closestTarget, closestDistance);
    }
    public (Needs[] needs, float distances) GetHighestNeed(IAnimal animal, float tolerance)
    {
        (Needs _, float highestNeed) = GetHighestNeed(animal);
        List<(Needs, float)> needs = new();
        if (animal.Hunger >= highestNeed - tolerance)
        {
            needs.Add((Needs.Hunger, animal.Hunger));
        }
        if (animal.Sleepiness >= highestNeed - tolerance)
        {
            needs.Add((Needs.Sleepiness, animal.Sleepiness));
        }
        if (animal.Horniness >= highestNeed - tolerance)
        {
            needs.Add((Needs.Horniness, animal.Horniness));
        }
        if (animal.Thirst >= highestNeed - tolerance)
        {
            needs.Add((Needs.Thirst, animal.Thirst));
        }
        // sort
        needs.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        return (needs.Select(x => x.Item1).ToArray(), highestNeed);
    }
    public (Needs need, float value) GetHighestNeed(IAnimal animal)
    {
        float highestNeed = 0f;
        Needs highestNeedType = Needs.Hunger;
        if (animal.Hunger > highestNeed)
        {
            highestNeed = animal.Hunger;
            highestNeedType = Needs.Hunger;
        }
        if (animal.Sleepiness > highestNeed)
        {
            highestNeed = animal.Sleepiness;
            highestNeedType = Needs.Sleepiness;
        }
        if (animal.Horniness > highestNeed)
        {
            highestNeed = animal.Horniness;
            highestNeedType = Needs.Horniness;
        }
        if (animal.Thirst > highestNeed)
        {
            highestNeed = animal.Thirst;
            highestNeedType = Needs.Thirst;
        }
        return (highestNeedType, highestNeed);
    }
    public float GetNeed(IAnimal animal, Needs need)
    {
        return need switch
        {
            Needs.Hunger => animal.Hunger,
            Needs.Sleepiness => animal.Sleepiness,
            Needs.Horniness => animal.Horniness,
            Needs.Thirst => animal.Thirst,
            _ => throw new System.NotImplementedException()
        };
    }
    #endregion Logic
    #region Actions
    public void StartAction(IAnimal animal, IEntity target, Needs need)
    {
        ActionCancellation cancellation = Actions.RegisterAction((Animal)animal);
        switch (need)
        {
            case Needs.Hunger:
                _ = animal.Type switch
                {
                    EntityType.Herbivore => StartCoroutine(Eat(animal, (ICreature)target, cancellation)),
                    EntityType.Carnivorous => StartCoroutine(Hunt(animal, (IAnimal)target, cancellation)),
                    _ => throw new System.NotImplementedException()
                };
                break;
            case Needs.Sleepiness:
                StartCoroutine(Sleep(animal, cancellation));
                break;
            case Needs.Horniness:
                StartCoroutine(Mate(animal, (IAnimal)target, cancellation));
                break;
            case Needs.Thirst:
                StartCoroutine(Drink(animal, target, cancellation));
                break;
        }
    }
    private IEnumerator Hunt(IAnimal animal, IAnimal target, ActionCancellation cancellation)
    {
        LogAction(animal, target, "hunting");
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Hunting);
        while(target != null && target.Health > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                animal.Movement.Stop();
                yield break;
            }
            if(InRange(animal, target))
            {
                // attack
                animal.Movement.Stop();
                Attack(animal, target);
                yield return new WaitForSeconds(attackDelay);
            }
            else
            {
                // move towards
                animal.Movement.MoveTowards(target.Transform.localPosition);
            }
            yield return null;
        }
        LogEndAction(animal, target, "hunting");
        Eat(animal, target, cancellation);
    }
    private void Birth(IAnimal animal1, IAnimal animal2)
    {
        LogAction(animal1, animal2, "birthing with");
        var child = Instantiate(animal1.Transform.gameObject, animal1.Transform.parent);
        child.transform.localPosition = Vector3.Lerp(animal1.Transform.localPosition, animal2.Transform.localPosition, 0.5f);
        Animal childAnimal = new Animal()
        {
            Type = animal1.Type,
            Transform = child.transform,
            Movement = child.GetComponent<NPCMovement>(),
            Animation = child.GetComponent<NPCAnimation>(),
            Speed = Mathf.Lerp(animal1.Speed, animal2.Speed, 0.5f),
            Strength = Mathf.Lerp(animal1.Strength, animal2.Strength, 0.5f),
            Hunger = 0,
            Sleepiness = 0,
            Horniness = 0,
            Age = 0,
            GrowthRate = Mathf.Lerp(animal1.GrowthRate, animal2.GrowthRate, 0.5f),
            AdultAge = Mathf.Lerp(animal1.AdultAge, animal2.AdultAge, 0.5f),
            Health = 1,
            AdultHealth = Mathf.Lerp(animal1.AdultHealth, animal2.AdultHealth, 0.5f),
            Thirst = 0,
            Substance = 0,
            Size = Mathf.Lerp(animal1.Size, animal2.Size, 0.5f),
            InteractionRange = Mathf.Lerp(animal1.InteractionRange, animal2.InteractionRange, 0.5f),
            ChildSize = Mathf.Lerp(animal1.ChildSize, animal2.ChildSize, 0.5f),
            AdultSize = Mathf.Lerp(animal1.AdultSize, animal2.AdultSize, 0.5f),
        };
        AddEntity(childAnimal);
    }
    private void Attack(IAnimal animal, IAnimal target)
    {
        LogAction(animal, target, "attacking");
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Attacking);
        target.Animation.SetAnimationState(NPCAnimation.AnimationState.Hurt);
        target.Health -= animal.Strength;
        Actions.Cancel((Animal)animal);
        Flee(target, animal);
    }
    private IEnumerator Flee(IAnimal animal, IAnimal target)
    {
        ActionCancellation cancellation = Actions.RegisterAction((Animal)animal);
        LogAction(animal, null, "fleeing");
        animal.Movement.Stop();
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Fleeing);
        while (animal.Health > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                yield break;
            }
            if(Vector3.Distance(animal.Transform.localPosition, target.Transform.localPosition) > animal.InteractionRange * 2)
            {
                LogEndAction(animal, null, "fleeing");
                FindNextAction(animal);
                yield break;
            }
            animal.Movement.MoveTowards(Vector3.LerpUnclamped(animal.Transform.localPosition, target.Transform.localPosition, -1));
            yield return null;
        }
        LogEndAction(animal, null, "fleeing");
        FindNextAction(animal);
    }
    private IEnumerator Sleep(IAnimal animal, ActionCancellation cancellation)
    {
        LogAction(animal, null, "sleeping");
        animal.Movement.Stop();
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Sleeping);
        while (animal.Sleepiness > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                yield break;
            }
            animal.Sleepiness -= Time.deltaTime * sleepingSpeed;
            yield return null;
        }
        LogEndAction(animal, null, "sleeping");
        FindNextAction(animal);
    }
    private IEnumerator Mate(IAnimal animal, IAnimal target, ActionCancellation cancellation)
    {
        LogAction(animal, target, "mating with");
        Actions.Cancel((Animal)target);
        animal.Movement.Stop();
        target.Movement.Stop();
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Mating);
        target.Animation.SetAnimationState(NPCAnimation.AnimationState.Mating);
        while (target != null && target.Horniness > 0 && animal.Horniness > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                yield break;
            }
            animal.Horniness -= Time.deltaTime * matingSpeed;
            target.Horniness -= Time.deltaTime * matingSpeed;
            yield return null;
        }
        LogEndAction(animal, target, "mating with");
        Birth(animal, target);
        FindNextAction(animal);
        FindNextAction(target);
    }
    private IEnumerator Drink(IAnimal animal, IEntity target, ActionCancellation cancellation)
    {
        LogAction(animal, target, "drinking");
        animal.Movement.Stop();
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Drinking);
        while (target != null && target.Substance > 0 && animal.Thirst > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                yield break;
            }
            animal.Thirst -= Time.deltaTime * drinkingSpeed;
            target.Substance -= Time.deltaTime * drinkingSpeed;
            yield return null;
        }
        LogEndAction(animal, target, "drinking");
        FindNextAction(animal);
    }
    private IEnumerator Eat(IAnimal animal, ICreature target, ActionCancellation cancellation)
    {
        LogAction(animal, target, "eating");
        animal.Movement.Stop();
        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Eating);
        while(target != null && target.Substance > 0 && animal.Hunger > 0)
        {
            if(cancellation.IsCancelled(Actions))
            {
                yield break;
            }
            animal.Hunger -= Time.deltaTime * eatingSpeed;
            target.Substance -= Time.deltaTime * eatingSpeed;
            yield return null;
        }
        LogEndAction(animal, target, "eating");
        FindNextAction(animal);
    }
    private void LogAction(IEntity entity1, IEntity entity2, string action)
    {
        Debug.Log($"{entity1} started {action} {entity2}");
    }
    private void LogEndAction(IEntity entity1, IEntity entity2, string action)
    {
        Debug.Log($"{entity1} finished {action} {entity2}");
    }
    #endregion Actions
    #region Life
    void UpdateEntities()
    {
        List<int> toRemove = new List<int>();
        for(int i = 0; i < Entities.Count; i++)
        {
            if (Entities[i] is ICreature creature)
            {
                if(creature.Health <= 0 && creature.Substance <= 0)
                {
                    Destroy(creature.Transform.gameObject);
                    toRemove.Add(i);
                    continue;
                }
                if(creature.Health <= 0)
                {
                    if(creature is Animal animal)
                    {
                        animal.Movement.Stop();
                    }
                    continue;
                }
                creature.Age += Time.deltaTime * growthSpeed * creature.GrowthRate;
                creature.Size = Mathf.Lerp(creature.ChildSize, creature.AdultSize, creature.Age / creature.AdultAge);
                creature.Transform.localScale = Vector3.one * creature.Size;;
                if (creature.Age < creature.AdultAge)
                {
                    creature.Substance += Time.deltaTime * growthSpeed * creature.GrowthRate;
                }
                if (creature.Type == EntityType.Carnivorous)
                {
                    var animal = (IAnimal)creature;
                    if (creature.Age >= creature.AdultAge)
                    {
                        animal.Hunger += Time.deltaTime;
                        animal.Horniness += Time.deltaTime;
                    } else
                    {
                        float tickHealthIncrease = Time.deltaTime * growthSpeed * animal.GrowthRate * animal.AdultHealth / animal.AdultAge;
                        animal.Health += tickHealthIncrease; // growth from age, it's better than lerping as it doesn't prevent taking damage
                    }
                    animal.Thirst += Time.deltaTime;
                    animal.Sleepiness += Time.deltaTime;
                    if(creature.Age >= ageDeathThreshold)
                    {
                        animal.Health = 0;
                        Debug.Log($"{creature} died of old age");
                    }
                    if(animal.Hunger >= hungerDeathThreshold)
                    {
                        animal.Health -= Time.deltaTime;
                        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Dying);
                        if(animal.Health <= 0)
                            Debug.Log($"{creature} died of hunger");
                    }
                    if(animal.Sleepiness >= sleepDeathThreshold)
                    {
                        animal.Health -= Time.deltaTime;
                        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Dying);
                        if(animal.Health <= 0)
                            Debug.Log($"{creature} died from lack of sleep");
                    }
                }
                else if (creature.Type == EntityType.Herbivore)
                {
                    var animal = (IAnimal)creature;
                    if (creature.Age >= creature.AdultAge)
                    {
                        animal.Horniness += Time.deltaTime;
                    }
                    animal.Hunger += Time.deltaTime;
                    animal.Thirst += Time.deltaTime;
                    animal.Sleepiness += Time.deltaTime;
                    if(creature.Age >= ageDeathThreshold)
                    {
                        animal.Health = 0;
                        Debug.Log($"{creature} died of old age");
                    }
                    if(animal.Hunger >= hungerDeathThreshold)
                    {
                        animal.Health -= Time.deltaTime;
                        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Dying);
                        if(animal.Health <= 0)
                            Debug.Log($"{creature} died of hunger");
                    }
                    if(animal.Sleepiness >= sleepDeathThreshold)
                    {
                        animal.Health -= Time.deltaTime;
                        animal.Animation.SetAnimationState(NPCAnimation.AnimationState.Dying);
                        if(animal.Health <= 0)
                            Debug.Log($"{creature} died from lack of sleep");
                    }
                }
                else if (creature.Type == EntityType.Plant)
                {
                    var plant = (IPlant)creature;
                    if (plant.Substance < plant.AdultAge * plant.GrowthRate * growthSpeed)
                    {
                        plant.Substance += Time.deltaTime * growthSpeed * plant.GrowthRate;
                        plant.Thirst += Time.deltaTime * growthSpeed * plant.GrowthRate;
                    }
                    plant.Thirst += Time.deltaTime;
                    // find nearest water source
                    var waterSource = GetWaterSources(plant).OrderByDescending(t => t.Substance).ToArray();
                    if (waterSource.Length > 0 && waterSource[0].Substance > 0)
                    {
                        plant.Thirst -= Time.deltaTime * plant.DrinkRate;
                        waterSource[0].Substance -= Time.deltaTime * plant.DrinkRate;
                    }
                    plant.ReproductionProgress += Time.deltaTime * plant.ReproductionRate;
                    if (plant.ReproductionProgress >= 1)
                    {
                        plant.ReproductionProgress = 0;
                        var childPlant = new Plant()
                        {
                            Substance = 0,
                            Type = EntityType.Plant,
                            ChildSize = plant.ChildSize,
                            AdultSize = plant.AdultSize,
                            Age = 0,
                            Size = plant.ChildSize,
                            AdultAge = plant.AdultAge,
                            GrowthRate = plant.GrowthRate,
                            ReproductionRate = plant.ReproductionRate,
                            DrinkRate = plant.DrinkRate,
                            Thirst = plant.Thirst,
                            ReproductionProgress = 0,
                            InteractionRange = plant.InteractionRange,
                            Health = 1
                        };
                        var point = Tiler.Instance.GetFreeTile(plant.Coordinates, "entity", plant.InteractionRange);
                        if(point == -1)
                            continue;
                        var transform = Tiler.Instance.SpawnTile(point, plant.Transform.gameObject);
                        childPlant.Transform = transform;
                        AddEntity(childPlant);
                    }
                }
                if(creature.Thirst > thirstDeathThreshold)
                {
                    creature.Health -= Time.deltaTime;
                    if(creature.Health <= 0)
                    {
                        Debug.Log($"{creature} died of thirst");
                    }
                }
                if(creature.Health <= 0 && creature is Animal deadAnimal)
                {
                    Actions.Cancel(deadAnimal);
                    deadAnimal.Animation.SetAnimationState(NPCAnimation.AnimationState.Dead);
                }
                Entities[i] = creature;
            }
        }
        for (int i = toRemove.Count - 1; i >= 0 ; i--)
        {
            Entities.RemoveAt(toRemove[i]);
        }
    }
    #endregion Life
}
