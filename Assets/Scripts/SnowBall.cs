using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.TakeDamage(damage);
        }
        Destroy(gameObject);  // Destroi o projetil após a colisão
    }
}