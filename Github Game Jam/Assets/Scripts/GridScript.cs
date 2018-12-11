using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridScript : MonoBehaviour
{
    public Transform[,] grid = new Transform[10, 18];
    GridSize gs;
    public LayerMask playerLayer;
    public static int highestpoint;
    public static float highestRow;
    // Use this for initialization
    void Start()
    {
        highestpoint = -1;
        SnapMovement.gameOver = false;
        gs = FindObjectOfType<GridSize>();
        for (int i = 0; i < gs.gridWidth; i++)
        {
            for (int j = 0; j < gs.gridHeight; j++)
            {
                grid[i, j] = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int CheckRow()
    {
        List<int> rowList = new List<int>();
        for (int i = 0; i < gs.gridHeight; i++)
        {
            bool check = false;

            for (int j = 0; j < gs.gridWidth; j++)
            {
                if (grid[j, i] == null)
                {
                    check = true;
                }
            }
            if (check == false)
            {
                rowList.Add(i);
            }
        }

        highestRow = FindHighestRow(rowList);

        if (rowList.Count > 0)
        {
            Score.currentScore += 100 * rowList.Count * rowList.Count;
            FindObjectOfType<Score>().UpdateText(rowList.Count);
            if (HealthSystem.hp + rowList.Count <= 6)
            {
                HealthSystem.hp += rowList.Count;
            }
            else
            {
                HealthSystem.hp = 6;
            }
            StartCoroutine(DestroyRow(rowList));
        }
        return rowList.Count;
    }

    public float FindHighestRow(List<int> rowList)
    {
        if (rowList.Count > 0)
        {
            int temp = rowList[0];
            for (int i = 0; i < rowList.Count; i++)
            {
                if (rowList[i] > temp)
                {
                    temp = rowList[i];
                }
            }
            return FindObjectOfType<SnapMovement>().CoorToPos(temp, gs.gridHeight);
        }
        return 0;
    }

    public void FindHighestPoint()
    {
        highestpoint = -1;
        for (int i = 0; i < gs.gridWidth; i++)
        {
            for (int j = 0; j < gs.gridHeight; j++)
            {
                if (grid[i, j] != null && highestpoint < j)
                {
                    highestpoint = j;
                }
            }
        }
    }

    IEnumerator DestroyRow(List<int> row)
    {
        for (int x = row.Count - 1; x >= 0; x--)
        {
            for (int i = 0; i < gs.gridWidth; i++)
            {
               StartCoroutine(grid[i,row[x]].GetComponent<BlinkMino>().BlinkRow());
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
                        if (grid[j, i + 1] != null)
                        {
                            grid[j, i + 1].gameObject.transform.position += Vector3.down * GridSize.scale;
                        }
                        grid[j, i] = grid[j, i + 1];
                    }
                }
            }
        }

        FindHighestPoint();
        FindObjectOfType<SnapMovement>().SpawnMino();
        for (int i = 0; i < gs.gridWidth; i++)
        {
            for (int j = 0; j < gs.gridHeight; j++)
            {
                if (grid[i, j] != null)
                {
                    Transform t = grid[i, j];
                    if (Physics2D.Linecast(t.position + new Vector3(0, GridSize.scale / 2f), t.position + new Vector3(0, -GridSize.scale / 2), playerLayer) ||
                    Physics2D.Linecast(t.position + new Vector3(GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(GridSize.scale / 2f, -GridSize.scale / 2), playerLayer) ||
                    Physics2D.Linecast(t.position + new Vector3(-GridSize.scale / 2f, GridSize.scale / 2f), t.position + new Vector3(-GridSize.scale / 2f, -GridSize.scale / 2), playerLayer))
                    {
                        FindObjectOfType<PlayerMovement>().Spawn(false);
                        break;
                    }
                }
            }
        }
    }    
}
