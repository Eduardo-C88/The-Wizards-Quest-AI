using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static bool GameIsPaused = false;
	public GameObject pauseMenuUI;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
			{
				Resume();
			}
			else
			{
				Pause();
			}
		}
	}	
	
	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		GameIsPaused = false;
		// Lock the cursor and make it invisible
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	public void Pause()
	{
		pauseMenuUI.SetActive(true);
		// Unlock the cursor and make it visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
		Time.timeScale = 0f;
		GameIsPaused = true;

	}
	
	public void LoadMenu()
	{
		Time.timeScale = 1f;
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
}
