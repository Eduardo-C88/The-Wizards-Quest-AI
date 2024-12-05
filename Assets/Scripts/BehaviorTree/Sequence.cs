using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class Sequence : Node
	{
		public Sequence(List<Node> children) : base(children) { }
		public Sequence() : base() { }
		
		public override NodeState Evaluate()
		{
			bool anyChildRunning = false;
			foreach(Node node in children)
			{
				switch(node.Evaluate())
				{
					case NodeState.Failure:
						state = NodeState.Failure;
						return state;
					case NodeState.Success:
						continue;
					case NodeState.Running:
						anyChildRunning = true;
						continue;
					default:
						state = NodeState.Success;
						return state;	
				}
			}
			
			state = anyChildRunning ? NodeState.Running : NodeState.Success;
			return state;
		}
	}
}