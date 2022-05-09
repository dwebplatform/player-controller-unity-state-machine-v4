using UnityEngine;
using System;
using CollisionStuff;
// using PlayerInput;
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
  private IgnoreSurroundings _ignoreSurroundings;
  private HitConsumerLogic _hitConsumerLogic;
  private PositionAdjustManager _positionAdjustManager;
  public static InputManagerStrategy inputManagerStrategy;
  private StateMachine _playerStateMachine;
  private float _groundIgnoreTimeStart;

  private Player _player;
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

    PlayerController.inputManagerStrategy = new InputManagerStrategy(new InputBaseHandler());
    _box = BoxFromGO();

    _player = new Player(transform, _rigidBody.velocity, _box);
    _collisionContext = new CollisionContext(new CollisionStrategyBasic(_hitConsumer, _box));
    _playerStateMachine = new StateMachine();

    StateMachine.idleState = new IdleState("IdleState", _player, _hitConsumer, _adjustments);
    StateMachine.walkingState = new WalkingState("WalkingState", _player, _hitConsumer, _adjustments, _ignoreSurroundings);
    StateMachine.jumpingState = new JumpingState("JumpingState", _player, _hitConsumer, _ignoreSurroundings);
    StateMachine.jumpNearWallState = new JumpNearWallState("JumpingNearWallState", _player, _hitConsumer, _ignoreSurroundings,
    () =>
    {
      if (_adjustments.ContainsKey(_playerStateMachine.GetCurrentState().GetType()))
      {
        _adjustments[_playerStateMachine.GetCurrentState().GetType()] = new PositionAdjustWithGravityIgnoreStrategy(_hitConsumer, _player, PlayerController.inputManagerStrategy, _ignoreSurroundings);
      }
    },
    () =>
    {
      _adjustments[_playerStateMachine.GetCurrentState().GetType()] = new PositionAdjustWithIgnoreNearWallInitialStrategy(_hitConsumer, _player, PlayerController.inputManagerStrategy, _ignoreSurroundings);
    });

    StateMachine.grabWallState = new GrabWallState("GrabWallState", _player, _hitConsumer, _ignoreSurroundings);
    StateMachine.jumpAwayFromWallGrabState = new JumpAwayFromWallGrabState("JumpAwayFromWallGrabState",_player, _hitConsumer, _ignoreSurroundings);

    CreatePositionAdjustments();

    When(StateMachine.idleState, StateMachine.jumpingState, () => _hitConsumerLogic.GetClosestWall() == null && PlayerController.inputManagerStrategy.GetJumpPressed());
    When(StateMachine.walkingState, StateMachine.jumpingState, () => _hitConsumerLogic.GetClosestWall() == null && PlayerController.inputManagerStrategy.GetJumpPressed());

    When(StateMachine.jumpAwayFromWallGrabState, StateMachine.idleState,()=>!_ignoreSurroundings.IsGroundIgnored && _hitConsumer.isHittedBottom);
    When(StateMachine.jumpAwayFromWallGrabState, StateMachine.grabWallState,()=>!_ignoreSurroundings.IsWallIgnored && _hitConsumerLogic.HittedAnyWalls());

    When(StateMachine.idleState, StateMachine.jumpNearWallState, () => _hitConsumerLogic.GetClosestWall() != null && PlayerController.inputManagerStrategy.GetJumpPressed());
    When(StateMachine.walkingState, StateMachine.jumpNearWallState, () => _hitConsumerLogic.GetClosestWall() != null && PlayerController.inputManagerStrategy.GetJumpPressed());
    When(StateMachine.jumpNearWallState, StateMachine.grabWallState, () => Input.GetKeyDown(KeyCode.D) && _hitConsumer._closestWall != null && _hitConsumer._closestWall.Value.normal.x < 0f);
    When(StateMachine.jumpNearWallState, StateMachine.grabWallState, () => Input.GetKeyDown(KeyCode.A) && _hitConsumer._closestWall != null && _hitConsumer._closestWall.Value.normal.x > 0f);

    When(StateMachine.jumpNearWallState, StateMachine.idleState, () => !_ignoreSurroundings.IsGroundIgnored && _hitConsumer.isHittedBottom);

    When(StateMachine.jumpingState, StateMachine.idleState, () => _hitConsumer.isHittedBottom && !_ignoreSurroundings.IsGroundIgnored);

    When(StateMachine.jumpingState, StateMachine.grabWallState, () => _hitConsumerLogic.HittedAnyWalls());


    //* разрешать входить в это состояние, ТОЛЬКО при условии, что не нажали на кнопку придавливающую к стене
    When(StateMachine.grabWallState,StateMachine.jumpAwayFromWallGrabState,() => !CheckPressTowardWall() && Input.GetKey(KeyCode.Space));
    When(StateMachine.grabWallState, StateMachine.idleState, () => _hitConsumer.isHittedBottom);

    When(StateMachine.idleState, StateMachine.walkingState, () => Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Epsilon);
    When(StateMachine.walkingState, StateMachine.idleState, () => Mathf.Abs(PlayerController.inputManagerStrategy.GetHorizontalMovement()) < Mathf.Epsilon);
    void When(BaseState from, BaseState to, Func<bool> predicate)
    {
      _playerStateMachine.AddTransition(from, to, predicate);
    }
    Init();
  }

  private bool CheckPressTowardWall()
  {
    bool hasPressedTowardWall = false;
    if (_hitConsumer._closestWall != null)
    {
      Vector2 normal = _hitConsumer._closestWall.Value.normal;
      bool pressedTowardLeftWall = (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.LeftArrow))) && normal.x > 0f;
      bool pressedTowardRIghtWall = (Input.GetKey(KeyCode.D) || (Input.GetKey(KeyCode.RightArrow))) && normal.x < 0f;

      if (pressedTowardLeftWall || pressedTowardRIghtWall)
      {
        hasPressedTowardWall = true;
      }
    }
    return hasPressedTowardWall;
  }

  private void CreatePositionAdjustments()
  {
    _adjustments.Add(StateMachine.idleState.GetType(), new PositionAdjustBaseStrategy(_hitConsumer, _player, _ignoreSurroundings));
    _adjustments.Add(StateMachine.walkingState.GetType(), new PositionAdjustWithIgnoreWallStrategy(_hitConsumer, _player, _ignoreSurroundings));
    _adjustments.Add(StateMachine.jumpingState.GetType(), new PositionAdjustWithGravityIgnoreStrategy(_hitConsumer, _player, PlayerController.inputManagerStrategy, _ignoreSurroundings));
    _adjustments.Add(StateMachine.jumpNearWallState.GetType(), new PositionAdjustWithIgnoreNearWallInitialStrategy(_hitConsumer, _player, PlayerController.inputManagerStrategy, _ignoreSurroundings));
    _adjustments.Add(StateMachine.jumpAwayFromWallGrabState.GetType(), new PositionAdjustWithJumpAwayInitialStrategy(_hitConsumer, _player, PlayerController.inputManagerStrategy, _ignoreSurroundings) );
  }

  private IPositionAdjustStrategy _currentStrategy;
  private void Init()
  {
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
  private Dictionary<Type, IPositionAdjustStrategy> _adjustments = new Dictionary<Type, IPositionAdjustStrategy>();
  private IPositionAdjustStrategy GetAdjustStrategy()
  {
    if (_adjustments.ContainsKey(_playerStateMachine.GetCurrentState().GetType()))
    {
      return _adjustments[_playerStateMachine.GetCurrentState().GetType()];
    }
    else
    {
      return null;
    }
  }

  private bool _jumpedNearWall;
  private Nullable<RaycastHit2D> _nearestWall;
  void FixedUpdate()
  {
    _collisionContext.CollisionCheck();
    _currentStrategy = GetAdjustStrategy();
    _positionAdjustManager.SetStrategy(_currentStrategy);
    _positionAdjustManager.MakeAdjustment();

    _playerStateMachine.PhysicsUpdate();

    _rigidBody.velocity = _player._velocity;

    if (_ignoreSurroundings.IsWallLeftIgnored || _ignoreSurroundings.IsWallRightIgnored)
    {
      CheckFinishIgnoreWall();
    }
    if (_ignoreSurroundings.IsGroundIgnored)
    {
      CheckFinishIgnoreGravity();
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
    float delta = Time.time - _ignoreSurroundings.GroundStartTime;
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
