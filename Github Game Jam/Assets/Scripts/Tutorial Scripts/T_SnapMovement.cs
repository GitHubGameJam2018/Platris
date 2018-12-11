using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_SnapMovement : MonoBehaviour {

    public static bool gameOver = false;
    public static int nextObjIndex, spawnCount;
    public bool canFall = true, canMove = true, canRotate = true, canDrop = false, canMakeFall = false, lockRotation = false,canStore=false;
    public LayerMask minoLayer, playerLayer;
    int blinkCount = 0, playerCheckCount = 0;
    public Sprite red, white;
    [SerializeField] float fallspeed;
    public bool lockTetramino = false;
    GameObject player;
    GridSize gs;
    GridScript g;
    SoundManager sm;
    int packageNumber;
    // Use this for initialization
    void Start()
    {
        packageNumber = Random.Range(0, 4);
        if (Time.timeSinceLevelLoad <= Time.deltaTime)
            Invoke("LateStart", Time.deltaTime / 2f);
        else
            LateStart();
        gs = FindObjectOfType<GridSize>();
        g = FindObjectOfType<GridScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Fall());
        fallspeed = 1.3f;
    }
    void LateStart()
    {
        sm = FindObjectOfType<SoundManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!T_LevelController.freezeGame)
        {
            Movement();
            Rotate();
            LockTetramino();
        }
        Store();
    }
    void FixedUpdate()
    {
        if (!T_LevelController.freezeGame)
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
        if (canDrop && !lockTetramino)
        {
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && canMakeFall) 
            {
                lockTetramino = true;
                StartCoroutine(Blink());
            }
            if (IsInGrid(player.transform.position))
            {
                tempPos = transform.position;
                float tetraMinoPosX = Mathf.RoundToInt((player.transform.position.x / GridSize.scale) - GridSize.scale / 2f) * GridSize.scale + GridSize.scale / 2f;
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
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canRotate && !lockTetramino)
        {
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
                            break;
                        }
                    }
                    FindObjectOfType<GridScript>().CheckRow();
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
                    if (!FindObjectOfType<T_LevelController>().gotHit)
                    {
                        FindObjectOfType<T_LevelController>().dodgeEvent = true;
                        FindObjectOfType<T_LevelController>().timer = 0;
                    }
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

   
    void Store()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightControl)) && canStore)
        {
            if (T_StoreTetramino.storedMino == -1)
            {
                T_StoreTetramino.storedMino = T_StoreTetramino.currentMino;
                Destroy(this.gameObject);
            }
            else
            {
                GameObject temp;
                int temp2;
                temp=Instantiate(FindObjectOfType<T_Spawner>().prefabs[T_StoreTetramino.storedMino], transform.position, Quaternion.identity);
                temp.GetComponent<T_SnapMovement>().canStore = true;
                FindObjectOfType<T_LevelController>().temp = temp;
                temp2 = T_StoreTetramino.currentMino;
                T_StoreTetramino.currentMino = T_StoreTetramino.storedMino;
                T_StoreTetramino.storedMino = temp2;
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator HitCheckPlayer()
    {
        if (playerCheckCount < 3)
        {
            foreach (Transform t in transform)
            {
                if (Physics2D.Linecast(t.position + new Vector3(0, GridSize.scale / 2f), t.position + new Vector3(0, -GridSize.scale / 2), playerLayer) ||
                    Physics2D.Linecast(t.position + new Vector3(GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(GridSize.scale / 2f, -GridSize.scale / 2), playerLayer) ||
                    Physics2D.Linecast(t.position + new Vector3(-GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(-GridSize.scale / 2f, -GridSize.scale / 2), playerLayer))
                {
                    FindObjectOfType<T_LevelController>().gotHit = true;
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

    public IEnumerator Blink()
    {
        if (canMakeFall)
        {
            lockTetramino = true;
            canFall = false;
            while (blinkCount < 4 && canMove)
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
        if (grid == 18)
            return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 1));
        return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 0.5f));
    }

    public float CoorToPos(int coor, int grid)
    {
        if (grid == 18)
            return ((-grid / 2f + coor + 1) * GridSize.scale);
        return ((-grid / 2f + coor + 0.5f) * GridSize.scale);
    }
}
