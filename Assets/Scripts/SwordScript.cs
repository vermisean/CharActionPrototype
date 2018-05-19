using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour 
{
	//public ParticleSystem pSystem;
	public static bool isAttacking = false;
	public bool hasSwung = false;
	public float stunTime = 2.5f;

	void Start () 
	{
		//pSystem = GetComponentInChildren<ParticleSystem> ();
		hasSwung = false;
	}

	void Update () 
	{
		
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Enemy" && isAttacking)
		{
			//pSystem.Play ();
			EnemyScript enemyScript = col.gameObject.GetComponent<EnemyScript> ();
			StartCoroutine (enemyScript.StunEnemy (stunTime));
			Animator enemyAnim = col.gameObject.GetComponent<Animator> ();
			enemyAnim.SetTrigger ("isHit");
		}

	}


}
