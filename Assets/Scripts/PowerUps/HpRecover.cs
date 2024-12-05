using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HpRecover : MonoBehaviour
{
	public int hpAmount = 40;
	
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.GetComponent<Player>() != null)
		{
			Player player = other.gameObject.GetComponent<Player>();
			
			if (player.health == player.maxHealth)
			{
				Debug.Log("Player is already at full health!");
				return;
			}else{
				Debug.Log("HP Recover Collected!");
				player.RecoverHealth(hpAmount);
				Destroy(gameObject);
			}
		}
	}
}
