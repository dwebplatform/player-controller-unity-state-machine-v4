using UnityEngine;
using CollisionStuff;



class IgnoreSurroundings
{
  public bool IsGroundIgnored;
}
public class PlayerController : MonoBehaviour
{
  private CollisionContext _collisionContext;
  private HitConsumer _hitConsumer;
  private IBox _box;
  private BoxCollider2D _boxCollider;

  private Rigidbody2D _rigidBody;
  private float _horizontalInput;
  private float _speed = 5f;
  private float _gravity = 6f;
  private IgnoreSurroundings _ignoreSurroundings;
  private IBox BoxFromGO()
  {
    BasicBox box = new BasicBox(transform, _boxCollider.size);
    return box;
  }
  private void Awake()
  {
    _boxCollider = GetComponent<BoxCollider2D>();
    _rigidBody = GetComponent<Rigidbody2D>();
    _ignoreSurroundings = new IgnoreSurroundings();
    _hitConsumer = new HitConsumer();
    _box = BoxFromGO();
    _collisionContext = new CollisionContext(new CollisionStrategyBasic(_hitConsumer, _box));
  }
  void Update()
  {
    _horizontalInput = Input.GetAxis("Horizontal");
  }
  private float _wallTimeIgnoreStart;

  private void EnableIgnoreGroundTimer()
  {
    _ignoreSurroundings.IsGroundIgnored = true;
    _wallTimeIgnoreStart = Time.time;
  }
  //   private void 
  private void LandDown(ref Vector2 targetVelocity)
  {
    targetVelocity.y = 0f;
    transform.position = new Vector3(transform.position.x, _hitConsumer.surfacePoint.y + _box.getSize().y / 2, transform.position.z);
  }
  void FixedUpdate()
  {
    _collisionContext.CollisionCheck();

    Vector2 targetVelocity = _rigidBody.velocity;
    targetVelocity.x = _horizontalInput * _speed;
    if (!_hitConsumer.isHittedBottom)
    {
      targetVelocity.y -= _gravity * Time.fixedDeltaTime;
    }
    else
    {
      if (!_ignoreSurroundings.IsGroundIgnored)
      {
        LandDown(ref targetVelocity);
      }
    }
    if (Input.GetKey(KeyCode.Space))
    {
      EnableIgnoreGroundTimer();
      targetVelocity.y = 5f;
    }
    if (_ignoreSurroundings.IsGroundIgnored)
    {
        CheckFinishIgnoreGravity();
    }
    _rigidBody.velocity = targetVelocity;
  }
  private void CheckFinishIgnoreGravity(){
    float delta = Time.time - _wallTimeIgnoreStart;
      if (delta < 0.2f)
      {
        _ignoreSurroundings.IsGroundIgnored = true;
      }
      else
      {
        _ignoreSurroundings.IsGroundIgnored = false;
      }
  }
}
