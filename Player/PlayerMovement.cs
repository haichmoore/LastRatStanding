using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    ContactFilter2D groundFilter = new ContactFilter2D();
    
    [SerializeField] Rigidbody2D rb;

    public float xVelocity,yVelocity;
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        InitialSetup();
    }


    void Update()
    {
        //Input Calculations
        CanJumpCheck();
        SetPlayerInputs();
    }   
    void FixedUpdate() {
        
        //Physics Calculations
        ApexCheck();
        XMovement();
        FakeGravity();
        Jump();

        //MoveCharacter
        SpecialModifiers();
        MoveCharacter();

    }

    #region Setup
    

    void InitialSetup(){
        groundFilter.SetLayerMask(JumpableLayers());
        groundFilter.useTriggers = true;
        springPower = 0;
    }

    #endregion

    #region Inputs
    float xInput;

    [SerializeField] bool jumpPressed, jumpReleased;
    [SerializeField] bool leftOnly, rightOnly;

    public bool blastAwayActive, amDead, acceptingAnyInputs = true;
    
    void SetPlayerInputs(){
        if(!GetComponent<PlayerStats>().paused){
            xInput=Input.GetAxisRaw("Horizontal");
            if(rightOnly){
                xInput = Mathf.Max(xInput,0);
            }
            else if(leftOnly){
                xInput = Mathf.Min(xInput,0);
            }
            if(Input.GetButtonDown("Jump")){
                if(canJump){
                    jumpPressed = true;
                }
                else{
                    StartCoroutine(BufferJump());
                }
                jumpReleased = false;
                
            }
            if(Input.GetButtonUp("Jump")){
                jumpReleased = true;
            }
        }
        
    }
    #endregion

    #region Accepting Inputs

    public bool justJumped;
    public IEnumerator LimitHorizontalInputs(bool right = true, float time = 0.25f){
        if(right){
            rightOnly = true;
        }else{
            leftOnly = true;
        }
        yield return new WaitForSeconds(time);
        rightOnly = false; leftOnly=false;
    }

    public IEnumerator JustJumped(){
        justJumped = true;
        yield return new WaitForSeconds(0.2f);
        justJumped = false;
    }
    #endregion

    #region Collider Checks
    public LayerMask JumpableLayers(){
        LayerMask jumpLayers = LayerMask.GetMask("JumpableFloor", "MovingPlatform");
        return jumpLayers;
    }

    //left, right and down are Collider2D's that tell this script if the player is on the floor or wall 
    [SerializeField] BoxCollider2D left,right,down;
    [SerializeField] bool canJump = true, onWall = false, usedWallJump, onWallLeft, onWallRight;

    public bool onFloor = false;

    void CanJumpCheck(){
        //Check if on wall / floor
        OnWallCheck();
        OnFloorCheck();
        if(onWall || onFloor){
            canJump = true;
        }
        else{
            canJump = false;
        }
    }
    void OnWallCheck(){
        if(CheckCollider(left)){
            onWall = true;
            onWallLeft = true;
        }
        else if(CheckCollider(right)){
            onWall = true;
            onWallRight = true;
        }
        else{
            onWall = false;
            onWallLeft = false;
            onWallRight = false;
        }
        
    }
    void OnFloorCheck(){
        onFloor = CheckCollider(down);
    }
    bool CheckCollider(Collider2D collCheck){
        return collCheck.IsTouchingLayers(JumpableLayers());
    }
    #endregion

    #region Apex Checks
    
    [SerializeField] float apexBonus, apexCheck;

    void ApexCheck(){
        //check if I am at the apex of a jump, and if I am allow extra x movement
        if(!onFloor){
            apexCheck = Mathf.InverseLerp(yMax,0,Mathf.Abs(yVelocity));
        }
        else{
            apexCheck = 0;
        } 
    }



    #endregion
    
    #region Define Movement Values
    [SerializeField] float xAcceleration,xDecceleration, xMax;

    void XMovement(){
        //Set xVelocity for player
        if(xInput != 0){
            xVelocity += xAcceleration * Time.deltaTime * xInput;
            //clamp absolute value below xMax
            xVelocity = Mathf.Clamp(xVelocity,-xMax,xMax);
            //add apex bonus
            xVelocity += apexCheck*apexBonus* xInput;

        }
        else if(xVelocity != 0){
            //If no input and current velocity is > 0 we want to slow character down
            xVelocity = Mathf.MoveTowards(xVelocity, 0, xDecceleration * Time.deltaTime);
        }
    }

    [SerializeField] private float yMax = 20, wallPush = 20;
    [SerializeField] bool jumpEnded, jumpBuffered;

    void Jump(){
        //Set yVelocity for player
        if((jumpBuffered ||jumpPressed )&& canJump){
            StartCoroutine(JustJumped());
            jumpPressed = false;
            jumpBuffered = false;
            yVelocity = yMax;
            jumpEnded = false;
            canJump =  false;

            //Bonuses provided if player is on wall:
            if(!onFloor & onWallRight & xInput >=0){
                xVelocity -= wallPush;
                yVelocity *= 0.75f;
            }
            else if(!onFloor & onWallLeft & xInput <=0){
                xVelocity += wallPush;
                yVelocity *= 0.75f;
            }
        }
        //End jump early if player releases jump:
        if(jumpReleased & !onFloor & !jumpEnded & yVelocity > 0){
            jumpReleased = false;
            jumpEnded = true;
        }
    }

    IEnumerator BufferJump(float time = 0.1f){
        //seeks to allow player to buffer a jump 0.2s before landing:
        jumpBuffered = true;
        yield return new WaitForSeconds(time);
        jumpBuffered = false;

    }
    #endregion

    #region Fake Gravity
    [SerializeField] float terminalVelocity =  -10, jumpEndedScaler = 1.5f, fallMax = -10;
    [SerializeField] float fallVelocity;

    void FakeGravity(){
        //Apply gravity when player is not on a platform
        if(onFloor & !justJumped){
            yVelocity = 0;
        }
        else{
            fallVelocity = jumpEnded & yVelocity > 0 ? fallMax * jumpEndedScaler : fallMax;

            //Add this to yvelocity
            yVelocity += fallVelocity * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);
        }
    }
    #endregion

    #region Specials
    public float springPower, damageBoost;

    void SpecialModifiers(){
        //Checks if player has been affected by any special forces:
        //Spring
        if(springPower != 0 & !justJumped & yVelocity <=0){
            Spring();
        }
        //Perk effect
        if(damageBoost != 0){
            Invoke("ResetDamageBoost",0.5f);
            xVelocity *= damageBoost;
        }
        //In air tube
        if(inAir){
            yVelocity = yMax/2;
        }
        //Being pushed
        if(pushAway){
            PushAway(pushSource);
        }
    }
    void Spring(){
        //Boosted by spring
        StartCoroutine(JustJumped());
        yVelocity = springPower;
        springPower = 0;
    }

    void ResetDamageBoost(){
        damageBoost = 0;
    }

    void AcceptingAnyInputs(){
        acceptingAnyInputs = true;
    }

    void PushAway(Vector2 pushSource){
        //If pushed away by terrain, push away from terrain & restrict player input for 0.25s
        acceptingAnyInputs = false;
        Vector2 pushVelocity = ((Vector2)transform.position - pushSource).normalized;
        yVelocity = yMax * pushVelocity.y;
        GetComponent<PlayerStats>().DamageSelf();
        Invoke("AcceptingAnyInputs", 0.1f);
    }
    #endregion

    #region Move Character
    void MoveCharacter(){
        rb.velocity = new Vector2(xVelocity,yVelocity);
    }
    
    #endregion

    #region Collision Manager
    //Collider checks if damaged / in air tube:
    [SerializeField] bool inAir, pushAway;
    Vector2 pushSource;
    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag == "AirTube"){
            inAir = true;
        }
        if(other.tag == "Damage"){
            pushAway = true;
            pushSource = other.transform.position;
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.tag == "AirTube"){
            inAir = false;
        }
        if(other.tag == "Damage"){
            pushAway = false;
        }
    }
    #endregion
  
}
