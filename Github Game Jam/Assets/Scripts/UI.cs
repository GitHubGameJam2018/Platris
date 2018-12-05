using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UI : MonoBehaviour {
    public Animator MainmenuAnim;
    public GameObject pausePanel;
    public Canvas canvas;
    public GameObject tutPanel, credPanel;
    public AudioMixer am;
    public Toggle sfx, music;
    public List<GameObject> panels = new List<GameObject>();
    // Use this for initialization
    void Start() {
        Cursor.visible = true;
        canvas = FindObjectOfType<Canvas>();
        if (pausePanel)
            pausePanel.SetActive(false);
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (!PlayerPrefs.HasKey("SFX"))
                sfx.isOn = false;
            else if (PlayerPrefs.GetInt("SFX") != 1)
                sfx.isOn = true;
            else if(PlayerPrefs.GetInt("SFX") != 0)
                sfx.isOn = false;

            if (!PlayerPrefs.HasKey("Music"))
                music.isOn = false;
            else if (PlayerPrefs.GetInt("Music") != 0)
                music.isOn = false;
            else if(PlayerPrefs.GetInt("Music") != 1)
                music.isOn = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!SnapMovement.gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                Pause();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale == 0 && SceneManager.GetActiveScene().buildIndex == 1)
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        MainmenuAnim.SetBool("Play", true);
        FindObjectOfType<SoundManager>().FadeMainBG();        
    }
    public void Resume()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        FindObjectOfType<SoundManager>().main.volume = 0.7f;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        int i;
        if (PlayerPrefs.GetInt("SFX") == 1 && PlayerPrefs.GetInt("Music") == 1)
        {
            i = 0;
        }
        else if (PlayerPrefs.GetInt("SFX") == 0 && PlayerPrefs.GetInt("Music") == 1)
        {
            i = 1;
        }
        else if(PlayerPrefs.GetInt("SFX") == 1 && PlayerPrefs.GetInt("Music") == 0)
        {
            i = 2;
        }
        else
        {
            i = 3;
        }
        for (int x = 0; x < panels.Count; x++)
        {
            if (x != i)
            {
                panels[x].SetActive(false);
            }
            else
            {
                panels[x].SetActive(true);
            }
        }
        FindObjectOfType<SoundManager>().main.volume = 0.7f;
        Time.timeScale = 1;
        Camera.main.orthographicSize = 5;
        Vector3 wPosition = Camera.main.ScreenToWorldPoint(canvas.gameObject.transform.position);
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.gameObject.transform.position = new Vector3(wPosition.x, wPosition.y, 0);
        canvas.gameObject.transform.localScale = canvas.gameObject.transform.localScale * 10 / Screen.height;
        MainmenuAnim.Play("ToMenuSlide",-1,0);
    }
    public void Tutorial()
    {
        tutPanel.SetActive(true);
        MainmenuAnim.SetBool("PlayTut", true);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Credits()
    {
        credPanel.SetActive(true);
        MainmenuAnim.SetBool("PlayCred", true);
    }

    public void muteSFX(bool mute)
    {
        if (mute)
        {
            am.SetFloat("SFXVolume", -80f);
            PlayerPrefs.SetInt("SFX", 0);
        }
        else
        {
            am.SetFloat("SFXVolume", 0);
            PlayerPrefs.SetInt("SFX", 1);
        }
    }
    public void muteMusic(bool mute)
    {
        if (mute)
        {
            am.SetFloat("MusicVolume", -80f);
            PlayerPrefs.SetInt("Music", 0);
        }
        else
        {
            am.SetFloat("MusicVolume", 0);
            PlayerPrefs.SetInt("Music", 1);
        }
    }
}
