
using UnityEngine;

namespace PlayerInput
{
  public class PlayerInputLogic
  {
    private HitConsumerLogic _hitConsumerLogic;
    public PlayerInputLogic(HitConsumerLogic hitConsumerLogic)
    {
      _hitConsumerLogic = hitConsumerLogic;
    }
    public bool IsPressedInveredToWall()
    {
      return (_hitConsumerLogic.HittedOnlyLeftWall() &&
      (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) ||
      (_hitConsumerLogic.HittedOnlyRightWall() && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)));
    }
  }
}

