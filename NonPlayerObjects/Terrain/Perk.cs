
using UnityEngine;

public class Perk : MonoBehaviour
{
    //Attached to all perk objects 
    
    public int cost;
    public bool isActive;
    public string perkName;
    [SerializeField] GameObject ps;
    public void ActivatePerk(bool activate=true){
        isActive = activate;
        ps.SetActive(activate);
        
    }
}
