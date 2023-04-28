using UnityEngine;

public class PickUpObject : MonoBehaviour
{
	private bool isHeld = false;
	private Transform originalParent;
	private Camera mainCamera;
	private Vector3 pickupOffset;

	[SerializeField] private float pickupDistance = 2f; // the desired distance from the origin

	void Start()
	{
		originalParent = transform.parent; // store the original parent
		mainCamera = Camera.main; // get the main camera
		pickupOffset = transform.position - Vector3.zero; // calculate the offset between the object's position and the origin of the world
	}

	void OnMouseDown()
	{
		isHeld = true;
		transform.parent = null; // detach from parent when picked up
	}

	void OnMouseUp()
	{
		isHeld = false;
		transform.parent = originalParent; // reattach to original parent when released
	}

	void Update()
	{
		if (isHeld)
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = mainCamera.transform.position.y - transform.position.y; // calculate the depth of the object relative to the camera
			Vector3 objectPosition = mainCamera.ScreenToWorldPoint(mousePosition);

			// calculate the desired position of the object based on the pickupDistance and the pickupOffset
			Vector3 desiredPosition = transform.localPosition + Vector3.forward * pickupDistance;

			// set the object's local position to the desired position
			transform.localPosition = desiredPosition;
		}
	}
}

