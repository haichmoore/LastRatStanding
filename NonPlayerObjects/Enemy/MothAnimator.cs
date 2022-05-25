using UnityEngine;

public class MothAnimator : MonoBehaviour
{
    Transform target;
    
    void Start()
    {
        target= gameObject.GetComponentInParent<MothMovement>().setter.target;
    }    
    void Update()
    {
        PointCharacter();
    }

    #region PointCharacter
    bool facingRight = false;

    void PointCharacter(){
        if(target){
            if(target.position.x > transform.position.x & !facingRight){
                Flip();
            }
            if(target.position.x < transform.position.x & facingRight){
                Flip();
            }
        }
    }

    void Flip(){
        facingRight = !facingRight;
        gameObject.transform.localScale = !facingRight ? new Vector3(1,1,1): new Vector3(-1,1,1);
        if(GetComponentInChildren<ParticleSystem>()){
            gameObject.GetComponentInChildren<ParticleSystem>().transform.localEulerAngles = !facingRight ? new Vector3(0,0,-45): new Vector3(0,0,135);
        }
    }
    #endregion
}
