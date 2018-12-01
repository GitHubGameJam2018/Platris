using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreTetramino : MonoBehaviour {

    [SerializeField] Image sp;
    public static int storedMino;
    public static int currentMino;
    // Use this for initialization
    void Start () {
        storedMino = -1;
        sp.sprite = null;
	}
	
	// Update is called once per frame
	void Update () {
        if (storedMino != -1)
        {
            sp.sprite = FindObjectOfType<NextTeraMino>().nextMinoList[storedMino];
            sp.SetNativeSize();
        }
    }
}
