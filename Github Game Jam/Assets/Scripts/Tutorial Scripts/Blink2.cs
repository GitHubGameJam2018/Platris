﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink2 : MonoBehaviour {
    public static int hit;
    void Start()
    {
        hit = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Snap2 sm = transform.parent.GetComponent<Snap2>();
        if (sm.lockTetramino && sm.canMove && sm.canFall)
        {
            if (col.tag == "Player")
            {
                hit++;
                if (hit == 1)
                    FindObjectOfType<T_LevelController>().gotHit = true;
            }
        }
    }

    public IEnumerator BlinkRow()
    {
        int blinkCount = 0;
        while (blinkCount < 3 && this)
        {
            GetComponent<Renderer>().enabled = false;
            blinkCount++;
            if (blinkCount == 3)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(0.1f);
            if (blinkCount < 3)
            {
                GetComponent<Renderer>().enabled = true;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
