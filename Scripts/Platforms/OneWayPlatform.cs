using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField]
    private bool _isUp;
    private Collider2D platformCollider;
    private void Awake(){
        platformCollider = transform.parent.GetComponent<BoxCollider2D>();
    }    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerSelector playerSelector;
        //* player встретился с платформой
        if(other.gameObject.TryGetComponent<PlayerSelector>(out playerSelector)){
            platformCollider.enabled = _isUp;
        }
    }

    
}
