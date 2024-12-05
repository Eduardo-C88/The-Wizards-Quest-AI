using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spell4 : MonoBehaviour
{
	public KeyCode spellKey = KeyCode.Alpha4; // Key to activate the spell
	public GameObject beamPrefab; // Assign the beam prefab in the Inspector
	public Camera playerCamera;    // Reference to the player's camera
	public Image cooldownImage; // UI image to show cooldown progress

	private GameObject activeBeam;
	public float beamDuration = 3.0f; // Duration the beam stays active
	public float cooldownDuration = 10.0f; // Time in seconds between each beam shot
	private float cooldownTimer = 0f; // Timer for cooldown
	private bool onCooldown = false; // Is the spell on cooldown
	
	[Header("Slow Effect")]
	[Tooltip("MoveSpeed will be multiplied by this value while the beam is active")]
	public float slowFactor = 1f; // The slow factor to apply
	
	private FpPlayerMovement playerMov;
	private Player player;

	public Animator anim;
	public bool animTrack = false;
	
	void Start()
	{
		playerMov = FindAnyObjectByType<FpPlayerMovement>();
		if (playerMov == null)
		{
			Debug.LogError("Player script not found!");
		}
		
		player = FindAnyObjectByType<Player>();
		cooldownImage.fillAmount = 1; // Start filled, indicating the skill is available
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
		
		// Check if the player presses the Alpha3 key and if cooldown has expired
		if (Input.GetKeyDown(spellKey) && !onCooldown && !player.isSpellActive)
		{
			ShootBeam();
			animTrack = true;
		}

		// Update the beam's position and rotation if it is active
		if (activeBeam != null)
		{
			UpdateBeamPosition();
		}

		if (animTrack == true)
		{
			anim.SetBool("Spell4", true);
			StartCoroutine(Wait());
		}
		else {
			anim.SetBool("Spell4", false);
		}

    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(3);
        animTrack = false;
    }

    private void ShootBeam()
	{
		// Prevent multiple beams from being shot
		if (activeBeam == null)
		{
			cooldownImage.fillAmount = 0; // Start cooldown animation
			
			// Set isSpellActive to true to prevent other spells
			player.isSpellActive = true;
			
			// Get half the beam's length from the beam prefab's scale
			float halfBeamLength = beamPrefab.transform.localScale.z / 2;

			// Calculate the spawn position in front of the camera
			Vector3 spawnPosition = playerCamera.transform.position + playerCamera.transform.forward * (1.5f + halfBeamLength);

			// Instantiate the beam
			activeBeam = Instantiate(beamPrefab, spawnPosition, Quaternion.LookRotation(playerCamera.transform.forward));
			
			// Slow down player movement while the beam is active
			playerMov.moveSpeed = playerMov.defaultMoveSpeed * slowFactor;

			// Destroy the beam after its duration
			Destroy(activeBeam, beamDuration);
			
			// Start the cooldown timer
			StartCoroutine(EndBeamAndCooldown());
		}
		else
		{
			Debug.Log("Beam is already active. Ignoring input.");
		}
	}

	private void UpdateBeamPosition()
	{
		// Get half the beam's length from the beam prefab's scale
		float halfBeamLength = activeBeam.transform.localScale.z / 2;

		// Calculate the new spawn position for the active beam
		Vector3 spawnPosition = playerCamera.transform.position + playerCamera.transform.forward * (1.5f + halfBeamLength);

		// Update the beam's position and rotation
		activeBeam.transform.position = spawnPosition;
		activeBeam.transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);
	}
	
	private IEnumerator EndBeamAndCooldown()
	{
		animTrack = false;

		yield return new WaitForSeconds(beamDuration);

		// Restore the player's movement speed to the default speed
		playerMov.moveSpeed = playerMov.defaultMoveSpeed;

		// Set isSpellActive to false to allow casting other spells
		player.isSpellActive = false;

		// Start cooldown
		onCooldown = true;
		cooldownTimer = cooldownDuration;
		cooldownImage.fillAmount = 0; // Start cooldown animation
	}	
}
