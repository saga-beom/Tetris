using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockControl : MonoBehaviour
{
    public Vector3 RotationPoint;
    public static int height = 20;
    public static int width = 10;
    public float pivotTime = 0.1f;
    public float delayTime = 0.2f;
    private bool gameOver = false;

    private static Transform[,] occupied = new Transform[width, height];


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!VaildMove())
            {
                transform.position -= new Vector3(1, 0, 0);

            }
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(1, 0, 0);
            if (!VaildMove())
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0, 1, 0);
            if(!VaildMove())
            {
                transform.position += new Vector3(0, 1, 0);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            transform.RotateAround(transform.TransformPoint(RotationPoint),
                new Vector3(0, 0, 1), 90);
            if(!VaildMove())
            {
                transform.RotateAround(transform.TransformPoint(RotationPoint),
                    new Vector3(0, 0, 1), -90);
            }
        }

        if(pivotTime < delayTime)
        {
            GravityDown();
            pivotTime += 0.2f;
        } else
        {
            delayTime += Time.deltaTime;
        }


    }

    void GravityDown()
    {
        if (VaildMove())
        {
            transform.position -= new Vector3(0, 1, 0);
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            JudgeGameOver();
            AddOccupy();
            foreach (Transform Children in transform)
            {
                if (CheckLine(Mathf.FloorToInt(Children.position.y)))
                {
                    deleteLine(Mathf.FloorToInt(Children.position.y));
                    RowDown(Mathf.FloorToInt(Children.position.y));
                }
            }
            this.enabled = false;
            if (gameOver == true)
            {
                Debug.Log("Game over");
            }
            else
            {
                FindObjectOfType<CreateBlock>().NewTetris();
            }

        }


    }

    bool VaildMove()
    {
        foreach(Transform Children in transform)
        {
            int locationX = Mathf.FloorToInt(Children.transform.position.x);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (locationX < 0 || locationX >= width || locationY < 0 || locationY >= height)
            {
                return false;
            }

            if (occupied[locationX, locationY] != null)
            {
                return false;
            }

        }
        return true;
    }


    void AddOccupy()
    {
        int locationX;
        int locationY;
        foreach(Transform Children in transform)
        {
            locationX = Mathf.FloorToInt(Children.position.x);
            locationY = Mathf.FloorToInt(Children.position.y);

            occupied[locationX, locationY] = Children;

        }
    }

    bool CheckLine(int targetLine)
    {
        
        for(int i=0; i<width; i++)
        {
            if(occupied[i, targetLine] == null)
            {
                return false;
            }
        }
        

        return true;
    }

    void deleteLine(int targetLine)
    {
        for(int i=0; i < width; i++)
        {
            Destroy(occupied[i, targetLine].gameObject);
            occupied[i, targetLine] = null;
        }
    }

    void RowDown(int targetLine)
    {
        for (int i=targetLine; i<height; i++)
        {
            for(int j=0; j<width; j++)
            {
                if (occupied[j, i] != null)
                {
                    occupied[j, i - 1] = occupied[j, i];
                    occupied[j, i] = null;
                    occupied[j, i - 1].transform.position -= new Vector3(0, 1, 0);
                }
             }
        }
    }

    void JudgeGameOver()
    {
        foreach(Transform Children in transform)
        {
            if(Mathf.FloorToInt(Children.position.y) >= 18)
            {
                gameOver = true;
            }
        }
    }

    
}
