using UnityEngine;
public interface IBox
  {
    //* width, height
    public Vector2 GetSize();
    //* x, y of center
    public Vector2 GetPosition();
  }