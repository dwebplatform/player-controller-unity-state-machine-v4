using UnityEngine;
using System;
using System.Collections.Generic;
using CollisionStuff;
using PositionAdjustManagerStrategy;
 public class IdleState : BaseState{
  private Player _player;
  private HitConsumer _hitConsumer;
  Dictionary<Type, IPositionAdjustStrategy> _adjustments;
  public IdleState(string name,Player player,HitConsumer hitConsumer,Dictionary<Type, IPositionAdjustStrategy> adjustments ) : base(name)
  {
    _player = player;
    _hitConsumer = hitConsumer;
    _adjustments = adjustments;
  }
  public override void Enter()
  {
    base.Enter();
    _player._velocity = Vector2.zero;
  }
  public override void HandleInput()
  {
    base.HandleInput();
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    if(!_hitConsumer.isHittedBottom){
      _player._velocity.y-= _player._gravity* Time.fixedDeltaTime;
    } else {
      _player._velocity.y = 0f;
    }
  }
  public override void Exit()
  {
    base.Exit();
  }
}
