using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap2 : MonoBehaviour
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
    public GridSize gs;
    GridScript g;
    int packageNumber;
    SoundManager sm;
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
        canFall = true;
        canMove = true;
        fallspeed = 1.6f;
    }
    void LateStart()
    {
        sm = FindObjectOfType<SoundManager>();
    }
    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotate();
        LockTetramino();
    }
    void FixedUpdate()
    {
        HitCheck();
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
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
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
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && canMove && canRotate && !lockTetramino)
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
                    CheckRow();
                    StartCoroutine(HitCheckPlayer());


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

    IEnumerator Blink()
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
        if (grid == gs.gridHeight)
            return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 1));
        return Mathf.RoundToInt((pos / GridSize.scale + (grid / 2f) - 0.5f));
    }

    public float CoorToPos(int coor, int grid)
    {
        if (grid == gs.gridHeight)
            return ((-grid / 2f + coor + 1) * GridSize.scale);
        return ((-grid / 2f + coor + 0.5f) * GridSize.scale);
    }


    public void CheckRow()
    {
        List<int> rowList = new List<int>();
        for (int i = 0; i < gs.gridHeight; i++)
        {
            bool check = false;

            for (int j = 0; j < gs.gridWidth; j++)
            {
                if (g.grid[j, i] == null)
                {
                    check = true;
                }
            }
            if (check == false)
            {
                rowList.Add(i);
            }
        }
        if (rowList.Count > 0)
        {
            sm.PlaySFX(sm.audioList[1]);
            StartCoroutine(DestroyRow(rowList));
        }
        else
            FindObjectOfType<T_LevelController>().gotHit = true;
    }
    IEnumerator DestroyRow(List<int> row)
    {
        for (int x = row.Count - 1; x >= 0; x--)
        {
            for (int i = 0; i < gs.gridWidth; i++)
            {
                if(g.grid[i, row[x]].GetComponent<T_BlinkMino>())
                    StartCoroutine(g.grid[i, row[x]].GetComponent<T_BlinkMino>().BlinkRow());
                else
                    StartCoroutine(g.grid[i, row[x]].GetComponent<Blink2>().BlinkRow());
            }
        }
        yield return new WaitForSeconds(0.5f);
        {
            for (int x = row.Count - 1; x >= 0; x--)
            {
                for (int i = row[x]; i < gs.gridHeight - 1; i++)
                {
                    for (int j = 0; j < gs.gridWidth; j++)
                    {
                        if (g.grid[j, i + 1] != null)
                        {
                            g.grid[j, i + 1].gameObject.transform.position += Vector3.down * GridSize.scale;
                        }
                        g.grid[j, i] = g.grid[j, i + 1];
                    }
                }
            }
        }
        FindObjectOfType<T_LevelController>().makeLineEvent = true;
    }
}
