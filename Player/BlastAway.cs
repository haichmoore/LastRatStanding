using UnityEngine;

public class BlastAway : MonoBehaviour
{
    //BlastAway active controller
    
    public GameObject castingPlayer;

    void Awake(){
        Invoke("DestroySelf", 0.25f);
    }

    [SerializeField] int pointsOnHit = 50;
    private void OnTriggerEnter2D(Collider2D hit) {
        //Stun enemies hit by collider, give casting player points
        MothMovement enemy = hit.GetComponent<MothMovement>();
        if( enemy != null){
            enemy.Stun();
            castingPlayer.GetComponent<PlayerStats>().AddPoints(pointsOnHit);
        }

    }
    void DestroySelf(){
        Destroy(gameObject);
    }
}
