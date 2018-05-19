﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
	// Class wrapping up settings concerning the character movement
	[System.Serializable]
	public class MoveSettings
	{
		// How fast the player moves
		public float forwardVel = 4;
		// How fast the player turns
		public float rotateVel = 1000;
		// How high the player jumps
		public float jumpVel = 6;

		// The distance to the ground to count the player as fully grounded
		public float distToGround = 0.0f;
		// The minimal distance to the ground to count the player falling (distToGround + eps)
		public float minFallingDistToGround = 0.1f;
		// The offset from the transform position to cast the down ray from
		public Vector3 pivotOffset = new Vector3(0.0f, 0.5f, 0.0f);
		// The mask defines what objects are counted as ground
		public LayerMask ground;
	}

	// Class containing all input fields
	class CharacterInput
	{
		// The input in the forward direction
		public float forward = 0;
		// The input in the sideward direction
		public float sideward = 0;
		// The input for jumping
		public float jump = 0;
	}

	// Instance of move settings
	public MoveSettings moveSettings = new MoveSettings();
	// Gravity acceleration
	public float downAccel = 18.0f;
	// Used for storing velocity before applying to the rigid body
	Vector3 velocity = Vector3.zero;
	// Store if the player is grounded
	bool grounded = false;
	// Stores the raycast hit to the ground
	RaycastHit groundHit = new RaycastHit();
	// Is set if the player is during a jump
	bool onJump = false;
	// Instance of character input
	CharacterInput input = new CharacterInput();
	// Reference to camera
	Camera playerCamera;
	// Rigidbody component
	Rigidbody rigidBody = null;
	// Reference for animator component
	Animator animator = null;
	// Flag for attacking
	bool hasSwung = false;
	// Time per swings
	float lightSwingTime = 2.0f;
	float heavySwingTime = 2.0f;
	// Sword particle system
	public ParticleSystem pSystem;

	void Start()
	{
		playerCamera = Camera.main;
		rigidBody = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
	}
	void Update()
	{
		GetInput();
		Turn();
	}
	void FixedUpdate()
	{
		// Light attack
		if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
		{
			animator.SetTrigger ("LAttack");
			//m_anim.anim
			StartCoroutine (SwordAttack (lightSwingTime));
		}
		// Heavy attack
		if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.G))
		{
			animator.SetTrigger ("HAttack");
			//m_anim.anim
			StartCoroutine (SwordAttack (heavySwingTime));
		}
		// Debug hit
		if(Input.GetKeyDown(KeyCode.H))
		{
			animator.SetTrigger ("isHit");
			//m_anim.anim
		}

		// Check Grounded once per FixedUpdate()
		CheckGrounded();
		// Process player running and jumping
		Run();
		Jump();
		// Player rotation (on XZ-plane)
		Quaternion playerRotation = transform.rotation;
		// Adjust movement to move along the surface normal of the ground (reduces falling when walking on slopes)
		if (grounded)
		{
			playerRotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal) * transform.rotation;
		}
		// Set velocity in player look direction (rotation)
		rigidBody.velocity = playerRotation * new Vector3(0.0f, 0.0f, velocity.z)
			+ new Vector3(0.0f, velocity.y, 0.0f);
	}

	// Gets control input
	void GetInput()
	{
		input.forward = Input.GetAxis("Vertical");
		input.sideward = Input.GetAxis("Horizontal");
		// Keep jump value at 1.0f till it gets recognized by the Jump() function
		if (input.jump == 0.0f)
			input.jump = Input.GetAxis("Jump");
	}

	// Method processing the running according to the forward (and sideward) input
	void Run()
	{
		// Player can only move forward in his look direction -> take length of both input movement parameters to get speed
		// Adjusting the look direction is done in the Turn() function
		float velInput = Mathf.Clamp((new Vector2(input.forward, input.sideward)).magnitude, 0.0f, 1.0f);
		velocity.z = velInput * moveSettings.forwardVel;
		// Update animator
		animator.SetFloat("Speed", velocity.z);
	}
	// Method processing the player turning
	void Turn()
	{
		// Forward direction: camPos -> playerPos
		Vector3 cameraVector = new Vector3(transform.position.x - playerCamera.transform.position.x, 0.0f, 
			transform.position.z - playerCamera.transform.position.z);
		// Calculate the look direction of the player based on the input and the cameraVector
		Vector3 playerLookDirection = Quaternion.LookRotation(cameraVector) * new Vector3(input.sideward, 0.0f, input.forward);
		if (playerLookDirection != Vector3.zero)
		{
			// Calculate the specific player rotation to match the player look direction
			Quaternion destRot = Quaternion.LookRotation(playerLookDirection);
			// Perform smoothing from current player look direction to new look direction
			transform.rotation = Quaternion.RotateTowards(transform.rotation, destRot, moveSettings.rotateVel * Time.deltaTime);
		}
	}
	// Method for processing the jump action
	void Jump()
	{
		// Check if player is standing on ground or is mid air
		if (grounded)
		{
			// Check for jump input and if the player is currently not jumping
			if (input.jump > 0 && !onJump)
			{
				// Update animator
				animator.SetBool ("isJumping", true);
				// Set y-velocity accordingly
				velocity.y = moveSettings.jumpVel;
				onJump = true;
			}
			else if (grounded)
			{
				// Zero out y-velocity if player is standing on the ground and not jumping
				velocity.y = 0;
				// Update animator
				animator.SetBool ("isJumping", false);
			}
		}
		else
		{
			// Falling -> Apply gravity
			velocity.y -= downAccel * Time.fixedDeltaTime;
			// Update animator
			animator.SetBool ("isJumping", true);
		}
		// Reset jump input parameter
		input.jump = 0.0f;
	}

	// Checks if the player is standing on a ground
	void CheckGrounded()
	{
		bool lastGrounded = grounded;
		// Cast a ray downwards in y-axis with a max distance of pivotOffset.y + minFallingDistToGround to see if it hit the ground layer
		bool hit = Physics.Raycast(transform.position + moveSettings.pivotOffset, 
			Vector3.down, out groundHit, moveSettings.pivotOffset.y + moveSettings.minFallingDistToGround, moveSettings.ground);
		// Subtract the pivotOffset.y from the distance passed
		groundHit.distance -= moveSettings.pivotOffset.y;
		// The player is counted as grounded if 
		// his distance to the ground is smaller than the set distance to ground 
		// or
		// his distance to the ground smaller is smaller than the minimum falling distance set and
		// he was standing on the ground the last time checked and he is not during a jump
		if (hit && (groundHit.distance < moveSettings.distToGround || (lastGrounded && !onJump))) { // Player is grounded -> place him exactly on the ground with a distance of distToGround
			transform.position = new Vector3(transform.position.x, groundHit.point.y + moveSettings.distToGround, transform.position.z);
			grounded = true;
			// Set jump as finished
			onJump = false;
		}
		else
		{
			// Not grounded
			grounded = false;
		}
	}

	public IEnumerator SwordAttack(float swingTime)
	{
		SwordScript.isAttacking = true;
		pSystem.Play ();
		yield return new WaitForSeconds (swingTime);
		pSystem.Stop ();
		SwordScript.isAttacking = false;
	}
}
