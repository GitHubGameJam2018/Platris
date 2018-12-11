using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class T_LevelController : MonoBehaviour {
    public bool ADEvent, jumpEvent, doubleJumpEvent, rotateEvent, dropEvent, dodgeEvent, storeEvent, swapEvent, respawnEvent, makeLineEvent;
    public static bool freezeGame;
    public float timer = 0;
    public Text tuts;
    public T_PlayerMovement tpm;
    public GameObject rotateTutPrefab, swapTutPrefab, lineTutPrefab, temp, respawnObj;
    public List<GameObject> lineObject = new List<GameObject>();
    bool spawned = false;
    public bool dj, gotHit, retry=false;
    GameObject player;
    GridSize gs;
    GridScript g;
    // Use this for initialization
    void Start () {
        tuts.text = "Press A & D or LEFT ARROW && RIGHT ARROW \nto move";
        gs = FindObjectOfType<GridSize>();
        g = FindObjectOfType<GridScript>();
        FindObjectOfType<T_PlayerMovement>().disableRespawn = true;
        player = GameObject.FindGameObjectWithTag("Player");
        gotHit = false;
        freezeGame = true;
        timer = 0;
        dj = ADEvent = jumpEvent = doubleJumpEvent = rotateEvent = dropEvent = dodgeEvent = storeEvent = swapEvent = respawnEvent = makeLineEvent = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (Input.GetAxisRaw("Horizontal") != 0 && !ADEvent && timer > 1f)
        {
            ADEvent = true;
            tuts.text = "Press SPACE to jump";
            timer = 0;
            freezeGame = false;            
        }

        if(ADEvent && !jumpEvent && timer > 1f)
        {
            tpm.canJump = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpEvent = true;
                tuts.text = "Press SPACE while in air to double jump";
                timer = 0;
                FindObjectOfType<T_PlayerMovement>().canDoubleJump = true;
            }
        }

        if (jumpEvent && !doubleJumpEvent && timer > 1f)
        {
            if (dj)
            {
                doubleJumpEvent = true;
                tuts.text = "";
                timer = 0;
            }
        }

        if (timer > 2 && doubleJumpEvent && !rotateEvent)
        {
            tuts.text = "Press W or UP ARROW \nto rotate tetramino";
            if (!spawned)
            {
                temp = Instantiate(rotateTutPrefab);
                temp.GetComponent<T_SnapMovement>().canMakeFall = false;
                temp.GetComponent<T_SnapMovement>().canRotate = true;
                spawned = true;
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                FindObjectOfType<T_PlayerMovement>().canMove = true;
                rotateEvent = true;
                timer = 0;
            }
        }

        

        if (timer > 1 && rotateEvent && ! dropEvent)
        {
            tuts.text = "Press S or DOWN ARROW \nto confirm drop Location";
            temp.GetComponent<T_SnapMovement>().canMakeFall = true;
            temp.GetComponent<T_SnapMovement>().canDrop = true;
            temp.GetComponent<T_SnapMovement>().canMove = true;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                dropEvent = true;
                tuts.text = "";
                timer = 0;
                FindObjectOfType<T_PlayerMovement>().canMove = false;
            }
        }

        if(timer>0.7f && dropEvent && !dodgeEvent)
        {
            if (!retry)
            {               
                tuts.text = "Don't get hit by falling tetraminos.\n A & D or LEFT ARROW && RIGHT ARROW \nto move";
                if (Time.timeScale != 0)
                    Time.timeScale = 0;

                if (Input.GetAxisRaw("Horizontal")!=0)
                {
                    retry = true;
                    FindObjectOfType<T_PlayerMovement>().canMove = true;
                    Time.timeScale = 1;
                }
            }
            else if (gotHit)
            {
                tuts.text = "Don't get hit.\nTry Again";
                gotHit = false;
                Destroy(temp);
                temp = Instantiate(rotateTutPrefab);
                temp.GetComponent<T_SnapMovement>().canDrop = true;
                temp.GetComponent<T_SnapMovement>().canMove = true;
                temp.GetComponent<T_SnapMovement>().canFall = true;
                temp.GetComponent<T_SnapMovement>().canRotate = true;
                temp.GetComponent<T_SnapMovement>().lockTetramino = true;
                StartCoroutine(temp.GetComponent<T_SnapMovement>().Blink());
            }
        }

        if(timer <=1 && dodgeEvent && !storeEvent)
        {
            tuts.text = "";
            spawned = false;
        }
        else if(timer > 1 && dodgeEvent && !storeEvent)
        {
            if (!spawned)
            {
                Destroy(temp);
                temp = Instantiate(rotateTutPrefab);
                tuts.text = "Press E or RCTRL to store the tetramino";
                temp.GetComponent<T_SnapMovement>().canStore = true;
                T_StoreTetramino.currentMino = 2;
                spawned = true;
            }            
            if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightControl))
            {
                storeEvent = true;
                tuts.text = "Press E or RCTRL\nto swap between stored and current tetraminos";
                timer = 0;
                temp = Instantiate(swapTutPrefab);
            }
        }

        if (timer > 1 && storeEvent && !swapEvent)
        {
            temp.GetComponent<T_SnapMovement>().canStore = true;
            T_StoreTetramino.currentMino = 4;
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightControl))
            {
                swapEvent = true;
                tuts.text = "";
                timer = 0;
                spawned = false;
            }
        }

        if (timer > 1 && swapEvent && !respawnEvent)
        {
            if (!spawned)
            {
                tuts.text = "You can respawn at the cost of a Health point.\n\nPress R or RSHIFT to respawn";
                T_StoreTetramino.storedMino = -1;
                T_StoreTetramino.currentMino = -1;
                FindObjectOfType<T_PlayerMovement>().disableRespawn = false;
                player.transform.position = new Vector3(-1.5f, -4f, 0);
                Destroy(temp);
                temp = Instantiate(respawnObj);
                spawned = true;
            }
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.RightShift))
            {
                respawnEvent = true;
                tuts.text = "";
                timer = 0;
            }
        }
        if (timer <= 2 && respawnEvent && !makeLineEvent)
        {            
            tuts.text = "";
            spawned = false;
            retry = false;
        }

        if (timer > 2 && respawnEvent && !makeLineEvent)
        {
            if (!retry)
            {
                if (!spawned)
                {
                    Destroy(temp);
                    tuts.text = "Tetraminos auto-confirms 3 blocks away.\n\nComplete a row";
                    FindObjectOfType<T_PlayerMovement>().disableRespawn = true;
                    temp = Instantiate(lineTutPrefab);
                    gotHit = false;
                    for (int i = 0; i < lineObject.Count; i++)
                    {
                        GameObject obj = Instantiate(lineObject[i]);
                        foreach (Transform t in obj.transform)
                        {
                            int x, y;
                            x = FindObjectOfType<T_SnapMovement>().PosToCoor(t.position.x, gs.gridWidth);
                            y = FindObjectOfType<T_SnapMovement>().PosToCoor(t.position.y, gs.gridHeight);
                            g.grid[x, y] = t;
                        }
                    }
                    spawned = true;
                    retry = true;
                    player.transform.position = new Vector3(0, -2.88f, player.transform.position.z);
                }
            }
            else if(gotHit)
            {
                tuts.text = "Complete a row and avoid getting hit";
                Destroy(temp);
                temp = Instantiate(lineTutPrefab);
                gotHit = false;
            }
        }

        if(makeLineEvent)
        {
            tuts.text = "Congrats!\nYou are good to go.\n\npress ESC to return to Menu";
        }
    }
}
