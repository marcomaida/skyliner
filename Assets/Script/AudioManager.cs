using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour{

    public static AudioManager activeManager;//istanza accessibile da ogni classe;

    public AudioClip [] clipLibrary;//Lista delle clip da definire in unity

    List<AudioObject> freeAudioObjects;
    List<AudioObject> busyAudioObjects;
    int numberOfClips = 0;
    int INITIAL_CLIPS_NUMBER = 10;

    public bool paused = false;
    private float volume = 1;
    public float Volume
    {
        get { return volume; }
        set
        {
            if (value < 0)
                volume = 0;
            else if (value > 1)
                volume = 1;
            else
                volume = value;

            foreach (AudioObject audioObj in busyAudioObjects)
            {
                audioObj.SetVolume(volume);
            }
        }
    }
    private float pitch = 1;
    public float Pitch
    {
        get { return pitch; }
        set
        {
            if (value < 0)
                pitch = 0;
            else if (value > 2)
                pitch = 2;
            else
                pitch = value;

            foreach (AudioObject audioObj in busyAudioObjects)
            {
                audioObj.SetPitch(pitch);
            }
        }
    }

    public AudioManager()
    {
        freeAudioObjects = new List<AudioObject>();
        busyAudioObjects = new List<AudioObject>();

        if (activeManager == null)
            activeManager = this;
    }

    void Start()
    {
        for (int i = 1; i <= INITIAL_CLIPS_NUMBER; i++)
            NewAudioObject(i);
    }

    void NewAudioObject(int index)
    {//Aggiunge una clip alla pool. Index rappresenta il nome che la clip avrà.

        AudioObject audioObj = AudioObject.CreateNewAudioObject("Clip " + index, this);

        freeAudioObjects.Add(audioObj);
        numberOfClips++;
    }

    public void FreeAudioObject(AudioObject audioObj)
    {
        busyAudioObjects.Remove(audioObj);
        freeAudioObjects.Add(audioObj);
    }
    public void UseAudioObject(AudioObject audioObj)
    {
        freeAudioObjects.Remove(audioObj);
        busyAudioObjects.Add(audioObj);
    }

    public AudioObject PlayClip(AudioClip audioClip, Vector3 audioPosition, bool loop)
    {
        if (!paused)
        {
            if (freeAudioObjects.Count <= 0)
                NewAudioObject(numberOfClips + 1);

            AudioObject audioObject = freeAudioObjects[0];
            audioObject.Play(audioClip, audioPosition, loop);

            return audioObject;
        }
        else return null;
    }

    public AudioObject PlayClipFromLibrary(int index, Vector3 audioPosition, bool loop)
    {
        if (index < clipLibrary.Length)
            return PlayClip(clipLibrary[index], audioPosition, loop);
        else return null;
    }

    public static AudioObject PlayWithActiveAudioManager(AudioClip audioClip, Vector3 audioPosition, bool loop)
    {//Se si accede alla classe dall'istanza statica, si deve controllare che esista
        if (activeManager != null)
        {
            return activeManager.PlayClip(audioClip, audioPosition, loop);
        }
        else return null;
    }

    public void StopClips()
    {//Ferma tutto l'audio
        foreach (AudioObject audioObj in busyAudioObjects)
        {
            audioObj.Stop(false);
            freeAudioObjects.Add(audioObj);
        }

        busyAudioObjects.Clear();
    }

    public void PauseClips()
    {//Mette in pausa tutto l'audio
        foreach (AudioObject audioObj in busyAudioObjects)
        {
            audioObj.Pause();
        }
        paused = true;
    }

    public void UnPauseClips()
    {//Riprende tutto l'audio
        foreach (AudioObject audioObj in busyAudioObjects)
        {
            if (audioObj.paused)
                audioObj.UnPause();
        }
        paused = false ;
    }
}
