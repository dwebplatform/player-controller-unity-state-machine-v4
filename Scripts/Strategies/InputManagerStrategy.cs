using UnityEngine;

namespace InputStrategy {
  public interface IInputStrategy {
    public float GetHorizontalMovement();
  }
  public class InputBaseHandler:IInputStrategy {
    public float GetHorizontalMovement(){
      return Input.GetAxis("Horizontal");
    }
  }
  public class InputManagerStrategy {
    private IInputStrategy _strategy;
    public InputManagerStrategy(IInputStrategy strategy){
      _strategy = strategy;
    }
    public float GetHorizontalMovement(){
      return _strategy.GetHorizontalMovement();
    }
  }
}