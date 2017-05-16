using UnityEngine;
using System.Collections;
using LoLSDK;

public class AudioManager : MonoBehaviour {

    public AudioSource MusicPlayer;
    public AudioSource SfxPlayer;

    public AudioClip[] Musics; 
    public AudioClip[] Sfxs;


    public string[] MscNames;

	// Use this for initialization
	void Start () {
        gameObject.tag = "AudioMan";
        //LOLSDK.Instance.PlaySound(AudioManager.GetMscName(0));
        _PlayM(0);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void PlayM(int MusicNum)
    {
        GetRef()._PlayM(MusicNum);
    }

    public static void PlayS(int SfxNum)
    {
        GetRef()._PlayM(SfxNum);
    }

    private void _PlayM(int musicNum)
    {
        if (Application.isEditor)
        {
            MusicPlayer.Stop();
            MusicPlayer.clip = Musics[musicNum];
            MusicPlayer.Play();
        }
        else
        {
            LOLSDK.Instance.PlaySound(AudioManager.GetMscName(musicNum), true, true);
        }
        CBUG.Do("AUdio name?: " + Musics[musicNum].name);
        //LOLSDK.Instance.PlaySound(Musics[musicNum].name
    }

    private void _PlayS(int sfxNum)
    {
        if (Application.isEditor)
        {
            SfxPlayer.Stop();
            SfxPlayer.clip = Sfxs[sfxNum];
            SfxPlayer.Play();
        }
        else
        {
            LOLSDK.Instance.PlaySound(AudioManager.GetMscName(sfxNum));
        }

    }

    public static AudioManager GetRef()
    {
        return GameObject.FindGameObjectWithTag("AudioMan").GetComponent<AudioManager>();
    }

    public static string GetMscName(int musicNum)
    {
        return GetRef().MscNames[musicNum];
    }
}
