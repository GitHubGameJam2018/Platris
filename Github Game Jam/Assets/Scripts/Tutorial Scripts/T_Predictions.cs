using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Predictions : MonoBehaviour {


    [SerializeField] GameObject prefab;
    public LayerMask minoLayer;
    public int anglelock;
    public bool applyOffset = false;
    public GameObject shadow;
    GridSize gs;
    GridScript g;
    Snap2 sm;
    // Use this for initialization
    void Start()
    {
        shadow = Instantiate(prefab);
        sm = FindObjectOfType<Snap2>();
        gs = FindObjectOfType<GridSize>();
        g = FindObjectOfType<GridScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Predict();
    }

    void Predict()
    {
        if (!GetComponent<Snap2>().lockTetramino)
        {
            int childcount = 0;
            shadow.transform.position = new Vector3(transform.position.x, sm.CoorToPos(0, gs.gridHeight));
            shadow.transform.rotation = transform.rotation;

            foreach (Transform t in shadow.transform)
            {
                if (Mathf.Abs(t.position.y) > (GridSize.scale * (gs.gridHeight - 1) / 2f))
                {
                    shadow.transform.position += Vector3.up * GridSize.scale;
                }
                int xCoor = sm.PosToCoor(t.position.x, gs.gridWidth);
                int yCoor = sm.PosToCoor(t.position.y, gs.gridHeight);

                for (int i = sm.PosToCoor(transform.GetChild(childcount).position.y, gs.gridHeight) - 1; i >= 0; i--)
                {
                    if (xCoor < gs.gridWidth && xCoor >= 0 && i < gs.gridHeight)
                    {
                        if (g.grid[xCoor, i] != null && sm.CoorToPos(i + 1, gs.gridHeight) > shadow.transform.position.y)
                        {
                            shadow.transform.position = new Vector3(transform.position.x, sm.CoorToPos(i + 1, gs.gridHeight));
                            if (anglelock != 0 && applyOffset)
                            {
                                if (transform.rotation.eulerAngles.z == anglelock)
                                {
                                    shadow.transform.position += t.localPosition.y * GridSize.scale * Vector3.up;
                                }
                            }
                            else if (applyOffset)
                            {
                                if (transform.rotation.eulerAngles.z == 0)
                                {
                                    shadow.transform.position += t.localPosition.y * GridSize.scale * Vector3.up;
                                }
                            }
                            yCoor = sm.PosToCoor(t.position.y, gs.gridHeight);
                            break;
                        }
                    }
                }

                while ((xCoor >= 0 && xCoor < gs.gridWidth && yCoor >= 0 && yCoor < gs.gridHeight) && g.grid[xCoor, yCoor] != null)
                {
                    shadow.transform.position += Vector3.up * GridSize.scale;
                    yCoor = sm.PosToCoor(t.position.y, gs.gridHeight);

                }

                childcount++;
            }
        }
        else
        {
            Destroy(shadow);
        }
    }
}
