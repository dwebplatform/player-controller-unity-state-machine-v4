using UnityEngine;

namespace PositionAdjust
{
  interface ICollision
  {
    public Vector2 GetNormal();
    public Vector2 GetSize();
    public Vector2 GetPosition();
  }

  class CollisionBasic: ICollision {
    Vector2 _position;
    Vector2 _normal;
    Vector2 _size;

    public CollisionBasic(Vector2 position, Vector2 normal,Vector2 size ){
      _position = position;
      _normal = normal;
      _size = size;
    }

    public Vector2 GetNormal()
    {
      return _normal;
    }

    public Vector2 GetPosition()
    {
      return _position;
    }

    public Vector2 GetSize()
    {
      return _size;
    }
  }
  class PositionAdjustMediator
  {
    private IBox _box;
    private ICollision _collision;
    public PositionAdjustMediator(ICollision collision, IBox box)
    {
      _collision = collision;
      _box = box;
    }
    public Vector2 AdjustPosition()
    {
      Vector2 normal = _collision.GetNormal();
      Vector2 difference = -Mathf.Sign(normal.x) * (_collision.GetPosition() - _box.GetPosition());
      Vector2 collisionSize = _collision.GetSize();
      Vector2 boxSize = _box.GetSize();
      float gap = difference.x - collisionSize.x / 2 - boxSize.x / 2;
      Vector2 correctedPosition = _box.GetPosition() - new Vector2(normal.x, normal.y) * gap;
      return correctedPosition;
    }
  }
}

