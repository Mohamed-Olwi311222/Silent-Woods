using System;
using System.Collections.Generic;

public class PlayerStateMachine
{
    private IPlayerState _currentState;
    private Dictionary<IPlayerState, List<Transition>> transitions = new Dictionary<IPlayerState, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public void FrameUpdate()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);
        
        _currentState?.FrameUpdate();
    }
    public void FixedFrameUpdate()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);
        
        _currentState?.PhysicsUpdate();
    }

    public void SetState(IPlayerState state)
    {
        if (state == _currentState)
            return;
        _currentState?.OnExitState();
        _currentState = state;

      transitions.TryGetValue(_currentState, out _currentTransitions);
      if (_currentTransitions == null)
         _currentTransitions = EmptyTransitions;
      
      _currentState.OnEnterState();
    }

    public void AddTransition(IPlayerState from, IPlayerState to, Func<bool> condition)
    {
        if (transitions.TryGetValue(from, out var newTransitions) == false)
        {
            newTransitions = new List<Transition>();
            transitions[from] = newTransitions;
        }
        newTransitions.Add(new Transition(to, condition));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public IPlayerState To { get; }

        public Transition(IPlayerState To, Func<bool> Condition)
        {
            this.To = To;
            this.Condition = Condition;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}