using UnityEngine;
using System.Collections;

public class AudioObject : MonoBehaviour {

    AudioManager audioManager;//Padre
    AudioSource audioSource;//componente Unity
    Vector3 soundPosition;//Dove verrà riprodotto il suono
    bool canPlay;
    public bool paused;


	void Start ()
    {
	
	}
	
	void Update () {
        if (!canPlay && !paused && !audioSource.isPlaying)
        {
            canPlay = true;
            audioManager.FreeAudioObject(this);
        }
	}

    public void Play(AudioClip audioClip, Vector3 audioPosition, bool loop)
    {//Suona la clip passata dalla posizione richiesta

        transform.position = audioPosition;
        SetVolume(audioManager.Volume);
        audioSource.clip = audioClip;

        audioManager.UseAudioObject(this);
        audioSource.Play();
        audioSource.loop = loop;
        canPlay = false;
        paused = false;
    }

    public void Stop()
    {
        audioSource.Stop();
        canPlay = true;
        audioManager.FreeAudioObject(this);
    }

    public void Stop(bool freeInManagerPool)
    {
        audioSource.Stop();
        canPlay = true;
        if(freeInManagerPool)
            audioManager.FreeAudioObject(this);
    }

    public void Pause()
    {
        audioSource.Pause();
        paused = true;
    }
    
    public void UnPause()
    {
        audioSource.UnPause();
        paused = false;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public static AudioObject CreateNewAudioObject(string name, AudioManager audioManager)
    {
        AudioObject audioObj = new GameObject(name).AddComponent<AudioObject>();
        audioObj.initialize(audioManager);
        return audioObj;
    }

    private void initialize(AudioManager audioManager)
    {//inizializza i valori necessari per funzionare con l'audioManager

        audioSource = this.gameObject.AddComponent<AudioSource>();

        this.audioManager = audioManager;
        transform.SetParent(audioManager.transform);
        canPlay = true;
    }
}
