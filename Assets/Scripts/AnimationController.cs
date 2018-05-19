using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour 
{
	private Animator m_anim;
	private Rigidbody m_rb;

	void Awake () 
	{
		m_anim = GetComponent<Animator> ();
		m_rb = GetComponent<Rigidbody> ();
	}
	

	void FixedUpdate () 
	{
		m_anim.SetFloat ("Speed", Mathf.Abs(m_rb.velocity.z + m_rb.velocity.x));

		if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
		{
			m_anim.SetTrigger ("Attack");
			//m_anim.anim
		}
	}
}
