using Unity.VisualScripting;
using UnityEngine;

public class StateMachine
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
        Debug.Log("State changed to: " + currentState.GetType().Name);
    }

    public void Update()
    {
        currentState?.Execute();
    }
}
