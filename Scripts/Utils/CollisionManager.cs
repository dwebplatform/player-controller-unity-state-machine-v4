using UnityEngine;

namespace CollisionStuff
{
  public interface ICollisionStrategy
  {
    public void CollisionCheck();
  }
  public class HitConsumer
  {
    public bool isHittedRight;
    public bool isHittedLeft;
    public bool isHittedBottom;
    public bool isHittedTop;
    public Vector2 surfacePoint;
  }

  public interface IBox
  {
    //* width, height
    Vector2 getSize();
    //* x, y of center
    Vector2 getPosition();
  }
  class CollisionStrategyBasic : ICollisionStrategy
  {
    private HitConsumer _hitConsumer;
    private LayerMask _whatIsWall;
    private ContactFilter2D _whatIsGroundFilter;
    private ContactFilter2D _whatIsCellFilter;
    private Collider2D[] bottomResults = new Collider2D[1];
    private Collider2D[] topResults = new Collider2D[1];
    private float _offset = 0.1f;
    private IBox _box;
    public CollisionStrategyBasic(HitConsumer hitConsumer, IBox box)
    {
      _hitConsumer = hitConsumer;
      _box = box;
      _whatIsGroundFilter.SetLayerMask(LayerMask.GetMask("Ground"));
      _whatIsCellFilter.SetLayerMask(LayerMask.GetMask("Cell"));
      _whatIsWall = LayerMask.GetMask("Wall");
    }
    private void CheckGround()
    {
      Vector2 boxSize = new Vector2(_box.getSize().x, _box.getSize().y);
      Vector2 bottomPoint = _box.getPosition() + Vector2.down * _offset;
      if (Physics2D.OverlapBox(bottomPoint, boxSize, 0, _whatIsGroundFilter, bottomResults) > 0)
      {
        Vector2 surfacePoint = Physics2D.ClosestPoint(_box.getPosition(), bottomResults[0]);
        _hitConsumer.isHittedBottom = true;
        _hitConsumer.surfacePoint = surfacePoint;
      }
      else
      {
        _hitConsumer.isHittedBottom = false;
      }
    }
    private void CheckCell()
    {
      Vector2 boxSize = new Vector2(_box.getSize().x, _box.getSize().y);
      Vector2 topPoint = _box.getPosition() + Vector2.up * _offset;
      if (Physics2D.OverlapBox(topPoint, boxSize, 0, _whatIsCellFilter, topResults) > 0)
      {
        Vector2 surfacePoint = Physics2D.ClosestPoint(_box.getPosition(), topResults[0]);
        _hitConsumer.isHittedTop = true;
      }
      else
      {
        _hitConsumer.isHittedTop = false;
      }
    }
    private void CheckWall()
    {
      Vector2 position = _box.getPosition();
      Vector2 boxSize = _box.getSize();

      Vector2 topPosition = new Vector2(position.x, position.y + boxSize.y / 2);
      Vector2 middlePosition = new Vector2(position.x, position.y);
      Vector2 bottomPosition = new Vector2(position.x, position.y - boxSize.y / 2);
      //* three point top, middle, bottom
      Vector2[] positions = { bottomPosition, middlePosition, topPosition };
      bool isHittedRight = false;
      bool isHittedLeft = false;

      foreach (var positionEl in positions)
      {
        RaycastHit2D rightHit = Physics2D.Raycast(positionEl, Vector2.right, boxSize.y / 2 + _offset,
           _whatIsWall);
        if (rightHit.collider != null)
        {
          isHittedRight = true;
        }
        RaycastHit2D leftHit = Physics2D.Raycast(positionEl, Vector2.right, boxSize.y / 2 - _offset,
           _whatIsWall);
        if (leftHit.collider != null)
        {
          isHittedLeft = true;
        }
      }
      _hitConsumer.isHittedRight = isHittedRight;
      _hitConsumer.isHittedLeft = isHittedLeft;
    }
    public void CollisionCheck()
    {
      CheckGround();
      CheckCell();
      CheckWall();
    }
  }
  public class CollisionContext
  {
    ICollisionStrategy _strategy;
    public CollisionContext(ICollisionStrategy strategy)
    {
      _strategy = strategy;
    }
    public void CollisionCheck()
    {
      _strategy.CollisionCheck();
    }
  }

  class BasicBox : IBox
  {
    private Vector2 _size;
    private Vector2 _position;
    Transform _transform;
    public BasicBox(Transform transform, Vector2 size)
    {
      _position = transform.position;
      _size = size;
      _transform = transform;
    }
    public Vector2 getSize()
    {
      return _size;
    }
    public Vector2 getPosition()
    {
      return _transform.position;
    }
  }
}
