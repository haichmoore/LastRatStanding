using UnityEngine;

public class Aim_Controller : MonoBehaviour
{
    //Controller pointing gameobject towards mouse pointer
    
    Vector3 mousePos;

    void Update(){
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    void FixedUpdate() {
        Point_MC(mousePos);
    }
    void Point_MC(Vector3 mousePos){
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }
}
