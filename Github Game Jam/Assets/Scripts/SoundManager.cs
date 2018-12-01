using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;
    public AudioSource main,SFX;
    public List<AudioClip> audioList = new List<AudioClip>();
    public List<AudioClip> package1 = new List<AudioClip>();
    public List<AudioClip> package2 = new List<AudioClip>();
    public List<AudioClip> package3 = new List<AudioClip>();
    public List<AudioClip> package4 = new List<AudioClip>();
    public List<List<AudioClip>> SFXList = new List<List<AudioClip>>();
    public Coroutine c;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    // Use this for initialization
    void Start () {
        SFXList.Add(package1);
        SFXList.Add(package2);
        SFXList.Add(package3);
        SFXList.Add(package4);
        main.clip = audioList[0];
        main.Play();
        main.volume = 0.7f;
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    public void FadeMainBG()
    {
        c=StartCoroutine(FadeOut(main, 0.3f, 150f));
    }

    public void MainBGFadeGO()
    {
        StartCoroutine(FadeOut(main, 0.1f, 1f));
    }

    public void StopFadeMainBG()
    {
        StopCoroutine(c);
    }

    public IEnumerator FadeIn(AudioSource audioSource,float MaxVolume, float FadeTime)
    {
        MaxVolume = Mathf.Clamp01(MaxVolume);
        audioSource.Play();
        audioSource.volume = 0;
        while (audioSource.volume <= MaxVolume)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    public IEnumerator FadeOut(AudioSource audioSource, float MinVolume, float FadeTime)
    {
        float startVolume = audioSource.volume;        
        while (audioSource.volume > MinVolume)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
        if (MinVolume == 0)
        {
            audioSource.Stop();
            audioSource.volume = startVolume;
        }
    }
}
/*
        if (Spawner.spawnCount == 7 && !bassIsPlaying)
        {
            FadeinBass();
        }
        if (GridScript.highestpoint >= 13 && !riserIsPlaying)
        {
            FadeInRiser();
        }
        else if(GridScript.highestpoint < 13 && riserIsPlaying)
        {
            FadeOutRiser();
        }*/
