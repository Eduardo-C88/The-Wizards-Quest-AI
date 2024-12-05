using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	[Header("References")]
	public GameObject projectilePrefab;
	public Transform firePoint;
	public Camera cam;
	
	[Header("Keybinds")]
	public KeyCode fireKey = KeyCode.Mouse0;
	
	[Header("Projectile Stats")]
	public float maxRange = 1000;
	public float projectileSpeed = 10;
	public float fireRate = 1.25f;
	public float projectileLifetime = 3f;

	private Vector3 destination;
	private float TimeToFire;
	private Player player;

	public bool animTrack = false;
	public Animator anim;
	
	void Start()
	{
		player = FindObjectOfType<Player>();
	}

	// Update is called once per frame
	void Update()
	{
		firePoint.rotation = cam.transform.rotation;
		if(Input.GetKey(fireKey) && Time.time >= TimeToFire && !player.isSpellActive)
		{
			TimeToFire = Time.time + 1/fireRate;
			ShootProjectile();
		}

		if (animTrack == true)
		{
			anim.SetBool("BasicAttack", true);
			StartCoroutine(Wait());
		}
		else
		{
            anim.SetBool("BasicAttack", false);
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
		
		// Draw a debug ray in the Scene view to see where the camera is aiming
		Debug.DrawRay(ray.origin, ray.direction * maxRange, Color.red, 2f);
		
		if(Physics.Raycast(ray, out hit))
		{
			destination = hit.point;
		}else
		{
			destination = ray.GetPoint(maxRange);
		}

        animTrack = true;
        InstantiateProjectile();

		
	}
	
	void InstantiateProjectile()
	{
		var projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity) as GameObject;
		projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;

		Destroy(projectileObj, projectileLifetime);
		
	}

	
}
