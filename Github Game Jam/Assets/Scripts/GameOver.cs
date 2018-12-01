using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    [SerializeField] GameObject player;
    public Animator camAnim;
    public GameObject GOPanel, IngameUI;
    public Text GOTimerText, GOScoreText, inGameTimerText, inGameScoreText;
    bool done;
	// Use this for initialization
	void Start () {
        done = false;
        GOPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if(SnapMovement.gameOver)
        {            
            CamMovement();
        }
	}

    void CamMovement()
    {
        IngameUI.SetActive(false);
        if (Camera.main.orthographicSize >= 1 && !done)
        {
            camAnim.enabled = false;
            Camera.main.orthographicSize = Vector3.Lerp(Vector3.right * Camera.main.orthographicSize, Vector3.right, 0.05f).x;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10), 0.05f);
        }
        if (Camera.main.orthographicSize <= 1.01f)
        {
            done = true;
            camAnim.enabled = true;
            GOPanel.SetActive(true);
            GOTimerText.text = inGameTimerText.text;
            GOScoreText.text = inGameScoreText.text;
        }
    }
}
