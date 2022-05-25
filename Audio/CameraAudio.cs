using UnityEngine.UI;
using UnityEngine;

public class CameraAudio : MonoBehaviour
{
    float volume;
    [SerializeField] Slider soundSlider;

    void Start(){
        MasterVolume();
        if(!PlayerPrefs.HasKey("Volume")){
            PlayerPrefs.SetFloat("Volume",1);
        }
        soundSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    
    public void MasterVolume () {
        AudioListener.volume = 0.1f*soundSlider.value;
        PlayerPrefs.SetFloat("Volume",soundSlider.value);
    }
}
