using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
	public TextMeshProUGUI randomText;
	
	private string[] gameOverPhrases = {
		"Oops! That didn’t go as planned.",
		"Well, that was unexpected… or was it?",
		"Game over? More like try again!",
		"You tried your best... kinda.",
		"Back to the drawing board!",
		"Pro tip: Don’t do that again.",
		"Oopsie daisy! Better luck next time.",
		"Did you forget the controls?",
		"Hey, at least you’re consistent.",
		"That’s one way to end things!"
	};
	
	public void Setup()
	{
		gameObject.SetActive(true);
		randomText.text = GetRandomPhrase();
	}
	
	private string GetRandomPhrase()
	{
		int index = Random.Range(0, gameOverPhrases.Length);
		return gameOverPhrases[index];
	}
	
	public void RestartButton()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		Time.timeScale = 1f;
		// Lock the cursor and make it invisible
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	public void MainMenuButton()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
}
