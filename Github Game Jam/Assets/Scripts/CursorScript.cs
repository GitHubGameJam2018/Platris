using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorScript : MonoBehaviour {
    public float cursorTimer=0;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        cursorTimer += Time.deltaTime;
		if(Input.GetAxisRaw("Mouse X")!=0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            Cursor.visible = true;
            cursorTimer = 0;
        }
        if (cursorTimer > 2)
        {
            Cursor.visible = false;
        }
	}
}
