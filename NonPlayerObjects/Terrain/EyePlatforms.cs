using UnityEngine;

public class EyePlatforms : MonoBehaviour
{
    //Script attached to "London Eye" platforms to allow player to move with the platforms

    Rigidbody2D rb;
    [SerializeField] Transform units;

    private void OnTriggerEnter2D(Collider2D other) {
        PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
        if(player){
            other.transform.parent = transform;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
        if(player){
            other.transform.parent = units;
        }
    }
}
