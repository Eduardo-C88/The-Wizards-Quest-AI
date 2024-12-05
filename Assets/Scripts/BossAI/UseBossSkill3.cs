using BehaviorTree;

public class UseBossSkill3 : Node
{
	public override NodeState Evaluate()
	{
		BossBT.bossSpell3.TriggerSpell();
		
		state = NodeState.Success;
		return state;
	}
}