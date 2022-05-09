using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionStuff;
public class JumpAwayFromWallGrabState : BaseState
{
  private Player _player;
  private HitConsumer _hitConsumer;
  private bool _isStartWithInialZeroInput;
  private IgnoreSurroundings _ignoreSurroundings;
  public JumpAwayFromWallGrabState(string name, Player player, HitConsumer hitConsumer, IgnoreSurroundings ignoreSurroundings) : base(name)
  {
    _player = player;
    _hitConsumer = hitConsumer;
    _ignoreSurroundings = ignoreSurroundings;
  }

  private float _targetVelocity;
  private Vector2 wallHitNormal;
  public override void Enter()
  {
    base.Enter();
    _player._velocity.y = 5f;
    _isStartWithInialZeroInput = Mathf.Abs(PlayerController.inputManagerStrategy.GetHorizontalMovement()) < Mathf.Epsilon;
    wallHitNormal = _hitConsumer._closestWall.Value.normal;
    EnableWallCheckIgnorance();
  }
  private void EnableWallCheckIgnorance()
  {
    if (_hitConsumer._closestWall == null)
    {
      return;
    }

    _ignoreSurroundings.WallStartTime = Time.time;
    Vector2 normal = _hitConsumer._closestWall.Value.normal;

    if (normal.x > 0f)
    {
      _ignoreSurroundings.IsWallLeftIgnored = true;
    }

    if (normal.x < 0f)
    {
      _ignoreSurroundings.IsWallRightIgnored = true;
    }
  }
  public override void LogicUpdate()
  {
    base.LogicUpdate();
  }
  private float GetWeight(float target)
  {
    float weight = 0.4f;
    if (Mathf.Abs(PlayerController.inputManagerStrategy.GetHorizontalMovement()) < Mathf.Epsilon)
    {
      weight = 0.02f;
    }
    return weight;
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    if (_isStartWithInialZeroInput)
    {
      if ((PlayerController.inputManagerStrategy.GetHorizontalMovement() > 0f) && (_player._velocity.x < 0f))
      {
        _isStartWithInialZeroInput = false;
      }
      if ((PlayerController.inputManagerStrategy.GetHorizontalMovement() < 0f) && (_player._velocity.x > 0f))
      {
        _isStartWithInialZeroInput = false;
      }
      if(( _hitConsumer.isHittedLeft || _hitConsumer.isHittedRight) && !_ignoreSurroundings.IsWallIgnored){
        _player._velocity.x = 0f;    
      } else {
          _player._velocity.x = Mathf.Lerp(_player._velocity.x, Mathf.Sign(wallHitNormal.x) * _player.CANON_SPEED, 1f);
      }
    }
    else
    {
      float targetVelocity = PlayerController.inputManagerStrategy.GetHorizontalMovement() * _player.MAX_SPEED;
      _player._velocity.x = Mathf.Lerp(_player._velocity.x, targetVelocity, GetWeight(_targetVelocity));
    }

    if (!_hitConsumer.isHittedBottom)
    {
      _player._velocity.y -= _player._gravity * Time.fixedDeltaTime;
    }
  }
}
