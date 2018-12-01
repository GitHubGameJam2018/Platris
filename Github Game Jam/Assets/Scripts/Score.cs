using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public static int currentScore;
    public Text scoreText, timerText;
    int s1, s2;
    string st1, st2;
    public Image comboImage;
    public List<Sprite> comboList = new List<Sprite>();
	// Use this for initialization
	void Start () {
        currentScore = 0;
        scoreText.text = currentScore.ToString();
        comboImage.sprite = null;
        timerText.text = "00:00";
    }
	
	// Update is called once per frame
	void Update () {
        if (!SnapMovement.gameOver)
        {
            Timer();
        }
    }

    public void UpdateText(int combo)
    {
        scoreText.text = currentScore.ToString();
        if (combo > 1)
        {
            comboImage.GetComponent<Animator>().Play("ComboPop", -1, 0);
            comboImage.transform.parent.transform.position = new Vector3(comboImage.transform.parent.transform.position.x, Camera.main.WorldToScreenPoint(Vector3.up * GridScript.highestRow).y, comboImage.transform.parent.transform.position.z);
            comboImage.transform.localScale = new Vector3(1, 1, 1) * (1 - 0.25f * (4 - combo));
            comboImage.sprite = comboList[combo - 2];
        }
    }

    public void Timer()
    {
        s1 = (int)(Time.timeSinceLevelLoad / 60);
        s2 = (int)(Time.timeSinceLevelLoad % 60);
        st1 = s1.ToString();
        st2 = s2.ToString();
        if (s1 < 10)
        {
            st1 = "0" + st1;
        }
        if (s2 < 10)
        {
            st2 = "0" + st2;
        }
        timerText.text = st1 + ":" + st2;
    }
}
