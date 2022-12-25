using UnityEngine;



public class BlockControl : MonoBehaviour
{
    //define variable
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
        // move to left, if block move to out of background, location is restored 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (RightVaildMove())
            {
                transform.position += new Vector3(1, 0, 0);

            }
        }

        // move to right, if block move to out of background, location is restored
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (LeftVaildMove())
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }

        // Block's moved down by pressing downArrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (VerticalVaildMove())
            {
                transform.position -= new Vector3(0, 1, 0);
            }
        }

        // change rotation, if block move to out of background, location is restored
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.RotateAround(transform.TransformPoint(RotationPoint),
                new Vector3(0, 0, 1), 90);
            if (!VaildMove())
            {
                transform.RotateAround(transform.TransformPoint(RotationPoint),
                    new Vector3(0, 0, 1), -90);
            }
        }

        // make delay for slow down
        if (pivotTime < delayTime)
        {
            GravityDown();
            pivotTime += 0.2f;
        }
        else
        {
            delayTime += Time.deltaTime;
        }


    }

    // block move down without pressing a key 
    void GravityDown()
    {
        if (VerticalVaildMove())
        {
            transform.position -= new Vector3(0, 1, 0);
        }
        else
        {
            JudgeGameOver();
            AddOccupy();
            foreach (Transform Children in transform)
            {
                if (CheckLine(Mathf.FloorToInt(Children.position.y)))
                {
                    DeleteLine(Mathf.FloorToInt(Children.position.y));
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
        foreach (Transform Children in transform)
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

    // check a block is in background
    bool VerticalVaildMove()
    {
        foreach (Transform Children in transform)
        {
            int locationX = Mathf.FloorToInt(Children.transform.position.x);
            int locationY = Mathf.FloorToInt(Children.transform.position.y - 1);

            if (locationY < 0 || locationY >= height)
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

    bool LeftVaildMove()
    {
        foreach(Transform Children in transform)
        {
            int leftLocationX = Mathf.FloorToInt(Children.transform.position.x - 1);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (leftLocationX < 0)
            {
                return false;
            }

            if (occupied[leftLocationX, locationY]  != null)
            {
                return false;
            }
        }

        return true;
    }


    bool RightVaildMove()
    {
        foreach (Transform Children in transform)
        {
            int rightLocationX = Mathf.FloorToInt(Children.transform.position.x + 1);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (rightLocationX >= width)
            {
                return false;
            }

            if (occupied[rightLocationX, locationY] != null)
            {
                return false;
            }
        }

        return true;
    }


    // to indicate occupied space in background
    void AddOccupy()
    {
        int locationX;
        int locationY;
        foreach (Transform Children in transform)
        {
            locationX = Mathf.FloorToInt(Children.position.x);
            locationY = Mathf.FloorToInt(Children.position.y);

            occupied[locationX, locationY] = Children;

        }
    }

    // check the target line has "null" value
    bool CheckLine(int targetLine)
    {

        for (int i = 0; i < width; i++)
        {
            if (occupied[i, targetLine] == null)
            {
                return false;
            }
        }


        return true;
    }

    // delete the target line's game object 
    void DeleteLine(int targetLine)
    {
        for (int i = 0; i < width; i++)
        {
            Destroy(occupied[i, targetLine].gameObject);
            occupied[i, targetLine] = null;
        }
    }

    // the blocks located in over deleted blocks move down
    void RowDown(int targetLine)
    {
        for (int i = targetLine; i < height; i++)
        {
            for (int j = 0; j < width; j++)
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

    // if height is higher than 18, game over
    void JudgeGameOver()
    {
        foreach (Transform Children in transform)
        {
            if (Mathf.FloorToInt(Children.position.y) >= 18)
            {
                gameOver = true;
            }
        }
    }


}
