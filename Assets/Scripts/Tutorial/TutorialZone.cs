using UnityEngine;

public class TutorialZone : MonoBehaviour
{
	public TextMesh tutorialText; // Reference to the TextMesh

	private void Start()
	{
		// Make sure the text is initially hidden
		tutorialText.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) // Check if the object is the player
		{
			tutorialText.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			// Hide the text
			tutorialText.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		// Rotate the text to always face the player
		if (tutorialText.gameObject.activeSelf)
		{
			Vector3 direction = tutorialText.transform.position - Camera.main.transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction);
			tutorialText.transform.rotation = rotation;
		}
	}
	
}
