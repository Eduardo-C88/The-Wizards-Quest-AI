using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MiniBossController : MonoBehaviour
{
	private NavMeshAgent nv;
	[SerializeField] private float range = 1000f;
	private Transform player;

	[Header("Mini Boss Stats")]
	public int health = 500;
	public int damage = 50;
	public float knockbackForce = 20f;
	private bool isDead = false;
	public bool skillActive = false;

	private Shield shield; // Referência ao escudo
	private PortalManager portalManager;

	void Start()
	{
		nv = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		shield = GetComponentInChildren<Shield>(); // Encontra o escudo no objeto filho
		portalManager = FindObjectOfType<PortalManager>();
	}

	void Update()
	{
		if (!isDead && Vector3.Distance(player.position, transform.position) < range)
		{
			nv.SetDestination(player.position);
		}
	}

	public void TakeDamage(int damage)
	{
		if (shield != null)
		{
			// Se o escudo está ativo, o dano é aplicado a ele
			shield.TakeDamage(damage);
			if (shield.shieldDurability <= 0)
			{
				shield = null; // Remove a referência ao escudo, pois ele foi destruído
			}
		}
		else
		{
			// Se o escudo foi destruído, o mini boss recebe dano
			health -= damage;
			Debug.Log("Mini Boss took damage. Remaining health: " + health);

			if (health <= 0)
			{
				isDead = true;
				Debug.Log("Mini Boss defeated!");
				portalManager.SpawnPortal();
				Destroy(gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Player>() != null)
		{
			Player player = collision.gameObject.GetComponent<Player>();
			player.TakeDamage(damage);

			Vector3 knockbackDirection = collision.transform.position - transform.position;
			player.ApplyKnockback(knockbackDirection, knockbackForce);
		}
	}
	
	public void ApplySlow(float slowAmount, float duration)
	{
		StartCoroutine(Slow(slowAmount, duration));
	}

	private IEnumerator Slow(float slowAmount, float duration)
	{
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		float originalSpeed = agent.speed;

		agent.speed *= slowAmount;

		float remainingTime = duration;
		while (remainingTime > 0)
		{
			Debug.Log("Slow effect active. Remaining time: " + remainingTime + " seconds.");
			yield return new WaitForSeconds(1f);
			remainingTime -= 1f;
		}

		agent.speed = originalSpeed;
		Debug.Log("Slow effect ended. Speed reset to normal.");
	}
	
	public void PauseMovement(bool stop)
{
    if (nv != null)
    {
        nv.isStopped = stop; // Stops pathfinding and movement
        nv.velocity = Vector3.zero; // Clears current velocity
    }
}
}