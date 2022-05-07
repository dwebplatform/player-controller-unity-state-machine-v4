using UnityEngine;
public interface IBox
  {
    //* width, height
    Vector2 GetSize();
    //* x, y of center
    Vector2 GetPosition();
  }