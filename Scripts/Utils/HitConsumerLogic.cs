using CollisionStuff;


public class HitConsumerLogic {
  
  private HitConsumer _hitConsumer;
  public HitConsumerLogic(HitConsumer hitConsumer){
    _hitConsumer = hitConsumer;
  }
  
  public bool HittedOnlyLeftWall(){
    return _hitConsumer.isHittedLeft && !_hitConsumer.isHittedRight;
  }
  public bool HittedOnlyRightWall(){
    return _hitConsumer.isHittedRight && !_hitConsumer.isHittedLeft;
  }
  public bool HittedAnyWalls(){
    return _hitConsumer.isHittedLeft|| _hitConsumer.isHittedRight;
  }
}