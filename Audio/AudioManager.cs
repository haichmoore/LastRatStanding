using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Sound[] sounds;

    private void Awake() {
        foreach(Sound sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(string name, float minPitch = 1, float maxPitch = 1){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        float pitchHolder = s.source.pitch;
        if(minPitch != 1 || maxPitch != 1){
            s.source.pitch = s.source.pitch* UnityEngine.Random.Range(minPitch,maxPitch);
        }
        s.source.Play();
        s.source.pitch = pitchHolder;
    }
    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
    
}
