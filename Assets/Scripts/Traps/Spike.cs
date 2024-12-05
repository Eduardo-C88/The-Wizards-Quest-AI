using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
	public void Shoot()
	{
		if(this.transform.localPosition.y < 0)
		{
			StartCoroutine(_Shoot());
		}
	}

	IEnumerator _Shoot()
	{
		float delay = Random.Range(0.0f, 0.15f);
		
		yield return new WaitForSeconds(delay);
		
		this.transform.localPosition += (Vector3.up * 1.0f);
	}
	
	public void Reload()
	{
		if(this.transform.localPosition.y > 0)
		{
			StartCoroutine(_Reload());
		}
	}
	
	IEnumerator _Reload()
	{
		float delay = Random.Range(0.0f, 0.15f);
		
		yield return new WaitForSeconds(delay);
		
		this.transform.localPosition -= (Vector3.up * 1.0f);
	}
}
