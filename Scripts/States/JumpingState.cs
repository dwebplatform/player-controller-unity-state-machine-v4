using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionStuff;
public class JumpingState : BaseState
{
  private Player _player;
  private HitConsumer _hitConsumer;
  private IgnoreSurroundings _ignoreSurroundings;
  public JumpingState(string name, Player player, HitConsumer hitConsumer, IgnoreSurroundings ignoreSurroundings) : base(name)
  {
    _player = player;
    _hitConsumer = hitConsumer;
    _ignoreSurroundings = ignoreSurroundings;
  }
  public override void Enter()
  {
    base.Enter();
    //*
    _player._velocity.y = 5f;
    //*EnableTie
    if (_hitConsumer.isHittedBottom)
    {
      EnableGroundIgnorance();
    }
  }
  
  private void EnableGroundIgnorance()
  {
    _ignoreSurroundings.IsGroundIgnored = true;
    _ignoreSurroundings.GroundStartTime = Time.time;
  }
  private bool IsInputInvertedToWall(){
    return (_hitConsumer.isHittedRight && PlayerController.inputManagerStrategy.GetHorizontalMovement() < 0f)||(_hitConsumer.isHittedLeft && PlayerController.inputManagerStrategy.GetHorizontalMovement() > 0f); 
  }
  private bool HasHittedWall(){
    return (_hitConsumer.isHittedLeft || _hitConsumer.isHittedRight);
  }

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();

    if(IsInputInvertedToWall() || !HasHittedWall()){
      _player._velocity.x = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
    }
    
    if (!_hitConsumer.isHittedBottom)
    {
      _player._velocity.y -= _player._gravity * Time.fixedDeltaTime;
    }
  }
}
