using UnityEngine;

public class Player {
  public Vector2 _velocity;
  public Transform _transform;
  public readonly float MAX_SPEED =  5f;
  public float _gravity = 6f;
  public float _wallFriction = 3f;
  public IBox _box;
  public Player(Transform transform, Vector2 velocity, IBox box){
    _transform = transform;
    _velocity = velocity;
    _box = box;
  }

}