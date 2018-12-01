using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Spawner : MonoBehaviour {
    public List<GameObject> prefabs = new List<GameObject>();
    // Use this for initialization
    void Start()
    {        
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindObjectOfType<UI>().BackToMenu();
        }
    }

    public GameObject spawn(int y)
    {
        return Instantiate(prefabs[y]);
    }
    public int RandNumGen()
    {
        return Random.Range(0, prefabs.Count);
    }
}
