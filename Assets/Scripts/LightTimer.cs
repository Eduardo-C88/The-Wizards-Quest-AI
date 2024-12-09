using UnityEngine;

public class LightTimer : MonoBehaviour
{
    // Reference to the Light component
    public Light targetLight;
    // The prefab to detect collisions with
    public GameObject targetPrefab;
    // Duration to keep the light on (in seconds)
    public float lightDuration = 10f;

    public int lightIntensity = 5;
    // Internal timer to track the light duration
    private float timer;
    private bool isLightOn;

    void Start()
    {
        timer =30;
        isLightOn = true;
    }

    void Update()
    {
        // Countdown the timer if the light is on
        if (isLightOn)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Debug.Log("Light Reduced");
                // Turn off the light and reset the timer
                targetLight.intensity = lightIntensity;
                
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the target prefab
        if (collision.gameObject == targetPrefab && !isLightOn)
        {
            // Turn the light on and set the timer
            targetLight.intensity = 80;
            Destroy(targetPrefab);
            
        }
    }
}
