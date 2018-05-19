using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float m_speed = 5.0f;
	public float m_jumpHeight = 5.0f;

	public float m_linearSpeed = 2.1f;
	public float m_angularSpeed = 3.0f;
	public float m_jumpSpeed = 2.5f;

	private bool hasSwung = false;
	private Rigidbody m_rb;

	void Start ()
	{
		m_rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate ()
	{
		Move ();
		//UpdatePlayerMovement ();
	}


	void Move()
	{
		float rotationInput = Input.GetAxis("Horizontal");
		float forwardInput = Input.GetAxis("Vertical");

		Vector3 linearVelocity = this.transform.forward * (forwardInput * m_linearSpeed);

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Vector3 jumpVelocity = Vector3.up * m_jumpSpeed;
			m_rb.velocity += jumpVelocity;
		}

		float yVelocity = m_rb.velocity.y;


		linearVelocity.y = yVelocity;
		m_rb.velocity = linearVelocity;

		Vector3 angularVelocity = this.transform.up * (rotationInput * m_angularSpeed);
		m_rb.angularVelocity = angularVelocity;
	}

	void UpdatePlayerMovement()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		Vector3 forwardVector = this.transform.forward;
		Vector3 linearVelocity = forwardVector * (moveVertical * m_speed);

		float yVelocity = m_rb.velocity.y;
		linearVelocity.y = yVelocity;
		m_rb.velocity = linearVelocity;

		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			Vector3 jumpVelocity = Vector3.up * m_jumpHeight;
			m_rb.velocity += jumpVelocity;
		}
	}
		
}
