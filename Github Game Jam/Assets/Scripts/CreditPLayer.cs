using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditPLayer : MonoBehaviour {
    public GameObject player;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<UI>().BackToMenu();
        }
        if (Physics2D.Raycast(transform.position, Vector2.down, 0.4f))
            player.SetActive(true);
	}
}
