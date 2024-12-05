using BehaviorTree;
using UnityEngine;

public class CheckPlayerInRange : Node
{
	private UnityEngine.Transform bossTransform;
	private UnityEngine.Transform playerTransform;
	private float maxRange;
	private float minRange;

	public CheckPlayerInRange(UnityEngine.Transform bossTransform, UnityEngine.Transform playerTransform, float maxRange, float minRange)
	{
		this.bossTransform = bossTransform;
		this.playerTransform = playerTransform;
		this.maxRange = maxRange;
		this.minRange = minRange;
	}

	public override NodeState Evaluate()
	{
		float distance = Vector3.Distance(bossTransform.position, playerTransform.position);
		Debug.Log("Distance: " + distance);

		// Check if the player is within the min and max range
		if (distance < minRange || distance > maxRange)
		{
			state = NodeState.Failure; // Fail the node if player is too close or too far
			return state;
		}

		state = NodeState.Success; // Success if the player is within the valid range
		return state;
	}
}

