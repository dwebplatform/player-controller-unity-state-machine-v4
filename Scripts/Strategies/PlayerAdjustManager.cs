using UnityEngine;
using CollisionStuff;
namespace PositionAdjustManagerStrategy
{
  public class AdjustFinder {
    public static Vector2 AdjustWall(RaycastHit2D hittedCollider, IBox box)
    {
      Vector2 normal = hittedCollider.normal;
      Vector2 colliderPosition = hittedCollider.collider.transform.position;

      Vector2 difference = -Mathf.Sign(normal.x) * (colliderPosition - box.GetPosition());
      Vector2 collisionSize = hittedCollider.collider.bounds.size;
      float gap = difference.x - collisionSize.x / 2 - box.GetSize().x / 2;
      Vector2 correctedPosition = box.GetPosition() - new Vector2(normal.x, normal.y) * gap;
      return correctedPosition;
    }
  }
  public interface IPositionAdjustStrategy
  {
    public void MakeAdjustment();
  }
  class PositionAdjustWithIgnoreWallStrategy : IPositionAdjustStrategy
  {
    private HitConsumer _hitConsumer;
    private Player _player;
    private IgnoreSurroundings _ignoreSurroundings;
    public PositionAdjustWithIgnoreWallStrategy(HitConsumer hitConsumer, Player player, IgnoreSurroundings ignoreSurroundings)
    {
      _hitConsumer = hitConsumer;
      _player = player;
      _ignoreSurroundings = ignoreSurroundings;
    }
    public void MakeAdjustment()
    {
      if (_hitConsumer.isHittedLeft && !_ignoreSurroundings.IsWallIgnored)
      {
        _player._transform.position = AdjustFinder.AdjustWall(_hitConsumer.GetLeftHit(), _player._box);
         _player._velocity.x = 0f;
      }
      else if (_hitConsumer.isHittedRight && !_ignoreSurroundings.IsWallIgnored)
      {
        _player._transform.position = AdjustFinder.AdjustWall(_hitConsumer.GetRightHit(), _player._box);
         _player._velocity.x = 0f;
      }

      if (_hitConsumer.isHittedBottom)
      {
        _player._transform.position = new Vector2(_player._transform.position.x, _hitConsumer.surfacePoint.y + _player._box.GetSize().y / 2);
        _player._velocity.y = 0f;
      }
    }
  }
  class PositionAdjustBaseStrategy : IPositionAdjustStrategy
  {
    private HitConsumer _hitConsumer;
    private Player _player;
    public PositionAdjustBaseStrategy(HitConsumer hitConsumer, Player player, IgnoreSurroundings _ignoreSurroundings)
    {
      _hitConsumer = hitConsumer;
      _player = player;
    }
    public void MakeAdjustment()
    {
      if (_hitConsumer.isHittedLeft)
      {
        _player._transform.position = AdjustFinder.AdjustWall(_hitConsumer.GetLeftHit(), _player._box);
      }
      else if (_hitConsumer.isHittedRight)
      {
        _player._transform.position = AdjustFinder.AdjustWall(_hitConsumer.GetRightHit(), _player._box);
      }

      if (_hitConsumer.isHittedBottom)
      {
        _player._transform.position = new Vector2(_player._transform.position.x, _hitConsumer.surfacePoint.y + _player._box.GetSize().y / 2);
        _player._velocity.y = 0f;
      }
    }

    // private Vector2 AdjustWall(RaycastHit2D hittedCollider, IBox box)
    // {
    //   Vector2 normal = hittedCollider.normal;
    //   Vector2 colliderPosition = hittedCollider.collider.transform.position;

    //   Vector2 difference = -Mathf.Sign(normal.x) * (colliderPosition - box.GetPosition());
    //   Vector2 collisionSize = hittedCollider.collider.bounds.size;
    //   float gap = difference.x - collisionSize.x / 2 - box.GetSize().x / 2;
    //   Vector2 correctedPosition = box.GetPosition() - new Vector2(normal.x, normal.y) * gap;
    //   return correctedPosition;
    // }
  }
  public class PositionAdjustManager
  {
    private IPositionAdjustStrategy _strategy;
    public PositionAdjustManager(IPositionAdjustStrategy strategy)
    {
      _strategy = strategy;
    }
    public void SetStrategy(IPositionAdjustStrategy newStrategy){
      if(newStrategy == null){
        return;
      }
      if(newStrategy == _strategy){
        return;
      }
      _strategy = newStrategy;
    }
    public void MakeAdjustment()
    {
      _strategy.MakeAdjustment();
    }
  }
}