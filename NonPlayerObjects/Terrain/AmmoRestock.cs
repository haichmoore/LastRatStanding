using UnityEngine;

public class AmmoRestock : MonoBehaviour
{
    //Script attached to instantiated ammorestock prefabs

    GameManager gameManager;
    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();  
        gameManager.activeAmmo.Add(gameObject);gameManager.activeAmmoPostions.Add(transform.position);
    }

    [SerializeField] int ammo = 1;
    private void OnTriggerEnter2D(Collider2D other) {
        PlayerSpecials player = other.GetComponent<PlayerSpecials>();
        if(player){
            //Restore Ammo & destroy self if player collides
            player.AddAmmo(ammo);
            gameManager.SpawnAnotherAmmo();
            gameManager.activeAmmo.Remove(gameObject); gameManager.activeAmmoPostions.Remove(transform.position);
            FindObjectOfType<AudioManager>().Play("ammo");
            Destroy(gameObject);
        }
        
    }
}
