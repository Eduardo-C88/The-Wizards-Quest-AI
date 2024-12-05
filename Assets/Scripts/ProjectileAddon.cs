using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
	public int damage = 50;
	public LayerMask whatIsGround;

	[HideInInspector]public Player player;

	private Rigidbody rb;

	private bool targetHit;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		// Automatically assign the player if it's not set in the Inspector
		if (player == null)
		{
			player = FindObjectOfType<Player>();
			if (player == null)
			{
				Debug.LogError("Player not found! Make sure there is a Player object in the scene.");
			}
		}else
		{
			Debug.Log("Player found!");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (targetHit) {
			return;
		}
		else
			targetHit = true;

		if(collision.gameObject.GetComponent<FpPlayerMovement>() != null){
			return;
		}

		BasicEnemy enemy = collision.gameObject.GetComponent<BasicEnemy>();
		if (enemy != null)
		{
			int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Apply damage boost
			enemy.TakeDamage(modDamage);
			Destroy(gameObject); // Destroy the projectile after impact
			
		}

		// Similarly for MiniBossController:
		MiniBossController boss = collision.gameObject.GetComponent<MiniBossController>();
		if (boss != null)
		{
			int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Apply damage boost
			boss.TakeDamage(modDamage);
			Destroy(gameObject); // Destroy the projectile after impact
		}

		else if (collision.gameObject.GetComponent<ProjectileAddon>() == null || collision.gameObject.layer == whatIsGround){
			Destroy(gameObject);
		}

		// Colisão com BossController
        BossController mainBoss = collision.gameObject.GetComponent<BossController>();
        if (mainBoss != null)
        {
            int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Aplica o multiplicador de dano
            mainBoss.TakeDamage(modDamage);
            Destroy(gameObject); // Destrói o projétil após o impacto
        }
        
        else if (collision.gameObject.GetComponent<ProjectileAddon>() == null || collision.gameObject.layer == whatIsGround)
        {
            Destroy(gameObject);
        }
		// Add your code here
		
		//Bullet sticks to target
		rb.isKinematic = true;

		//transform.SetParent(collision.transform);*/
	}  
}
