using UnityEngine;

public class Scythe : MonoBehaviour
{
    AudioManager audioManager;
    void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
    }
    void OnEnable() {
        //Play attack animation & sound
        audioManager.Play("attack");
        Invoke("DisableSelf", 0.15f);
    }
    void DisableSelf(){
        gameObject.SetActive(false);
    }
}
