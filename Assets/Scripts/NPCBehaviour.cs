using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
	[SerializeField] private NPCMovement movementScript;
	[SerializeField] private Transform destination;
	[SerializeField] private float stoppingDistance = 0.1f;
	private bool isMoving = false;
	[SerializeField] private float hp = 100;
	[SerializeField] private float sleep = 50;
	[SerializeField] private float food = 50;
	[SerializeField] private float water = 50;
	[SerializeField] private float mate = 50;
	[SerializeField] private bool carnivorous = false;
	[SerializeField] private string foodTag = "Food";
	[SerializeField] private string waterTag = "Water";
	[SerializeField] private string herbivoreMateTag = "HerbivoreMate";
	[SerializeField] private string carnivoreMateTag = "CarnivoreMate";


	private void Start()
	{
		//destination = GameObject.Find("Tree1(Clone)").transform;
		//hp = Random.Range(0, 100);
		food = Random.Range(0, 100);
		//water = Random.Range(0, 100);
		//sleep = Random.Range(0, 100);
		mate = Random.Range(0, 100);
	}

	private void Update()
	{

		if (!isMoving)
		{
			SetDestination();
			movementScript.MoveTo(destination.position);
			isMoving = true;
			Debug.Log("imove");
		}

		float distance = Vector3.Distance(transform.position, destination.position);
		if (distance <= stoppingDistance)
		{
			isMoving = false;
		}
	}

	private void SetDestination()
	{
		// Use a switch statement to set the destination based on the lowest variable value
		switch (GetLowestValue())
		{
			case "hp":
				if (hp < 1) Destroy(gameObject); //ded
				break;
			case "food":
				destination = GameObject.FindWithTag(foodTag).transform;
				break;
			case "water":
				destination = GameObject.FindWithTag(waterTag).transform;
				break;
			case "sleep":
				//sleep
				break;
			case "mate":
				if (carnivorous)
				{
					destination = GameObject.FindWithTag(carnivoreMateTag).transform;
				}
				else
				{
					destination = GameObject.FindWithTag(herbivoreMateTag).transform;
				}
				break;
		}
	}
	private string GetLowestValue()
	{
		// Find the name of the variable with the lowest value
		string lowestVar = "";
		float lowestValue = Mathf.Min(hp, sleep, food, water, mate);
		if (lowestValue == hp)
		{
			lowestVar = "hp";
		}
		else if (lowestValue == sleep)
		{
			lowestVar = "sleep";
		}
		else if (lowestValue == food)
		{
			lowestVar = "food";
		}
		else if (lowestValue == water)
		{
			lowestVar = "water";
		}
		else if (lowestValue == mate)
		{
			lowestVar = "mate";
		}
		return lowestVar;
	}
}
