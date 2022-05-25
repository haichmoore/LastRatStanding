
using UnityEngine;
using TMPro;

public class GainPoints : MonoBehaviour
{
    //Gain points animation controller 
    public int points= 100;
    void Start()
    {
        SetScore(points);
        Invoke("DestroySelf", 0.3f);
    }
    void SetScore(int score){
        GetComponent<TextMeshPro>().text = "+" + score.ToString();
    }

    void DestroySelf(){
        Destroy(gameObject);
    }
}
