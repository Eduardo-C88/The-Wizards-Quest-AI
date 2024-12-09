using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // Speed of the projectile
    public float lifetime = 5f; // How long the projectile exists before being destroyed

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the projectile after a set time
    }

    private void Update()
    {
        // Move the projectile forward
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the projectile on collision
        Destroy(gameObject);
    }
}
