using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
	private NavMeshAgent nv;
	[SerializeField] private float detectionRange = 15f;
	[SerializeField] private float loseSightRange = 20f;
	[SerializeField] private Transform[] patrolPoints;
	private Transform player;
	private int currentPatrolIndex = 0;

	[Header("Boss Stats")]
	public int health = 1500;
	private int maxHealth; //para armazenar o valor inicial de saúde.
	public int damage = 50;
	public float knockbackForce = 20f;
	public float rageSpeedMultiplier = 1.5f;
	public float rageDamageMultiplier = 2f;
	private bool isDead = false;
	private bool isInRage = false;

	private PortalManager portalManager;

	// Estados da máquina de estados
	private enum BossState { Patrolling, Chasing, Attacking }
	private BossState currentState;

	void Start()
	{
		nv = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		portalManager = FindObjectOfType<PortalManager>();
    	maxHealth = health; // Armazena o valor inicial da vida.

		currentState = BossState.Patrolling;

		// Configura o primeiro ponto de patrulha
		if (patrolPoints.Length > 0)
		{
			nv.SetDestination(patrolPoints[currentPatrolIndex].position);
		}
	}

	void Update()
	{
		if (isDead) return;

		float playerDistance = Vector3.Distance(player.position, transform.position);

		// Verifica se o boss deve entrar em "rage"
		if (!isInRage && health <= maxHealth * 0.3f) // Verifica se está abaixo de 30% da vida total
		{
			EnterRageMode();
		}

		// Atualiza o comportamento com base no estado atual
		switch (currentState)
		{
			case BossState.Patrolling:
				HandlePatrolling(playerDistance);
				break;
			case BossState.Chasing:
				HandleChasing(playerDistance);
				break;
			case BossState.Attacking:
				HandleAttacking();
				break;
		}
	}

	void ChangeState(BossState newState)
	{
		currentState = newState;
		Debug.Log($"Boss mudou para o estado: {newState}");

		switch (newState)
		{
			case BossState.Patrolling:
				GoToNextPatrolPoint();
				break;
			case BossState.Chasing:
				break;
			case BossState.Attacking:
				break;
		}
	}

	void HandlePatrolling(float playerDistance)
	{
		if (!nv.pathPending && nv.remainingDistance < 0.5f)
		{
			GoToNextPatrolPoint();
		}

		if (playerDistance < detectionRange)
		{
			ChangeState(BossState.Chasing);
		}
	}

	void HandleChasing(float playerDistance)
	{
		if (playerDistance > loseSightRange)
		{
			ChangeState(BossState.Patrolling);
		}
		else if (playerDistance <= nv.stoppingDistance)
		{
			ChangeState(BossState.Attacking);
		}
		else
		{
			nv.SetDestination(player.position);
		}
	}

	void HandleAttacking()
	{
		// Implementar lógica de ataque ao jogador
		Debug.Log("Boss está atacando o jogador!");

		// Transição de volta para Chasing caso o jogador fuja
		float playerDistance = Vector3.Distance(player.position, transform.position);
		if (playerDistance > nv.stoppingDistance)
		{
			ChangeState(BossState.Chasing);
		}
	}

	void EnterRageMode()
	{
		isInRage = true;
		nv.speed *= rageSpeedMultiplier; // Aumenta a velocidade do boss
		damage = Mathf.RoundToInt(damage * rageDamageMultiplier); // Aumenta o dano do boss
		Debug.Log("Boss entrou no modo Rage! Velocidade e dano aumentados.");
	}

	void GoToNextPatrolPoint()
	{
		if (patrolPoints.Length == 0) return;

		nv.SetDestination(patrolPoints[currentPatrolIndex].position);
		currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
	}

	public void TakeDamage(int damageAmount)
	{
		health -= damageAmount;
		Debug.Log("Boss tomou dano. Vida restante: " + health);

		if (health <= 0)
		{
			isDead = true;
			Debug.Log("Boss derrotado!");
			portalManager.SpawnPortal();
			Destroy(gameObject);
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
		float originalSpeed = nv.speed;
		nv.speed *= slowAmount;

		float remainingTime = duration;
		while (remainingTime > 0)
		{
			Debug.Log("Efeito de slow ativo. Tempo restante: " + remainingTime + " segundos.");
			yield return new WaitForSeconds(1f);
			remainingTime -= 1f;
		}

		nv.speed = originalSpeed;
		Debug.Log("Efeito de slow terminado. Velocidade normalizada.");
	}
}

