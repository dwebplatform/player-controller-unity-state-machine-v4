using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollisionStuff;

public class GrabWallState : BaseState
{
  private Player _player;
  private HitConsumer _hitConsumer;
  private IgnoreSurroundings _ignoreSurroundings;

  public GrabWallState(string name, Player player, HitConsumer hitConsumer, IgnoreSurroundings ignoreSurroundings) : base(name)
  {
    _player = player;
    _ignoreSurroundings = ignoreSurroundings;
    _hitConsumer = hitConsumer;
  }

  public override void Enter()
  {
    base.Enter();
    _player._velocity.y = 0f;
  }
  public override void PhysicsUpdate()
  {
    base.PhysicsUpdate();
    if (!_hitConsumer.isHittedBottom)
    {
      _player._velocity.y -= _player._wallFriction * Time.fixedDeltaTime;
    }
    
  }
}
