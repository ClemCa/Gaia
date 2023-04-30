using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntitySpawner : MonoBehaviour
{
	[SerializeField] private Tiler tiler;
	[SerializeField] private int divisions = 10;
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private int numTreesToSpawn = 1;
	[SerializeField] private GameObject berryBushPrefab;
	[SerializeField] private int numBerryBushesToSpawn = 1;
	[SerializeField] private GameObject herbivores;
	[SerializeField] private int numHerbivoresToSpawn = 1;
	[SerializeField] private GameObject carnivorous;
	[SerializeField] private int numCarnivorousToSpawn = 1;
	[SerializeField] private string seed;
	[SerializeField] private Nature.Animal defaultHerbivore;
	[SerializeField] private Nature.Animal defaultCarnivore;
	[SerializeField] private Nature.Plant defaultTree;
	[SerializeField] private Nature.Plant defaultBerryBush;
	[SerializeField] private Nature.Water defaultWater;
	void Start()
	{
		#if(UNITY_EDITOR)
		seed = (seed == "" || seed == null) ? Random.Range(0, 1000000).ToString() : seed;
		#else
		seed = Random.Range(0, 1000000).ToString();
		#endif
		Randomize(seed);
	}

	public void Reseed()
	{
		seed = Random.Range(0, 1000000).ToString();
		for(int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
		Randomize(seed);
	}

	private void Randomize(string seed)
	{
		Random.InitState(seed.GetHashCode());
		// list of indexes from 0 to divisions * divisions
		List<int> points = Enumerable.Range(0, divisions * divisions).ToList();
		for (int i = 0; i < numTreesToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			var transform = tiler.SpawnTile(randomPoint, treePrefab);
			var plant = defaultTree;
			plant.Transform = transform;
			plant.Coordinates = tiler[randomPoint].coordinates;
			Nature.Instance.AddEntity(plant);
		}
		for (int i = 0; i < numBerryBushesToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			var transform = tiler.SpawnTile(randomPoint, berryBushPrefab);
			var plant = defaultBerryBush;
			plant.Transform = transform;
			plant.Coordinates = tiler[randomPoint].coordinates;
			Nature.Instance.AddEntity(plant);
		}
		for (int i = 0; i < numHerbivoresToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			var transform = tiler.SpawnTile(randomPoint, herbivores);
			var animal = defaultHerbivore;
			animal.Transform = transform;
			animal.Movement = transform.GetComponent<NPCMovement>();
			animal.Animation = transform.GetComponent<NPCAnimation>();
			Nature.Instance.AddEntity(animal);
		}
		for (int i = 0; i < numCarnivorousToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			var transform = tiler.SpawnTile(randomPoint, carnivorous);
			var animal = defaultCarnivore;
			animal.Transform = transform;
			animal.Movement = transform.GetComponent<NPCMovement>();
			animal.Animation = transform.GetComponent<NPCAnimation>();
			Nature.Instance.AddEntity(animal);
		}
	}
}