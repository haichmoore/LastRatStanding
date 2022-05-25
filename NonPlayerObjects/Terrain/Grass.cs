using UnityEngine;

public class Grass : MonoBehaviour
{
    //Attached to grass objects causing wiggle
    Animator an;
    void Start()
    {
        an = gameObject.GetComponent<Animator>();
    }
    void OnTriggerEnter2D(Collider2D other) {
        //Wiggle if run through
        if(other.tag == "Player"){
            an.Play("Base Layer.Wiggle");
        }
        
    }
}
