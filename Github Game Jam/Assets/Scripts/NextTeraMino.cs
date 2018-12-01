using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextTeraMino : MonoBehaviour {
    [SerializeField] Image sp;
    public List<Sprite> nextMinoList = new List<Sprite>();

    void Start()
    {
        sp.sprite = null;
    }

	void Update () {
        sp.sprite = nextMinoList[SnapMovement.nextObjIndex];
        sp.SetNativeSize();
	}
}
