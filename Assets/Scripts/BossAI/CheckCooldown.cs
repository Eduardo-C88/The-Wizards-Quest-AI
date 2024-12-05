using BehaviorTree;

public class CheckCooldown : Node{
	private BossSpell1 bossSpell1;
	private BossSpell3 bossSpell3;
	
	public CheckCooldown(BossSpell1 bossSpell1){
		this.bossSpell1 = bossSpell1;
	}
	public CheckCooldown(BossSpell3 bossSpell3){
		this.bossSpell3 = bossSpell3;
	}
	
	public override NodeState Evaluate(){
		// Check cooldown for BossSpell1 if it's not null
        if (bossSpell1 != null)
        {
            if (!bossSpell1.isReady) // If spell1 is not ready
            {
                state = NodeState.Failure; // Fail the node
                return state;
            }
        }

        // Check cooldown for BossSpell3 if it's not null
        if (bossSpell3 != null)
        {
            if (!bossSpell3.isReady) // If spell3 is not ready
            {
                state = NodeState.Failure; // Fail the node
                return state;
            }
        }

        // If neither spell is on cooldown, succeed the node
        state = NodeState.Success;
        return state;
	}
}
