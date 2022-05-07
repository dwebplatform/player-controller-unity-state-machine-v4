// using UnityEngine;
using System;
using System.Collections.Generic;


namespace PlayerStateMachine
{
  public class PlayerStateMachine
  {
    private class Transition
    {
      public BaseState From;
      public BaseState To;
      public Func<bool> predicate;
      public Transition(BaseState From, BaseState To, Func<bool> predicate)
      {
        this.From = From;
        this.To = To;
        this.predicate = predicate;
      }
    }

    private BaseState _currentState;


    //* static states
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> currentTransitions = new List<Transition>();
    private List<Transition> emptyTransitions = new List<Transition>(0);
    public void AddTransition(BaseState from, BaseState to, Func<bool> predicate)
    {
      if (_transitions.ContainsKey(from.GetType()))
      {
        _transitions[from.GetType()].Add(new Transition(from, to, predicate));
      }
      else
      {
        _transitions[from.GetType()] = new List<Transition>();
        _transitions[from.GetType()].Add(new Transition(from, to, predicate));
      }
    }
    #region 
    public static IdleState idleState;
    public static WalkingState walkingState;

    #endregion
    public void Initialize()
    {
      _currentState = StateMachine.idleState;
      _currentState.Enter();
    }
    public void ChangeState(BaseState newState)
    {
      if (newState == _currentState)
      {
        return;
      }
      _currentState.Exit();
      _currentState = newState;
      if (_transitions.ContainsKey(_currentState.GetType()))
      {
        currentTransitions = _transitions[_currentState.GetType()];
      }
      else
      {
        currentTransitions = emptyTransitions;
      }
      newState.Enter();
    }
    public void HandleInput()
    {
      _currentState.HandleInput();
    }

    private Transition GetTransition()
    {
      foreach (var transition in currentTransitions)
      {
        if (transition.predicate())
        {
          return transition;
        }
      }
      return null;

    }
    public void LogicUpdate()
    {
      var transition = GetTransition();
      if (transition != null)
      {
        ChangeState(transition.To);
      }
      _currentState?.LogicUpdate();
    }
    public void PhysicsUpdate()
    {
      _currentState.PhysicsUpdate();
    }
  }
}