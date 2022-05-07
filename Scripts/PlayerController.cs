using UnityEngine;
using CollisionStuff;
using PositionAdjust;
using PlayerInput;
class IgnoreSurroundings
{
  public bool IsGroundIgnored;
  public bool IsWallIgnored;
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
  private readonly float MAX_SPEED =  5f;
  private float _gravity = 6f;
  private IgnoreSurroundings _ignoreSurroundings;
  private HitConsumerLogic _hitConsumerLogic;
  private PlayerInputLogic _playerInputLogic;
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
    _hitConsumerLogic = new HitConsumerLogic(_hitConsumer);
    _playerInputLogic = new PlayerInputLogic(_hitConsumerLogic);
    _box = BoxFromGO();
    _collisionContext = new CollisionContext(new CollisionStrategyBasic(_hitConsumer, _box));
  }
  void Update()
  {
    _horizontalInput = Input.GetAxis("Horizontal");
  }
  private float _groundIgnoreTimeStart;
  private float _wallIgnoreTimeStart;

  private void EnableIgnoreWallTimer(){
    _wallIgnoreTimeStart = Time.time;
    _ignoreSurroundings.IsWallIgnored = true;
  }

  private void EnableIgnoreGroundTimer()
  {
    _ignoreSurroundings.IsGroundIgnored = true;
    _groundIgnoreTimeStart = Time.time;
  }
  private void LandDown(ref Vector2 targetVelocity)
  {
    targetVelocity.y = 0f;
    transform.position = new Vector3(transform.position.x, _hitConsumer.surfacePoint.y + _box.GetSize().y / 2, transform.position.z);
  }

  private void MovementWithoutWall()
  {
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
    _rigidBody.velocity = targetVelocity;
  }
  private bool _isJumpAwayFromWall;
  private bool _hasInitialInput;
  private float _velocityAtBeginingOfWallJump;
  void FixedUpdate()
  {
    _collisionContext.CollisionCheck();
    if(_isJumpAwayFromWall && _hasInitialInput){
      //* is pressed inversed to current
      Vector2 targetVelocity = _rigidBody.velocity;
      targetVelocity.y = 2f;

      //* fastify during first second not fastify during next second
      _velocityAtBeginingOfWallJump -= 2f * Time.fixedDeltaTime;
      targetVelocity.x = Mathf.Lerp(targetVelocity.x, -_velocityAtBeginingOfWallJump,Time.fixedDeltaTime);
      _rigidBody.velocity = targetVelocity;
    } 
    else if (!_hitConsumerLogic.HittedAnyWalls()&& !_ignoreSurroundings.IsWallIgnored)
    {
      MovementWithoutWall();
    }
    else
    {
      if(_ignoreSurroundings.IsWallIgnored){
          //* ускоряться до скорости, потом в зависимости
        Vector2 targetVelocity = _rigidBody.velocity;
        targetVelocity.x = 5f;
        _rigidBody.velocity = Vector2.Lerp(_rigidBody.velocity,targetVelocity, 0.2f);       
      } else if (_hitConsumerLogic.HittedOnlyLeftWall() && !_ignoreSurroundings.IsWallIgnored)
      {

        RaycastHit2D  leftHit = _hitConsumer.GetLeftHit();
        Vector2 leftHitSize = leftHit.collider.bounds.size;
        Vector2 leftHitPosition = leftHit.collider.transform.position;

        CollisionBasic collisionBasic = new CollisionBasic(leftHitPosition, leftHit.normal, leftHitSize);
        PositionAdjustMediator positionAdjustMediator = new PositionAdjustMediator(collisionBasic, _box);
         transform.position = positionAdjustMediator.AdjustPosition();
        Vector2 targetVelocity = _rigidBody.velocity;
        //* if input direction into wall
        if(Input.GetKey(KeyCode.Space)){

          _isJumpAwayFromWall = true;
          _hasInitialInput = Mathf.Abs(_horizontalInput)>0f;
          _velocityAtBeginingOfWallJump  = _horizontalInput*_speed;
        }
        if(_playerInputLogic.IsPressedInveredToWall()){
          EnableIgnoreWallTimer();
        } else {
          targetVelocity.x = 0f;
        }
        _rigidBody.velocity = targetVelocity; 
      }
      else if (_hitConsumerLogic.HittedOnlyRightWall())
      {

      }
    }

    if (_ignoreSurroundings.IsGroundIgnored)
    {
      CheckFinishIgnoreGravity();
    }
    if(_ignoreSurroundings.IsWallIgnored){
      CheckFinishIgnoreWall();
    }

  }
  private void CheckFinishIgnoreWall(){
    float delta = Time.time - _wallIgnoreTimeStart;
    if (delta < 0.2f)
    {
      _ignoreSurroundings.IsWallIgnored = true;
    }
    else
    {
      _ignoreSurroundings.IsWallIgnored = false;
    } 
  }
  private void CheckFinishIgnoreGravity()
  {
    float delta = Time.time - _groundIgnoreTimeStart;
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
