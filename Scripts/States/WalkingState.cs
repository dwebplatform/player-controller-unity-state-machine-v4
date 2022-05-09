
using UnityEngine;
using System;
using System.Collections.Generic;
using CollisionStuff;
using PositionAdjustManagerStrategy;
public class WalkingState : BaseState
{
  private Player _player;
  private HitConsumer _hitConsumer;
  private IgnoreSurroundings _ignoreSurroundings;
  public WalkingState(string name, Player player, HitConsumer hitConsumer, Dictionary<Type, IPositionAdjustStrategy> adjustments, IgnoreSurroundings ignoreSurroundings) : base(name)
  {
    _player = player;
    _hitConsumer = hitConsumer;
    _ignoreSurroundings = ignoreSurroundings;
  }
  public override void Enter()
  {
    base.Enter();
  }

  public override void HandleInput()
  {
    base.HandleInput();
  }

  private bool IsMoveInversedLeftWall(){
    return _hitConsumer.isHittedLeft && PlayerController.inputManagerStrategy.GetHorizontalMovement() > 0f;
  }
  private bool IsMoveInversedRightWall(){
    return _hitConsumer.isHittedRight && PlayerController.inputManagerStrategy.GetHorizontalMovement() < 0f;
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
    if (IsMoveInversedLeftWall())
    {
      EnableLeftWallIgnorance();
    }
    if (IsMoveInversedRightWall())
    {
      EnagleRightWallIgnorance();
    }
  }

  private void EnagleRightWallIgnorance()
  {
    _ignoreSurroundings.IsWallRightIgnored = true;
    _ignoreSurroundings.WallStartTime = Time.time;
  }

  private void EnableLeftWallIgnorance()
  {
    _ignoreSurroundings.IsWallLeftIgnored = true;
    _ignoreSurroundings.WallStartTime = Time.time;
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
      if(_player._velocity.y>0f){
        _player._velocity.y -= _player._gravity * Time.fixedDeltaTime;
      } else {
        _player._velocity.y -= _player.DOWN_GRAVITY * Time.fixedDeltaTime;
      }
    }
   
  }
  public override void Exit()
  {
    base.Exit();
  }
}