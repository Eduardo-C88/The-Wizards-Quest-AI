using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class Spell3 : MonoBehaviour
{
    public int damage = 25;
    public float radius = 5;
    public float expansionDuration = 1.0f; // Duration for the AoE to reach full size
    public float knockbackForce = 10f; // Adjust this value to control knockback strength
    public KeyCode key = KeyCode.Alpha3;

    [HideInInspector] public Player player;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Image cooldownImage; // UI image to show cooldown progress
    public GameObject aoeEffect; // The visual effect for the AoE

    private float currentRadius = 0f; // To track the expanding radius
    private bool isExpanding = false; // To check if the AoE is expanding
    private HashSet<BasicEnemy> damagedEnemies = new HashSet<BasicEnemy>(); // Track damaged enemies
    private HashSet<MiniBossController> damagedBosses = new HashSet<MiniBossController>(); // Track damaged BossEnemies
    private HashSet<BossController> damagedFinalBoss = new HashSet<BossController>(); // Track damaged FinalBoss

    // Cooldown variables
    public float cooldownDuration = 3.0f; // Cooldown time in seconds
    private float cooldownTimer = 0f; // Timer for cooldown
    private bool onCooldown = false; // Is the spell on cooldown

    public bool animTrack = false;
    public Animator anim;

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

        cooldownImage.fillAmount = 1; // Start full, indicating the skill is ready
    }

    private void Update()
    {
        // Handle cooldown UI
        if (onCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = 1 - (cooldownTimer / cooldownDuration);

            if (cooldownTimer <= 0)
            {
                onCooldown = false;
                cooldownImage.fillAmount = 1; // Fully recharge
            }
        }

        // Check if the spell can be cast
        if (Input.GetKeyDown(key) && !isExpanding && !onCooldown && !player.isSpellActive)
        {
            StartCoroutine(ExpandAoE());
        }

        if (animTrack == true)
        {
            anim.SetBool("Spell3", true);
            StartCoroutine(Wait());
        }
        else
        {
            anim.SetBool("Spell3",false);
        }

    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        animTrack = false;
    }

    private IEnumerator ExpandAoE()
    {
        isExpanding = true;
        player.isSpellActive = true; // Lock other spells during the expansion
        cooldownImage.fillAmount = 0; // Start empty while the AoE is active (optional visual indication)

        animTrack = true;

        float startTime = Time.time;
        damagedEnemies.Clear(); // Clear the list of damaged enemies at the start of the attack
        damagedBosses.Clear(); // Clear the list of damaged bosses at the start of the attack
        damagedFinalBoss.Clear(); // Clear the list of damaged final boss at the start of the attack

        // Instantiate the AoE effect and store the reference
        GameObject aoe = Instantiate(aoeEffect, transform.position, Quaternion.identity);
        Destroy(aoe, expansionDuration + 0.5f); // Destroy the AoE effect after the expansion duration

        // Reset currentRadius for this spell cast
        currentRadius = 0f;

        while (currentRadius < radius)
        {
            // Update the position of the AoE effect to follow the player
            aoe.transform.position = player.transform.position;

            // Calculate how far along the expansion is
            float elapsed = Time.time - startTime;
            currentRadius = Mathf.Lerp(0, radius, elapsed / expansionDuration);

            // Check for enemies within the current expanding radius
            Collider[] hitColliders = Physics.OverlapSphere(aoe.transform.position, currentRadius, enemyLayer);
            int modDamage = Mathf.RoundToInt(damage * player.damageMultiplier); // Calculate damage once per expansion step

            foreach (Collider c in hitColliders)
            {
                BasicEnemy enemy = c.GetComponent<BasicEnemy>();
                MiniBossController boss = c.GetComponent<MiniBossController>();
                BossController finalBoss = c.GetComponent<BossController>();

                if (enemy != null && !damagedEnemies.Contains(enemy))
                {
                    // Apply damage to BasicEnemy and add to damaged list
                    enemy.TakeDamage(modDamage);

                    // Calculate direction for knockback
                    Vector3 knockbackDirection = (enemy.transform.position - aoe.transform.position).normalized;
                    enemy.ApplyKnockback(knockbackDirection, knockbackForce);

                    damagedEnemies.Add(enemy);
                }
                else if (boss != null && !damagedBosses.Contains(boss))
                {
                    // Apply damage to MiniBossController and add to damaged list
                    boss.TakeDamage(modDamage);
                    damagedBosses.Add(boss);
                }
                else if (finalBoss != null && !damagedFinalBoss.Contains(finalBoss))
                {
                    // Apply damage to BossController and add to damaged list
                    finalBoss.TakeDamage(modDamage);
                    damagedFinalBoss.Add(finalBoss);
                }
            }

            // Wait until the next frame
            yield return null;
        }

        // Reset the values after the expansion is complete
        currentRadius = 0f; // Reset radius to allow for the next cast
        isExpanding = false; // Set isExpanding back to false to allow the skill to be used again
        player.isSpellActive = false; // Unlock other spells

        // Start the cooldown
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        cooldownTimer = cooldownDuration;

        animTrack = false;

        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownImage.fillAmount = 1 - (cooldownTimer / cooldownDuration);
            yield return null;
        }

        cooldownImage.fillAmount = 1; // Fully recharge
        onCooldown = false;
    }

    private void OnDrawGizmos()
    {
        if (isExpanding) // Only draw the gizmo when expanding
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }
    }
}
