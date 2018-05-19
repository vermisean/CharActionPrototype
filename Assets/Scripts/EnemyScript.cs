using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour 
{
	//public float speed = 2.5f;
	public GameObject target;
	public bool isStunned = false;
	public bool isAttacking = false;
	public float attackCooldown = 4.0f;
	public float timeStamp = 0.0f;

	private NavMeshAgent navAgent;
	private Animator anim;

	void Awake()
	{
		navAgent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
	}

	void Start () 
	{
		isStunned = false;
		isAttacking = false;
		timeStamp = Time.time;
	}

	void Update () 
	{
/*		if(!isStunned && && Vector3.Distance(this.transform.position, target.transform.position) <= 2.5f)
		{
			StartCoroutine (EnemyAttack (attackCooldown));
		}*/

		if(!isStunned && !isAttacking)
		{
			navAgent.SetDestination (target.transform.position);		
		}
		else
		{
			navAgent.isStopped = true;
		}

		if(Mathf.Abs(navAgent.velocity.z + navAgent.velocity.x) > 0.01f)
		{
			anim.SetFloat ("Speed", Mathf.Abs(navAgent.velocity.z + navAgent.velocity.x));
		}
		else
		{
			anim.SetFloat ("Speed", 0.0f);
		}

	}

	public IEnumerator StunEnemy(float stunTime)
	{
		isStunned = true;
		yield return new WaitForSeconds (stunTime);
		isStunned = false;
		//Debug.Log ("Switched back");
		navAgent.isStopped = false;
	}

	public IEnumerator EnemyAttack(float attackTime)
	{
		//Debug.Log ("Attack Time");
		//isStunned = true;
		isAttacking = true;
		anim.SetTrigger ("LAttack");
		yield return new WaitForSeconds (attackTime);
		//isStunned = false;
		isAttacking = false;
		navAgent.isStopped = false;
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == "Player" && Time.time >= timeStamp)
		{
			//Debug.Log ("Trigger enter");
			//anim.SetTrigger ("LAttack");
			StartCoroutine(EnemyAttack (2.5f));
			timeStamp = Time.time + attackCooldown;
		}
	}
}
