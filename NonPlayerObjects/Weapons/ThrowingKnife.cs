using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float speed;

    public GameObject castingPlayer;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        InitialSetup();
    }

    [SerializeField] int pointsOnHit = 100;
    private void OnTriggerEnter2D(Collider2D hit) {
        //If enemy hit, stun enemy and grant player points
        MothMovement enemy = hit.GetComponent<MothMovement>();
        if( enemy != null){
            enemy.Stun();
            castingPlayer.GetComponent<PlayerStats>().AddPoints(pointsOnHit);
        }
        //Do not target castingPlayer
        if(hit.gameObject != castingPlayer){
            DestroySelf();
        }
        

    }

    #region Initialise
    void InitialSetup(){
        RandomiseColor();
        Invoke("DestroySelf", 1.5f);
    }

    void RandomiseColor(){
        int rnd = Random.Range(0,4);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch(rnd){
            case 0:
                sr.color = Color.blue;break;
            case 1:
                sr.color = Color.green;break;
            case 2:
                sr.color = Color.cyan;break;
            case 3:
                sr.color = Color.red;break;
            case 4:
                sr.color = Color.magenta;break;
        }
    }
    #endregion

    void DestroySelf(){
        Destroy(gameObject);
    }
}
