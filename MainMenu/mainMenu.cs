using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayGame(int i){
        SceneManager.LoadScene(i);
    }
}
