using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Anims : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CamBools()
    {
        GetComponent<Animator>().SetBool("playRight", false);
        GetComponent<Animator>().SetBool("playLeft", false);
    }

    public void Change()
    {
        GetComponent<Animator>().SetBool("Play", false);
        GetComponent<Renderer>().enabled = false;
    }
    public void MainMenuAnim()
    {
        SceneManager.LoadScene(1);
    }
    public void ToMenuAnim()
    {
        SceneManager.LoadScene(0);
    }
    public void ToTutAnim()
    {
        SceneManager.LoadScene(2);
    }
    public void ToCredAnim()
    {
        SceneManager.LoadScene(3);
    }
}
