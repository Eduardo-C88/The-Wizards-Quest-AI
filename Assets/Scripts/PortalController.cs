using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) // Verifica se quem colidiu foi o jogador
		{
			Cursor.lockState = CursorLockMode.None; // Mostrar o cursor do mouse
			
			if (SceneManager.GetActiveScene().buildIndex == 1) // Se a cena atual for a cena do menu
			{
				SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1); // Mudar para a próxima cena
			}else
			{
				SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); // Mudar para a cena anterior
			}
			
			Debug.Log("TP");
		}
	}
}
