
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
	[Header("References")]
	private FpPlayerMovement pm;
	public Transform playerCam;
	public Transform gunTip;
	public LayerMask whatIsGrappleable;
	public LineRenderer lr;
	public Transform predictionPoint; // Prediction indicator

	[Header("Grappling")]
	public float grappleRange;
	public float grappleDelayTime;
	public float overshootYAxis;

	private Vector3 grapplePoint;

	[Header("Prediction")]
	public float predictionSphereCastRadius = 2f; // Radius for SphereCast to predict grapple points
	private RaycastHit predictionHit;

	[Header("Cooldown")]
	public float grapplingCooldown;
	private float grapplingCooldownTimer;

	[Header("Input")]
	public KeyCode grappleKey = KeyCode.Mouse1;

	private bool isGrappling;

	void Start()
	{
		pm = GetComponent<FpPlayerMovement>();
	}

	void Update()
	{
		if(Input.GetKeyDown(grappleKey)) StartGrapple();

		if(grapplingCooldownTimer > 0) grapplingCooldownTimer -= Time.deltaTime;

		CheckForGrapplePoints();
	}

	private void LateUpdate(){
		if(isGrappling) lr.SetPosition(0, gunTip.position);
	}

	private void CheckForGrapplePoints()
	{
		// Check for grapple points using Raycast and SphereCast
		RaycastHit sphereCastHit;
		Physics.SphereCast(playerCam.position, predictionSphereCastRadius, playerCam.forward, out sphereCastHit, grappleRange, whatIsGrappleable);
		
		RaycastHit raycastHit;
		Physics.Raycast(playerCam.position, playerCam.forward, out raycastHit, grappleRange, whatIsGrappleable);

		Vector3 realHitPoint;

		if (raycastHit.point != Vector3.zero)
			realHitPoint = raycastHit.point;
		else if (sphereCastHit.point != Vector3.zero)
			realHitPoint = sphereCastHit.point;
		else
			realHitPoint = Vector3.zero;

		// Set prediction indicator position
		if (realHitPoint != Vector3.zero)
		{
			predictionPoint.gameObject.SetActive(true);
			predictionPoint.position = realHitPoint;
		}
		else
		{
			predictionPoint.gameObject.SetActive(false);
		}

		predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
	}

	private void StartGrapple(){
		if(grapplingCooldownTimer > 0 || pm.activeSwing) return;

		GetComponent<Swing>().StopSwing();

		isGrappling = true;
		pm.freeze = true;

		if(predictionHit.point != Vector3.zero){
			grapplePoint = predictionHit.point;
			Invoke(nameof(ExecuteGrapple), grappleDelayTime);
		} else {
			grapplePoint = playerCam.position + playerCam.forward * grappleRange;
			Invoke(nameof(StopGrapple), grappleDelayTime);
		}

		lr.enabled = true;
		lr.SetPosition(1, grapplePoint);
	}

	private void ExecuteGrapple(){
		pm.freeze = false;

		Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

		float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
		float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

		if(grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

		pm.JumpToPosition(grapplePoint, highestPointOnArc);

		Invoke(nameof(StopGrapple), 1f);
	}

	public void StopGrapple(){
		pm.freeze = false;
		isGrappling = false;
		grapplingCooldownTimer = grapplingCooldown;

		lr.enabled = false;

		// Reset movement speed to default
		pm.moveSpeed = pm.defaultMoveSpeed;
	}
}
