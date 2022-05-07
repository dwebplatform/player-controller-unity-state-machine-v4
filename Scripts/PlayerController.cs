
using UnityEngine;
using System;
using CollisionStuff;
using PositionAdjust;
using PlayerInput;
using InputStrategy;
using System.Collections.Generic;
using PositionAdjustManagerStrategy;

public class IgnoreSurroundings
{
  public bool IsGroundIgnored;
  public bool IsWallIgnored;
  public bool IsWallLeftIgnored;
  public bool IsWallRightIgnored;

  public float WallStartTime;

  public float GroundStartTime;
}

public class PlayerController : MonoBehaviour
{
  private CollisionContext _collisionContext;
  private HitConsumer _hitConsumer;
  private IBox _box;
  private BoxCollider2D _boxCollider;
  private Rigidbody2D _rigidBody;
  private float _horizontalInput;
  private readonly float MAX_SPEED = 5f;
  private float _gravity = 6f;
  private IgnoreSurroundings _ignoreSurroundings;
  private HitConsumerLogic _hitConsumerLogic;
  private PlayerInputLogic _playerInputLogic;
  private PositionAdjustManager _positionAdjustManager;
  public static InputManagerStrategy inputManagerStrategy;
  private StateMachine _playerStateMachine;

  private Player _player;
  private IBox BoxFromGO()
  {
    BasicBox box = new BasicBox(transform, _boxCollider.size);
    return box;
  }

  private bool PressedMovement()
  {
    return Mathf.Abs(PlayerController.inputManagerStrategy.GetHorizontalMovement()) > Mathf.Epsilon;
  }

  private void Awake()
  {
    _boxCollider = GetComponent<BoxCollider2D>();
    _rigidBody = GetComponent<Rigidbody2D>();

    _ignoreSurroundings = new IgnoreSurroundings();
    _hitConsumer = new HitConsumer();
    _hitConsumerLogic = new HitConsumerLogic(_hitConsumer);
    _playerInputLogic = new PlayerInputLogic(_hitConsumerLogic);

    PlayerController.inputManagerStrategy = new InputManagerStrategy(new InputBaseHandler());
    _box = BoxFromGO();

    _player = new Player(transform, _rigidBody.velocity, _box);
    _collisionContext = new CollisionContext(new CollisionStrategyBasic(_hitConsumer, _box));
    _playerStateMachine = new StateMachine();

    StateMachine.idleState = new IdleState("IdleState", _player, _hitConsumer, _adjustments);
    StateMachine.walkingState = new WalkingState("WalkingState", _player, _hitConsumer,_adjustments,_ignoreSurroundings);


    _adjustments.Add(StateMachine.idleState.GetType(), new PositionAdjustBaseStrategy(_hitConsumer,_player,_ignoreSurroundings));
    _adjustments.Add(StateMachine.walkingState.GetType(), new PositionAdjustWithIgnoreWallStrategy(_hitConsumer,_player,_ignoreSurroundings));
    
    When(StateMachine.idleState, StateMachine.walkingState, () => Mathf.Abs(Input.GetAxis("Horizontal")) > 0f);
    When(StateMachine.walkingState, StateMachine.idleState, () => Mathf.Abs(PlayerController.inputManagerStrategy.GetHorizontalMovement()) < Mathf.Epsilon);

    void When(BaseState from, BaseState to, Func<bool> predicate)
    {
      _playerStateMachine.AddTransition(from, to, predicate);
    }
    Init();
  }
  private IPositionAdjustStrategy _currentStrategy;
  private void Init(){

    _playerStateMachine.Initialize();
    BaseState currentState = _playerStateMachine.GetCurrentState();
    _currentStrategy = _adjustments[currentState.GetType()];
     _positionAdjustManager = new PositionAdjustManager(_currentStrategy);
  }
  void Update()
  {
    _horizontalInput = PlayerController.inputManagerStrategy.GetHorizontalMovement();
    _playerStateMachine.LogicUpdate();
  }
  private float _groundIgnoreTimeStart;
  private float _wallIgnoreTimeStart;

  private void EnableIgnoreWallTimer()
  {
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

  // private void MovementWithoutWall()
  // {
  //   Vector2 targetVelocity = _rigidBody.velocity;
  //   targetVelocity.x = _horizontalInput * MAX_SPEED;
  //   if (!_hitConsumer.isHittedBottom)
  //   {
  //     targetVelocity.y -= _gravity * Time.fixedDeltaTime;
  //   }
  //   else
  //   {
  //     if (!_ignoreSurroundings.IsGroundIgnored)
  //     {
  //       LandDown(ref targetVelocity);
  //     }
  //   }
  //   if (Input.GetKey(KeyCode.Space))
  //   {
  //     EnableIgnoreGroundTimer();
  //     targetVelocity.y = 5f;
  //   }
  //   _rigidBody.velocity = targetVelocity;
  // }
  private bool _isJumpAwayFromWall;
  private bool _hasInitialInput;
  private float _velocityAtBeginingOfWallJump;
  private Dictionary<Type,IPositionAdjustStrategy> _adjustments = new Dictionary<Type, IPositionAdjustStrategy>();

  private IPositionAdjustStrategy GetAdjustStrategy(){
    return _adjustments[_playerStateMachine.GetCurrentState().GetType()];
  }
  void FixedUpdate()
  {
    _collisionContext.CollisionCheck();
    _currentStrategy = GetAdjustStrategy();
    _positionAdjustManager.SetStrategy(_currentStrategy);
    _positionAdjustManager.MakeAdjustment();

    _playerStateMachine.PhysicsUpdate();

    _rigidBody.velocity = _player._velocity;

    if(_ignoreSurroundings.IsWallLeftIgnored || _ignoreSurroundings.IsWallRightIgnored){
      CheckFinishIgnoreWall();
    }
  }

  private void CheckFinishIgnoreWall()
  {
    float delta = Time.time - _ignoreSurroundings.WallStartTime;
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
