using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}
	
	public void PlayGame()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 2);
	}
	
	public void Tutorial()
	{
		SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
