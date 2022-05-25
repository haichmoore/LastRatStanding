using UnityEngine;

public class IceCube : MonoBehaviour
{
    [SerializeField] GameObject moth;
    
    void OnTriggerEnter2D(Collider2D other) {
        PlayerStats player = other.GetComponent<PlayerStats>();
        if(player){
            player.AddPoints(100);
            moth.GetComponent<MothMovement>().Die();
        }
    }
}
