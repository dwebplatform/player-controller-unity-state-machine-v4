using UnityEngine;
using CollisionStuff;

public class PlayerController : MonoBehaviour
{
  private CollisionContext _collisionContext;
  private HitConsumer _hitConsumer;
  private IBox _box;
  private BoxCollider2D _boxCollider;

  private Rigidbody2D _rigidBody;

  private IBox BoxFromGO()
  {
    BasicBox box = new BasicBox(transform, _boxCollider.size);
    return box;
  }
  private void Awake()
  {
    _boxCollider = GetComponent<BoxCollider2D>();
    _rigidBody = GetComponent<Rigidbody2D>();

    _hitConsumer = new HitConsumer();
    _box = BoxFromGO();
    _collisionContext = new CollisionContext(new CollisionStrategyBasic(_hitConsumer, _box));
  }
  void Update()
  {
  }
  void FixedUpdate()
  {
    _collisionContext.CollisionCheck();
    Vector2 targetVelocity = _rigidBody.velocity;
    targetVelocity.x -= 5f * Time.fixedDeltaTime;
    _rigidBody.velocity = targetVelocity;
  }
}
