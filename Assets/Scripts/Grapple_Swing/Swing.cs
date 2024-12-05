using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
	[Header("References")]
	public LineRenderer lr;
	public Transform gunTip, playerCam, player;
	public LayerMask whatIsGrappleable;
	public FpPlayerMovement pm;

	[Header("Swinging")]
	public float maxSwingDistance = 25f;
	private Vector3 swingPoint;
	private SpringJoint joint;

	[Header("Air Movement")]
	public Transform orientation;
	public Rigidbody rb;
	public float horizontalThrustForce;
	public float forwardThrustForce;
	public float extendCableSpeed;

	[Header("Prediction")]
	public RaycastHit predictionHit;
	public float predictionSphereCastRadius;
	public Transform predictionPoint;

	[Header("Input")]
	public KeyCode swingKey = KeyCode.Mouse0;

	[Header("Animation")]
	public Animator anim;
	public bool animTrack;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(swingKey))
        {
            StartSwing();
        }

        if (Input.GetKeyUp(swingKey))
        {
            StopSwing();
        }

        CheckForSwingPoints();

        if (joint != null)
        {
            AirMovement();
        }
    }


    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(3);
        animTrack = false;
    }

    private void LateUpdate(){
		DrawRope();
	}

	private void CheckForSwingPoints()
	{
		if (joint != null) return;

		RaycastHit sphereCastHit;
		Physics.SphereCast(playerCam.position, predictionSphereCastRadius, playerCam.forward, 
							out sphereCastHit, maxSwingDistance, whatIsGrappleable);

		RaycastHit raycastHit;
		Physics.Raycast(playerCam.position, playerCam.forward, 
							out raycastHit, maxSwingDistance, whatIsGrappleable);

		Vector3 realHitPoint;

		// Option 1 - Direct Hit
		if (raycastHit.point != Vector3.zero)
			realHitPoint = raycastHit.point;

		// Option 2 - Indirect (predicted) Hit
		else if (sphereCastHit.point != Vector3.zero)
			realHitPoint = sphereCastHit.point;

		// Option 3 - Miss
		else
			realHitPoint = Vector3.zero;

		// realHitPoint found
		if (realHitPoint != Vector3.zero)
		{
			predictionPoint.gameObject.SetActive(true);
			predictionPoint.position = realHitPoint;
		}
		// realHitPoint not found
		else
		{
			predictionPoint.gameObject.SetActive(false);
		}

		predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
	}

    private void StartSwing()
    {
        // Ensure grapple is stopped before starting a swing
        GetComponent<GrapplingHook>().StopGrapple();
        pm.ResetRestrictions();

        // Return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        // Avoid re-triggering swing animation if it's already playing
        if (pm.activeSwing) return;

        // Deactivate active grapple
        if (GetComponent<GrapplingHook>() != null)
            GetComponent<GrapplingHook>().StopGrapple();

        pm.ResetRestrictions();
        pm.activeSwing = true;

        // Trigger animation once
        if (!animTrack)
        {
            animTrack = true;
            anim.SetBool("Swinging", true);
        }

        // Set up the swing point and joint
        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // The distance grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // Customize joint values as needed
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }

    public void StopSwing()
    {
        // End swing and reset related states
        pm.activeSwing = false;
        animTrack = false;

        lr.positionCount = 0;
        Destroy(joint);

        // Reset animation to default
        anim.SetBool("Swinging", false);

        // Reset movement speed to default
        pm.moveSpeed = pm.defaultMoveSpeed;
    }


    private void AirMovement()
	{
		// right
		if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
		// left
		if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);

		// forward
		if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);
		
		// backward
		if (Input.GetKey(KeyCode.S)) rb.AddForce(-orientation.forward * horizontalThrustForce * Time.deltaTime);

		// shorten cable
		if (Input.GetKey(KeyCode.Space))
		{
			Vector3 directionToPoint = swingPoint - transform.position;
			rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

			float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

			joint.maxDistance = distanceFromPoint * 0.8f;
			joint.minDistance = distanceFromPoint * 0.25f;
		}
		// extend cable
		if (Input.GetKey(KeyCode.LeftControl))
		{
			float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

			joint.maxDistance = extendedDistanceFromPoint * 0.8f;
			joint.minDistance = extendedDistanceFromPoint * 0.25f;
		}
	}

	private Vector3 currentGrapplePosition;

	private void DrawRope(){
		if(!joint) return;

		currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

		lr.SetPosition(0, gunTip.position);
		lr.SetPosition(1, swingPoint);
	}
}
