using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MothMovement : MonoBehaviour
{
    Rigidbody2D rb;
    GameManager gameManager;
    AudioManager audioManager;    
    AIPath aiPath;
    
    void Awake(){
        audioManager =FindObjectOfType<AudioManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();  
        rb = gameObject.GetComponent<Rigidbody2D>();
        EnemySetup();
        PlayerCheck();
    }

    void Update(){
        SwipeCheck();
    }
    #region Setup
    void EnemySetup(){
        aiPath = GetComponent<AIPath>();
        aiPath.canMove = true;
        gameManager.activeEnemies.Add(gameObject);
    }
    #endregion
    
    #region FindTarget
    List<GameObject> activePlayers;

    public AIDestinationSetter setter;
    
    void PlayerCheck(){
        //Target the closest player in multiplayer setting;
        setter = gameObject.GetComponent<AIDestinationSetter>();
        activePlayers = gameManager.activePlayers; 
        
        if(activePlayers.Count > 1){
            InvokeRepeating("UpdateTarget",0,1);
        }
        
        else if(activePlayers.Count > 0){
            setter.target = activePlayers[0].transform;
        }
    }

    float smallestDistance;
    GameObject closestPlayer;

    void UpdateTarget(){
        // Future proofing for multiplayer - target closest player
        closestPlayer = activePlayers[0];
        smallestDistance = Vector3.Distance(closestPlayer.transform.position,transform.position);
        foreach(GameObject player in activePlayers){
            if(Vector3.Distance(player.transform.position,transform.position) < smallestDistance){
                closestPlayer = player;
            } 
        }
        setter.target = closestPlayer.transform;
    }

    bool amStunned;

    void SwipeCheck(){
        //Attack if within range
        if(activePlayers.Count == 0){
            return;
        }
        if(setter.target){
            if(Vector3.Distance(setter.target.position,firingPoint.position) < range & !attack_cd & !amStunned){
                Swipe();
            }
        }
    }
    #endregion

    #region Attack
    bool attack_cd = false;
    

    [SerializeField] Transform firingPoint;
    [SerializeField] GameObject scythe;
    [SerializeField] float range;
    

    void Swipe(){
        StartCoroutine(SwipeCD());
        //Play attack animation
        scythe.SetActive(true);
        //Attack in a circle around firing point
        Collider2D[] playersHit = Physics2D.OverlapCircleAll(firingPoint.position, 0.8f*range, LayerMask.GetMask("Player"));
        for( int i=0; i < playersHit.Length; i++){
            //Damage player collider, excluding additional trigger colliders
            if(playersHit[i].tag == "Player" & !playersHit[i].isTrigger){
                playersHit[i].GetComponent<PlayerStats>().DamageSelf();
            }
        }
    }
    void OnDrawGizmosSelected() {
        //Gizmos for testing
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firingPoint.position, 0.8f*range);
    }

    IEnumerator SwipeCD(){
        //Cooldown Function
        attack_cd = true;
        yield return new WaitForSeconds(1f);
        attack_cd = false;
    }
    #endregion

    #region Stun and Die
    [SerializeField] GameObject particlesystem,iceCube;
    
    public void Stun(){
        if(!amStunned){
            //Disable pathfinding
            aiPath.enabled = false;
            setter.enabled = false;
            amStunned = true;
            aiPath = GetComponent<AIPath>();
            aiPath.canMove = false;
            //Animation
            iceCube.SetActive(true);
            audioManager.Play("freeze");
            particlesystem.SetActive(false);
            rb.freezeRotation = false;
            rb.gravityScale = 5;
            //Invoke enemy die
            Invoke("Die", 10f);
        }
        else{
            //If stunned already, enemy die
            Die();
        }
    }

    public void Die(){
        CancelInvoke("Die");
        gameManager.activeEnemies.Remove(gameObject);
        audioManager.Play("smash");
        DestroySelf();
    }
    
    void DestroySelf(){
        gameManager.spawnedUnits -=1;
        Destroy(gameObject);
    }
    #endregion
    
}
