using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
	[SerializeField] private float sphereRadius = 10f;
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private int numTreesToSpawn = 1;
	[SerializeField] private GameObject berryBushPrefab;
	[SerializeField] private int numBerryBushesToSpawn = 1;
	[SerializeField] private GameObject herbivores;
	[SerializeField] private int numHerbivoresToSpawn = 1;
	[SerializeField] private GameObject carnivorous;
	[SerializeField] private int numCarnivorousToSpawn = 1;

	void Start()
	{
		for (int i = 0; i < numTreesToSpawn; i++)
		{
			// Get a random point on the surface of the sphere
			Vector3 randomPoint = Random.onUnitSphere * sphereRadius;

			// Instantiate the tree at the random point as a child of the sphere game object
			GameObject newTree = Instantiate(treePrefab, randomPoint, Quaternion.identity, transform);

			// Make the Y axis of the tree face away from the center of the sphere
			Vector3 normal = newTree.transform.position - transform.position;
			Vector3 perpendicular = Vector3.Cross(normal, Vector3.up);
			newTree.transform.rotation = Quaternion.LookRotation(perpendicular, normal); //CHAT GPT TU PUE J'AI REUSSI TOUT SEUL
		}
		for (int i = 0; i < numBerryBushesToSpawn; i++)
		{
			Vector3 randomPoint = Random.onUnitSphere * sphereRadius;
			GameObject newBush = Instantiate(berryBushPrefab, randomPoint, Quaternion.identity, transform);
			Vector3 normal = newBush.transform.position - transform.position;
			Vector3 perpendicular = Vector3.Cross(normal, Vector3.up);
			newBush.transform.rotation = Quaternion.LookRotation(perpendicular, normal);
		}
		for (int i = 0; i < numHerbivoresToSpawn; i++)
		{
			Vector3 randomPoint = Random.onUnitSphere * sphereRadius;
			GameObject newHerbivores = Instantiate(herbivores, randomPoint, Quaternion.identity, transform);
			Vector3 normal = newHerbivores.transform.position - transform.position;
			Vector3 perpendicular = Vector3.Cross(normal, Vector3.up);
			newHerbivores.transform.rotation = Quaternion.LookRotation(perpendicular, normal);
		}
		for (int i = 0; i < numCarnivorousToSpawn; i++)
		{
			Vector3 randomPoint = Random.onUnitSphere * sphereRadius;
			GameObject newCarnivorous = Instantiate(carnivorous, randomPoint, Quaternion.identity, transform);
			Vector3 normal = newCarnivorous.transform.position - transform.position;
			Vector3 perpendicular = Vector3.Cross(normal, Vector3.up);
			newCarnivorous.transform.rotation = Quaternion.LookRotation(perpendicular, normal);
		}
	}
}