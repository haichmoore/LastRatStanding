using UnityEngine;
using TMPro;

public class PlayerSpecials : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager;
    int daggerAmmoLast;
    PlayerAnimation playeranim;

    [SerializeField] TextMeshProUGUI currentAmmoText;
    
    public int daggerAmmo, startingDaggerAmmo,maxDaggerAmmo;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>(); 
        audioManager = FindObjectOfType<AudioManager>();
        daggerAmmo = startingDaggerAmmo;
        playeranim = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        SetPlayerInputs();
        UpdateDaggerInfo();
    }

    void FixedUpdate(){
        if(fired){
            Fire();
        }else if(fired2){
            Fire2();
        }
    }
    #region Inputs
    float xInput;
    bool fired, fired2;
    
    void SetPlayerInputs(){
        if(gameManager.startedGame){
            if(!GetComponent<PlayerStats>().paused){
                if(Input.GetButtonDown("Fire1") & !f1CD & daggerAmmo > 0){
                fired = true;
                }
                if(blastaway & Input.GetButtonDown("Fire2") & !f2CD){
                    fired2 = true;
                }
            }
        }
        
    }
    #endregion
    #region Cooldowns & Ammo Management

    void UpdateDaggerInfo(){
        if(daggerAmmo != daggerAmmoLast){
            daggerAmmoLast = daggerAmmo;
            currentAmmoText.text = daggerAmmo.ToString();
        }
    }

    public bool marsupium, blastaway;

    public void AddAmmo(int n = 1){
        //perk effect
        if(marsupium){
            daggerAmmo=maxDaggerAmmo;
            return;
        }
        daggerAmmo= Mathf.Min(maxDaggerAmmo,daggerAmmo+n);
    }

    [SerializeField] bool f1CD, f2CD;

    public GameObject f2Indicator;

    void RefreshF1CD(){
        f1CD = false;
    }
    public void RefreshF2CD(){
        if(blastaway){
            f2CD = false;
            f2Indicator.SetActive(true);
        }
        
    }

    public void BlastAwayUnlocked(bool active = true){
        blastaway = active;
        f2Indicator.SetActive(active);
    }
    #endregion

    #region specials
    [SerializeField] Transform firingpoint;

    public bool tripleStrike;
    

    void Fire(){
        fired = false;
        daggerAmmo = Mathf.Max(0, daggerAmmo-1);
        f1CD = true; Invoke("RefreshF1CD", 0.15f);
        ThrowDagger();
        //perk effect
        if(tripleStrike){
            Invoke("ThrowDagger", 0.1f);
            Invoke("ThrowDagger", 0.2f);
        }
        
    }
    
    [SerializeField] GameObject dagger;

    void ThrowDagger(){
        audioManager.Play("throwdagger");
        GameObject daggerThrown = Instantiate(dagger, firingpoint.position, firingpoint.rotation);
        daggerThrown.GetComponent<ThrowingKnife>().castingPlayer = gameObject;
    }

    [SerializeField] GameObject blast;

    void Fire2(){
        fired2 = false;
        f2CD = true; f2Indicator.SetActive(false);
        GameObject blastBlasted = Instantiate(blast, firingpoint.position, firingpoint.rotation);
        blastBlasted.GetComponent<BlastAway>().castingPlayer = gameObject;
    }
    #endregion
}
