using UnityEngine;

public class Spring : MonoBehaviour
{
    //Attached to sprign object

    [SerializeField] float springPower;
    Animator an;
    void Start()
    {
        an = gameObject.GetComponent<Animator>();
    }


    void OnCollisionEnter2D(Collision2D other) {
        //Spring player & animate
        PlayerMovement pm = other.gameObject.GetComponent<PlayerMovement>();
        if(pm & other.transform.position.y > transform.position.y){
            pm.springPower = springPower;
            an.Play("Base Layer.spring");
        }
    }
}
