using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public List<GameObject> prefabs = new List<GameObject>();
    public static int spawnCount;
	// Use this for initialization
	void Start () {
        int x = Random.Range(0, prefabs.Count - 3);
        Instantiate(prefabs[x]);
        spawnCount = 1;
        StoreTetramino.currentMino = x;
        int y = RandNumGen();
        if (y == x)
        {
            do
            {
                y = FindObjectOfType<Spawner>().RandNumGen();
            }
            while (y == x);

        }
        SnapMovement.nextObjIndex = y;
        SnapMovement.spawnCount = 0;
    }

    public void spawn(int y)
    {
        Instantiate(prefabs[y]);
        spawnCount++;
    }
    public int RandNumGen()
    {
        return Random.Range(0, prefabs.Count);
    }
}
