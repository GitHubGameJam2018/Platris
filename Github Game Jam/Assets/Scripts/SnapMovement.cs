using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnapMovement : MonoBehaviour
{
    public static bool gameOver = false;
    public static int nextObjIndex, spawnCount;
    public bool canFall = true, canMove = true, canRotate = true, lockRotation = false;
    public LayerMask minoLayer, playerLayer;
    int blinkCount = 0, playerCheckCount = 0;
    public Sprite red, white;
    [SerializeField] float fallspeed;
    public bool lockTetramino = false;
    GameObject player;
    GridSize gs;
    GridScript g;
    int packageNumber;
    SoundManager sm;

    // Use this for initialization
    void Start()
    {
        if (Time.timeSinceLevelLoad <= Time.deltaTime)
            Invoke("LateStart", Time.deltaTime / 2f);
        else
            LateStart();
        packageNumber = Random.Range(0, 4);
        gs = FindObjectOfType<GridSize>();
        g = FindObjectOfType<GridScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Fall());
        canFall = true;
        canMove = true;
        fallspeed = 1.3f;
    }

    void LateStart()
    {
        sm = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (!gameOver)
        {
            Movement();
            Rotate();
            LockTetramino();
            Store();
        }
    }
    void FixedUpdate()
    {
        if (!gameOver)
        {
            HitCheck();
        }
    }

    void Movement()
    {
        Vector2 tempPos = transform.position;
        if (lockTetramino)
        {
            fallspeed = Time.fixedDeltaTime;
        }
        if (canMove && !lockTetramino)
        {
            if (Input.GetKey(KeyCode.S))
            {
                lockTetramino = true;
                StartCoroutine(Blink());
            }
            if (IsInGrid(player.transform.position))
            {
                tempPos = transform.position;
                float tetraMinoPosX = Mathf.RoundToInt((player.transform.position.x / GridSize.scale)-GridSize.scale / 2f) * GridSize.scale + GridSize.scale / 2f;
                transform.position = new Vector2(tetraMinoPosX, transform.position.y);
            }

            foreach (Transform t in transform)
            {                
                if (!IsInGrid(t.position))
                {
                    if (t.position.x < -(GridSize.scale * (gs.gridWidth - 1) / 2f))
                    {
                        transform.position += Vector3.right * GridSize.scale;
                    }
                    else if (t.position.x > (GridSize.scale * (gs.gridWidth - 1) / 2f))
                    {
                        transform.position -= Vector3.right * GridSize.scale;
                    }
                }

                int xCoor = PosToCoor(t.position.x, gs.gridWidth);
                int yCoor = PosToCoor(t.position.y, gs.gridHeight);
                if (xCoor >= 0 && xCoor < gs.gridWidth && yCoor >= 0 && yCoor < gs.gridHeight)
                {
                    if (g.grid[xCoor, yCoor] != null)
                    {
                        transform.position = tempPos;
                    }
                }
            }
        }

    }
    void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.W) && canMove && canRotate && !lockTetramino) 
        {
            if(sm)
                sm.PlaySFX(sm.SFXList[packageNumber][Random.Range(0, sm.SFXList[packageNumber].Count)]);
            transform.Rotate(new Vector3(0, 0, 90));
            if (lockRotation && (Mathf.RoundToInt(transform.rotation.eulerAngles.z / 180) % 2 != 0))
            {
                transform.Rotate(new Vector3(0, 0, -180));
            }
        }        
    }
    

    void HitCheck()
    {
        if (canMove)
        {
            int index = 0;
            RaycastHit2D[] rayArray = new RaycastHit2D[4];

            foreach (Transform t in transform)
            {
                rayArray[index] = Physics2D.Linecast(t.position, t.position + ((GridSize.scale / 2) + 0.05f) * Vector3.down, minoLayer);
                index++;
            }

            foreach (RaycastHit2D r in rayArray)
            {
                if (r)
                {
                    sm.PlaySFX(sm.audioList[8]);
                    canFall = false;
                    canMove = false;
                    if (PosToCoor(transform.position.x, gs.gridWidth) >= 5)
                    {
                        Camera.main.GetComponent<Animator>().SetBool("playRight", true);
                    }
                    else
                    {
                        Camera.main.GetComponent<Animator>().SetBool("playLeft", true);
                    }
                    foreach (Transform t in transform)
                    {
                        int xCoor = PosToCoor(t.position.x, gs.gridWidth);
                        int yCoor = PosToCoor(t.position.y, gs.gridHeight);
                        t.gameObject.layer = 9;
                        if (xCoor >= 0 && xCoor < gs.gridWidth && yCoor >= 0 && yCoor < gs.gridHeight)
                            g.grid[xCoor, yCoor] = t;
                        if (t.position.y > (GridSize.scale * (gs.gridHeight - 1) / 2f))
                        {
                            gameOver = true;
                            sm.PlaySFX(sm.audioList[5]);
                            StartCoroutine(FindObjectOfType<SoundManager>().FadeOut(FindObjectOfType<AudioSource>(), 0.1f, 1f));
                            break;
                        }
                    }
                    int a = FindObjectOfType<GridScript>().CheckRow();
                    
                    if (a == 0)
                        SpawnMino();
                    else if (a == 1)
                        sm.PlaySFX(sm.audioList[1]);
                    else if (a == 2 || a == 3)
                        sm.PlaySFX(sm.audioList[2]);
                    else
                        sm.PlaySFX(sm.audioList[3]);
                    StartCoroutine(HitCheckPlayer());
                    /*for(int i=0;i<gs.gridWidth;i++)
                    {
                        for (int j = 0; j < gs.gridHeight; j++)
                        {
                            if(g.grid[i, j]!=null)
                            {
                                Debug.Log(new Vector2(i, j));
                            }
                        }
                    }*/

                    g.FindHighestPoint();
                    foreach (Transform t in transform)
                    {
                        t.GetComponent<SpriteRenderer>().sprite = white;
                        t.GetComponent<BoxCollider2D>().isTrigger = false;
                    }
                    break;
                }
            }
        }
    }

    void LockTetramino()
    {
        if (!lockTetramino)
        {
            foreach (Transform t in transform)
            {
                int xCoor = PosToCoor(t.position.x, gs.gridWidth);
                int yCoor = PosToCoor(t.position.y, gs.gridHeight);

                for (int i = yCoor - 1; i >= -1; i--)
                {
                    if (xCoor < gs.gridWidth && xCoor >= 0 && i < gs.gridHeight)
                    {
                        int x;
                        if (i + 1 >= 11)
                        {
                            x = 1;
                        }
                        else
                            x = 3;
                        if ((i == -1 || g.grid[xCoor, i] != null) && yCoor - (i + 1) <= x && blinkCount == 0)
                        {
                            StartCoroutine(Blink());
                            break;
                        }
                    }
                }
                if (lockTetramino)
                {                    
                    break;
                }
            }
        }
    }

    public void SpawnMino()
    {
        FindObjectOfType<Spawner>().spawn(nextObjIndex);
        StoreTetramino.currentMino = nextObjIndex;
        int x = FindObjectOfType<Spawner>().RandNumGen();
        if (x == nextObjIndex)
        {
            do
            {
                x = FindObjectOfType<Spawner>().RandNumGen();
            }
            while (x == nextObjIndex);

        }
        nextObjIndex = x;
    }

    void Store()
    {
        if (!lockTetramino && Input.GetKeyDown(KeyCode.E))
        {
            if (StoreTetramino.storedMino == -1)
            {
                StoreTetramino.storedMino = StoreTetramino.currentMino;
                Destroy(this.gameObject);
                Destroy(GetComponent<Predictions>().shadow);
                SpawnMino();
            }
            else
            {
                int temp;
                Instantiate(FindObjectOfType<Spawner>().prefabs[StoreTetramino.storedMino], transform.position, Quaternion.identity);
                temp = StoreTetramino.currentMino;
                StoreTetramino.currentMino = StoreTetramino.storedMino;
                StoreTetramino.storedMino = temp;
                Destroy(this.gameObject);
                Destroy(GetComponent<Predictions>().shadow);
            }
        }
    }

    public IEnumerator HitCheckPlayer()
    {
        if (playerCheckCount < 3)
        {
            foreach (Transform t in transform)
            {
                if (Physics2D.Linecast(t.position + new Vector3(0, GridSize.scale / 2f), t.position + new Vector3(0, -GridSize.scale/2), playerLayer) || 
                    Physics2D.Linecast(t.position + new Vector3(GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(GridSize.scale / 2f, -GridSize.scale/2), playerLayer)||
                    Physics2D.Linecast(t.position + new Vector3(-GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(-GridSize.scale / 2f, -GridSize.scale/2), playerLayer))
                {
                    FindObjectOfType<PlayerMovement>().Spawn(true);
                    break;
                }
            }
            playerCheckCount++;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Fall()
    {
        while (!gameOver)
        {
            if (canFall)
            {
                transform.position += Vector3.down * GridSize.scale;
                yield return new WaitForSeconds(fallspeed);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator Blink()
    {
        lockTetramino = true;
        canFall = false;
        while (blinkCount<4 && canMove)
        {
            foreach (Transform t in transform)
            {
                t.GetComponent<SpriteRenderer>().sprite = red;
            }
            blinkCount++;
            if (blinkCount == 3)
            {
                canFall = true;                
            }
            yield return new WaitForSeconds(0.3f);
            if (blinkCount < 3)
            {
                foreach (Transform t in transform)
                {
                    t.GetComponent<SpriteRenderer>().sprite = white;
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
    }


    public bool IsInGrid(Vector3 v)
    {
        if (Mathf.Abs(v.x) > (GridSize.scale * (gs.gridWidth) / 2f))
        {
            return false;
        }
        return true;
    }

    public int PosToCoor(float pos, int grid)
    {
        if(grid==gs.gridHeight)
            return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 1));
        return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 0.5f));        
    }

    public float CoorToPos(int coor, int grid)
    {
        if (grid == gs.gridHeight)
            return ((-grid / 2f + coor + 1) * GridSize.scale);
        return ((-grid / 2f + coor + 0.5f) * GridSize.scale);
    }    
}   
