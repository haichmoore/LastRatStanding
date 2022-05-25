using System.Collections;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Controls all animations for player character

    PlayerMovement movement;
    AudioManager audioManager;
    Animator an;
    GameManager gameManager;

    void Awake()
    {
        movement = gameObject.GetComponentInParent<PlayerMovement>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        an = gameObject.GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    bool pointAtAmmo = false;

    void Update()
    {
        if(movement.amDead){
            return;
        }
        SetPlayerInputs();
        AmmoPointer();
        PointCharacter();
        ninjaAnimator();
    }

    #region PlayerInputs
    void SetPlayerInputs(){
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            pointAtAmmo = true;
        }
    }
    #endregion

    #region Point Ammo and Character

    [SerializeField] GameObject pointer, toLookAt;
    void AmmoPointer(){
        if(pointAtAmmo){
            //Animate pointer towards closest available ammo
            pointAtAmmo = false;
            toLookAt.transform.position = gameManager.ClosestAmmoPickup(transform.position);
            if((Vector2)toLookAt.transform.position != 1000*Vector2.up){
                pointer.transform.rotation = Quaternion.LookRotation(Vector3.forward, toLookAt.transform.position - pointer.transform.position);
                pointer.GetComponent<Animator>().Play("Base Layer.pointer");
            }
            else{
                pointer.GetComponent<Animator>().Play("Base Layer.nospawn");
            }
        }
    }

    bool facingRight = true;

    void PointCharacter(){
        if(movement.onFloor || movement.justJumped){
            if(movement.xVelocity > 0 & !facingRight){
                Flip();
            }
            if(movement.xVelocity < 0 & facingRight){
                Flip();
            }
        }
    }
    public void Flip(){
        facingRight = !facingRight;
        gameObject.transform.localScale = facingRight ? new Vector3(1,1,1): new Vector3(-1,1,1);
    }
    #endregion PointCharacter

    #region Animation
    bool landed = true, running;
    int currentAnim =0;

    void ninjaAnimator(){
        //Play appropriate animation for player model
        if(movement.onFloor){
            if(!landed){
                StartCoroutine(Landing());
                an.Play("Base Layer.Land");
                currentAnim = 4;
            }
            else{
                if(movement.xVelocity == 0 & currentAnim !=0){
                    an.Play("Base Layer.Idle");
                    running =false;
                    audioManager.Stop("run");
                    currentAnim = 0;
                }
            }
            if(movement.xVelocity!=0){

                an.Play("Base Layer.Scamper");
                if(!running){
                    running =true;
                    audioManager.Play("run");
                }
                currentAnim = 1;
            }
        }
        else{
            landed = false;
            if(movement.justJumped & currentAnim !=2){
                audioManager.Play("jump", 0.6f,1.4f);
                running =false;
                audioManager.Stop("run");
                an.Play("Base Layer.Jump");
                currentAnim = 2;
            }
            else if(movement.yVelocity < 3 & currentAnim !=3){
                an.Play("Base Layer.Fall");
                running =false;
                audioManager.Stop("run");
                currentAnim = 3;
            }
        }
    }
    
    IEnumerator Landing(){
        yield return new WaitForSeconds(0.2f);
        landed = true;
    }
    
    public void PlayerDie(){
        LosePerks();
    }
    #endregion
    
    #region Perks
    [SerializeField] Animator[] perkUI;

    void LosePerks(){
        foreach(Animator anim in perkUI){
            anim.Play("Base Layer.idle");
        }
    }

    public void BuyPerk(string perk){
        //play perk animations
        audioManager.Play("perkgain");
        switch (perk){
                    case "ScratchProof":
                    perkUI[0].Play("Base Layer.unlockPerk");
                    break;
                    case "BlastAway":
                    perkUI[4].Play("Base Layer.unlockPerk");
                    break;
                    case "3 Point Strike":
                    perkUI[1].Play("Base Layer.unlockPerk");
                    break;
                    case "Marsupium":
                    perkUI[2].Play("Base Layer.unlockPerk");
                    break;
                    case "Fight and Flight":
                    perkUI[3].Play("Base Layer.unlockPerk");
                    break;
                }
    }
    #endregion
}
