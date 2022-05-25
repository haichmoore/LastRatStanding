using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    #region Spawn an Enemy
    [SerializeField] GameObject[] enemies; 
    
    GameManager gameManager;
    void Awake() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();   
    }
    public void Spawn(int enemy_number){
        Instantiate(enemies[enemy_number],transform.position, Quaternion.identity);
        gameManager.spawnedUnits +=1;
        gameManager.unspawnedUnits -=1;
    }
    #endregion

}
