using UnityEngine;
using UnityEngine.SceneManagement; // If you want to load a new scene
using System.Collections;

public class EndGameTrigger : MonoBehaviour
{
	public GameObject boss; // Assign your boss GameObject in the Inspector

	private bool isBossDefeated = false;

	void Update()
	{
		// Check if the boss is dead
		if (boss == null && !isBossDefeated)
		{
			isBossDefeated = true; // Ensure it triggers only once
			Debug.Log("Boss defeated! You can now interact with the book.");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && isBossDefeated)
		{
			Debug.Log("Player touched the book. Ending the game!");
			
			// Inicia a coroutine para esperar antes de mudar de cena
			StartCoroutine(EndGameWithDelay(3f));
		}
		else if (other.CompareTag("Player"))
		{
			Debug.Log("The book is inactive. Defeat the boss first!");
		}
	}

	// Coroutine para aguardar o tempo antes de mudar de cena
	private IEnumerator EndGameWithDelay(float delay)
	{
		Debug.Log("Game Over! Changing scene in " + delay + " seconds...");
		
		yield return new WaitForSeconds(delay); // Aguarda o tempo especificado
		
		SceneManager.LoadScene("MainMenu"); // Carrega a cena principal
	}

}
