public interface IState
{
    void Enter();   // Called when the state is entered
    void Execute(); // Called every frame the state is active
    void Exit();    // Called when the state is exited
}
