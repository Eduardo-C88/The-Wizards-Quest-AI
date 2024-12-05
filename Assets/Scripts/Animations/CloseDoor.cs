using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoor : MonoBehaviour
{
	public Animator anim;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			anim.SetTrigger("CloseDoor");
		}
	}
}
