using UnityEngine;

public class funbox : MonoBehaviour
{
    //Script atttached to funbox prefab - randomising color each game
    void Start()
    {
        int rnd = Random.Range(0,5);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(rnd == 0){ sr.color = Color.red;}
        else if(rnd == 1){ sr.color = Color.blue;}
        else if(rnd == 2){ sr.color = Color.green;}
        else if(rnd == 3){ sr.color = Color.cyan;}
        else if(rnd == 4){ sr.color = Color.magenta;}
        else if(rnd == 5){ sr.color = Color.yellow;}


    }

}
