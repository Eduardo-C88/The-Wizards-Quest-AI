using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spell1 : MonoBehaviour
{
	[Header("References")]
	public GameObject initialProjectilePrefab; // Prefab for the first projectile
	public GameObject aoeProjectilePrefab; // Prefab for the AoE projectile
	public Transform firePoint;
	public Camera cam;
	public Image cooldownImage;

	[Header("Keybinds")]
	public KeyCode fireKey = KeyCode.Alpha1;

	[Header("Projectile Stats")]
	public float maxRange = 1000;
	public float projectileSpeed = 7;

	[Header("Cooldown")]
	public float cooldownDuration = 5.0f;
		private float cooldownTimer = 0; // Tracks the remaining cooldown time
	private bool isReady = true; // Indicates if the spell is ready to be cast

	private Vector3 destination;
	private Player player;

	public Animator anim;
	public bool animTrack = false;
	
	void Start()
	{
		player = FindObjectOfType<Player>();
		cooldownImage.fillAmount = 1; // Começa preenchido, indicando que a skill está disponível
	}

	void Update()
	{
		// Check if the fire key is pressed and the spell is ready
		if (Input.GetKeyDown(fireKey) && isReady && !player.isSpellActive)
		{
			ShootProjectile();
			StartCooldown();
		}
		
		 // Update the cooldown image fill amount during cooldown
		if (cooldownTimer > 0)
		{
			cooldownTimer -= Time.deltaTime;
			cooldownImage.fillAmount = 1 - (cooldownTimer / cooldownDuration);
		}

        if (animTrack == true)
        {
            anim.SetBool("Spell1", true);
			StartCoroutine(Wait());
        }
		else
		{
			 anim.SetBool("Spell1", false);
		}
    }

	IEnumerator Wait()
	{
        yield return new WaitForSecondsRealtime(1);
		animTrack = false;
    }
	void ShootProjectile()
	{
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;
		animTrack = true;

		if (Physics.Raycast(ray, out hit))
		{
			destination = hit.point;
		}
		else
		{
			destination = ray.GetPoint(maxRange);
		}

		InstantiateProjectile();
	}

	void InstantiateProjectile()
	{
		// Instantiate the initial projectile
		var projectileObj = Instantiate(initialProjectilePrefab, firePoint.position, Quaternion.identity);
		projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
	}

	 void StartCooldown()
    {
        isReady = false; // Spell is not ready
        cooldownTimer = cooldownDuration; // Set the cooldown timer
        cooldownImage.fillAmount = 0; // Start the cooldown by emptying the image
        StartCoroutine(Cooldown());
		
    }

	// Coroutine to handle cooldown
	private IEnumerator Cooldown()
	{
		yield return new WaitForSeconds(cooldownDuration); // Wait for the cooldown duration
		isReady = true; // Set the spell to ready again
	}
}
