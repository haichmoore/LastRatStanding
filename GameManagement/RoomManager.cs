using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public List<Spawner> roomSpawners;
    public List<Transform> roomAmmoSpawns;
    public BoxCollider2D roomCollider;
    [SerializeField] GameManager gameManager;

    void Start(){
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); 
        roomCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            gameManager.EnterRoom(this);
            roomCollider.enabled = false;
        }
        
    }

}
