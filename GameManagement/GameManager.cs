using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
//using System;
public class GameManager : MonoBehaviour
{
    /*
    GameManager script which controls spawning of enemies, power and round progression.
    Any variables marked public are used in adjacent scripts.
    */

    #region Awake and Update

    void Awake() {
        InitialSetup();
    }

    void Update()
    {
        EndRoundCheck();
    }

    #endregion

    #region Game Progression

    AudioManager audioManager;
    int currentRound,lastRound;

    [SerializeField] TextMeshProUGUI currentRoundText;

    public int unspawnedUnits, spawnedUnits;
    public List<GameObject> activePlayers, downedPlayers, deadPlayers, allPlayers;

    void InitialSetup(){
        //  Set current round, find players and audiomanager
        currentRound = 0;
        unspawnedUnits = -1;
        spawnedUnits = -1;
        allPlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        audioManager = FindObjectOfType<AudioManager>();
    }

    bool waitingBetweenRounds= true;

    void EndRoundCheck(){
        //  Check if currentround is over, if so end the round
        if(unspawnedUnits <=0 & spawnedUnits <=0 & !waitingBetweenRounds & activeEnemies.Count == 0){
            // Round is Over
            EndRound();
        }
        if(lastRound != currentRound){
            lastRound = currentRound;
            currentRoundText.text = currentRound.ToString();
        }
    }

    public bool startedGame = false;

    public void StartGame(){
        // Start game
        waitingBetweenRounds = false;
        startedGame = true;
        // Reset AI
        AstarPath.active.Scan();
        spawnedUnits = 0; 
        unspawnedUnits = 0;
        // Reset map
        SpawnAmmo();
        // Reset players
        foreach(GameObject player in allPlayers){
            activePlayers.Add(player);
            player.GetComponent<PlayerStats>().Respawn(true);
        }
    }

    [SerializeField] GameObject GameOverScreen;

    public void GameOver(){
        GameOverScreen.SetActive(true);
        startedGame = false;
        foreach(GameObject player in allPlayers){
            player.GetComponent<PlayerStats>().PauseGame(false);
        }

    }

    [SerializeField] GameObject readyMenu;
    
    [SerializeField] LockedDoor[] doors;

    public List<GameObject> activeEnemies;
    
    public void Restart(){
        //reset game values
        currentRound = 0; lastRound = 0;
        unspawnedUnits = -1;
        spawnedUnits = -1;
        readyMenu.SetActive(true);
        waitingBetweenRounds = true;
        startedGame = false;

        //Reset map
        foreach(LockedDoor door in doors){
            door.UnHideSelf();
        }
        ResetAmmo();
        ResetPower();
        ResetRooms();

        //Reset players and enemies
        foreach(GameObject player in deadPlayers){
            player.GetComponent<PlayerStats>().Respawn(true);
            player.GetComponent<PlayerStats>().UnPauseGame();
        }
        deadPlayers.Clear();
        foreach(GameObject enemy in activeEnemies){
            Destroy(enemy);
        }
        activeEnemies.Clear();
    }

    #endregion

    #region Round Management

    void StartRound(int n){
        //Reset player ammo
        foreach(GameObject player in activePlayers){
            player.GetComponent<PlayerSpecials>().daggerAmmo = player.GetComponent<PlayerSpecials>().maxDaggerAmmo;
            player.GetComponent<PlayerSpecials>().RefreshF2CD();
        }
        //Reset round values
        unspawnedUnits = 2*n;
        spawnedUnits = 0;
        waitingBetweenRounds = false;

        //Begin round
        audioManager.Play("roundstart");
        InvokeRepeating("SpawnHordeUnits", 0, 2f);
        
    }
    void SpawnHordeUnits(){
        //Spawn hordes of enemies at a time if required, else cancel
        if(unspawnedUnits > 0 & spawnedUnits <= 25){
            SpawnUnits();
        }
        if(unspawnedUnits <=0){
            CancelInvoke("SpawnHordeUnits");
        }
    }

    [SerializeField] List<Spawner> availableSpawners;
    [SerializeField] int maxEnemiesActive = 25;
    void SpawnUnits(){
        ShuffleSpawners();
    
        //Spawn required amount of enemies based on # of unspawned enemies
        if( unspawnedUnits < 5){
            availableSpawners[0].Spawn(0);
        }else if( unspawnedUnits>=5){
            for(int i=0; i< 2; i++){
                if(unspawnedUnits <= 0 || spawnedUnits > maxEnemiesActive){
                    return;
                }
                availableSpawners[i].Spawn(0);
            }
        }
        else if( unspawnedUnits >= 10){
            for(int i=0; i< 3; i++){
                if(unspawnedUnits <= 0 || spawnedUnits > maxEnemiesActive){
                    return;
                }
                availableSpawners[i].Spawn(0);
            }

        }else if( unspawnedUnits >= 15){
            for(int i=0; i< 4; i++){
                if(unspawnedUnits <= 0 || spawnedUnits > maxEnemiesActive){
                    return;
                }
                availableSpawners[i].Spawn(0);
            }
        }
    }

