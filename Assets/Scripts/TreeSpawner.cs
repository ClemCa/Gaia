using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private float sphereRadius = 10f;
	[SerializeField] private int numTreesToSpawn = 1;

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
			newTree.transform.rotation = Quaternion.LookRotation(perpendicular, normal);
		}
	}
}