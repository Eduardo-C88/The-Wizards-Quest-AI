//using UnityEngine;

//public class TrainingEnvironment : MonoBehaviour
//{
//    public WaveManagerML waveManager;  // Reference to the WaveManager
//    public GameObject player;          // Reference to the player

//    private float trainingTimeLimit = 60f; // Time limit in seconds for each episode
//    private float timer;

//    private void Start()
//    {
//        ResetEnvironment();
//    }

//    private void Update()
//    {
//        timer += Time.deltaTime;

//        if (timer >= trainingTimeLimit)
//        {
//            Debug.Log("Training time limit reached. Resetting environment.");
//            ResetEnvironment();
//        }

//        if (PlayerDiedCondition())
//        {
//            Debug.Log("Player died. Resetting environment.");
//            ResetEnvironment();
//        }
//    }

//    public void ResetEnvironment()
//    {
//        timer = 0f;

//        PlayerStats playerStats = player.GetComponent<PlayerStats>();
//        if (playerStats != null)
//        {
//            Debug.Log("Resetting player.");
//            playerStats.ResetPlayer();
//        }
//        else
//        {
//            Debug.LogError("PlayerStats component not found on the player object.");
//        }

//        if (waveManager != null)
//        {
//            Debug.Log("Resetting WaveManager and enemies.");
//            waveManager.ResetWaves();
//        }
//        else
//        {
//            Debug.LogError("WaveManager reference is not assigned.");
//        }
//    }

//    public bool PlayerDiedCondition()
//    {
//        PlayerStats playerStats = player.GetComponent<PlayerStats>();
//        return playerStats != null && playerStats.health <= 0;
//    }
//}