    Spawner spawnerTemp;

    void ShuffleSpawners(){
        //Randomise spawn location
        for (int i = 0; i < availableSpawners.Count; i++) {
            int rnd = Random.Range(0, availableSpawners.Count);
            spawnerTemp = availableSpawners[rnd];
            availableSpawners[rnd] = availableSpawners[i];
            availableSpawners[i] = spawnerTemp;
        }
    }

    [SerializeField] int roundCD, roundMaxCD;
    [SerializeField] TextMeshProUGUI roundCDText;

    void EndRound(){
        //Reset round values
        waitingBetweenRounds = true;
        currentRound +=1;
        roundCD = roundMaxCD;
        StartCoroutine(BeginNextRound()); 
    }

    IEnumerator BeginNextRound(){
        //Show and set CD before next round start
        roundCDText.enabled = true;
        while(roundCD > 0){
            roundCDText.text = roundCD.ToString();
            yield return new WaitForSeconds(1);
            roundCD-=1;
        }
        roundCDText.enabled = false;
        StartRound(currentRound);
    }   
    #endregion
    
    
    #region Bonus and Specials Manager

    [SerializeField] float timeBetweenAmmoSpawns = 0.5f;
    [SerializeField] List<Transform> ammoSpawnPoints;
    [SerializeField] GameObject Ammo;

    public List<GameObject> activeAmmo;
    public List<Vector3> activeAmmoPostions;

    public void SpawnAnotherAmmo(){
        Invoke("SpawnAmmo", timeBetweenAmmoSpawns);
    }
    void SpawnAmmo(){
        //Spawn ammo in random location if required
        if(activeAmmoPostions.Count < allPlayers.Count){
            int rnd = Random.Range(0, ammoSpawnPoints.Count);
            while(activeAmmoPostions.Contains(ammoSpawnPoints[rnd].position)){
            rnd = Random.Range(0, ammoSpawnPoints.Count);
            }
            Instantiate(Ammo,ammoSpawnPoints[rnd].position, Quaternion.identity);
        }
        
    }

    void ResetAmmo(){
        //Destroys existing ammospawns
        foreach(GameObject ammo in activeAmmo){
            Destroy(ammo);
        }
        activeAmmo.Clear(); activeAmmoPostions.Clear();
    }
    #endregion

    #region RoomManager

    RoomManager spawnRoom;

    public void EnterRoom(RoomManager room){
        //Add entered room's ammo spawn points and spawners to active lists
        ammoSpawnPoints.AddRange(room.roomAmmoSpawns);
        availableSpawners.AddRange(room.roomSpawners);
    }

    [SerializeField] List<RoomManager> RoomList;

    void ResetRooms(){
        //Reset ammo and spawner points
        ammoSpawnPoints.Clear();
        availableSpawners.Clear();
        foreach(RoomManager room in RoomList){
            room.roomCollider.enabled = true;
        }
    }
    #endregion

    #region Power & Perks Manager

    [SerializeField] Perk[] perks;
    [SerializeField] GameObject mainPowerDoor, PowerTorch;

    public bool powerOn = false;

    public void TurnOnPower(){
        powerOn = true;
        OpenMainDoor();
        foreach(Perk perk in perks){
            perk.ActivatePerk();
        }
        //Animation
        audioManager.Play("poweron");
        PowerTorch.GetComponent<Animator>().Play("Base Layer.turn on");
        
    }
    public void ResetPower(){
        powerOn = false;
        foreach(Perk perk in perks){
            perk.ActivatePerk(false);
        }
        //Animation
        OpenMainDoor(false);
        PowerTorch.GetComponent<Animator>().Play("Base Layer.power_idle");
    }
    void OpenMainDoor(bool open = true){
        if(open){
            mainPowerDoor.GetComponent<Animator>().Play("Base Layer.openUp");
            Invoke("AIScan", 1f);
        }
        else{
            mainPowerDoor.GetComponent<Animator>().Play("Base Layer.idle");
        }
    }
    void AIScan(){
        //Update enemy pathgfinding
        AstarPath.active.Scan();
    }
    #endregion


    #region Player
    public void PlayerDie(GameObject player){
        deadPlayers.Add(player);
        activePlayers.Remove(player); 
        if(activePlayers.Count == 0){
            GameOver();
        }
    }

    public Vector2 ClosestAmmoPickup(Vector2 playerPos){
        //Find ammo location closest to player

        Vector2 closestPos = 1000*Vector2.up;
        foreach(Vector3 pos in activeAmmoPostions){
            if(((Vector2)pos - playerPos).magnitude < (closestPos-playerPos).magnitude){
                closestPos = pos;
            }
        }
        return closestPos;
    }
    #endregion
}
