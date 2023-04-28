using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeSpawner : MonoBehaviour
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
			tiler.SpawnTile(randomPoint, treePrefab);
			var t = tiler[randomPoint];
			t["resources"] = "tree";
			t["health"] = 100;
			tiler[randomPoint] = t;
		}
		for (int i = 0; i < numBerryBushesToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			tiler.SpawnTile(randomPoint, berryBushPrefab);
			var t = tiler[randomPoint];
			t["resources"] = "berries";
			t["amount"] = 0;
			t["health"] = 100;
			tiler[randomPoint] = t;
		}
		for (int i = 0; i < numHerbivoresToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			tiler.SpawnTile(randomPoint, herbivores);
		}
		for (int i = 0; i < numCarnivorousToSpawn; i++)
		{
			int randomPoint = points[Random.Range(0, points.Count)];
			points.Remove(randomPoint);
			tiler.SpawnTile(randomPoint, carnivorous);
		}
	}
}