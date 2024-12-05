using System.Collections.Generic;
using BehaviorTree;

public class BossBT : Tree
{
	public UnityEngine.Transform bossTransform;    // Boss's transform
	public UnityEngine.Transform playerTransform;  // Player's transform
	
	public static BossSpell1 bossSpell1;
	public float Spell1maxRange;
	public float Spell1minRange;
	public static BossSpell3 bossSpell3;
	public float Spell3maxRange;
	public float Spell3minRange;
	
	void Awake()
	{
		bossSpell1 = GetComponent<BossSpell1>();
		bossSpell3 = GetComponent<BossSpell3>();
		
		// Initialize the transforms
		bossTransform = transform; // Set the boss's transform (since this script is likely attached to the boss)
		
		// Find the player in the scene or pass it as a reference
		playerTransform = UnityEngine.GameObject.FindWithTag("Player").transform; // Example for finding the player by tag
		
		// Initialize the ranges for the spells
		Spell1maxRange = bossSpell1.maxRange;
		Spell1minRange = bossSpell1.minRange;
		Spell3maxRange = bossSpell3.radius;
		Spell3minRange = 0; // No minimum range for the AoE spell
	}
	
	protected override Node CreateTree()
	{
		Node root = new Selector(new List<Node>
		{
			// Sequence for Skill 1
			new Sequence(new List<Node>
			{
				new CheckPlayerInRange(bossTransform, playerTransform, Spell1maxRange, Spell1minRange),  // Pass the transforms here
				new CheckCooldown(bossSpell1),
				new CheckLoS(bossTransform, playerTransform,bossSpell1.maxRange),            // Pass the transforms here
				new UseBossSkill1()
			}),

			// Sequence for Skill 3
			new Sequence(new List<Node>
			{
				new CheckPlayerInRange(bossTransform, playerTransform, Spell3maxRange, Spell3minRange),  // Pass the transforms here
				new CheckCooldown(bossSpell3),
				new CheckLoS(bossTransform, playerTransform, bossSpell3.radius),            // Pass the transforms here
				new UseBossSkill3()
			})
		});

		return root;
	}
}

