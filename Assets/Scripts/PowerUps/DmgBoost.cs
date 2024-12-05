using UnityEngine;

public class DmgBoost : MonoBehaviour
{
    public float damageMultiplier = 2f; // The multiplier value
    public float duration = 10f; // Buff duration in seconds

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            Debug.Log("Damage Boost Collected!");
            player.ApplyDamageBoost(damageMultiplier, duration); // Apply damage boost

            Destroy(gameObject); // Destroy the power-up after collection
        }
    }
}
