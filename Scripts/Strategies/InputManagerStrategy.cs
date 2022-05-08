using UnityEngine;

namespace InputStrategy {
  public interface IInputStrategy {
    public float GetHorizontalMovement();
    public bool GetJumpPressed();
    public bool PressedLeft();
    public bool PressedRight();

    public bool HasPressedAnyMovementKey();
  }
  
  public class InputBaseHandler:IInputStrategy {
    public float GetHorizontalMovement(){
      return Input.GetAxis("Horizontal");
    }
    public bool GetJumpPressed(){
      return Input.GetKey(KeyCode.Space);
    }
    public bool PressedRight(){
      return Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow);
    }
    public bool PressedLeft(){
      return Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow);
    }
    public bool HasPressedAnyMovementKey(){
      return Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.RightArrow);
    }
  }
  public class InputManagerStrategy {
    private IInputStrategy _strategy;
    public InputManagerStrategy(IInputStrategy strategy){
      _strategy = strategy;
    }

    public bool GetJumpPressed(){
      return _strategy.GetJumpPressed();
    }
    public bool PressedLeft(){
      return _strategy.PressedLeft();
    }
    public bool  PressedRight(){
      return _strategy.PressedRight();
    }
    public bool HasPressedAnyMovementKey(){
      return _strategy.HasPressedAnyMovementKey();
    }
    public float GetHorizontalMovement(){
      return _strategy.GetHorizontalMovement();
    }
  }
}