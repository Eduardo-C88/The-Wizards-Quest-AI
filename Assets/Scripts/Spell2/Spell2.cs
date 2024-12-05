using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spell2 : MonoBehaviour
{
	[HideInInspector] public Player player;

	[SerializeField] Transform shootPoint;
	[SerializeField] EnemyDetector playerEnemyDetector;
	[SerializeField] GameObject lightningPrefab;
	[SerializeField] float damage = 10f; // Base damage of the attack
	[SerializeField] float refreshRate = 0.1f; // Time between updates of the LineRenderer
	[SerializeField] int maximumEnemiesInChain = 3;
	[SerializeField] float damageInterval = 0.5f; // Time between each damage tick
	[SerializeField] float attackDuration = 3f; // Total time the attack is active
	[SerializeField] float cooldownDuration = 5f; // Cooldown duration after the attack ends
	[SerializeField] Image cooldownImage; // UI image to show cooldown progress

	public KeyCode shootKey = KeyCode.Alpha2;

	private bool shooting = false; // Is the spell being cast
	private bool shot = false; // Has the attack been initiated
	private bool onCooldown = false; // Is the spell on cooldown
	private float cooldownTimer = 0f; // Tracks remaining cooldown time
	private float attackTimer = 0f; // Timer to track the attack's duration
	private int chainCounter = 0; // How many enemies have been chained
	private List<GameObject> enemiesInChain = new List<GameObject>(); // Track enemies hit by the attack
	private GameObject currentEnemy;

	private List<GameObject> spawnedLineRenderers = new List<GameObject>(); // Track all spawned LineRenderer objects

	public Animator anim;
	public bool animTrack = false;

	void Start()
	{
		if (player == null)
		{
			player = FindObjectOfType<Player>();
			if (player == null)
			{
				Debug.LogError("Player not found! Make sure there is a Player object in the scene.");
			}
			else
			{
				Debug.Log("Player found!");
			}
		}
		
		cooldownImage.fillAmount = 1; // Start filled, indicating the skill is available
	}

	void Update()
	{
		// If on cooldown, update the cooldown image
		if (onCooldown)
		{
			cooldownTimer -= Time.deltaTime;
			cooldownImage.fillAmount = 1 - (cooldownTimer / cooldownDuration);

			// Exit early to prevent any other input
			if (cooldownTimer <= 0)
			{
				onCooldown = false;
				cooldownImage.fillAmount = 1; // Fully recharge
			}
			else
			{
				return;
			}
		}

		// Trigger the skill with a single press if enemies are in range
		if (Input.GetKeyDown(shootKey) && !player.isSpellActive)
		{
			if (playerEnemyDetector.GetEnemiesInRange().Count > 0)
			{
				StartShooting();
				animTrack = true;
			}
			else
			{
				// Feedback when no enemies are in range
				Debug.Log("No enemies in range.");
				// Optional: Add a sound effect or UI message here
			}
		}

		// If the skill is active, update the timer
		if (shooting)
		{
			attackTimer += Time.deltaTime;

			// If the attack duration has elapsed, stop the attack
			if (attackTimer >= attackDuration)
			{
				StopShooting();
			}
		}

		if(animTrack == true)
		{
			anim.SetBool("Spell2", true);
			StartCoroutine(Wait());
		}
		else
		{
			anim.SetBool("Spell2",false);
		}
	}

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(3);
        animTrack = false;
    }
    void StartShooting()
	{
		shooting = true;
		attackTimer = 0f;
		cooldownImage.fillAmount = 0; // Start cooldown animation

		player.isSpellActive = true;

		if (!shot)
		{
			shot = true;
			StartCoroutine(CheckForEnemiesThenActivate());
		}
	}

	void NewLineRenderer(Transform startPos, Transform endPos, bool fromPlayer = false)
	{
		GameObject lineR = Instantiate(lightningPrefab);
		spawnedLineRenderers.Add(lineR);
		StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, fromPlayer));
	}

	IEnumerator UpdateLineRenderer(GameObject lineR, Transform startPos, Transform endPos, bool fromPlayer = false)
	{
		if (shooting && shot && lineR != null)
		{
			if (endPos != null && endPos.gameObject.activeInHierarchy) // Ensure the end position (enemy) is valid
			{
				lineR.GetComponent<LineRendererController>().SetPosition(startPos, endPos);

				yield return new WaitForSeconds(refreshRate);

				// If fromPlayer, update to next closest enemy
				if (fromPlayer)
				{
					GameObject newClosestEnemy = playerEnemyDetector.GetClosestEnemy();
					if (newClosestEnemy != currentEnemy && newClosestEnemy != null && newClosestEnemy.activeInHierarchy)
					{
						Debug.Log("Updating LineRenderer: New closest enemy found");
						StartCoroutine(UpdateLineRenderer(lineR, startPos, newClosestEnemy.transform, true));
						currentEnemy = newClosestEnemy;
					}
					else
					{
						StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos, true));
					}
				}
				else
				{
					StartCoroutine(UpdateLineRenderer(lineR, startPos, endPos));
				}
			}
		}
	}

	IEnumerator ChainReaction(GameObject currentEnemy)
	{
		// Continue the chain reaction to the next enemy
		if (chainCounter >= maximumEnemiesInChain) yield break;

		chainCounter++;
		enemiesInChain.Add(currentEnemy);

		// Apply damage to the current enemy
		int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Example base damage
		var basicEnemy = currentEnemy.GetComponent<BasicEnemy>();
		var miniBoss = currentEnemy.GetComponent<MiniBossController>();

		if (basicEnemy != null)
		{
			Debug.Log($"Applying damage to BasicEnemy: {modDamage}");
			basicEnemy.TakeDamage(modDamage);
		}
		else if (miniBoss != null)
		{
			Debug.Log($"Applying damage to MiniBossController: {modDamage}");
			miniBoss.TakeDamage(modDamage);
		}

		// Continue damage over time
		StartCoroutine(DealDamageOverTime(currentEnemy));

		// Get the EnemyDetector of the current enemy and find the next closest enemy, excluding the current one
		EnemyDetector enemyDetector = currentEnemy.GetComponent<EnemyDetector>();
		GameObject nextEnemy = enemyDetector.GetClosestEnemy(currentEnemy); // Exclude the current enemy

		if (nextEnemy != null)
		{
			if (nextEnemy.activeInHierarchy) // Ensure the next enemy is still alive and active
			{
				float distance = Vector3.Distance(currentEnemy.transform.position, nextEnemy.transform.position);
				Debug.Log($"Distance to next enemy: {distance}");

				// Check if the next enemy is already in the chain
				if (enemiesInChain.Contains(nextEnemy))
				{
					Debug.Log($"Next enemy {nextEnemy.name} is already in the chain.");
				}
				else
				{
					Debug.Log($"Next enemy found: {nextEnemy.name}");
					NewLineRenderer(currentEnemy.transform, nextEnemy.transform);
					StartCoroutine(ChainReaction(nextEnemy)); // Continue chain reaction
				}
			}
			else
			{
				Debug.Log($"Next enemy {nextEnemy.name} is no longer active.");
			}
		}
		else
		{
			Debug.Log("No next enemy found.");
		}
	}

	IEnumerator DealDamageOverTime(GameObject enemy)
	{
		var basicEnemy = enemy.GetComponent<BasicEnemy>();
		var miniBoss = enemy.GetComponent<MiniBossController>();
		var boss = enemy.GetComponent<BossController>();

		while (enemy != null && enemy.activeInHierarchy && shooting)
		{
			int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Example base damage

			if (basicEnemy != null)
			{
				basicEnemy.TakeDamage(modDamage);
			}
			else if (miniBoss != null)
			{
				miniBoss.TakeDamage(modDamage);
			}
			else if (boss != null)
			{
				boss.TakeDamage(modDamage);
			}

			yield return new WaitForSeconds(damageInterval);
		}
	}

	private IEnumerator CheckForEnemiesThenActivate()
	{
		yield return new WaitForSeconds(0.5f); // Short delay to check if enemies are in range

		currentEnemy = playerEnemyDetector.GetClosestEnemy();
		if (currentEnemy != null && currentEnemy.activeInHierarchy) // Ensure enemy is still alive
		{
			Debug.Log("Attack started, found closest enemy.");
			NewLineRenderer(shootPoint, currentEnemy.transform, true);
			if (maximumEnemiesInChain > 1) StartCoroutine(ChainReaction(currentEnemy));
		}
		else
		{
			StopShooting();
		}
	}

	void StopShooting()
	{
		shooting = false;
		shot = false;
		attackTimer = 0f;
		animTrack = false;
		
		chainCounter = 0;

		// Destroy all line renderers and reset enemies
		foreach (var lineR in spawnedLineRenderers)
		{
			if (lineR != null) Destroy(lineR);
		}

		spawnedLineRenderers.Clear();
		enemiesInChain.Clear();
		
		// Set spellActive back to false to allow other spells
		player.isSpellActive = false;

		// Start cooldown
		cooldownTimer = cooldownDuration;
		cooldownImage.fillAmount = 0; // Start empty
		onCooldown = true;
	}
}
