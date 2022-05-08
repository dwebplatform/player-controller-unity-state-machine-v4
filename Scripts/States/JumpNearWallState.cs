using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionStuff;
public class JumpNearWallState : BaseState
{
  private Player _player;
  private HitConsumer _hitConsumer;
  private IgnoreSurroundings _ignoreSurroundings;
  Action _changeAdjustmentCallBack;
  Action _resetAdjustmentCallBack;
  public JumpNearWallState(string name, Player player, HitConsumer hitConsumer, 
  IgnoreSurroundings ignoreSurroundings,
  Action changeAdjustmentCallBack,
  Action resetAdjustmentCallBack
  ) : base(name)
  {
    _player = player;
    _hitConsumer = hitConsumer;
    _ignoreSurroundings = ignoreSurroundings;
    _changeAdjustmentCallBack = changeAdjustmentCallBack;
    _resetAdjustmentCallBack = resetAdjustmentCallBack;
  }
  public override void Enter()
  {
    base.Enter();
    _player._velocity.y = 5f;
    if (_hitConsumer.isHittedBottom)
    {
      EnableGroundIgnorance();
    }
  }
  public override void Exit(){
      base.Exit();
  }
  private void EnableGroundIgnorance()
  {
    _ignoreSurroundings.IsGroundIgnored = true;
    _ignoreSurroundings.GroundStartTime = Time.time;
  }
  private bool IsInputInvertedToWall()
  {
    return (_hitConsumer.isHittedRight && PlayerController.inputManagerStrategy.GetHorizontalMovement() < 0f) || (_hitConsumer.isHittedLeft && PlayerController.inputManagerStrategy.GetHorizontalMovement() > 0f);
  }
  private bool HasHittedWall()
  {
    return (_hitConsumer.isHittedLeft || _hitConsumer.isHittedRight);
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    if(IsInputInvertedToWall() || !HasHittedWall()){
        float targetVelocity = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
        _player._velocity.x = Mathf.Lerp(_player._velocity.x, targetVelocity, 1f);
    }
    if(_hitConsumer._closestWall == null){
        _changeAdjustmentCallBack();
    }
    if (!_hitConsumer.isHittedBottom)
    {
      _player._velocity.y -= _player._gravity * Time.fixedDeltaTime;
    }
  }
}
