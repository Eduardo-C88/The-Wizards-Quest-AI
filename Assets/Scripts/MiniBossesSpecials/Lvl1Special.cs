using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lvl1Special : MonoBehaviour
{
	[SerializeField] private LayerMask playerLayer;

	[Header("Stats")]
	public int damage = 15;
	public float radius = 3;
	public float knockbackForce = 3f; // Adjust this value to control knockback strength
	public float speedBoostMultiplier = 2f;
	public float attackDuration = 5.0f;
	public float damageInterval = 0.5f;

	[Header("Cooldown")]
	public float cooldownDuration = 10.0f;

	[SerializeField]private bool isReady = false;  // Indicates if the spell is ready to be cast
	[SerializeField]private bool isActive = false; // To track if the sphere is active
	private NavMeshAgent agent;
	private float defaultMoveSpeed;
	private Shield shield;
	
	
	[SerializeField]private bool shieldBroken = false;

	[Header("SpinAnimation")]
    public bool animTrack = false;
    public Animator anim;

    void Start()
	{
		shield = GetComponentInChildren<Shield>();
		agent = GetComponent<NavMeshAgent>();

		if (agent == null)
		{
			Debug.LogError("NavMeshAgent is missing from " + gameObject.name);
		}
		else
		{
			defaultMoveSpeed = agent.speed;
		}
	}

	void Update()
	{
		if(shield == null && !shieldBroken) 
		{
			shieldBroken = true;
			isReady = true;
		}

		if (isReady && !isActive && shieldBroken)
		{
			StartCoroutine(DamageOverTime());
			animTrack = true;
		}

		if (animTrack == true)
		{
			anim.SetBool("Spin", true);
		}
		else
		{
			anim.SetBool("Spin",false);
		}
	}

	IEnumerator DamageOverTime()
	{
		isActive = true;

		// Apply speed boost during the attack
		if (agent != null)
		{
			agent.speed = defaultMoveSpeed * speedBoostMultiplier;
		}

		float elapsed = 0f;
		while (elapsed < attackDuration)
		{
			DealDamage();
			elapsed += damageInterval;
			yield return new WaitForSeconds(damageInterval);
		}

		// Reset speed after attack is finished
		if (agent != null)
		{
			agent.speed = defaultMoveSpeed;
		}

		// End the attack and start the cooldown
		isActive = false;
		// Start cooldown after attack completes
		StartCoroutine(Cooldown());
	}

	private void DealDamage()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, playerLayer);
		foreach (var c in hitColliders)
		{
			Player player = c.GetComponent<Player>();
			if (player != null)
			{
				player.TakeDamage(damage);
				player.ApplyKnockback((player.transform.position - transform.position).normalized, knockbackForce);
			}
		}
	}

	private IEnumerator Cooldown()
	{
		isReady = false;
		animTrack = false;
		yield return new WaitForSeconds(cooldownDuration);
		isReady = true;
	}
	
	private void OnDrawGizmos()
	{
		if (isActive) // Only draw the gizmo when expanding
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, radius);
		}
	}
}
