using System.Collections;
using UnityEngine;
using TMPro;


public class PlayerStats : MonoBehaviour
{
    //Controls player points, hp and pausemenu
    
    GameManager gameManager;
    AudioManager audioManager;

    [SerializeField] int currentHealth = 3,maxHealth = 3,currentPoints, lastPoints ,lastHealth;
    
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); 
        audioManager = FindObjectOfType<AudioManager>(); 
    }

    void Update()
    {
        if(pressedEsc){
            PauseGame();
        }
        SetPlayerInputs();
        HealthManage();
        UpdatePoints();
    }

    #region CollectInputs
    bool pressedE,pressedEsc;
    void SetPlayerInputs(){
        if(Input.GetKeyDown(KeyCode.E)){
            pressedE = true;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            pressedEsc = true;
        }
    }
    #endregion

    #region Health
    bool healing;
    [SerializeField] TextMeshProUGUI currentHealthtext;
    
    void HealthManage(){
        if(!healing & currentHealth < maxHealth){
            healing = true;
            Invoke("HealSelf", 3);
        }
        if(lastHealth != currentHealth){
            currentHealthtext.text = currentHealth.ToString();
            lastHealth = currentHealth;
        }
    }

    bool fightAndFlight;

    public void DamageSelf(){
        currentHealth -=1;
        //Perk effect
        if(fightAndFlight){
            GetComponent<PlayerMovement>().damageBoost = 1.25f;
        }
        //Reset healing
        if(currentHealth >0){
            healing = false;
            CancelInvoke("HealSelf");
        }
        else{
            PlayerDie();
        }
        //Animation
        audioManager.Play("ouch", 1.5f,2f);        
    }

    public void HealSelf(){
        currentHealth = Mathf.Min(currentHealth+1, maxHealth);
        healing = false;
    }

    bool damageOverTime;
    
    IEnumerator DOT(){
        // Damage Over Time (DOT)
        while(damageOverTime){
            yield return new WaitForSeconds(2);
            if(damageOverTime){
                DamageSelf();
            }
        }
    }  

    public void PlayerDie(){
        GetComponent<PlayerMovement>().amDead = true;
        gameManager.PlayerDie(gameObject);
        //Animation
        gameObject.GetComponentInChildren<PlayerAnimation>().PlayerDie();
        
    }
    public void Revive(){
        currentHealth = maxHealth;
        GetComponent<PlayerMovement>().amDead = false;
        currentPoints = Mathf.Max(0,currentPoints-1000);
        maxHealth = 3;
        //Reset perks
        GetComponent<PlayerMovement>().blastAwayActive = false;
        GetComponent<PlayerMovement>().acceptingAnyInputs = true;
        GetComponent<PlayerSpecials>().tripleStrike = false;
        GetComponent<PlayerSpecials>().marsupium = false;
        GetComponent<PlayerSpecials>().maxDaggerAmmo = 3;
        GetComponent<PlayerSpecials>().BlastAwayUnlocked(false);
        fightAndFlight = false;
        
    }

    [SerializeField] Transform[] respawnPoints;
    
    public void Respawn(bool inloop = false){
        //Inloop bool allows the respawn function to be used in restart function for gamemanager without throwing an error
        // (allows iteration over deadPlayers)
        Revive();
        if(!inloop){
            gameManager.activePlayers.Add(gameObject);
            gameManager.deadPlayers.Remove(gameObject);
        }
        gameObject.GetComponent<PlayerSpecials>().daggerAmmo =gameObject.GetComponent<PlayerSpecials>().startingDaggerAmmo;
        currentPoints = 0;
        int rnd = Random.Range(0, respawnPoints.Length);
        gameObject.transform.position = respawnPoints[rnd].position;
    }
    #endregion

    #region Points
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] GameObject gainPoints;

    void UpdatePoints(){
        if(lastPoints != currentPoints){
            lastPoints = currentPoints;
            pointsText.text = currentPoints.ToString();
        }
    }

    public void AddPoints(int toAdd = 100){
        GameObject points = Instantiate(gainPoints, transform.position, Quaternion.identity, transform);
        points.GetComponent<GainPoints>().points = toAdd;
        currentPoints += toAdd;
    }

    #endregion

    #region BuyingWithPoints

    [SerializeField] GameObject buyPrompt;
    [SerializeField] TextMeshProUGUI buyText;

    public void BuyPrompt(int cost = 0, string isPerkorPower = null){
        if(isPerkorPower == null){
            buyText.text = "Open - " + cost.ToString();
        }
        else if(isPerkorPower == "Power"){
            buyText.text = "Ignite the Flame!";
        }
        else{
            if(cost == -1){
                //power is not yet on
                buyText.text = "The Gods haven't been called...";
            }
            else{
                buyText.text = "Buy " + isPerkorPower + " - "+ cost.ToString();
            }
        }
        buyPrompt.SetActive(true);
    }
    public void HideBuyPrompt(){
        buyPrompt.SetActive(false);
    }

    bool buyClosestObject,inDOT, inPerk, inDoor, inPower;

    void OnTriggerEnter2D(Collider2D other) {
        //Check if within buyable area
        Perk perk = other.GetComponent<Perk>();
        LockedDoor door = other.GetComponent<LockedDoor>();
        pressedE = false;
        if(perk & !inPerk){
            inPerk = true;
            if(perk.isActive){
                BuyPrompt(perk.cost, perk.perkName);
            }else{
                BuyPrompt(-1, perk.perkName);
            }
            
        }
        else if(door & !inDoor){
            inDoor = true;
            BuyPrompt(door.cost);
        }

        //Check if within Power or DOT area
        else if(other.tag == "Power" & !inPower){
            inPower = true;
            if(perk || other.tag == "Power"){
                BuyPrompt(0, "Power");
            }
            
        }
        else if(other.tag == "DOT" & !inDOT){
            inDOT = true;
            damageOverTime = true;
            StartCoroutine(DOT());
        }
        
    }
    void OnTriggerExit2D(Collider2D other) {
        //Hide buy prompts when exiting collider
        Perk perk = other.GetComponent<Perk>();
        LockedDoor door = other.GetComponent<LockedDoor>();
        if(door || perk || other.tag == "Power"){
            inPower = false;
            inPerk = false;
            inDoor = false;
            HideBuyPrompt();
        }
        //Stop DOT when exiting collider
        if(other.tag == "DOT"){
            inDOT = false;
            damageOverTime = false;
            StopCoroutine(DOT());
            
        }
        
    }
    void OnTriggerStay2D(Collider2D other) {
        //Check if player buys buyable
        LockedDoor door = other.GetComponent<LockedDoor>();
        Perk perk = other.GetComponent<Perk>();
        //Open door:
        if(door){
            if(pressedE & currentPoints >= door.cost){
                pressedE = false;
                currentPoints-=door.cost;                
                door.Open();
            }

        }
        else if(perk){
            //Activate relevant perk:
            if(pressedE & perk.isActive){
                pressedE = false;
                switch (perk.perkName){
                    case "ScratchProof": //ScratchProof
                    if(maxHealth < 5 & currentPoints > perk.cost){
                        currentPoints -= perk.cost;
                        maxHealth = 5;
                        GetComponentInChildren<PlayerAnimation>().BuyPerk(perk.perkName);
                    }
                    
                    break;
                    case "BlastAway": //blastaway
                    if(!GetComponent<PlayerSpecials>().blastaway & currentPoints > perk.cost){
                        currentPoints -= perk.cost;
                        GetComponent<PlayerSpecials>().BlastAwayUnlocked();
                        GetComponentInChildren<PlayerAnimation>().BuyPerk(perk.perkName);
                    }
                    break;
                    case "3 Point Strike": //3 point strike
                    if(!GetComponent<PlayerSpecials>().tripleStrike & currentPoints > perk.cost){
                        currentPoints -= perk.cost;
                        GetComponent<PlayerSpecials>().tripleStrike = true;
                        GetComponentInChildren<PlayerAnimation>().BuyPerk(perk.perkName);
                    }
                    break;
                    case "Marsupium": //marsupium
                    if(!GetComponent<PlayerSpecials>().marsupium & currentPoints > perk.cost){
                        currentPoints -= perk.cost;
                        GetComponent<PlayerSpecials>().marsupium = true;
                        GetComponent<PlayerSpecials>().maxDaggerAmmo = 5;
                        GetComponentInChildren<PlayerAnimation>().BuyPerk(perk.perkName);
                    }
                    break;
                    case "Fight and Flight": //FightandFlight
                    if(!fightAndFlight & currentPoints > perk.cost){
                        currentPoints -= perk.cost;
                        fightAndFlight = true;
                        GetComponentInChildren<PlayerAnimation>().BuyPerk(perk.perkName);
                    }
                    break;
                }
            }
        }
        //Activate Power:
        else if(other.tag == "Power"){
            if(pressedE & !gameManager.powerOn){
                pressedE = false;
                gameManager.TurnOnPower();
            }
        }
        
    }
    #endregion

    #region PauseGame
    //Region managing player pause functionality: 
    [SerializeField] GameObject pauseMenu;
    
    public bool paused = false;

    public void PauseGame(bool menu = true){
        pressedEsc = false;
        if(paused){
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }else{
            Time.timeScale = 0;
            if(menu){
                pauseMenu.SetActive(true);
            }
            
        }
        paused = !paused;
        
    }
    public void UnPauseGame(){
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        paused = false;
    }
    #endregion
}
