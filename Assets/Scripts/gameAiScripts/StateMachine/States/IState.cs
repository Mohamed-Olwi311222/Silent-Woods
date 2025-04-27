public interface IState
{
   public void OnEnterState();
   public void OnExitState();
   public void FrameUpdate();
   public void PhysicsUpdate();

}
