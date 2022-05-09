using UnityEngine;

public class Player {
  public Vector2 _velocity;
  public Transform _transform;
  public float HEIGHT = 4f;
  public readonly float JUMP_MAGNITUDE;
  public readonly float MAX_SPEED =  5f;
  public readonly float CANON_SPEED = 8f;
  public float _gravity = 12f;
  public float DOWN_GRAVITY = 24f;
  public float _wallFriction = 3f;
  public IBox _box;
  public Player(Transform transform, Vector2 velocity, IBox box){
    _transform = transform;
    _velocity = velocity;
    _box = box;
    JUMP_MAGNITUDE = Mathf.Sqrt(2* _gravity * HEIGHT);
  }

}