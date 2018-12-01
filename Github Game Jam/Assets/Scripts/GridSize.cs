using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSize : MonoBehaviour {
    public static float scale;
    public int gridWidth = 10;
    public int gridHeight = 18;
    public float playWidth = 2.8125f;
    public float playHeight = 2.8125f / 9 * 16;
    // Use this for initialization
    void Start () {
        gridWidth = 10;
        gridHeight = 18;
        scale = playWidth/(gridWidth/2f);
        transform.localScale = new Vector2(scale, scale);
	}    
}
