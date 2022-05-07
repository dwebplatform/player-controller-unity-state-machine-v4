
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
  public override void LogicUpdate()
  {
    base.LogicUpdate();

    if (_hitConsumer.isHittedLeft && PlayerController.inputManagerStrategy.GetHorizontalMovement() > 0f)
    {
      EnableLeftWallIgnorance();
    }
    if (_hitConsumer.isHittedRight && PlayerController.inputManagerStrategy.GetHorizontalMovement() < 0f)
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

  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();

    if(_hitConsumer.isHittedRight && PlayerController.inputManagerStrategy.GetHorizontalMovement()<0f){
      _player._velocity.x = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
    }
    else if(_hitConsumer.isHittedLeft && PlayerController.inputManagerStrategy.GetHorizontalMovement()>0f){
      _player._velocity.x = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
    }
   else if(!(_hitConsumer.isHittedLeft || _hitConsumer.isHittedRight)){
      _player._velocity.x = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
    }
  }
  public override void Exit()
  {
    base.Exit();
  }
}