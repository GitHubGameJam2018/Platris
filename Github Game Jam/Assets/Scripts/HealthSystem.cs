using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {
    public static int hp;
    public Image hpImage;
    public List<Sprite> hpSprites = new List<Sprite>();
	// Use this for initialization
	void Start () {
        hp = 6;
	}
	
	// Update is called once per frame
	void Update () {
        if (hp == 0)
        {
            SnapMovement.gameOver = true;
        }
        if (hp >= 0)
            hpImage.sprite = hpSprites[hp];
    }
}
