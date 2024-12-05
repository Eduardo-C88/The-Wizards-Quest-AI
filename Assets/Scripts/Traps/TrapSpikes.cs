using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpike : MonoBehaviour
{
	public List<GameObject> ListOnTrap = new List<GameObject>();
	public List<Spike> ListOfSpikes = new List<Spike>();
	public int damage = 10;
	
	Coroutine SpikeTriggerCoroutine;
	bool isReloaded = true;
	
	private void Start()
	{
		SpikeTriggerCoroutine = null;
		isReloaded = true;
		ListOnTrap.Clear();
		ListOfSpikes.Clear();
		
		Spike[] arr = this.GetComponentsInChildren<Spike>();
		foreach(Spike s in arr)
		{
			ListOfSpikes.Add(s);
		}
	}
	
	void Update()
	{
		if(ListOnTrap.Count > 0)
		{
			foreach(GameObject obj in ListOnTrap)
			{
				if(SpikeTriggerCoroutine == null && isReloaded)
				{
					SpikeTriggerCoroutine = StartCoroutine(SpikeTrigger());
				}
			}
		}
	}
	
	IEnumerator SpikeTrigger()
	{
		isReloaded = false;
		Debug.Log("Spike Triggered");
		
		yield return new WaitForSeconds(0.3f);
		
		foreach(Spike s in ListOfSpikes)
		{
			s.Shoot();
		}
		
		DealDamage();
		yield return new WaitForSeconds(1.0f);
		
		foreach(Spike s in ListOfSpikes)
		{
			s.Reload();
		}
		
		yield return new WaitForSeconds(1.0f);
		SpikeTriggerCoroutine = null;
		isReloaded = true;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Enemy"))
		{
			if(!ListOnTrap.Contains(other.gameObject))
			{
				ListOnTrap.Add(other.gameObject);
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player") || other.CompareTag("Enemy"))
		{
			if(ListOnTrap.Contains(other.gameObject))
			{
				ListOnTrap.Remove(other.gameObject);
			}
		}
	}
	
	private void DealDamage()
	{
		foreach(GameObject o in ListOnTrap)
		{
			if(o.CompareTag("Player"))
			{
				o.GetComponent<Player>().TakeDamage(damage);
			}
			else if(o.CompareTag("Enemy"))
			{
				if(o.GetComponent<BasicEnemy>() != null)
				{
					o.GetComponent<BasicEnemy>().TakeDamage(damage);
				}
				else if(o.GetComponent<MiniBossController>() != null)
				{
					o.GetComponent<MiniBossController>().TakeDamage(damage);
				}
			}
		}
	}
}
