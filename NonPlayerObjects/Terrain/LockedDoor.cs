using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    //Attached to door terrain objects. Manages cost and animations
    public int cost;
    Animator an;
    void Update()
    {  
        an = GetComponent<Animator>();
    }

    public void Open(){
        an.Play("Base Layer.openDoor");
        Invoke("HideSelf", 0.5f);
    }
    public void HideSelf(){
        //Disable object and update enemy pathfinding
        gameObject.SetActive(false);
        AstarPath.active.Scan();
    }
    public void UnHideSelf(){
        gameObject.SetActive(true);
    }

}
