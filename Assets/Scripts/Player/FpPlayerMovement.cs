using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class FpPlayerMovement : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed;
	public float swingSpeed;
	public float defaultMoveSpeed; // New variable for default speed
	public float groundDrag;

	[Header("Jumping")]
	public float jumpForce;
	public float jumpCooldown;
	public float airDrag;
	private bool readyToJump = true;
	public bool isJumping = false;

	public bool animTrack = false;
	public Animator anim;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;

	[Header("Ground Check")]
	public Transform groundCheck; // Transform to check for ground
	public float groundDistance = 0.4f; // Distance for ground detection
	public LayerMask whatIsGround; // LayerMask to define ground
	public bool isGrounded;

	public Transform orientation;

	private float horizontalInput;
	private float verticalInput;

	private Vector3 moveDirection;
	private Rigidbody rb;
	private Vector3 velocityToSet; // Declare this as a class-level variable

	public bool freeze;
	public bool activeGrapple;
	public bool activeSwing;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		defaultMoveSpeed = moveSpeed; // Store the default speed
	}

	private void Update()
	{
		transform.rotation = Quaternion.Euler(0f, orientation.eulerAngles.y, 0f);
		
		CheckGroundStatus();
		MyInput();

		// Only set movement drag in FixedUpdate to align with physics updates
		if (freeze) rb.velocity = Vector3.zero;

		if (activeSwing)
		{
			moveSpeed = swingSpeed; // Use swing speed while swinging
		}

		if (animTrack == true)
		{
			anim.SetBool("Jump", true);
			StartCoroutine(Wait());
        }
		else {
            
            anim.SetBool("Jump", false);
		}
	}

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(1);
        animTrack = false;
    }


    private void FixedUpdate()
	{
		MovePlayer();
		SpeedControl();
		ApplyDrag(); // Apply drag based on grounded state in FixedUpdate
	}

	private void MyInput()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");

		if (Input.GetKeyDown(jumpKey) && isGrounded && readyToJump)
		{
			StartCoroutine(JumpCooldown());
			Jump();
		}
	}

	private void MovePlayer()
	{
		if (activeGrapple || activeSwing) return; // Prevent movement during grapple or swing

		moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
		Vector3 force = moveDirection.normalized * moveSpeed * 10f;

		if (isGrounded)
			rb.AddForce(force, ForceMode.Force);
		else
			rb.AddForce(force * airDrag, ForceMode.Force);
	}

	private void SpeedControl()
	{
		if (activeGrapple) return;

		// Only apply speed limits on the ground to allow for mid-air momentum
		if (isGrounded)
		{
			Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
			if (flatVelocity.magnitude > moveSpeed)
			{
				Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
				rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
			}
		}
	}

	private void ApplyDrag()
	{
		if (isGrounded && !activeGrapple)
			rb.drag = groundDrag;
		else if (isJumping)
			rb.drag = airDrag * 5f; // Use no drag while jumping
		else
			rb.drag = airDrag; // Use low air drag to maintain momentum
	}

	private void Jump()
	{
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		isJumping = true;
		animTrack = true;
	}

	private IEnumerator JumpCooldown()
	{
		readyToJump = false;
		yield return new WaitForSeconds(jumpCooldown);
		isJumping = false;
		readyToJump = true;
		if (Input.GetKey(jumpKey) && isGrounded)
		{
			animTrack = true;
		}
		else
		{
			animTrack = false;
		}
	}

	private void CheckGroundStatus()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
	}

	private bool enableMovementOnNextTouch;

	public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
	{
		activeGrapple = true;
		velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
		Invoke(nameof(SetVelocity), 0.1f);
		Invoke(nameof(ResetRestrictions), 3f);
	}

	private void SetVelocity()
	{
		enableMovementOnNextTouch = true;
		rb.velocity = velocityToSet;
	}

	public void ResetRestrictions()
	{
		activeGrapple = false;
		moveSpeed = defaultMoveSpeed; // Reset to default speed after grappling
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (enableMovementOnNextTouch)
		{
			enableMovementOnNextTouch = false;
			ResetRestrictions();
			GetComponent<GrapplingHook>().StopGrapple();
		}
	}

	public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
	{
		float gravity = Physics.gravity.y;
		float displacementY = endPoint.y - startPoint.y;
		Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

		Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
		Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

		return velocityXZ + velocityY;
	}
}
