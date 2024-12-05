using BehaviorTree;

public class UseBossSkill1 : Node
{
	public override NodeState Evaluate()
	{
		BossBT.bossSpell1.TriggerSpell();
		
		state = NodeState.Success;
		return state;
	}
}